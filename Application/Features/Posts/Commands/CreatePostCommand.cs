using Application.Features.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Posts.Commands
{
    public class CreatePostCommand : IRequest<PostVm>
    {
        public string Caption { get; set; }
        public IFormFile Image { get; set; }
        public int CreatorId { get; set; }
    }
}
