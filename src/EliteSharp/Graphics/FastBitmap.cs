// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EliteSharp.Graphics
{
    public class FastBitmap : IDisposable
    {
        private const int BitDepth = 32;
        private const int HeaderLength = 54;
        private readonly int _height;
        private readonly int _imageLength;
        private readonly int _width;
        private readonly int[] _pixels = [];
        private bool _isDisposed;
        private GCHandle _bitmapHandle;

        public FastBitmap(int width, int height)
        {
            _width = width;
            _height = height;

            _imageLength = width * height * BitDepth / 8;
            _pixels = new int[_imageLength];

            _bitmapHandle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
        }

        public FastBitmap(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            // Identifier
            Debug.Assert(bytes[0] == 'B', "Identifier is incorrect");
            Debug.Assert(bytes[1] == 'M', "Identifier is incorrect");

            // Width
            byte[] widthBytes = new byte[4];
            Array.Copy(bytes, 18, widthBytes, 0, 4);
            _width = BitConverter.ToInt32(widthBytes, 0);

            // Height
            byte[] heightBytes = new byte[4];
            Array.Copy(bytes, 22, heightBytes, 0, 4);
            _height = BitConverter.ToInt32(heightBytes, 0);

            // Bits Per Pixel
            byte[] bppBytes = new byte[2];
            Array.Copy(bytes, 28, bppBytes, 0, 2);
            Debug.Assert(BitConverter.ToInt32(bppBytes, 0) == BitDepth, "Bit Depth is incorrect");

            // File Size
            byte[] lengthBytes = new byte[4];
            Array.Copy(bytes, 2, lengthBytes, 0, 4);
            Debug.Assert(
                BitConverter.ToInt32(lengthBytes, 0) == HeaderLength + _imageLength,
                "File Size is incorrect");

            _pixels = new int[_imageLength];
            for (int i = 0; i < _pixels.Length; i++)
            {
                byte[] colorBytes = [4];
                Array.Copy(bytes, HeaderLength + (i * 4), colorBytes, 0, 4);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(colorBytes);
                }

                _pixels[i] = BitConverter.ToInt32(bytes, 0);
            }

            _bitmapHandle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~FastBitmap()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public nint BitmapHandle => _bitmapHandle.AddrOfPinnedObject();

        public void Clear() => Array.Clear(_pixels);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public FastColor GetPixel(int x, int y) => new(_pixels[x + (y * _width)]);

        public void SetPixel(int x, int y, in FastColor color) => _pixels[x + (y * _width)] = color.Argb;

        public void SetPixel(int x, int y, in int argb) => _pixels[x + (y * _width)] = argb;

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
}
