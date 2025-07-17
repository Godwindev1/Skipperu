using Microsoft.EntityFrameworkCore;
using Skipperu.Data.Requests;

namespace Skipperu.Data.Repositories
{
    public class RequestsRepo : IRequestsRepo
    {
        private readonly UserAuthenticationDBcontext _context;

        public RequestsRepo(UserAuthenticationDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(RequestDBstore request)
        {
            await _context.requests.AddAsync(request);
        }

        public async Task<RequestDBstore?> GetByIdAsync(int id)
        {
            return await _context.requests
                .Include(r => r.ParentFolder)
                .FirstOrDefaultAsync(r => r.RequestID == id);
        }

        public async Task<IEnumerable<RequestDBstore>> GetAllByFolderAsync(string folderId)
        {
            return await _context.requests
                .Where(r => r.ParentFolderID == folderId.ToString())
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestDBstore>> GetAllByUserAsync(string globalUserId)
        {
            return await _context.requests
                .Include(r => r.ParentFolder)
                .Where(r => r.ParentFolder.GlobalUserID == globalUserId.ToString())
                .ToListAsync();
        }

        public Task UpdateAsync(RequestDBstore request)
        {
            _context.requests.Update(request);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var request = await _context.requests.FindAsync(id);
            if (request != null)
            {
                _context.requests.Remove(request);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
