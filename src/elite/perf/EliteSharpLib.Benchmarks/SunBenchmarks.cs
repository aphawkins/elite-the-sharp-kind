// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharp.Abstractions.Views.Suns;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Missions;
using EliteSharpLib.Suns;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Assets;
using Useful.Controls;
using Useful.Graphics;
using Useful.Graphics.Rendering;
using Useful.SDL;

namespace EliteSharpLib.Benchmarks;

[JsonExporterAttribute.FullCompressed]
public class SunBenchmarks : IDisposable
{
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
    private readonly SoftwareGraphics _graphics;
    private readonly Sun _solidSun;
    private readonly Sun _gradientSun;
    private bool _disposedValue;

    public SunBenchmarks()
    {
        IAssetLocator assetLocator = BenchmarkAssets.Locator();
        SoftwareKeyboard keyboard = new(new SDLInput());
        Useful.Abstraction.ScreenManager<Views.Screen, Views.IScreenController> views = new(keyboard);

        // These benchmarks are about planets and suns, so no missions are needed.
        GameState gameState = new(views, new MissionRegistry([], NullLogger<MissionRegistry>.Instance));
        _graphics = SoftwareGraphics.Create(ScreenWidth, ScreenHeight, (_) => { }, assetLocator);
        ZBufferRenderer shipRenderer = new(_graphics);
        RNG rng = new(Random.Shared);
        EliteDraw draw = new(gameState, _graphics, assetLocator, new SixteenBitRendition(), shipRenderer, rng);
        SixteenBitRendition rendition = new();
        _gradientSun = Sun(draw, rendition, rng, SunStyle.Gradient);
        _solidSun = Sun(draw, rendition, rng, SunStyle.Solid);
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

    // The renderer comes off the rendition now, so the benchmark builds one
    // the same way the game does.
    private static Sun Sun(EliteDraw draw, SixteenBitRendition rendition, RNG rng, SunStyle style)
        => new(draw, rendition.CreateSunRenderer(draw, new(style, rng)));
}
