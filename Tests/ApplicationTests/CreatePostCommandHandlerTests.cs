using Application.Contracts;
using Application.Exceptions;
using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.ApplicationTests
{
    public class CreatePostCommandHandlerTests
    {
        private readonly Mock<IAsyncRepository<Post>> _postRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly CreatePostCommandHandler _handler;

        public CreatePostCommandHandlerTests()
        {
            _postRepositoryMock = new Mock<IAsyncRepository<Post>>();
            _mapperMock = new Mock<IMapper>();
            _imageServiceMock = new Mock<IImageService>();
            _handler = new CreatePostCommandHandler(_postRepositoryMock.Object, _mapperMock.Object, _imageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreatePost()
        {
            // Arrange
            var command = new CreatePostCommand { Caption = "Test Post", Image = Mock.Of<IFormFile>(), CreatorId = 1 };
            var post = new Post { Id = Guid.NewGuid(), CreatedAt = DateTime.Now };
            var postVm = new PostVm();
            _mapperMock.Setup(m => m.Map<Post>(command)).Returns(post);
            _imageServiceMock.Setup(service => service.UploadImageAsync(command.Image)).ReturnsAsync("http://image.url");
            _postRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Post>())).ReturnsAsync(post);
            _mapperMock.Setup(m => m.Map<PostVm>(post)).Returns(postVm);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(postVm, result);
            _postRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Post>()), Times.Once);
            _imageServiceMock.Verify(service => service.UploadImageAsync(command.Image), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ShouldThrowValidationException()
        {
            // Arrange
            var command = new CreatePostCommand { Caption = "", Image = null, CreatorId = 0 };
            var validator = new CreatePostCommandValidator();
            var validationResult = await validator.ValidateAsync(command);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal(validationResult.Errors.Count, exception.ValidationErrors.Count);
        }
    }
}
