using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Data;
using YAPI.Domain;

namespace YAPI.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext dataContext;

        public PostService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await GetPostByIdAsync(id);
            if (post == null)
                return false;

            dataContext.Posts.Remove(post);
            var isDeleted = await dataContext.SaveChangesAsync() > 0;
            return isDeleted;
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            return await dataContext.Posts.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await dataContext.Posts.ToListAsync();
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            var post = await GetPostByIdAsync(postToUpdate.Id);
            if (post == null)
                return false;
            post.Name = postToUpdate.Name;
            dataContext.Posts.Update(post);
            var isUpdated = await dataContext.SaveChangesAsync() > 0;
            return isUpdated;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await dataContext.Posts.AddAsync(post);
            var isCraeted = await dataContext.SaveChangesAsync() > 0;
            return isCraeted;
        }
    }
}
