// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Security.Cryptography;

/// <summary>
/// Holds all the ciphers.
/// </summary>
/// <remarks>
/// Ciphers are keyed on <see cref="ICipher.CipherName"/>, so the repository holds one entry per
/// kind of cipher rather than one per configured instance. Adding two ciphers of the same kind
/// leaves <see cref="Delete(ICipher)"/> unable to tell them apart.
/// </remarks>
public sealed class CipherRepository : IRepository<ICipher>
{
    private readonly List<ICipher> _ciphers = [];

    /// <summary>
    /// Gets or sets the current cipher.
    /// </summary>
    public ICipher? CurrentItem { get; set; }

    /// <summary>
    /// Adds a new cipher to the repository.
    /// </summary>
    /// <param name="item">The new cipher to add.</param>
    public void Create(ICipher item) => _ciphers.Add(item);

    /// <summary>
    /// Removes a cipher from the repository.
    /// </summary>
    /// <param name="item">The cipher to delete.</param>
    public void Delete(ICipher item)
    {
        ArgumentNullException.ThrowIfNull(item);

        int removeAt = _ciphers.FindIndex(x => x.CipherName == item.CipherName);

        if (removeAt >= 0)
        {
            _ciphers.RemoveAt(removeAt);
        }
    }

    /// <summary>
    /// Retrieves all the ciphers.
    /// </summary>
    /// <returns>All the ciphers.</returns>
    public IEnumerable<ICipher> Read() => _ciphers;

    /// <summary>
    /// Sets the <see cref="CurrentItem" /> according to the match criteria.
    /// </summary>
    /// <param name="match">The criteria to find the current cipher.</param>
    /// <remarks><see cref="CurrentItem"/> is left unchanged if nothing matches.</remarks>
    public void SetCurrentItem(Func<ICipher, bool> match)
    {
        ArgumentNullException.ThrowIfNull(match);

        CurrentItem = _ciphers.FirstOrDefault(match) ?? CurrentItem;
    }

    /// <summary>
    /// Updates a cipher in the repository.
    /// </summary>
    /// <param name="item">The cipher to update.</param>
    public void Update(ICipher item)
    {
        ArgumentNullException.ThrowIfNull(item);

        for (int i = 0; i < _ciphers.Count; i++)
        {
            if (_ciphers[i].CipherName == item.CipherName)
            {
                _ciphers[i] = item;
            }
        }
    }
}
