using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public interface IRequestsRepo
    {
        Task AddAsync(RequestDBstore request);

        Task<RequestDBstore?> GetByIdAsync(string id);
        Task<IEnumerable<RequestDBstore>> GetAllByFolderAsync(string folderId);

        Task UpdateAsync(RequestDBstore request);

        Task DeleteAsync(string id);

        Task SaveAsync();
    }
}
