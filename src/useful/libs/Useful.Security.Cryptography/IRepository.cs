// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// A generic repository.
/// </summary>
/// <typeparam name="T">The type of items stored in the repository.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Gets the current item.
    /// </summary>
    public T? CurrentItem { get; }

    /// <summary>
    /// Sets the <see cref="CurrentItem" /> according to the match criteria.
    /// </summary>
    /// <param name="match">The criteria to find the current item.</param>
    public void SetCurrentItem(Func<T, bool> match);

    /// <summary>
    /// Adds a new item to the repository.
    /// </summary>
    /// <param name="item">The new item to add.</param>
    public void Create(T item);

    /// <summary>
    /// Retrieves all the items.
    /// </summary>
    /// <returns>All the items.</returns>
    public IEnumerable<T> Read();

    /// <summary>
    /// Updates an item in the repository.
    /// </summary>
    /// <param name="item">The item to update.</param>
    public void Update(T item);

    /// <summary>
    /// Removes an item from the repository.
    /// </summary>
    /// <param name="item">The item to delete.</param>
    public void Delete(T item);
}
