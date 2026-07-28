// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful;
using Useful.Assets;
using Useful.Graphics;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Graphics;

// Picks the ship rendering strategy from the live config once per frame, so
// changing the setting in the Settings view takes effect on the next frame
// rather than at the next restart. The strategy is fixed for the duration of
// a frame so Submit/EndFrame always reach the renderer that started it.
internal sealed class ConfigPolygonRenderer : IPolygonRenderer
{
    private readonly GameState _gameState;
    private readonly IPolygonRenderer _wireframe;
    private readonly IPolygonRenderer _painter;
    private readonly IPolygonRenderer _zBuffer;
    private IPolygonRenderer _current;

    internal ConfigPolygonRenderer(GameState gameState, IGraphics graphics, IAssetLocator assetLocator)
    {
        _gameState = gameState;
        _wireframe = new WireframeRenderer(graphics, assetLocator);
        _painter = new PainterRenderer(graphics);
        _zBuffer = new ZBufferRenderer(graphics);
        _current = Selected;
    }

    private IPolygonRenderer Selected => _gameState.Config.ShipWireframe
        ? _wireframe
        : _gameState.Config.ShipRenderMode == PolygonRenderMode.Painter
            ? _painter
            : _zBuffer;

    public void Submit(Vector2[] points, FastColor color, float z) => _current.Submit(points, color, z);

    public void StartFrame()
    {
        _current = Selected;
        _current.StartFrame();
    }

    public void EndFrame() => _current.EndFrame();
}
