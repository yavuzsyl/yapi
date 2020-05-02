using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;
using YAPI.Domain;

namespace YAPI.IntegrationTest
{
    public class PostControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_ReturnsEmptyResponse_WithoutAnyPosts()
        {
            //Arange 
            //will authenticate the client
            await AuthenticateAsync();

            //Act
            var response = await testClient.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<PagedResponse<PostResponse>>()).Data.Should().BeEmpty();
        }


        [Fact]
        public async Task Get_ReturnsPost_WhenPostExists()
        {
            //Arange
            await AuthenticateAsync();
            var willBeCreatedPost = await CreatePostAsync(new CreatePostRequest
            {
                Name = "test",
                Tags = new[] { "integrationTag1" }
            });

            //Act
            var response = await testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", willBeCreatedPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPostFromGet = (await response.Content.ReadAsAsync<Response<PostResponse>>()).Data;
            returnedPostFromGet.Id.Should().Be(willBeCreatedPost.Id);
            returnedPostFromGet.Name.Should().Be(willBeCreatedPost.Name);
            returnedPostFromGet.Tags.Single().Name.Should().Be("integrationTag1");
        }

    }
}
