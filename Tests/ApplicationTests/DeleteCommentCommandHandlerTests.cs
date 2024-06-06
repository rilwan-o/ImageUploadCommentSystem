using Application.Contracts;
using Application.Exceptions;
using Application.Features.Posts.Commands;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ApplicationTests
{
    public class DeleteCommentCommandHandlerTests
    {
        private readonly Mock<IAsyncRepository<Comment>> _commentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DeleteCommentCommandHandler _handler;

        public DeleteCommentCommandHandlerTests()
        {
            _commentRepositoryMock = new Mock<IAsyncRepository<Comment>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new DeleteCommentCommandHandler(_commentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldDeleteComment()
        {
            // Arrange
            var command = new DeleteCommentCommand { PostId = Guid.NewGuid(), CommentId = Guid.NewGuid(), CreatorId = 1 };
            var comment = new Comment { Id = command.CommentId, CreatorId = 1 };
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(command.CommentId)).ReturnsAsync(comment);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(DateTime.UtcNow.Date, comment.DeletedAt?.Date);
            _commentRepositoryMock.Verify(repo => repo.UpdateAsync(comment), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new DeleteCommentCommand { PostId = Guid.Empty, CommentId = Guid.Empty, CreatorId = 0 };
            var validator = new DeleteCommentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(validationResult.Errors.Count, exception.ValidationErrors.Count);
        }
    }

}
