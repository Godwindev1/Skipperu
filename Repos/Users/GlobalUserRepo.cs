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
            await _context.GlobalUsers.AddAsync(User);
        }

        public GlobalUser? GetByUserName(string  UserName)
        {
            return _context.GlobalUsers.Where(x => x.UserName.Equals(UserName)).FirstOrDefault() ?? null;
        }

        public async Task UpdateAsync(GlobalUser user)
        {
            _context.GlobalUsers.Update(user);
            return ;
        }

        public async Task DeleteAsync(string UserName)
        {
            var User = _context.GlobalUsers.Where(x => x.UserName == UserName).FirstOrDefault();
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
