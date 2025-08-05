
using global::Skipperu.Data;
using global::Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;


namespace Skipperu.Repos.Users
{
    public class GoogleUserRepo : IGoogleUserRepo
    {
        private readonly UserAuthenticationDBcontext _context;

        public GoogleUserRepo(UserAuthenticationDBcontext Contexts)
        {
            _context = Contexts;
        }

        public async Task CreateAsync(GoogleUser User)
        {
            User.NormalizedEmail = User.UserEmail.ToUpperInvariant();
            await _context.GoogleUsers.AddAsync(User);
        }

        public GoogleUser? GetByNormalizedUserEMail(string NormalizedUserEmail)
        {
            return _context.GoogleUsers.Where(x => x.NormalizedEmail.Equals(NormalizedUserEmail)).FirstOrDefault() ?? null;
        }

        public GoogleUser? GetByNormalizedUserName(string NormalizedUserName)
        {
            return _context.GoogleUsers.Where(x => x.NormalizedUserName.Equals(NormalizedUserName)).FirstOrDefault() ?? null;
        }

        public async Task UpdateAsync(GoogleUser user)
        {
            user.NormalizedEmail = user.UserEmail.ToUpperInvariant();

            _context.GoogleUsers.Update(user);
                return;
        }

        public async Task DeleteAsync(string NormalizedUserEmail)
        {
            var User = _context.GoogleUsers.Where(x => x.NormalizedEmail == NormalizedUserEmail).FirstOrDefault();
            if (User != null)
            {
                _context.GoogleUsers.Remove(User);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


