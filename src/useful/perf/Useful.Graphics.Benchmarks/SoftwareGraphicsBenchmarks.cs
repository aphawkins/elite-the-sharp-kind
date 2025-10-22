// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
using BenchmarkDotNet.Attributes;
using Useful.Assets;

namespace Useful.Graphics.Benchmarks;

public class SoftwareGraphicsBenchmarks : IDisposable
{
    private const int ScreenHeightPixels = 512;
    private const int ScreenWidthPixels = 512;

    private readonly FastBitmap _fontBitmap;
    private readonly SoftwareGraphics _graphics;
    private bool _isDisposed;

    public SoftwareGraphicsBenchmarks()
    {
        _graphics = new(ScreenWidthPixels, ScreenHeightPixels, (_) => { });
        _graphics.Initialize(new DummyAssetLocator(), [BaseColors.White]);
        _graphics.Images = new() { { 123, new FastBitmap(16, 16) } };

        _fontBitmap = new(8, 8);
        _graphics.Fonts = new() { { 0, new BitmapFont(_fontBitmap) } };
    }

    [Benchmark]
    public void Clear() => _graphics.Clear();

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Benchmark]
    public void DrawCircle() => _graphics.DrawCircle(new(255, 255), 100, BaseColors.White);

    [Benchmark]
    public void DrawCircleFilled() => _graphics.DrawCircleFilled(new(255, 255), 100, BaseColors.White);

    [Benchmark]
    public void DrawImage() => _graphics.DrawImage(123, new(1, 1));

    [Benchmark]
    public void DrawImageCentre()
            => _graphics.DrawImageCentre(123, 10f);

    [Benchmark]
    public void DrawLine() => _graphics.DrawLine(new(0, 0), new(512, 512), BaseColors.White);

    [Benchmark]
    public void DrawPixel() => _graphics.DrawPixel(new(255, 255), BaseColors.White);

    [Benchmark]
    public void DrawPolygon()
    {
        Vector2[] points = [new(0, 0), new(10, 0), new(10, 10), new(0, 10)];
        _graphics.DrawPolygon(points, BaseColors.White);
    }

    ////[Benchmark]
    ////public void DrawPolygonFilled()
    ////{
    ////    Vector2[] tri = [new(1, 1), new(3, 1), new(2, 3)];
    ////    _graphics.DrawPolygonFilled(tri, BaseColors.White);
    ////}

    ////// Rectangle functions
    ////[Benchmark]
    ////public void DrawRectangle() => _graphics.DrawRectangle(new(1, 1), 10, 6, BaseColors.White);

    ////[Benchmark]
    ////public void DrawRectangleCentre() => _graphics.DrawRectangleCentre(10f, 10, 6, BaseColors.White);

    ////[Benchmark]
    ////public void DrawRectangleFilled() => _graphics.DrawRectangleFilled(new(1, 1), 10, 6, BaseColors.White);

    ////// Text functions - use whitespace where appropriate to exercise early-return paths quickly
    ////[Benchmark]
    ////public void DrawTextCentreWhitespace() => _graphics.DrawTextCentre(0f, "   ", 0, BaseColors.White);

    ////[Benchmark]
    ////public void DrawTextLeftWhitespace() => _graphics.DrawTextLeft(new(0, 0), " ", 0, BaseColors.White);

    ////[Benchmark]
    ////public void DrawTextRightWhitespace() => _graphics.DrawTextRight(new(0, 0), string.Empty, 0, BaseColors.White);

    ////// Triangle functions
    ////[Benchmark]
    ////public void DrawTriangle()
    ////{
    ////    Vector2 a = new(1, 1);
    ////    Vector2 b = new(3, 1);
    ////    Vector2 c = new(2, 3);
    ////    _graphics.DrawTriangle(a, b, c, BaseColors.White);
    ////}

    ////[Benchmark]
    ////public void DrawTriangleFilled()
    ////{
    ////    Vector2 a = new(1, 1);
    ////    Vector2 b = new(3, 1);
    ////    Vector2 c = new(2, 3);
    ////    _graphics.DrawTriangleFilled(a, b, c, BaseColors.White);
    ////}

    ////[Benchmark]
    ////public void Initialize() => _graphics.Initialize(new DummyAssetLocator(), [BaseColors.White]);

    ////[Benchmark]
    ////public bool IsInitialized() => _graphics.IsInitialized;

    ////[Benchmark]
    ////public float Scale() => _graphics.Scale;

    ////[Benchmark]
    ////public float ScreenHeight() => _graphics.ScreenHeight;

    ////// Screen update and clip region
    ////[Benchmark]
    ////public void ScreenUpdate() => _graphics.ScreenUpdate();

    ////// Property getters
    ////[Benchmark]
    ////public float ScreenWidth() => _graphics.ScreenWidth;

    ////[Benchmark]
    ////public void SetClipRegion() => _graphics.SetClipRegion(new Vector2(1, 1), 10, 10);

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _graphics?.Dispose();
                _fontBitmap?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _isDisposed = true;
        }
    }

    // Minimal IAssetLocator implementation for initialize benchmark.
    private sealed class DummyAssetLocator : IAssetLocator
    {
        public IDictionary<int, string> FontBitmapPaths { get; } = new Dictionary<int, string>();

        public IDictionary<int, string> FontTrueTypePaths { get; } = new Dictionary<int, string>();

        public IDictionary<int, string> ImagePaths { get; } = new Dictionary<int, string>();

        public bool IsInitialized { get; private set; }

        public IDictionary<int, string> MusicPaths { get; } = new Dictionary<int, string>();

        public IDictionary<int, string> SfxPaths { get; } = new Dictionary<int, string>();

        public void Initialize() => IsInitialized = true;
    }
}
