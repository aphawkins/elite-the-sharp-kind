// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EliteSharp.Graphics;

public class FastBitmap : IDisposable
{
    private readonly uint[] _pixels = []; // Must stay uint for memalloc
    private GCHandle _bitmapHandle;
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
        _bitmapHandle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
    }

    // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~FastBitmap()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public nint BitmapHandle => _bitmapHandle.AddrOfPinnedObject();

    public int Height { get; }

    public int Width { get; }

    public int BitsPerPixel { get; } = 32;

    public void Clear() => Array.Fill(_pixels, EliteColors.Black.Argb);

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public FastColor GetPixel(int x, int y) => new(_pixels[x + (y * Width)]);

    public void SetPixel(int x, int y, in FastColor color) => _pixels[x + (y * Width)] = color.Argb;

    public void SetPixel(int x, int y, in uint argb) => _pixels[x + (y * Width)] = argb;

    public FastBitmap Resize(int newWidth, int newHeight)
    {
        FastBitmap temp = new(newWidth, newHeight);
        for (int y = 0; y < newHeight; y++)
        {
            if (y > Height)
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
                    if (x > Width)
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
            _bitmapHandle.Free();
        }
    }
}
