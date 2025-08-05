using Skipperu.Data;
using Skipperu.Data.Users.data;

namespace Skipperu.Repos.Users
{
    public class GlobalUserRepo : IGlobalUserRepo
    {
        private readonly UserAuthenticationDBcontext _context;

        public GlobalUserRepo (UserAuthenticationDBcontext Contexts)
        {
            _context = Contexts;
        }

        public async Task AddAsync(GlobalUser User)
        {
            User.NormalizedUserEmail =  User.NormalizedUserEmail.ToUpperInvariant();
            await _context.GlobalUsers.AddAsync(User);
        }

        public GlobalUser? GetByNormalizedUserEMail(string NormalizedUserEmail)
        {
            return _context.GlobalUsers.Where(x => x.NormalizedUserEmail.Equals(NormalizedUserEmail)).FirstOrDefault() ?? null;
        }

        public async Task UpdateAsync(GlobalUser user)
        {
            user.NormalizedUserEmail = user.NormalizedUserEmail.ToUpperInvariant();

            _context.GlobalUsers.Update(user);
            return ;
        }

        public async Task DeleteAsync(string NormalizedUserEmail)
        {
            var User = _context.GlobalUsers.Where(x => x.NormalizedUserEmail == NormalizedUserEmail).FirstOrDefault();
            if (User != null)
            {
                _context.GlobalUsers.Remove(User);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
