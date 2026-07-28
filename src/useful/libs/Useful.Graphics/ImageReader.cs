// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics;

// Single entry point for loading image assets. The format is taken from the
// file's own magic bytes rather than its extension, so a mislabelled asset
// still loads.
public static class ImageReader
{
    public static FastBitmap Read(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);

        return bytes.Length == 0 ? new(0, 0)
            : PngReader.IsPng(bytes) ? PngReader.Decode(bytes)
            : BitmapReader.IsBmp(bytes) ? BitmapReader.Decode(bytes)
            : throw new UsefulException($"Unrecognised image format: {path}");
    }
}
