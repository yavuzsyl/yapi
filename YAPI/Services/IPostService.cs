using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Contracts.Requests;
using YAPI.Domain;

namespace YAPI.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(Guid id);
        Task<bool> UpdatePostAsync(UpdatePostRequest postToUpdate, Guid postId);
        Task<bool> DeletePostAsync(Guid id);
        Task<bool> CreatePostAsync(Post postToCreate);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
    }
}
