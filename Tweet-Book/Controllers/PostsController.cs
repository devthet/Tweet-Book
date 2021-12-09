using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Services;
using Tweetbook.Contracts.v1;
using Tweetbook.Contracts.v1.Requests;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
       
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult Posts()
        {
            return Ok(_postService.GetPosts());
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute]Guid postId )
        {
            // var post = _posts.SingleOrDefault(x => x.Id == postId);
            var post = _postService.GetPostById(postId);
            if (post == null) 
                return NotFound();
            
            return Ok(post);
        }
        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute] Guid postId,[FromBody]UpdatePostRequest request )
        {

            var post = new Post
            {
                Id = postId,
                Name = request.Name
            };
            var updated = _postService.UpdatePost(post);
            if (updated)
                return Ok(post);
            else
                return NotFound();

          
        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete([FromRoute] Guid postId)
        {

            var deleted = _postService.DeletePost(postId);
            if (deleted) return NoContent();

            return NotFound();

        }
        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Posts([FromForm]PostRequest postrequest)
        {
            var post = new Post{ Id= postrequest.Id  };
            if(Guid.Empty == post.Id)
            {
                post.Id = Guid.NewGuid();
                post.Name = "new post";
            }
            //  _posts.Add(post);
            _postService.GetPosts().Add(post);
          
            var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUri +"/"+ ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            var postResponse = new PostResponse { Id= post.Id };
            return Created(locationUri, postResponse);
        }
    }
}
