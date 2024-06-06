using Application.Features.Posts.Commands;
using FluentValidation;

namespace Application.Validation
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(x => x.Caption)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters");

            RuleFor(x => x.Image).NotEmpty()
                .WithMessage("{PropertyName} is required.");

            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

        }
    }
}
