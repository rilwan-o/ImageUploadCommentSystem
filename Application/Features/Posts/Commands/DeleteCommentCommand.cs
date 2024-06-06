using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Posts.Commands
{
    public class DeleteCommentCommand : IRequest
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public int CreatorId { get; set; }
    }
}
