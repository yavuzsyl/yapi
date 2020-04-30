using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;
using YAPI.Domain;
using YAPI.Extensions.AuthExtensions;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin,Poster")]//remove roles for policy
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces(contentType:"application/json")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly IMapper mapper;

        public TagsController(IMapper mapper, IPostService postService)
        {
            this.postService = postService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="200">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await postService.GetAllTagsAsync();
            return Ok(new Response<List<TagResponse>>(mapper.Map<List<TagResponse>>(tags)));
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute, Required] string tagName)
        {
            var tag = await postService.GetTagByNameAsync(tagName);

            if (tag == null)
                return NoContent();

            return Ok(new Response<TagResponse>(mapper.Map<TagResponse>(tag)));
        }

        /// <summary>
        /// Creates the tag in the system
        /// </summary>
        /// <response code="201">Creates the tag in the system</response>
        /// <response code="400">Unable to create the tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [Authorize(Policy = "WorksForDude")]
        [ProducesResponseType(typeof(TagResponse), statusCode: 201)]
        [ProducesResponseType(typeof(ErrorResponse), statusCode: 400)]
        public async Task<IActionResult> Create([FromBody, Required] CreateTagRequest tagReqModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest("this is not good way to handle validations errors cuz we have to write this everywhere instead of this we are going to write a middleware and that will handle all validations"); ValidationFİlter.cs
            //}
            var tag = new Tag()
            {
                Name = tagReqModel.TagName,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var result = await postService.CreateTagAsync(tag);

            if (!result)
                return BadRequest(new ErrorResponse { Errors = new List<ErrorModel> { new ErrorModel { Message = "Unable to create tag" } } });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", tag.Name);

            return Created(locationUri, new Response<TagResponse>(mapper.Map<TagResponse>(tag)));
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Poster")]
        public async Task<IActionResult> Delete([FromRoute, Required] string tagName)
        {
            var deleteResult = await postService.DeleteTagAsync(tagName);
            if (!deleteResult)
                return NotFound();

            return NoContent();
        }
    }
}