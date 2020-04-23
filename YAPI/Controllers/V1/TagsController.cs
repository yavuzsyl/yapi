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
using YAPI.Contracts.V1;
using YAPI.Contracts.V1.Requests;
using YAPI.Contracts.V1.Responses;
using YAPI.Domain;
using YAPI.Extensions.AuthExtensions;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin,Poster")]//remove roles for policy
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

        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await postService.GetAllTagsAsync();
            return Ok(mapper.Map<List<TagResponse>>(tags));
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute,Required] string tagName)
        {
            var tag = await postService.GetTagByNameAsync(tagName);

            if (tag == null)
                return NoContent();

            return Ok(mapper.Map<TagResponse>(tag));
        }   
        
        [HttpPost(ApiRoutes.Tags.Create)]
        [Authorize(Policy ="WorksForDude")]
        public async Task<IActionResult> Create([FromBody,Required] CreateTagRequest tagReqModel)
        {
            var tag = new Tag()
            {
                Name = tagReqModel.Name,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var result = await postService.CreateTagAsync(tag);

            if (!result)
                return BadRequest("Unable to create tag");

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", tag.Name);

            return Created(locationUri,mapper.Map<TagResponse>(tag));
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles ="Admin")]
        [Authorize(Roles ="Poster")]
        public async Task<IActionResult> Delete([FromRoute,Required] string tagName)
        {
            var deleteResult = await postService.DeleteTagAsync(tagName);
            if (!deleteResult)
                return NotFound();

            return NoContent();
        }
    }
}