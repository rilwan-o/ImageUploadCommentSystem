using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostCommand command)
        {
            try
            {
                var post = await _mediator.Send(command);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
           
        }

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> AddComment(Guid postId, [FromBody] AddCommentCommand command)
        {
            try
            {
                command.PostId = postId;
                var commentId = await _mediator.Send(command);
                return Ok(commentId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }

        [HttpDelete("{postId}/comments/{commentId}/userId/{userId}")]
        public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId, int userId)
        {
            try
            {
                DeleteCommentCommand command = new DeleteCommentCommand
                {
                    PostId = postId,
                    CommentId = commentId,
                    CreatorId = userId
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] GetPostsQuery query)
        {
            try
            {
                var posts = await _mediator.Send(query);
                return Ok(posts);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
