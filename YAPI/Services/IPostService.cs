using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Domain;

namespace YAPI.Services
{
    public interface IPostService
    {
        List<Post> GetPosts();
        Post GetPostById(Guid id);
        bool UpdatePost(Post postToUpdate);
        bool DeletePost(Guid id);
    }
}
