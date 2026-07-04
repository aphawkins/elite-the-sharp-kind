// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerLib.Rendering;

// Draws the track as flat-shaded polygons: one road surface and two side
// surfaces per segment (as the original CreateUpdatePieceInVBMode1), using
// a painter's sort instead of the Direct3D z-buffer. Road line textures
// are not drawn yet.
public sealed class TrackRenderer
{
    private const int MaxPolygonPoints = 5; // quad plus one clip point

    private readonly Track _track;

    private readonly IGraphics _graphics;

    private readonly Scene3D _scene = new();

    private readonly List<DepthPolygon> _polygons = [];

    public TrackRenderer(Track track, IGraphics graphics)
    {
        Guard.ArgumentNull(track);
        Guard.ArgumentNull(graphics);

        _track = track;
        _graphics = graphics;
    }

    public void Draw(SceneCamera camera)
    {
        _scene.SetView(camera, _graphics.ScreenWidth, _graphics.ScreenHeight);
        _polygons.Clear();

        Span<Coord3D> world = stackalloc Coord3D[4];
        for (int pieceIndex = 0; pieceIndex < _track.NumPieces; pieceIndex++)
        {
            TrackPiece piece = _track.Pieces[pieceIndex];

            // piece's front left corner within the world, in track units
            int pieceX = piece.CubeX << (Track.CubeSizeLog2 - Track.LogPrecision);
            int pieceY = piece.CubeY << (Track.CubeSizeLog2 - Track.LogPrecision);
            int pieceZ = piece.CubeZ << (Track.CubeSizeLog2 - Track.LogPrecision);

            for (int segment = 0; segment < piece.NumSegments; segment++)
            {
                int offset = segment * 4;

                // road surface (top left, top right, next top right, next top left)
                byte roadColour = piece.RoadColours[segment];
                if (pieceIndex == _track.StartLinePiece && segment == piece.NumSegments - 1)
                {
                    // set colour to white for the start line
                    roadColour = Track.ScrBaseColour + 15;
                }

                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 1);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 5);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 4);
                AddPolygon(world, ScrPalette.Colour(roadColour));

                // left side (bottom left, top left, next top left, next bottom left)
                uint sideColour = ScrPalette.Colour(piece.SidesColour);
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 2);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 4);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 6);
                AddPolygon(world, sideColour);

                // right side (top right, bottom right, next bottom right, next top right)
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 1);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 3);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 7);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 5);
                AddPolygon(world, sideColour);
            }
        }

        // painter's algorithm: draw the furthest polygons first
        _polygons.Sort(static (a, b) => b.Depth.CompareTo(a.Depth));

        foreach (DepthPolygon polygon in _polygons)
        {
            _graphics.DrawPolygonFilled(polygon.Points, polygon.Colour);
        }
    }

    // A piece vertex in track units (as the original GetPieceVertex:
    // y coordinates are divided by 4 for display).
    private static Coord3D PieceVertex(TrackPiece piece, int pieceX, int pieceY, int pieceZ, int offset)
    {
        Coord3D coord = piece.Coords[offset];
        return new(coord.X + pieceX, (coord.Y / 4) + pieceY, coord.Z + pieceZ);
    }

    private void AddPolygon(in ReadOnlySpan<Coord3D> world, uint colour)
    {
        Span<Coord3D> cameraSpace = stackalloc Coord3D[4];
        long depth = 0;
        for (int i = 0; i < 4; i++)
        {
            cameraSpace[i] = _scene.TransformPoint(world[i].X, world[i].Y, world[i].Z);
            depth += cameraSpace[i].Z;
        }

        Span<Coord3D> clipped = stackalloc Coord3D[MaxPolygonPoints];
        int count = Scene3D.ClipPolygonToNearPlane(cameraSpace, clipped);
        if (count < 3)
        {
            return; // fully behind the near plane
        }

        Vector2[] points = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            points[i] = _scene.ProjectPoint(clipped[i]);
        }

        _polygons.Add(new(points, colour, depth));
    }

    private sealed record DepthPolygon(Vector2[] Points, uint Colour, long Depth);
}
