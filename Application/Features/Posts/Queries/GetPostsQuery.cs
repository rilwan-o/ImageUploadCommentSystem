using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Posts.Queries
{
    public class GetPostsQuery : IRequest<List<PostVm>>
    {
        public int CreatorId { get; set; }
        public DateTime? CreatedAtCursor { get; set; }
        public int PageSize { get; set; }   
    }
}
