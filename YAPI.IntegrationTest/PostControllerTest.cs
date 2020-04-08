using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YAPI.Contracts.V1;
using YAPI.Contracts.V1.Requests;
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
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
        }


        [Fact]
        public async Task Get_ReturnsPost_WhenPostExists()
        {
            //Arange
            await AuthenticateAsync();
            var willBeCreatedPost = await CreatePostAsync(new CreatePostRequest { Name = "test" });

            //Act
            var response = await testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", willBeCreatedPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPostFromGet = await response.Content.ReadAsAsync<Post>();
            returnedPostFromGet.Id.Should().Be(willBeCreatedPost.Id);
         }

    }
}
