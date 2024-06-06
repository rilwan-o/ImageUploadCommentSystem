using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Shouldly;
using System.Net.Http.Headers;

namespace Tests.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

       
        [Fact]
        public async Task CreatePost_ShouldReturnOkWithPost()
        {
            var command = new MultipartFormDataContent();
            command.Add(new StringContent("Test Caption"), "Caption");
            command.Add(new StringContent("1"), "CreatorId");
            // Assuming Image is an IFormFile, adjust accordingly
            command.Add(new ByteArrayContent(File.ReadAllBytes("IELTS.jpg")), "Image", "IELTS.jpg");

            var response = await _client.PostAsync("/api/posts", command);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var post = await response.Content.ReadAsStringAsync();
            post.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task AddCommentToPost_ShouldReturnOkWithCommentId()
        {
            var postId = Guid.Parse("57FF183B-E13A-46FD-C49C-08DC866B2D23"); //Guid.NewGuid();
            var command = new AddCommentCommand
            {
                 PostId = postId,
                Content = "Test Comment",
                CreatorId = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(command));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync($"/api/posts/{postId}/comments", content);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var commentId = await response.Content.ReadAsStringAsync();
            commentId.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteCommentFromPost_ShouldReturnNoContent()
        {
            var postId = Guid.Parse("57FF183B-E13A-46FD-C49C-08DC866B2D23");//Guid.NewGuid();
            var commentId = Guid.Parse("2C5F8658-608B-4DA4-E15B-08DC866C9085");//Guid.NewGuid();
            var creatorId = 1;


            var response = await _client.DeleteAsync($"/api/posts/{postId}/comments/{commentId}/userId/{creatorId}");

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetPosts_ShouldReturnOkWithPosts()
        {
            var query = new GetPostsQuery
            {
                CreatorId = 1,
                CreatedAtCursor = null,
                PageSize = 10
            };

            var content = new StringContent(JsonConvert.SerializeObject(query));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.GetAsync("/api/posts");

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var posts = await response.Content.ReadAsStringAsync();
            posts.ShouldNotBeNullOrEmpty();
        }
    }
}
