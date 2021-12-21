using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tweet_Book.Data;
using Tweet_Book.Domain;
using Tweet_Book.Options;
using System.Security.Claims;

namespace Tweet_Book.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _dbContext;

        public IdentityService(UserManager<IdentityUser> userManager,JwtSettings jwtSettings,TokenValidationParameters tokenValidationParameters,ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dbContext = dbContext;
        }

       

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists " }
                };
            }
            //var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            //if (!userHasValidPassword)
            //{
            //    return new AuthenticationResult
            //    {
            //        Errors = new[] { "User/Password combination is wrong " }
            //    };
            //}
            // Claims
            var newUserId = Guid.NewGuid();
            //
            var newUser = new IdentityUser
            {// Claims
                Id = newUserId.ToString(),
                Email = email,
                UserName = email
            };
           
            var createdUser = await _userManager.CreateAsync(newUser, password);
            
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }
            //
            await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));
            //

            return await GenerateAuthenticationResultForUser (newUser);
        }
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exists " }
                };
            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User/Password combination is wrong " }
                };
            }
            return await GenerateAuthenticationResultForUser (user);
        }
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null) return new AuthenticationResult { Errors = new[] { "Invalid token" } };
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDatetimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).
                AddMinutes(expiryDateUnix);
            if (expiryDatetimeUtc > DateTime.UtcNow) return new AuthenticationResult { Errors = new[] {"This token hasn't expired yet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if(storedRefreshToken ==null ) 
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exists" } };
            if(DateTime.UtcNow> storedRefreshToken.ExpiryDate)
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            if(storedRefreshToken.IsValidated)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            if(storedRefreshToken.Used)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            if(storedRefreshToken.JwtId != jti)
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match JWT" } };
            storedRefreshToken.Used = true;
            _dbContext.RefreshTokens.Update(storedRefreshToken);
            await _dbContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUser(user);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
               // _tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
               // _tokenValidationParameters.ValidateLifetime = true;
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch 
            {
                return null;
            }

        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && 
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim("id",user.Id)
                    };
            var userClaim = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaim);

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject =new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
               
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };
           await _dbContext.RefreshTokens.AddAsync(refreshToken);
           await _dbContext.SaveChangesAsync();
            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        
    }
}
