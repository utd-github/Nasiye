using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nasiye.Services
{
    public interface IFirebaseAuthService
    {
        /// <summary>
        /// Login / Signup with email and password.
        /// </summary>
        /// <returns>OAuth token</returns>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        Task<string> LoginWithEmailPasswordAsync(string email, string password);
        Task<bool> LoginWithPhoneAsync(string phone);


        Task<string> CreateUserAsync(string email, string password);

        void Singout();

        /// <returns>User Object</returns>
        // Get Current User
        Task<string> GetCurrentUser();
        Task<string> GetUserPhone();
    }
}
