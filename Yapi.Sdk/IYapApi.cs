using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;

namespace Yapi.Sdk
{
    public interface IYapApi
    {
        [Get("/api/v1/posts")]
        Task<ApiResponse<List<PostResponse>>> GetAllAsync();

        [Get("/api/v1/posts/{postId}")]
        Task<ApiResponse<PostResponse>> GetAsync(Guid postId);

        [Post("/api/v1/posts")]
        Task<ApiResponse<PostResponse>> CreateAsync([Body] CreatePostRequest createPostRequest);

        [Put("/api/v1/posts/{postId}")]
        Task<ApiResponse<PostResponse>> UpdateAsync(Guid postId, [Body] UpdatePostRequest updatePostRequest);

        [Delete("/api/v1/posts/{postId}")]
        Task<ApiResponse<string>> DeleteAsync(Guid postId);
    }
}
