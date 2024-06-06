using Application.Contracts;
using Application.Exceptions;
using Application.Validation;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Posts.Commands
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;

        public DeleteCommentCommandHandler(IAsyncRepository<Comment> commentRepository, IMapper mapper)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
        }

        public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {

            var validator = new DeleteCommentCommandValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.Errors.Count > 0)
            {
                throw new ValidationException(validationResult);
            }
            var comment = await _commentRepository.GetByIdAsync(request.CommentId);
            if (comment is not null)
            {
                if(request.CreatorId == comment.CreatorId)
                {
                    comment.DeletedAt = DateTime.UtcNow;
                    await _commentRepository.UpdateAsync(comment);
                }

            }
            
        }
    }
}
