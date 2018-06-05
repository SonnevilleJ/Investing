using System;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Users;
using Sonneville.Investing.Users;

namespace Sonneville.Investing.Persistence.Users
{
    public interface IUserRepository : IRepository<User>
    {
        User FindByUserName(string userName);
        void CreateNewUser(User user);
        bool UserNameIsAvailable(string userName);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public UserRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
            _applicationUserRepository = dataContext.ApplicationUserRepository;
        }

        public User FindByUserName(string userName)
        {
            var applicationUser = _applicationUserRepository.FindByUserName(userName);
            return MapToDomain(applicationUser);
        }

        public void CreateNewUser(User user)
        {
            if (!UserNameIsAvailable(user.UserName))
                throw new InvalidOperationException($"Username {user.UserName} is already taken by another user!");
            _applicationUserRepository.Add(MapFromDomain(user));
        }

        public bool UserNameIsAvailable(string userName)
        {
            return !_applicationUserRepository.IsUserNameTaken(userName);
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

        public void Dispose()
        {
            _dataContext?.Dispose();
            _applicationUserRepository?.Dispose();
        }
    }
}
