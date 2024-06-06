using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
