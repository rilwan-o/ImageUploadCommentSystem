using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Posts.Queries
{
    public class PostVm
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<Comment> Comments { get; set; } 
    }
}
