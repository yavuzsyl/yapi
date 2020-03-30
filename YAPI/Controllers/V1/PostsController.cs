using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Contracts.Requests;
using YAPI.Contracts.Responses;
using YAPI.Contracts.V1;
using YAPI.Domain;

namespace YAPI.Controllers.V1
{
    public class PostsController : Controller
    {
        private List<Post> posts;
        public PostsController()
        {
            posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                posts.Add(new Post { Id = Guid.NewGuid().ToString() });
            }
        }


        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(posts);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post() { Id = postRequest.Id };

            if (string.IsNullOrEmpty(post.Id))
                post.Id = Guid.NewGuid().ToString();

            posts.Add(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id);

            var response = new PostResponse() { Id = post.Id };
            return Created(location, response);
        }
    }
}
