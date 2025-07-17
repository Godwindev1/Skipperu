using Microsoft.EntityFrameworkCore;

using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public interface ICollectionsRepo
    {
        Task AddAsync(Collection collection);

        Task<Collection?> GetByIdAsync(string id);
        Task<IEnumerable<Collection>> GetAllByUserAsync(string globalUserId);

        Task<IEnumerable<Collection>> GetAllByParentFolderAsync(string ParentID);

        Task UpdateAsync(Collection collection);

        Task DeleteAsync(string id);

        Task SaveAsync();
    }
}
