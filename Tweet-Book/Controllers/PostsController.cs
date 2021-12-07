using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Contracts.v1;
using Tweetbook.Contracts.v1.Requests;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.Controllers
{
    public class PostsController : Controller
    {
        private List<Post> _posts;
        public PostsController()
        {
            _posts = new List<Post>();
           for(int i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid().ToString() });
            }
        }
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult Posts()
        {
            return Ok(_posts);
        }
        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Posts([FromForm]PostRequest postrequest)
        {
            var post = new Post{ Id= postrequest.Id  };
            if(String.IsNullOrEmpty(post.Id))
            {
                post.Id = Guid.NewGuid().ToString();
            }
            _posts.Add(post);
            var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUri +"/"+ ApiRoutes.Posts.Get.Replace("{postId}", post.Id);
            var postResponse = new PostResponse { Id= post.Id };
            return Created(locationUri, postResponse);
        }
    }
}
