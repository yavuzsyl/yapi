﻿using Refit;
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
        Task<ApiResponse<PagedResponse<PostResponse>>> GetAllAsync();

        [Get("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> GetAsync(Guid postId);

        [Post("/api/v1/posts")]
        Task<ApiResponse<Response<PostResponse>>> CreateAsync([Body] CreatePostRequest createPostRequest);

        [Put("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> UpdateAsync(Guid postId, [Body] UpdatePostRequest updatePostRequest);

        [Delete("/api/v1/posts/{postId}")]
        Task<ApiResponse<string>> DeleteAsync(Guid postId);
    }
}
