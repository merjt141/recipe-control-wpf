using RecipeControl.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Credential
{
    /// <summary>
    /// Defines methods for managing user authentication, including logging in and logging off.
    /// </summary>
    /// <remarks>This interface provides asynchronous methods for handling user credentials.  Implementations
    /// of this interface should ensure secure handling of sensitive data,  such as passwords, and may include
    /// additional authentication mechanisms as needed.</remarks>
    public interface ICredentialService
    {
        /// <summary>
        /// Attempts to log in a user with the specified credentials.
        /// </summary>
        /// <param name="name">The username of the user attempting to log in. Cannot be null or empty.</param>
        /// <param name="password">The password associated with the specified username. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the login is
        /// successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> LoginAsync(string name, string password);
        /// <summary>
        /// Logs off the current user from the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the logoff
        /// operation was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> Logoff();

        /// <summary>
        /// Retrieves the currently authenticated user.
        /// </summary>
        /// <returns>An instance of <see cref="Usuario"/> representing the currently authenticated user. Returns <see
        /// langword="null"/> if no user is authenticated.</returns>
        Usuario GetCurrentUser();
    }
}
