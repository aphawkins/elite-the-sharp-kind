// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharpLib.Graphics;
using EliteSharpLib.Suns;
using Useful.Controls;
using Useful.Fakes.Assets;
using Useful.Graphics;
using Useful.SDL;

namespace EliteSharpLib.Benchmarks;

public class SunBenchmarks : IDisposable
{
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
    private readonly SoftwareGraphics _graphics;
    private readonly SolidSun _solidSun;
    private readonly GradientSun _gradientSun;
    private bool _disposedValue;

    public SunBenchmarks()
    {
        FakeAssetLocator assetLocator = new();
        SoftwareKeyboard keyboard = new(new SDLInput());
        Dictionary<Views.Screen, Views.IView> views = [];
        GameState gameState = new(keyboard, views);
        _graphics = SoftwareGraphics.Create(ScreenWidth, ScreenHeight, (_) => { }, assetLocator);
        EliteDraw draw = new(gameState, _graphics, assetLocator);
        _gradientSun = new(draw);
        _solidSun = new(draw);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Benchmark]
    public void SolidSun() => _solidSun.Draw();

    [Benchmark]
    public void GradientSun() => _gradientSun.Draw();

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
