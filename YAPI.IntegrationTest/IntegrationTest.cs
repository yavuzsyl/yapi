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
using YAPI.Contracts.V1;
using YAPI.Contracts.V1.Requests;
using YAPI.Contracts.V1.Responses;
using YAPI.Data;
using YAPI.Domain;

namespace YAPI.IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient testClient;

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
            testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }
        private async Task<string> GetJwtAsync()
        {
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegistrationRequest
            {
                Email = "integration@mail.com",
                Password = "123456It!"
            });

            var regsitrationResponse = await response.Content.ReadAsAsync<AuthenticationResult>();
            return regsitrationResponse.Token;
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest createPostRequest)
        {
            var response = await testClient.PostAsJsonAsync(ApiRoutes.Posts.Create, createPostRequest);
            return (await response.Content.ReadAsAsync<PostResponse>());
        }
    }
}
