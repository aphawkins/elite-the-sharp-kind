// 'Useful Libraries' - Andy Hawkins 2025.

using BenchmarkDotNet.Attributes;

namespace Useful.Graphics.Benchmarks;

public class SoftwareGraphicsBenchmarks : IDisposable
{
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
    private readonly SoftwareGraphics _graphics;
    private bool _disposedValue;

    public SoftwareGraphicsBenchmarks()
        => _graphics = new(ScreenWidth, ScreenHeight, (_) => { });

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Benchmark]
    public void Clear() => _graphics.Clear();

    [Benchmark]
    public void DrawCircle() => _graphics.DrawCircle(new(255, 255), 100, BaseColors.White);

    [Benchmark]
    public void DrawCircleFilled() => _graphics.DrawCircleFilled(new(255, 255), 100, BaseColors.White);

    [Benchmark]
    public void DrawLine() => _graphics.DrawLine(new(0, 0), new(511, 511), BaseColors.White);

    [Benchmark]
    public void DrawPixel() => _graphics.DrawPixel(new(255, 255), BaseColors.White);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                _graphics.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _disposedValue = true;
        }
    }
}
