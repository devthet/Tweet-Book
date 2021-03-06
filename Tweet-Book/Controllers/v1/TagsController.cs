using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Requests;
using Tweet_Book.Contracts.v1.Responses;
using Tweet_Book.Domain;
using Tweet_Book.Services;
using Tweetbook.Contracts.v1;

namespace Tweet_Book.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin,Poster")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class TagsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        //private readonly ITagSerivce _tagSerivce;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postService"></param>
        /// <param name="mapper"></param>
        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
            // _tagSerivce = tagSerivce;
        }
        /// <summary>
        /// Return all the tags in the system
        /// </summary>
        /// <response code="200">Return all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAllTags()
        {
            //return Ok(_tagSerivce.GetTags());
            var tags = await _postService.GetAllTagsAsync();
            var tagsResponse = tags.Select(x => new TagResponse { Name = x.TagName }).ToList();
            // var tagsResponse = _mapper.Map<List<TagResponse>>(tags);
            return Ok(tagsResponse);
        }
        [HttpGet(ApiRoutes.Tags.Get, Name = "Get")]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);
            if (tag == null) return NotFound();
            //return Ok(new TagResponse { Name = tag.TagName });
            return Ok(_mapper.Map<TagResponse>(tag));


            //var tag =  _tagSerivce.GetTagById(tagName);
            //if (tag == null) return NotFound();
            //return Ok(tag);
        }
        /// <summary>
        /// Create a tag in the system
        /// </summary>
        /// <response code="201">Create a tag in the system</response>
        /// <response code="400">Enable to create the tag due to validation errors</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest Request)
        {
            //if (Guid.Empty == tagRequest.TagId)
            //{
            //    tagRequest.TagId = Guid.NewGuid();
            //}
            //if (!ModelState.IsValid)
            //{

            //}

            var tag = new Tag
            {
                CreatorId = Guid.NewGuid(),
                TagName = Request.Name,
                CreatedOn = DateTime.Now
            };
            //tags.Add(tag);
            var created = await _postService.CreateTagAsync(tag);
            if (!created) return BadRequest(new { error = "Enable to create tag" });
            //if (!created)
            //{
            //    return BadRequest(new ErrorResponse { Errors = new List<ErrorModel> { new ErrorModel { Message = "Enable to create tag" } } });
            //}


            var response = new TagResponse { Name = tag.TagName };
            // var response = _mapper.Map<TagResponse>(tag);
            return CreatedAtRoute("Get", new { tagName = response.Name }, response);
            // return Ok(response);
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        //[Authorize(Roles ="Admin")]
        [Authorize(Policy = "MustWorkForChapsas")]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);
            if (deleted) return NoContent();
            return NotFound();
        }



    }
}
