using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.EFCore.Users
{
    public interface IApplicationUserRepository : IEntityFrameworkRepository<ApplicationUser, long>
    {
        ApplicationUser FindByUserName(string userName);
        bool IsUserNameTaken(string userName);
    }

    public class ApplicationUserRepository : BaseEntityFrameworkRepository<ApplicationUser, long>, IApplicationUserRepository
    {
        public ApplicationUserRepository(DbContext dataContext) : base(dataContext)
        {
        }

        public ApplicationUser FindByUserName(string userName)
        {
            return DbSet.SingleOrDefault(user => user.UserName == userName);
        }

        public bool IsUserNameTaken(string userName)
        {
            return DbSet.Any(user => user.UserName == userName);
        }
    }
}
