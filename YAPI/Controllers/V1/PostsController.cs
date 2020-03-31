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
        public IActionResult GetAll()
        {
            return Ok(postService.GetPosts());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute]Guid postId)
        {
            var post = postService.GetPostById(postId);
            if (post == null)
                return NotFound();
            return Ok(post);
        }


        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post() { Id = postRequest.Id };

            if (post.Id != Guid.Empty)
                post.Id = Guid.NewGuid();

            postService.GetPosts().Add(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse() { Id = post.Id };
            return Created(location, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute]Guid postId, [FromBody]UpdatePostRequest request)
        {
            var post = new Post()
            {
                Id = postId,
                Name = request.Name
            };
            var isUpdated = postService.UpdatePost(post);

            if (isUpdated)
                return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete ([FromRoute]Guid postId)
        {
            var isDeleted = postService.DeletePost(postId);
            if (isDeleted)
                return NoContent();

            return NotFound();
        }
    }
}
