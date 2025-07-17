using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public interface IRequestsRepo
    {
        Task AddAsync(RequestDBstore request);

        Task<RequestDBstore?> GetByIdAsync(int id);
        Task<IEnumerable<RequestDBstore>> GetAllByFolderAsync(string folderId);
        Task<IEnumerable<RequestDBstore>> GetAllByUserAsync(string globalUserId);

        Task UpdateAsync(RequestDBstore request);

        Task DeleteAsync(string id);

        Task SaveAsync();
    }
}
