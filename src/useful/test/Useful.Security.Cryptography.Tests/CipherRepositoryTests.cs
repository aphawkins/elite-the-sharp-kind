// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Moq;
using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class CipherRepositoryTests
{
    private readonly Mock<ICipher> _moqCipher;

    public CipherRepositoryTests()
    {
        _moqCipher = new Mock<ICipher>();
        _moqCipher.Setup(x => x.CipherName).Returns("MoqCipherName");
    }

    [Fact]
    public void RepositoryCreate()
    {
        CipherRepository repository = new();
        int count = repository.Read().Count();
        repository.Create(_moqCipher.Object);
        Assert.Equal(count + 1, repository.Read().Count());
    }

    [Fact]
    public void RepositoryRead()
    {
        CipherRepository repository = new();
        Assert.Empty(repository.Read());
    }

    [Fact]
    public void RepositoryUpdate()
    {
        CipherRepository repository = new();
        int count = repository.Read().Count();
        repository.Update(_moqCipher.Object);
        Assert.Equal(count, repository.Read().Count());
    }

    [Fact]
    public void RepositoryDelete()
    {
        CipherRepository repository = new();
        repository.Create(_moqCipher.Object);
        int count = repository.Read().Count();
        repository.Delete(repository.Read().ToList()[0]);
        Assert.Equal(count - 1, repository.Read().Count());
    }

    [Fact]
    public void RepositorySetCurrentItem()
    {
        CipherRepository repository = new();
        repository.Create(_moqCipher.Object);
        repository.SetCurrentItem(x => x.CipherName == "MoqCipherName");
        Assert.Equal(repository.CurrentItem, _moqCipher.Object);
    }
}
