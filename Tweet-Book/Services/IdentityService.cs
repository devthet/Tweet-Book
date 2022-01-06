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
        private readonly IFacebookAuthService _facebookAuthService;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, ApplicationDbContext dbContext, IFacebookAuthService facebookAuthService = null)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dbContext = dbContext;
            _facebookAuthService = facebookAuthService;
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
                       // new Claim(ClaimTypes.Role, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim("id",user.Id)
                    };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var userClaim = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaim);

            
            // var userRoles = await _userManager.GetRolesAsync(user);
            //foreach (var userRole in userClaim)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, userRole));
            //    var role = await _roleManager.FindByNameAsync(userRole);
            //    if (role == null) continue;
            //    var roleClaims = await _roleManager.GetClaimsAsync(role);

            //    foreach (var roleClaim in roleClaims)
            //    {
            //        if (claims.Contains(roleClaim))
            //            continue;

            //        claims.Add(roleClaim);
            //    }
            //}

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

        public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        {
            var validatedTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if (!validatedTokenResult.Data.IsValid)
            {
                return new AuthenticationResult {
                    Errors = new[] { "Invalid facebook token." }
                };
            }
            var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(userInfo.Email);
            if(user == null)
            {
                
                    var identityUser = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = userInfo.Email,
                        UserName = userInfo.Email

                    };
               var createdResult=  await _userManager.CreateAsync(identityUser);
                if (!createdResult.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Something went wrong." }
                    };
                }
                return await GenerateAuthenticationResultForUser(identityUser);
            }
            return await GenerateAuthenticationResultForUser(user);
        }
    }
}
