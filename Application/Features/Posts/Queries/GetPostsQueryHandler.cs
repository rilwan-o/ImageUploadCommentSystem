using Application.Contracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using MediatR;
using Application.Exceptions;
using Application.Validation;

namespace Application.Features.Posts.Queries
{
    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, List<PostVm>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public GetPostsQueryHandler(IMapper mapper, IPostRepository postRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
        }
        public async Task<List<PostVm>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            var validator = new GetPostsQueryValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.Errors.Count > 0)
            {
                throw new ValidationException(validationResult);
            }
            // Get all posts with the last 2 comments eagerly loaded
            var allPosts = await _postRepository.GetAllWithLastTwoCommentsAsync(request.CreatorId, request.CreatedAtCursor, request.PageSize);
            if(allPosts == null)
            {
                throw new NotFoundException(nameof(request), request);
            }

            var postVms = _mapper.Map<List<PostVm>>(allPosts);

            return postVms;
        }

      
    }
}
