using global::Skipperu.Data;
using global::Skipperu.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Skipperu.Data.Requests;


    namespace Skipperu.Data.Repositories
    {
        public class CollectionsRepo : ICollectionsRepo
        {
            private readonly UserAuthenticationDBcontext _context;

            public CollectionsRepo(UserAuthenticationDBcontext context)
            {
                _context = context;
            }

            public async Task AddAsync(Collection collection)
            {
                await _context.GlobalRequestCollection.AddAsync(collection);
            }

            public async Task<Collection?> GetByIdAsync(string id)
            {
                return await _context.GlobalRequestCollection
                    .Include(c => c.SavedRequests) 
                    .FirstOrDefaultAsync(c => c.FolderRootID == id.ToString());
            }

            public async Task<IEnumerable<Collection>> GetAllByUserAsync(string globalUserId)
            {
                return await _context.GlobalRequestCollection
                    .Where(c => c.GlobalUserID == globalUserId.ToString())
                    .Include(c => c.SavedRequests)
                    .ToListAsync();
            }

        public async Task<IEnumerable<Collection>> GetAllByParentFolderAsync(string ParentID)
        {
            return await _context.GlobalRequestCollection
                .Where(c => c.ParentFolderID == ParentID)
                .Include(c => c.SavedRequests)
                .ToListAsync();
        }

        public async Task<bool> DoesFolderExist(string GlobalUserID, string FolderPath)
        {
            return await _context.GlobalRequestCollection
                .Where(c => c.GlobalUserID == GlobalUserID && c.FolderPath == FolderPath)
                .AnyAsync();
        }


        public Task UpdateAsync(Collection collection)
            {
                _context.GlobalRequestCollection.Update(collection);
                return Task.CompletedTask;
            }

            public async Task DeleteAsync(string id)
            {
                var collection = await _context.GlobalRequestCollection.FindAsync(id);
                if (collection != null)
                {
                    _context.GlobalRequestCollection.Remove(collection);
                }
            }

            public async Task SaveAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }


