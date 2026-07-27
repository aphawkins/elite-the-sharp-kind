// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Useful.Graphics;

public class FastBitmap : IDisposable
{
    private readonly uint[] _pixels = []; // Must stay uint for memalloc
    private GCHandle _bitmapHandle;
    private bool _isPinned;
    private bool _isDisposed;

    public FastBitmap(int width, int height)
        : this(width, height, new uint[width * height])
    {
    }

    internal FastBitmap(int width, int height, uint[] pixels)
    {
        Width = width;
        Height = height;
        Debug.Assert(width * height == pixels.Length, "Array must be correct length");
        _pixels = pixels;
    }

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~FastBitmap()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    // Pinned lazily: most bitmaps (cached text glyphs, intermediates) never
    // cross into native code, so pinning only the handful that actually
    // call this avoids fragmenting the GC heap with permanently pinned
    // short-lived arrays.
    public nint BitmapHandle
    {
        get
        {
            if (!_isPinned)
            {
                _bitmapHandle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
                _isPinned = true;
            }

            return _bitmapHandle.AddrOfPinnedObject();
        }
    }

    public int Height { get; }

    public int Width { get; }

    public int BitsPerPixel { get; } = 32;

    public void Clear() => Array.Fill(_pixels, BaseColors.Black.Argb);

    public void Clear(in FastColor color) => Array.Fill(_pixels, color.Argb);

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public uint GetPixel(int x, int y) => _pixels[x + (y * Width)];

    public void SetPixel(int x, int y, in FastColor color) => _pixels[x + (y * Width)] = color.Argb;

    public void SetPixel(int x, int y, in uint argb) => _pixels[x + (y * Width)] = argb;

    public FastBitmap Resize(int newWidth, int newHeight)
    {
        FastBitmap temp = new(newWidth, newHeight);
        for (int y = 0; y < newHeight; y++)
        {
            if (y >= Height)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    temp.SetPixel(x, y, BaseColors.TransparentBlack);
                }
            }
            else
            {
                for (int x = 0; x < newWidth; x++)
                {
                    if (x >= Width)
                    {
                        temp.SetPixel(x, y, BaseColors.TransparentBlack);
                    }
                    else
                    {
                        temp.SetPixel(x, y, GetPixel(x, y));
                    }
                }
            }
        }

        return temp;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
            if (_isPinned)
            {
                _bitmapHandle.Free();
            }
        }
    }
}
