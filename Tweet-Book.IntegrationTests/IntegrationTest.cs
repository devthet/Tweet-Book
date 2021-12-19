using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tweet_Book.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tweetbook.Contracts.v1;
using System.Net.Http.Json;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Contracts.v1.Responses;
using Tweetbook.Contracts.v1.Responses;

namespace Tweet_Book.IntegrationTests
{
   // public class IntegrationTest:IDisposable
     public class IntegrationTest 
    {
        protected readonly HttpClient TestClient;
        //private readonly IServiceProvider _serviceProvider;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder=>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(ApplicationDbContext));
                        //services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb2");
                        });
                        //var descriptor = services.SingleOrDefault(d =>
                        //    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        //if (descriptor != null)
                        //{
                        //    services.Remove(descriptor);
                        //}

                        //services.AddDbContext<ApplicationDbContext>(options =>
                        //{
                        //    options.UseInMemoryDatabase("TestDB2");
                        //});
                    });
                });
           // _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        //public void Dispose()
        //{
        //    using var serviceScope = _serviceProvider.CreateScope();
        //    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        //    context.Database.EnsureDeleted();
        //}

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create,request);
            return await response.Content.ReadAsAsync<PostResponse>();

        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register,new UserRegistrationRequest { 
                Email = "tester3@tester.com",
                Password = "Tester1234!"
            });
            //var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }
    }
}
