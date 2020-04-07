using Cosmonaut;
using Cosmonaut.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Contracts.V1.Requests;
using YAPI.Domain;

namespace YAPI.Services
{
    public class CosmosPostService : IPostService
    {
        private readonly ICosmosStore<CosmosPostDto> cosmosStore;
        public CosmosPostService(ICosmosStore<CosmosPostDto> cosmosStore)
        {
            this.cosmosStore = cosmosStore;
        }
        public async Task<bool> CreatePostAsync(Post postToCreate)
        {
            var cosmposPost = new CosmosPostDto()
            {
                Id = Guid.NewGuid().ToString(),
                Name = postToCreate.Name
            };
            var response = await cosmosStore.AddAsync(cosmposPost);
            return response.IsSuccess;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await cosmosStore.FindAsync(id.ToString());
            if (post == null)
                return false;

            var response = await cosmosStore.RemoveAsync(x => x.Id == post.Id.ToString());
            return response.IsSuccess;
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            var post = await cosmosStore.FindAsync(id.ToString());
            if (post == null)
                return null;

            return new Post { Id = Guid.Parse(post.Id), Name = post.Name };
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            var posts = await cosmosStore.Query().ToListAsync();
            return posts.Select(x => new Post { Id = Guid.Parse(x.Id), Name = x.Name }).ToList();
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            var post = await cosmosStore.FindAsync(postToUpdate.Id.ToString());
            if (post == null)
                return false;

            var cosmposPost = new CosmosPostDto()
            {
                Id = postToUpdate.Id.ToString(),
                Name = postToUpdate.Name
            };

            var response = await cosmosStore.UpdateAsync(cosmposPost);

            return response.IsSuccess;
        }

        public Task<bool> UpdatePostAsync(UpdatePostRequest postToUpdate, Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
