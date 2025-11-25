using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeControl.Services.Credential
{
    public class CredentialService : ICredentialService
    {
        public readonly IUsuarioRepository _usuarioRepository;
        private Usuario _currentUser = new Usuario();

        public CredentialService(
            IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> LoginAsync(string name, string password)
        {
            // Retrieve user by name
            Usuario loginUser = await _usuarioRepository.GetByNameAsync(name);

            // Validate user existence
            if (loginUser.UsuarioId == 0)
            {
                MessageBox.Show("No existe el usuario especificado.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validate password
            if (loginUser.ClaveHash != password)
            {
                MessageBox.Show("Contraseña incorrecta", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Set the current user upon successful login
            MessageBox.Show("Login exitoso", "Información", MessageBoxButton.OK);
            _currentUser = loginUser;
            return true;
        }

        public Task<bool> Logoff()
        {
            // Clear the current user
            _currentUser = new Usuario();
            return Task.FromResult(true);
        }

        public Usuario GetCurrentUser()
        {
            return _currentUser;
        }
    }
}
