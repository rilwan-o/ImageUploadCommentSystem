using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Posts.Commands
{
    public class AddCommentCommand : IRequest<Guid>
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public int CreatorId { get; set; }
    }
}
