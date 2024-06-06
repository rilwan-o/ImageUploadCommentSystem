using Application.Features.Posts.Commands;
using Application.Features.Posts.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class GetPostsQueryValidator : AbstractValidator<GetPostsQuery>
    {
        public GetPostsQueryValidator()
        {
            RuleFor(x => x.CreatorId)
                .NotNull().WithMessage("{PropertyName} is required.")
                .GreaterThanOrEqualTo(1);


            RuleFor(x => x.PageSize)
                .NotNull().WithMessage("{PropertyName} is required.")
                .GreaterThanOrEqualTo(1);
        }
    }

}