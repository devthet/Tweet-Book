using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Data;
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
        public async Task<IActionResult> Posts()
        {
            return Ok(await _postService.GetPostsAsync());
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId )
        {
            // var post = _posts.SingleOrDefault(x => x.Id == postId);
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null) 
                return NotFound();
            
            return Ok(post);
        }
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId,[FromBody]UpdatePostRequest request )
        {

            var post = new Post
            {
                Id = postId,
                Name = request.Name
            };
            var updated = await _postService.UpdatePostAsync(post);
            if (updated)
                return Ok(post);
            else
                return NotFound();

          
        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {

            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted) return NoContent();

            return NotFound();

        }
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromForm]CreatePostRequest postrequest)
        {
            var post = new Post{ Name= postrequest.Name  };
            //if(Guid.Empty == post.Id)
            //{
            //    post.Id = Guid.NewGuid();
            //    post.Name = "new post";
            //}
            //  _posts.Add(post);
            //await _postService.GetPosts().Add(post);
           await _postService.CreatePostAsync(post);
          
            var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUri +"/"+ ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            var postResponse = new PostResponse { Id= post.Id };
            return Created(locationUri, postResponse);
        }
    }
}
