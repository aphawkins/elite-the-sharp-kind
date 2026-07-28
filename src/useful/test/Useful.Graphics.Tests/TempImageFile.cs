// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Tests;

// The readers take a path rather than bytes, so the hand-built files the
// decoder tests use have to reach disk somehow.
internal sealed class TempImageFile : IDisposable
{
    private TempImageFile(string path) => Path = path;

    public string Path { get; }

    public static TempImageFile From(byte[] bytes)
    {
        string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid():N}.img");
        File.WriteAllBytes(path, bytes);
        return new(path);
    }

    public void Dispose() => File.Delete(Path);
}
