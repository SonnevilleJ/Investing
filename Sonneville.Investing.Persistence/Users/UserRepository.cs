using System;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Security;
using Sonneville.Investing.Persistence.EFCore.Users;
using Sonneville.Investing.Users;
using Sonneville.Utilities.Security;

namespace Sonneville.Investing.Persistence.Users
{
    public interface IUserRepository : IRepository<User>
    {
        User FindUserByUserName(string userName);
        void CreateNewUser(User user, PasswordHash passwordHash);
        bool UserNameIsAvailable(string userName);
        PasswordHash FindPasswordByUserName(string userName);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IPasswordDigestRepository _passwordDigestRepository;

        public UserRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
            _applicationUserRepository = dataContext.ApplicationUserRepository;
            _passwordDigestRepository = dataContext.PasswordDigestRepository;
        }

        public User FindUserByUserName(string userName)
        {
            var applicationUser = _applicationUserRepository.FindByUserName(userName);
            return MapToDomain(applicationUser);
        }

        public void CreateNewUser(User user, PasswordHash passwordHash)
        {
            if (!UserNameIsAvailable(user.UserName))
                throw new InvalidOperationException($"Username {user.UserName} is already taken by another user!");

            var applicationUser = MapFromDomain(user);
            var passwordDigest = MapFromDomain(passwordHash);
            passwordDigest.User = applicationUser;

            _applicationUserRepository.Add(applicationUser);
            _passwordDigestRepository.Add(passwordDigest);
        }

        public bool UserNameIsAvailable(string userName)
        {
            return !_applicationUserRepository.IsUserNameTaken(userName);
        }

        public PasswordHash FindPasswordByUserName(string userName)
        {
            return MapToDomain(_passwordDigestRepository.FindByUserName(userName));
        }

        private static ApplicationUser MapFromDomain(User userAccount)
        {
            return new ApplicationUser
            {
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                UserName = userAccount.UserName,
            };
        }

        private PasswordDigest MapFromDomain(PasswordHash passwordHash)
        {
            return new PasswordDigest
            {
                Cryptor = passwordHash.CryptorName,
                Digest = passwordHash.HashDigest,
                HashingAlgorithm = passwordHash.HashAlgorithm.Name,
                Iterations = passwordHash.Iterations,
                SaltUsed = passwordHash.Salt,
            };
        }

        private static User MapToDomain(ApplicationUser applicationUser)
        {
            return applicationUser == null
                ? null
                : new User
                {
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    UserName = applicationUser.UserName,
                };
        }

        private static PasswordHash MapToDomain(PasswordDigest passwordDigest)
        {
            return new PasswordHash
            {
                CryptorName = passwordDigest.Cryptor,
                HashAlgorithm = HashAlgorithm.Parse(passwordDigest.HashingAlgorithm),
                HashDigest = passwordDigest.Digest,
                Iterations = passwordDigest.Iterations,
                Salt = passwordDigest.SaltUsed,
            };
        }

        public void Dispose()
        {
            _dataContext?.Dispose();
            _applicationUserRepository?.Dispose();
            _passwordDigestRepository?.Dispose();
        }
    }
}
