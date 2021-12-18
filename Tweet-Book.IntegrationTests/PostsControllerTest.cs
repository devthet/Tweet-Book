using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.v1;
using Tweetbook.Domain;
using Xunit;
using Tweet_Book.Contracts.v1.Requests;

namespace Tweet_Book.IntegrationTests
{
    public class PostsControllerTest:IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnEmptyResponse()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll) ;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
        }
        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInTheDatabase()
        {
            //Arrange
            await AuthenticateAsync();
            var createPost = await CreatePostAsync(new CreatePostRequest
            { 
                Name = "Test Post"
            });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await response.Content.ReadAsAsync<Post>();
            returnedPost.Id.Should().Be(createPost.Id);
            returnedPost.Name.Should().Be("Test Post");
        }
    }
}
