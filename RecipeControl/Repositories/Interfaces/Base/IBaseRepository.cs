using RecipeControl.Models.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Repositories.Interfaces.Base
{
    public interface IBaseRepository<T> where T : IBaseEntity
    {
        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch the entity. Ensure that the
        /// identifier provided is valid  and corresponds to an existing entity in the data source.</remarks>
        /// <param name="id">The unique identifier of the entity to retrieve. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
        /// name="T"/>  if found; otherwise, <see langword="null"/>.</returns>
        Task<T> GetByIdAsync(int id);
        
        /// <summary>
        /// Asynchronously retrieves all items of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>This method returns an enumerable collection of all items. The operation is performed
        /// asynchronously to avoid blocking the calling thread. The returned collection may be empty if no items are
        /// available.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of all items of type <typeparamref name="T"/>.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Asynchronously inserts the specified entity into the data store.
        /// </summary>
        /// <param name="entity">The entity to insert. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the inserted entity, 
        /// potentially with updated values (e.g., generated identifiers).</returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Updates the specified entity in the data store asynchronously.
        /// </summary>
        /// <remarks>Ensure that the entity being updated exists in the data store prior to calling this
        /// method. The operation may fail if the entity does not exist or if there are concurrency conflicts.</remarks>
        /// <param name="entity">The entity to update. The entity must not be null and must exist in the data store.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the update
        /// was successful; otherwise, <see langword="false"/>.</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Deletes the entity with the specified identifier asynchronously.
        /// </summary>
        /// <remarks>This method performs the deletion operation asynchronously. If the entity with the
        /// specified identifier does not exist, the method returns <see langword="false"/> without throwing an
        /// exception.</remarks>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
