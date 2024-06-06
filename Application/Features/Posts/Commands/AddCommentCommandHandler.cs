using Application.Contracts;
using Application.Exceptions;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Posts.Commands
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Guid>
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;

        public AddCommentCommandHandler(IAsyncRepository<Comment> commentRepository, IMapper mapper)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
        }

        public async Task<Guid> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var validator = new AddCommentCommandValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.Errors.Count > 0) 
            {
                throw new ValidationException(validationResult);
            }

            var comment = _mapper.Map<Comment>(request);

            comment = await _commentRepository.AddAsync(comment);
            return comment.Id;
        }
    }
}
