using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Domain;

namespace YAPI.Services
{
    public class PostService : IPostService
    {
        private List<Post> posts;

        public PostService()
        {
            posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                posts.Add(new Post
                {
                    Id = Guid.NewGuid(),
                    Name = $"Post Name{i}"
                });
            }
        }

        public bool DeletePost(Guid id)
        {
            var post = GetPostById(id);
            if (post == null)
                return false;

            posts.Remove(post);
            return true;
        }

        public Post GetPostById(Guid id)
        {
            return posts.SingleOrDefault(x => x.Id == id);
        }

        public List<Post> GetPosts()
        {
            return posts;
        }

        public bool UpdatePost(Post postToUpdate)
        {
            var exists = GetPostById(postToUpdate.Id) != null;
            if (!exists)
                return false;

            var index = posts.FindIndex(x => x.Id == postToUpdate.Id);
            posts[index] = postToUpdate;
            return true;
        }
    }
}
