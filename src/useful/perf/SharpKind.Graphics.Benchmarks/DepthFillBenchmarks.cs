// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using BenchmarkDotNet.Attributes;
using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Benchmarks;

// Decomposes what a full-screen depth-tested face costs, one ingredient at a
// time. The docking frame is nothing but a handful of these, so the question
// "where does the time go when a station fills the view" is answered by the
// gaps between these numbers rather than by any one of them:
//
//   ClearDepth              the fixed per-frame cost, drawing nothing
//   FlatNoDepth             rasterise only - no depth buffer touched
//   Flat                    + the per-pixel depth test
//   FlatClipped             + the per-pixel clip test (clip narrowed)
//   Dithered                + a per-pixel IColourQuantiser call
//   Gouraud                 + per-pixel colour interpolation and quantise
//
// Everything covers the same pixels, so the differences are per-pixel costs
// and nothing else.
[JsonExporterAttribute.FullCompressed]
public class DepthFillBenchmarks : IDisposable
{
    private const int ScreenHeightPixels = 512;
    private const int ScreenWidthPixels = 512;

    // A quad covering the whole screen exactly, so every variant rasterises
    // the same 512x512 pixels.
    private static readonly Vector2[] s_fullScreen =
    [
        new(0, 0),
        new(ScreenWidthPixels, 0),
        new(ScreenWidthPixels, ScreenHeightPixels),
        new(0, ScreenHeightPixels),
    ];

    private static readonly float[] s_depths = [1000f, 1000f, 1000f, 1000f];

    private static readonly FastColor[] s_vertexColors =
    [
        BaseColors.White,
        BaseColors.Red,
        BaseColors.Green,
        BaseColors.Blue,
    ];

    private readonly SoftwareGraphics _graphics;
    private readonly IColourQuantiser _nearest = new ChannelGridQuantiser(4);
    private readonly IColourQuantiser _dither;
    private readonly IColourQuantiser _paletteDither;
    private bool _isDisposed;

    public DepthFillBenchmarks()
    {
        _graphics = new(
            ScreenWidthPixels,
            ScreenHeightPixels,
            (_) => { },
            [],
            FontRasteriserSet.Only(new BitmapFontRasteriser([])));
        _dither = new OrderedDitherQuantiser(_nearest);

        // What an indexed rendition uses: a nearest-entry search over the
        // whole palette, per pixel. Sixteen entries is the 8-bit tier's set.
        IColourQuantiser palette = new PaletteQuantiser(SixteenEntries());
        _paletteDither = new OrderedDitherQuantiser(palette);
        _graphics.ClearDepth();
    }

    // What RenderStart costs before a single pixel is drawn: two full-screen
    // arrays zeroed.
    [Benchmark]
    public void ClearDepth() => _graphics.ClearDepth();

    // The rasteriser's floor: same pixels, no depth buffer, no quantiser.
    [Benchmark(Baseline = true)]
    public void FlatNoDepth()
        => _graphics.DrawTriangleFilled(new(0, 0), new(1024, 0), new(0, 1024), BaseColors.White);

    [Benchmark]
    public void Flat() => _graphics.DrawPolygonFilledDepth(s_fullScreen, s_depths, BaseColors.White);

    // The same fill with the clip narrowed off full screen, which is what
    // Elite's viewport does for every frame of the universe. The clip costs
    // nothing per pixel - the fill clamps its scanlines and spans to the clip
    // rectangle once - so this should land at Flat scaled by the area the
    // narrower rectangle leaves, and a result above Flat means a per-pixel
    // test has crept back in.
    [Benchmark]
    public void FlatClipped()
    {
        _graphics.SetClipRegion(new(0, 0), ScreenWidthPixels, ScreenHeightPixels - 56);
        _graphics.DrawPolygonFilledDepth(s_fullScreen, s_depths, BaseColors.White);
        _graphics.SetClipRegion(new(0, 0), ScreenWidthPixels, ScreenHeightPixels);
    }

    [Benchmark]
    public void Dithered() => _graphics.DrawPolygonFilledDepth(s_fullScreen, s_depths, BaseColors.White, _dither);

    // The indexed path: the same dither, but its inner quantiser searches
    // sixteen palette entries for every pixel rather than rounding to a grid.
    [Benchmark]
    public void DitheredPalette()
        => _graphics.DrawPolygonFilledDepth(s_fullScreen, s_depths, BaseColors.White, _paletteDither);

    [Benchmark]
    public void Gouraud() => _graphics.DrawPolygonFilledDepth(s_fullScreen, s_depths, s_vertexColors, _nearest);

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

    // Sixteen distinct entries, as an indexed rendition's palette. The
    // values are not what is measured, only how many the search walks.
    private static FastColor[] SixteenEntries()
    {
        FastColor[] entries = new FastColor[16];
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = new(255, (byte)(i * 17), (byte)(255 - (i * 17)), (byte)(i * 9));
        }

        return entries;
    }
}
