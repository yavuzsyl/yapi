using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
