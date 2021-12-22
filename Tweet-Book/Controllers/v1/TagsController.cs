using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin,Poster")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly ITagSerivce _tagSerivce;

        public TagsController(IPostService postService,ITagSerivce tagSerivce)
        {
            _postService = postService;
            _tagSerivce = tagSerivce;
        }
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public  IActionResult GetAllTags()
        {
            return Ok(_tagSerivce.GetTags());
        }
        [HttpGet(ApiRoutes.Tags.Get,Name ="Get")]
        public IActionResult GetById([FromRoute]string tagName)
        {
            var tag = _tagSerivce.GetTagById(tagName);
            if (tag == null) return NotFound();
            return Ok(tag);
        }
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse),(int)HttpStatusCode.OK)]
        public IActionResult CreateTag([FromBody]TagRequest tagRequest)
        {
            //if (Guid.Empty == tagRequest.TagId)
            //{
            //    tagRequest.TagId = Guid.NewGuid();
            //}
            var tag = new Tag { CreatorId = Guid.NewGuid(), Name= tagRequest.TagName ,CreatedOn=DateTime.Now};
            //tags.Add(tag);
            _tagSerivce.CreateTag(tag);

            var response = new TagResponse { TagName = tag.Name };
             return CreatedAtRoute("Get",new {tagId= response.TagId }, response);
           // return Ok(response);
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles ="Admin")]
        public IActionResult Delete([FromRoute] string tagName)
        {
            var deleted = _tagSerivce.DeleteTag(tagName);
            if (deleted) return NoContent();
            return NotFound();
        }



    }
}
