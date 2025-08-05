using Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;

namespace Skipperu.Repos.Users
{
    
        public interface IGoogleUserRepo
        {
            Task CreateAsync(GoogleUser User);

            GoogleUser? GetByNormalizedUserEMail(string NormalizedUserEmail);
            GoogleUser? GetByNormalizedUserName(string NormalizedUserEmail);

            Task UpdateAsync(GoogleUser User);

            Task DeleteAsync(string UserName);

            Task SaveAsync();
        }
    
}
