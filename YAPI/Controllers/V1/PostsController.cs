using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Contracts.Requests;
using YAPI.Contracts.Responses;
using YAPI.Contracts.V1;
using YAPI.Domain;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    public class PostsController : Controller
    {
        private readonly IPostService postService;
        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }


        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid postId)
        {
            var post = await postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();
            return Ok(post);
        }


        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post() { Name = postRequest.Name };

            var result = await postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse() { Id = post.Id };
            return Created(location, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute]Guid postId, [FromBody]UpdatePostRequest request)
        {
            var post = new Post()
            {
                Id = postId,
                Name = request.Name
            };
            var isUpdated = await postService.UpdatePostAsync(post);

            if (isUpdated)
                return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid postId)
        {
            var isDeleted = await postService.DeletePostAsync(postId);
            if (isDeleted)
                return NoContent();

            return NotFound();
        }
    }
}
