using Refit;
using System;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests;

namespace Yapi.Sdk.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");
            var tweetbookApi = RestService.For<IYapApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });

            var registerResponse = await identityApi.RegisterAsync(new RegistrationRequest
            {
                Email = "sdkaccount@dude.com",
                Password = "1234567Sdk!"
            });

            var loginResponse = await identityApi.LoginAsync(new LoginRequest
            {
                Email = "sdkaccount@dude.com",
                Password = "1234567Sdk!"
            });

            cachedToken = loginResponse.Content.Token;

            var allPosts = await tweetbookApi.GetAllAsync();

            var createdPost = await tweetbookApi.CreateAsync(new CreatePostRequest
            {
                Name = "This is created by the SDK",
                Tags = new[] { "sdk" }
            });

            var retrievedPost = await tweetbookApi.GetAsync(createdPost.Content.Id);

            var updatedPost = await tweetbookApi.UpdateAsync(createdPost.Content.Id, new UpdatePostRequest
            {
                Name = "This is updated by the SDK"
            });

            var deletePost = await tweetbookApi.DeleteAsync(createdPost.Content.Id);
        }
    }
    
}
