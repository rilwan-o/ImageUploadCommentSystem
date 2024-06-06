using Application.Features.Posts.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Content)
           .NotEmpty().WithMessage("{PropertyName} is required.")
           .NotNull()
           .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters");

            RuleFor(x => x.PostId).NotEmpty()
                .WithMessage("{PropertyName} is required.");

            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
