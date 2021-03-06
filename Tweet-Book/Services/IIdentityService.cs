using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Domain;

namespace Tweet_Book.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);

        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
    }
}
