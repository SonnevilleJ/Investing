using System.Security.Authentication;
using Sonneville.Investing.Persistence.Users;
using Sonneville.Investing.Users;

namespace Sonneville.Investing.App.Authentication
{
    public class AuthenticationService
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IPasswordService passwordService, IUserRepository userRepository)
        {
            _passwordService = passwordService;
            _userRepository = userRepository;
        }

        public void CreateNewUser(User user, string password)
        {
            var passwordHash = _passwordService.HashPassword(password);

            _userRepository.CreateNewUser(user, passwordHash);
        }

        public User LogIn(string userName, string password)
        {
            if (_passwordService.ValidatePassword(password, _userRepository.FindPasswordByUserName(userName)))
            {
                return _userRepository.FindUserByUserName(userName);
            }

            throw new AuthenticationException("Incorrect username or password!");
        }
    }
}