using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Contracts.v1.Responses;
using Tweet_Book.Services;
using Tweetbook.Contracts.v1;

namespace Tweet_Book.Controllers.v1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse {
                   Errors= ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(
                    new AuthFailResponse
                    {
                        Errors = authResponse.Errors
                    }
                );
            }
            return Ok(new AuthSuccessResponse {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(
                    new AuthFailResponse
                    {
                        Errors = authResponse.Errors
                    }
                );
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken

            });
        }
        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!authResponse.Success)
            {
                return BadRequest(
                    new AuthFailResponse
                    {
                        Errors = authResponse.Errors
                    }
                );
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken

            });
        }
        [HttpPost(ApiRoutes.Identity.FacebookAuth)]
        public async Task<IActionResult> FacebookAuth([FromBody] UserFacebookAuthRequest request)
        {
            var authResponse = await _identityService.LoginWithFacebookAsync(request.AccessToken);
            if (!authResponse.Success)
            {
                return BadRequest(
                    new AuthFailResponse
                    {
                        Errors = authResponse.Errors
                    }
                );
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken

            });
        }
    }
}
