using Application.Contracts;
using Application.Exceptions;
using Application.Features.Posts.Queries;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Tests.ApplicationTests
{
    public class GetPostsQueryHandlerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPostsQueryHandler _handler;

        public GetPostsQueryHandlerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetPostsQueryHandler(_mapperMock.Object, _postRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnPosts()
        {
            // Arrange
            var query = new GetPostsQuery { CreatorId = 1, PageSize = 10 };
            var posts = new List<Post> { new Post { Id = Guid.NewGuid() } };
            var postVms = new List<PostVm> { new PostVm { Id = Guid.NewGuid() } };
            _postRepositoryMock.Setup(repo => repo.GetAllWithLastTwoCommentsAsync(query.CreatorId, query.CreatedAtCursor, query.PageSize)).ReturnsAsync(posts);
            _mapperMock.Setup(m => m.Map<List<PostVm>>(posts)).Returns(postVms);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(postVms, result);
        }

        [Fact]
        public async Task Handle_InvalidQuery_ShouldThrowValidationException()
        {
            // Arrange
            var query = new GetPostsQuery { CreatorId = 0, PageSize = 0 };
            var validator = new GetPostsQueryValidator();
            var validationResult = await validator.ValidateAsync(query);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal(validationResult.Errors.Count, exception.ValidationErrors.Count);
        }

        [Fact]
        public async Task Handle_NoPostsFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetPostsQuery { CreatorId = 1, PageSize = 10 };
            _postRepositoryMock.Setup(repo => repo.GetAllWithLastTwoCommentsAsync(query.CreatorId, query.CreatedAtCursor, query.PageSize)).ReturnsAsync((List<Post>)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
