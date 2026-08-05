// 'Useful Libraries' - Andy Hawkins 2023-2026.

using BenchmarkDotNet.Attributes;

namespace Useful.Graphics.Benchmarks;

// Measures what a face that lands wholly or partly outside the viewport
// costs the rasteriser, against the on-screen cases as a yardstick. Backs
// the "no frustum side-plane clipping" question: side planes are handled
// after projection, per scanline, by the span clamps in DrawTriangleFilled,
// so an off-screen face still walks its clamped Y range.
[JsonExporterAttribute.FullCompressed]
public class OffScreenTriangleBenchmarks : IDisposable
{
    private const int ScreenHeightPixels = 512;
    private const int ScreenWidthPixels = 512;

    private readonly SoftwareGraphics _graphics;
    private bool _isDisposed;

    public OffScreenTriangleBenchmarks()
        => _graphics = new(ScreenWidthPixels, ScreenHeightPixels, (_) => { }, [], []);

    // A face covering the whole viewport: the worst legitimate case.
    [Benchmark(Baseline = true)]
    public void FullScreen()
        => _graphics.DrawTriangleFilled(new(-256, -256), new(768, -256), new(256, 768), BaseColors.White);

    // Far off to the left, spanning the full height: every scanline is
    // evaluated, every span is clamped away to nothing.
    [Benchmark]
    public void OffScreenLeft()
        => _graphics.DrawTriangleFilled(new(-40000, -256), new(-30000, -256), new(-35000, 768), BaseColors.White);

    // Far above the viewport: the Y clamp alone rejects it.
    [Benchmark]
    public void OffScreenAbove()
        => _graphics.DrawTriangleFilled(new(-5000, -40000), new(5000, -40000), new(0, -30000), BaseColors.White);

    // Partly on screen, mostly off to the right - the common "large ship
    // close to the camera" shape.
    [Benchmark]
    public void StraddlingRight()
        => _graphics.DrawTriangleFilled(new(400, -8000), new(9000, -8000), new(4000, 8000), BaseColors.White);

    // A typical small on-screen face, for scale.
    [Benchmark]
    public void SmallOnScreen()
        => _graphics.DrawTriangleFilled(new(100, 100), new(140, 100), new(120, 140), BaseColors.White);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _graphics?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
