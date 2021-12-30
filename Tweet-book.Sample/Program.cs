using Refit;
using System.Threading.Tasks;
using Tweet_Book.Sdk;
using Tweet_Book.Contracts.v1.Requests;
using System.Linq;
using System.Collections.Generic;

namespace Tweet_book.Sample
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            var cachedToken = string.Empty;
            var identityApi = RestService.For<IIdentityApi>("http://localhost:5000");
            var tweetbookApi = RestService.For<ITweetbookApi>("http://localhost:5000",
                new RefitSettings { 
                    AuthorizationHeaderValueGetter = ()=> Task.FromResult(cachedToken)
                });

            var registerResponse = await identityApi.RegisterAsync(
                new UserRegistrationRequest
                {
                    Email ="sdk@sdk.com",
                    Password ="Test1234!"

                });
            var loginResponse = await identityApi.LoginAsync(
                new UserLoginRequest
                {
                    Email = "sdk@sdk.com",
                    Password = "Test1234!"

                });
            cachedToken = loginResponse.Content.Token;

            var allPosts = await tweetbookApi.GetAllAsync();
            //var tag = new TagRequest { Name= "sdk" };
            //IEnumerable<TagRequest> postTag = tag; 
            var createdPost = await tweetbookApi.CreateAsync(
                new CreatePostRequest
                {
                    Name = "Created by sdk",
                    Tags = new[] { new TagRequest { Name = "sdk" } }
                });
            var retrivedPost = await tweetbookApi.GetAsync(createdPost.Content.Id);
            var updatedPost = await tweetbookApi.UpdateAsync(createdPost.Content.Id,
                new UpdatePostRequest { 
                    Name = "Updated by the sdk"
                });
            var deletedPost = await tweetbookApi.DeleteAsync(createdPost.Content.Id);
        }
    }
}
