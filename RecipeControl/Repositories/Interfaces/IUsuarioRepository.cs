using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        /// <summary>
        /// Retrieves an entity of type <typeparamref name="Usuario"/> by its attribute Nombre.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch the entity. Ensure that the
        /// identifier provided is valid  and corresponds to an existing entity in the data source.</remarks>
        /// <param name="nombre">The Nombre attribute of the entity to retrieve. Must be a string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
        /// name="Usuario"/>  if found; otherwise, <see langword="null"/>.</returns>
        Task<Usuario> GetByNameAsync(string nombre);
    }
}
