using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.EFCore.Security
{
    public interface IPasswordDigestRepository : IEntityFrameworkRepository<PasswordDigest, long>
    {
        PasswordDigest FindByUserName(string userName);
    }

    public class PasswordDigestRepository : BaseEntityFrameworkRepository<PasswordDigest, long>, IPasswordDigestRepository
    {
        public PasswordDigestRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public PasswordDigest FindByUserName(string userName)
        {
            return DbSet.SingleOrDefault(digest => digest.User.UserName == userName);
        }
    }
}
