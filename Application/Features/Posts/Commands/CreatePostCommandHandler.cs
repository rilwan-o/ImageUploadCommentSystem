using Application.Contracts;
using Application.Exceptions;
using Application.Features.Posts.Queries;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using MediatR;

namespace Application.Features.Posts.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostVm>
    {
        private readonly IAsyncRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public CreatePostCommandHandler(
            IAsyncRepository<Post> postRepository, 
            IMapper mapper,
            IImageService imageService
            )
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _imageService = imageService;
        }

        public async Task<PostVm> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var post = _mapper.Map<Post>(request);

            var validator = new CreatePostCommandValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.Errors.Count > 0)
            {
                throw new ValidationException(validationResult);
            }

            var imageUrl = await _imageService.UploadImageAsync(request.Image);
            post.ImageUrl = imageUrl;
            post.CreatedAt = DateTime.Now;  
            post = await _postRepository.AddAsync(post);
            var postVm = _mapper.Map<PostVm>(post);
            return postVm;
        }
    }
}
