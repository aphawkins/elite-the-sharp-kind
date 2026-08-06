// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharp.Abstractions.Views.Planets;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Missions;
using EliteSharpLib.Planets;
using Microsoft.Extensions.Logging.Abstractions;
using Useful;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Graphics;
using Useful.Graphics.Rendering;
using Useful.Input;

namespace EliteSharpLib.Benchmarks;

[JsonExporterAttribute.FullCompressed]
public class PlanetBenchmarks : IDisposable
{
    private const int ScreenWidth = 512;
    private const int ScreenHeight = 512;
    private readonly SoftwareGraphics _graphics;
    private readonly Planet _solidPlanet;
    private readonly Planet _wireframePlanet;
    private readonly Planet _fractalPlanet;
    private readonly Planet _stripedPlanet;
    private bool _disposedValue;

    public PlanetBenchmarks()
    {
        FakeInput input = new();
        IAssetLocator assetLocator = BenchmarkAssets.Locator();
        SoftwareKeyboard keyboard = new(input);
        Useful.Abstraction.ScreenManager<Views.Screen, Views.IScreenController> views = new(keyboard);

        // These benchmarks are about planets and suns, so no missions are needed.
        GameState gameState = new(views, new MissionRegistry([], NullLogger<MissionRegistry>.Instance));
        _graphics = SoftwareGraphics.Create(ScreenWidth, ScreenHeight, (_) => { }, assetLocator);
        ZBufferRenderer shipRenderer = new(_graphics);
        RNG rng = new(Random.Shared);
        EliteDraw draw = new(gameState, _graphics, assetLocator, new SixteenBitRendition(), shipRenderer, rng);
        SixteenBitRendition rendition = new();
        _wireframePlanet = Planet(draw, rendition, PlanetStyle.Wireframe);
        _solidPlanet = Planet(draw, rendition, PlanetStyle.Solid);
        _fractalPlanet = Planet(draw, rendition, PlanetStyle.Fractal);
        _stripedPlanet = Planet(draw, rendition, PlanetStyle.Striped);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Benchmark]
    public void SolidPlanet() => _solidPlanet.Draw();

    [Benchmark]
    public void WireframePlanet() => _wireframePlanet.Draw();

    [Benchmark]
    public void FractalPlanet() => _fractalPlanet.Draw();

    [Benchmark]
    public void StripedPlanet() => _stripedPlanet.Draw();

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
    private static Planet Planet(EliteDraw draw, SixteenBitRendition rendition, PlanetStyle style)
    {
        Random random = new(12345);
        PlanetLook look = new(style, false, new RandomSource(random));

        return new(draw, rendition.CreatePlanetRenderer(draw, look), style == PlanetStyle.Wireframe);
    }
}
