using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests.Queries;
using Tweet_book.Contracts.v1.Responses;
using Tweet_Book.Cache;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Contracts.v1.Responses;
using Tweet_Book.Domain;
using Tweet_Book.Extensions;
using Tweet_Book.Helpers;
using Tweet_Book.Services;
using Tweetbook.Contracts.v1;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {

        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService,IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }


        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery]PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var posts = await _postService.GetPostsAsync(pagination);
            var postResponses = _mapper.Map<List<PostResponse>>(posts);
            if(pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PostResponse>(postResponses));
            }
            //var nextPage = pagination.PageNumber >= 1 ? _uriService
            //    .GetAllPostsUri(new PaginationQuery(pagination.PageNumber +1,pagination.PageSize)).ToString() 
            //    :null;
            //var previousPage = pagination.PageNumber-1 >= 1 ? _uriService
            //   .GetAllPostsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
            //   : null;
            ////var postResponses = posts.Select(post => new PostResponse
            ////{
            ////    Id = post.Id,
            ////    Name = post.Name,
            ////    UserId = post.UserId,
            ////    Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
            ////}).ToList();

            //var pageResponses = new PagedResponse<PostResponse>(postResponses)
            //{
            //    Data = postResponses,
            //    PageNumber = pagination.PageNumber >= 1? pagination.PageNumber:(int?)null,
            //    PageSize = pagination.PageSize >=1? pagination.PageSize:(int?)null,
            //    NextPage = postResponses.Any()?nextPage:null,
            //    PreviousPage = previousPage
            //};
            var pageResponses = PaginationHelpers.CreatePaginatedResponse(_uriService,pagination,postResponses);
            return Ok(pageResponses);
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute]Guid postId )
        {
            // var post = _posts.SingleOrDefault(x => x.Id == postId);
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null) 
                return NotFound();

            //return Ok(new PostResponse { 
            //    Id = post.Id, 
            //    Name = post.Name,
            //    UserId = post.UserId,
            //    Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName })
            //});
            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///     Sample **request**:
        ///     Post /api/v1/posts
        ///     {
        ///         "name":"some name"
        ///     }
        /// </remarks>
        /// <param name="postrequest"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody]CreatePostRequest postrequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post {
                Id = newPostId,
                Name = postrequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postrequest.Tags.Select(x => new PostTag {PostId=newPostId,TagName=x.Name }).ToList()
                //Tags = postrequest.Tags.Select(x => new Tag { PostId = newPostId, TagName = x.Name }).ToList()
            };
            //if(Guid.Empty == post.Id)
            //{
            //    post.Id = Guid.NewGuid();
            //    post.Name = "new post";
            //}
            //  _posts.Add(post);
            //await _postService.GetPosts().Add(post);
           await _postService.CreatePostAsync(post);

            //var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var locationUri = baseUri +"/"+ ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            var locationUri = _uriService.GetPostUri(post.Id.ToString());
            //var postResponse = new PostResponse 
            //{ Id = post.Id,
            //  Name = post.Name,
            //  UserId = post.UserId,
            //  Tags = post.Tags.Select(x => new TagResponse {Name=x.TagName }) 
            //};
            var postResponse = new Response<PostResponse>( _mapper.Map<PostResponse>(post));
            return Created(locationUri, postResponse);
        }
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnPost = await _postService.UserOwnPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }
            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = request.Name;
            //var post = new Post
            //{
            //    Id = postId,
            //    Name = request.Name
            //};
            var updated = await _postService.UpdatePostAsync(post);
            var response = new PostResponse
            {
                Id = post.Id,
                Name = post.Name,
                UserId = post.UserId,
                Tags = post.Tags.Select(x => new TagResponse { Name = x.TagName })
            };
            if (updated)
                return Ok(new Response<PostResponse>(response));
            else
                return NotFound();


        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnPost = await _postService.UserOwnPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }

            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted) return NoContent();

            return NotFound();

        }
    }
}
