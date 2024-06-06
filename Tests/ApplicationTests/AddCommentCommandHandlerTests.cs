using Application.Contracts;
using Application.Exceptions;
using Application.Features.Posts.Commands;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using Moq;


namespace Tests.ApplicationTests
{
    public class AddCommentCommandHandlerTests
    {
        private readonly Mock<IAsyncRepository<Comment>> _commentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AddCommentCommandHandler _handler;

        public AddCommentCommandHandlerTests()
        {
            _commentRepositoryMock = new Mock<IAsyncRepository<Comment>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new AddCommentCommandHandler(_commentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldAddComment()
        {
            // Arrange
            var command = new AddCommentCommand { PostId = Guid.NewGuid(), Content = "Test Comment", CreatorId = 1 };
            var comment = new Comment { Id = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<Comment>(command)).Returns(comment);
            _commentRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Comment>())).ReturnsAsync(comment);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(comment.Id, result);
            _commentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new AddCommentCommand { PostId = Guid.Empty, Content = "", CreatorId = 0 };
            var validator = new AddCommentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(validationResult.Errors.Count, exception.ValidationErrors.Count);
        }
    }
}
