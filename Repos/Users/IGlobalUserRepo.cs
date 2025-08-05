using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;

namespace Skipperu.Repos.Users
{
    public interface IGlobalUserRepo
    {
        Task AddAsync(GlobalUser User);

        GlobalUser? GetByNormalizedUserEMail(string NormalizedUserEmail);

        Task UpdateAsync(GlobalUser User);

        Task DeleteAsync(string UserName);

        Task SaveAsync();
    }
}
