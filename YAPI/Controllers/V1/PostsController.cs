using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Requests.Queries;
using Yapi.Contracts.V1.Responses;
using YAPI.Cache;
using YAPI.Domain;
using YAPI.Extensions.AuthExtensions;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly IMapper mapper;
        private readonly IUriService uriService;
        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            this.postService = postService;
            this.mapper = mapper;
            this.uriService = uriService;
        }


        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(1)]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllPostsQuery query,[FromQuery] PaginationQuery paginationQuery)
        {
            var paginationFilter = mapper.Map<PaginationFilter>(paginationQuery);
            var filter = mapper.Map<GetAllPostsFilter>(query);
            var posts = await postService.GetPostsAsync(filter, paginationFilter);
            var pagedPosts = mapper.Map<List<PostResponse>>(posts);

            if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
            {
                return Ok(new PagedResponse<PostResponse>(pagedPosts));

            }

            var pagedResponse = PaginationHelper.CreatePaginatedResponse<PostResponse>(uriService, paginationFilter, pagedPosts);

            return Ok(pagedResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid postId)
        {
            var post = await postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();

            var posRes = mapper.Map<PostResponse>(post);
            return Ok(new Response<PostResponse>(posRes));
        }


        [HttpPost(ApiRoutes.Posts.Create)]
        [Authorize(Policy = "WorksForDude")]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePostRequest postRequest)
        {
            var postId = Guid.NewGuid();
            var post = new Post()
            {
                Id = postId,
                Name = postRequest.Name,
                AppUserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(x => new PostTag { PostId = postId, TagName = x }).ToList()

            };

            var result = await postService.CreatePostAsync(post);
            if (!result)
                return BadRequest();

            //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = mapper.Map<PostResponse>(post);
            var location = uriService.GetPostUri(post.Id.ToString());
            return Created(location, new Response<PostResponse>(response));
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute]Guid postId, [FromBody]UpdatePostRequest request)
        {
            var userOwnsPost = await postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
                return BadRequest(new { Error = "User has not permission to edit this post" });

            var isUpdated = await postService.UpdatePostAsync(request, postId);

            if (isUpdated)
            {
                var post = await postService.GetPostByIdAsync(postId);
                return Ok(new Response<PostResponse>(mapper.Map<PostResponse>(post)));

            }


            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid postId)
        {
            var userOwnsPost = await postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
                return BadRequest(new { Error = "User has not permission to edit this post" });

            var isDeleted = await postService.DeletePostAsync(postId);
            if (isDeleted)
                return NoContent();

            return NotFound();
        }
    }
}
