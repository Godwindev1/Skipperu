using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public interface IRequestsRepo
    {
        Task AddAsync(RequestDBstore request);

        Task<RequestDBstore?> GetByIdAsync(int id);
        Task<IEnumerable<RequestDBstore>> GetAllByFolderAsync(Guid folderId);
        Task<IEnumerable<RequestDBstore>> GetAllByUserAsync(Guid globalUserId);

        Task UpdateAsync(RequestDBstore request);

        Task DeleteAsync(Guid id);

        Task SaveAsync();
    }
}
