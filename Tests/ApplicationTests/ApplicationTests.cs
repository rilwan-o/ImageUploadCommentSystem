using Application.Contracts;
using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ApplicationTests
{
    public class ApplicationTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IAsyncRepository<Comment>> _mockCommentRepository;
        private readonly Mock<IImageService> _mockImageService;

        public ApplicationTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // AutoMapper configuration
                cfg.CreateMap<CreatePostCommand, Post>();
                cfg.CreateMap<AddCommentCommand, Comment>();
                cfg.CreateMap<Post, PostVm>();
            });
            _mapper = config.CreateMapper();

            _mockPostRepository = new Mock<IPostRepository>();
            _mockCommentRepository = new Mock<IAsyncRepository<Comment>>();
            _mockImageService = new Mock<IImageService>();
        }

        [Fact]
        public async Task Should_Create_Post_With_Image_And_Caption()
        {
            var command = new CreatePostCommand
            {
                Caption = "Test Post",
                Image = Mock.Of<IFormFile>(),
                CreatorId = 1
            };

            _mockImageService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("http://image.url");
            _mockPostRepository.Setup(x => x.AddAsync(It.IsAny<Post>())).ReturnsAsync(new Post { Id = Guid.NewGuid(), Caption = command.Caption, ImageUrl = "http://image.url", CreatorId = command.CreatorId });

            var handler = new CreatePostCommandHandler(_mockPostRepository.Object, _mapper, _mockImageService.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(command.Caption, result.Caption);
            Assert.Equal("http://image.url", result.ImageUrl);
        }

        [Fact]
        public async Task Should_Add_Comment_To_Post()
        {
            var command = new AddCommentCommand
            {
                PostId = Guid.NewGuid(),
                Content = "Test Comment",
                CreatorId = 1
            };

            _mockCommentRepository.Setup(x => x.AddAsync(It.IsAny<Comment>())).ReturnsAsync(new Comment { Id = Guid.NewGuid(), Content = command.Content, PostId = command.PostId, CreatorId = command.CreatorId });

            var handler = new AddCommentCommandHandler(_mockCommentRepository.Object, _mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task Should_Delete_Comment()
        {
            var commentId = Guid.NewGuid();
            var command = new DeleteCommentCommand
            {
                PostId = Guid.NewGuid(),
                CommentId = commentId,
                CreatorId = 1
            };

            _mockCommentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Comment { Id = commentId, CreatorId = command.CreatorId });
            _mockCommentRepository.Setup(x => x.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            var handler = new DeleteCommentCommandHandler(_mockCommentRepository.Object, _mapper);
            await handler.Handle(command, CancellationToken.None);

            _mockCommentRepository.Verify(x => x.UpdateAsync(It.Is<Comment>(c => c.Id == commentId && c.DeletedAt != null)), Times.Once);
        }

        [Fact]
        public async Task Should_Get_All_Posts_With_Last_Two_Comments()
        {
            var posts = new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                Caption = "Test Post 1",
                CreatorId = 1,
                Comments = new List<Comment>
                {
                    new Comment { Id = Guid.NewGuid(), Content = "Test Comment 1", CreatedAt = DateTime.UtcNow },
                    new Comment { Id = Guid.NewGuid(), Content = "Test Comment 2", CreatedAt = DateTime.UtcNow.AddMinutes(-1) }
                }
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Caption = "Test Post 2",
                CreatorId = 1,
                Comments = new List<Comment>
                {
                    new Comment { Id = Guid.NewGuid(), Content = "Test Comment 3", CreatedAt = DateTime.UtcNow },
                    new Comment { Id = Guid.NewGuid(), Content = "Test Comment 4", CreatedAt = DateTime.UtcNow.AddMinutes(-1) }
                }
            }
        };

            _mockPostRepository.Setup(x => x.GetAllWithLastTwoCommentsAsync(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<int>())).ReturnsAsync(posts);

            var handler = new GetPostsQueryHandler(_mapper, _mockPostRepository.Object);
            var result = await handler.Handle(new GetPostsQuery { CreatorId = 1, PageSize = 10 }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[0].Comments.Count);
            Assert.Equal(2, result[1].Comments.Count);
        }
    }
}
