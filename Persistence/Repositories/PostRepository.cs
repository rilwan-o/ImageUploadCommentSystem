using Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class PostRepository: BaseRepository<Post>, IPostRepository 
    {
        public PostRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<List<Post>> GetAllWithLastTwoCommentsAsync(int creatorId, DateTime? createdAtCursor, int pageSize)
        {
            var query = _appDbContext.Posts
                .Where(x => x.CreatorId == creatorId)
                .Include(p => p.Comments.Where(c=>c.DeletedAt == null).OrderByDescending(c => c.CreatedAt).Take(2))
                .OrderByDescending(p => p.CreatedAt);

            if (createdAtCursor.HasValue)
            {
                query = (IOrderedQueryable<Post>)query.Where(p => p.CreatedAt < createdAtCursor.Value);
            }

            return await query.Take(pageSize).ToListAsync();
        }

    }
}
