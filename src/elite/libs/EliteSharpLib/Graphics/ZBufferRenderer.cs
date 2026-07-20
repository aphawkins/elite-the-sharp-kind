// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful.Assets;
using Useful.Assets.Palettes;
using Useful.Graphics;

namespace EliteSharpLib.Graphics;

// The 2026-07-14 z-buffer spike's behaviour (per-pixel depth test via
// Graphics.DrawPolygonFilledDepth), plus the still-unsplit ShipWireframe
// branch (the wireframe/filled item is separate). The face-root
// decal-inheritance z key (FindFaceRoots/FaceMeanZ) that makes the
// per-pixel test tie correctly for decals stays in ShipBase — it's
// submitted through IShipRenderer.SubmitFace's z parameter the same way
// for every implementation, painter's included, so it isn't
// z-buffer-specific despite fixing a z-buffer-only defect (the open
// decal-seam issue, see CHANGELOG).
internal sealed class ZBufferRenderer : IShipRenderer
{
    private const int MAXPOLYS = 100;
    private readonly uint _colorWhite;
    private readonly GameState _gameState;
    private readonly IGraphics _graphics;
    private readonly PolygonData[] _polyChain = new PolygonData[MAXPOLYS];
    private int _startPoly;
    private int _totalPolys;

    internal ZBufferRenderer(GameState gameState, IGraphics graphics, IAssetLocator assetLocator)
    {
        _gameState = gameState;
        _graphics = graphics;
        _colorWhite = PaletteReader.Read(assetLocator.PalettePath)["White"];
    }

    public void SubmitFace(Vector2[] points, uint faceColor, float z)
    {
        int i;

        if (_totalPolys == MAXPOLYS)
        {
            return;
        }

        int x = _totalPolys;
        _totalPolys++;

        _polyChain[x].FaceColor = faceColor;
        _polyChain[x].Z = z;
        _polyChain[x].Next = -1;
        _polyChain[x].PointList = new Vector2[points.Length];
        _polyChain[x].Depths = new float[points.Length];

        for (i = 0; i < points.Length; i++)
        {
            _polyChain[x].PointList[i] = points[i];
            _polyChain[x].Depths[i] = z;
        }

        if (x == 0)
        {
            return;
        }

        if (z > _polyChain[_startPoly].Z)
        {
            _polyChain[x].Next = _startPoly;
            _startPoly = x;
            return;
        }

        for (i = _startPoly; _polyChain[i].Next != -1; i = _polyChain[i].Next)
        {
            int nx = _polyChain[i].Next;

            if (z > _polyChain[nx].Z)
            {
                _polyChain[i].Next = x;
                _polyChain[x].Next = nx;
                return;
            }
        }

        _polyChain[i].Next = x;
    }

    public void StartFrame()
    {
        _startPoly = 0;
        _totalPolys = 0;

        if (!_gameState.Config.ShipWireframe)
        {
            _graphics.ClearDepth();
        }
    }

    public void EndFrame()
    {
        if (_totalPolys == 0)
        {
            return;
        }

        for (int i = _startPoly; i != -1; i = _polyChain[i].Next)
        {
            uint color = _gameState.Config.ShipWireframe ? _colorWhite : _polyChain[i].FaceColor;

            if (_polyChain[i].PointList.Length == 2)
            {
                _graphics.DrawLine(_polyChain[i].PointList[0], _polyChain[i].PointList[1], color);
                continue;
            }

            if (_gameState.Config.ShipWireframe)
            {
                _graphics.DrawPolygon(_polyChain[i].PointList, color);
            }
            else
            {
                _graphics.DrawPolygonFilledDepth(_polyChain[i].PointList, _polyChain[i].Depths, color);
            }
        }
    }
}
