using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.EFCore.Users
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser, long>
    {
        ApplicationUser FindByUserName(string userName);
        bool IsUserNameTaken(string userName);
    }

    public class ApplicationUserRepository : EntityFrameworkBaseRepository<ApplicationUser, long>, IApplicationUserRepository
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
