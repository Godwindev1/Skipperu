using Microsoft.EntityFrameworkCore;

using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public interface ICollectionsRepo
    {
        Task AddAsync(Collection collection);

        Task<Collection?> GetByIdAsync(Guid id);
        Task<IEnumerable<Collection>> GetAllByUserAsync(Guid globalUserId);

        Task UpdateAsync(Collection collection);

        Task DeleteAsync(Guid id);

        Task SaveAsync();
    }
}
