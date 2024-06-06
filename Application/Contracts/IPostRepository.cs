using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IPostRepository : IAsyncRepository<Post>
    {
        Task<List<Post>> GetAllWithLastTwoCommentsAsync(int creatorId, DateTime? createdAtCursor, int pageSize);
    }
}
