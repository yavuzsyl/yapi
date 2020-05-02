using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;
using YAPI.Data;

namespace YAPI.IntegrationTest
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient testClient;
        private readonly IServiceProvider serviceProvider;

        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                //creating in memory database instead of using the real one
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("yapiIntegrationTestDB");
                        });
                    });
                });
            serviceProvider = appFactory.Services;
            testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }
        private async Task<string> GetJwtAsync()
        {
            testClient.DefaultRequestHeaders.Add("ApiKey", "Secretsstts");
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegistrationRequest
            {
                Email = "user@dude.com",
                Password = "9585200Ys!"
            });

            var regsitrationResponse = await response.Content.ReadAsAsync<AuthenticationResponse>();
            return regsitrationResponse.Token;
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest createPostRequest)
        {
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Posts.Create, createPostRequest);
            return (await response.Content.ReadAsAsync<Response<PostResponse>>()).Data;
        }

        /// <summary>
        /// create scope level service : at every request new datacontext and deletes that in memory database at every request this is some next level shit for me xD
        /// </summary>
        public void Dispose()
        {
            using var serviceScope = serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}
