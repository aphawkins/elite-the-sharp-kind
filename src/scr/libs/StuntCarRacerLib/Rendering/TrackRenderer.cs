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
// a painter's sort instead of the Direct3D z-buffer. Road segments near the
// player are textured with the road-line strips.
public sealed class TrackRenderer
{
    private const int MaxPolygonPoints = 5; // quad plus one clip point

    // Road segments this far around the player draw with road lines (the
    // original TEXTURED_SEGMENTS_AROUND_PLAYER).
    private const int TexturedSegmentsAroundPlayer = 11;

    // Draw order within a segment's depth (the original face order).
    private const int SideOrder = 0;

    private const int RoadOrder = 1;

    private const int ExtraOrder = 2;

    // The road quad's texture coordinates: u = 0 on the left edge, 1 on the
    // right, matching the original StorePieceTriangle (tv was always 0).
    private static readonly Vector2[] s_roadTextureCoords =
        [new(0, 0), new(1, 0), new(1, 0), new(0, 0)];

    private readonly Track _track;

    private readonly IGraphics _graphics;

    private readonly Scene3D _scene = new();

    private readonly List<DepthPolygon> _polygons = [];

    private readonly int[] _segmentTextures;

    public TrackRenderer(Track track, IGraphics graphics)
    {
        Guard.ArgumentNull(track);
        Guard.ArgumentNull(graphics);

        _track = track;
        _graphics = graphics;
        _segmentTextures = RoadTextures.SegmentTextures(track);
    }

    public void Draw(SceneCamera camera) => Draw(camera, null);

    public void Draw(SceneCamera camera, IEnumerable<WorldPolygon>? extraPolygons)
        => Draw(camera, extraPolygons, -1, 0);

    // Draws the track plus optional extra world polygons (e.g. the opponent),
    // all depth-sorted together. A segment's three faces share one depth and
    // draw in the original's face order (sides first, then road) so the road
    // surface is never painted over by its own side walls. Road segments
    // near the player's position draw their road lines (playerPiece -1
    // disables the road lines entirely).
    public void Draw(SceneCamera camera, IEnumerable<WorldPolygon>? extraPolygons, int playerPiece, int playerSegment)
    {
        _scene.SetView(camera, _graphics.ScreenWidth, _graphics.ScreenHeight);
        _polygons.Clear();

        int playerGlobalSegment = playerPiece < 0
            ? -1
            : _track.Pieces[playerPiece].FirstSegment + playerSegment;

        if (extraPolygons != null)
        {
            foreach (WorldPolygon polygon in extraPolygons)
            {
                AddPolygon(polygon.Points, polygon.Colour, null, ExtraOrder);
            }
        }

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

                // one depth for the whole segment, from the road corners
                long depth = 0;
                for (int i = 0; i < 4; i++)
                {
                    depth += _scene.TransformPoint(world[i].X, world[i].Y, world[i].Z).Z;
                }

                depth /= 4;

                int globalSegment = piece.FirstSegment + segment;
                if (IsRoadLined(globalSegment, playerGlobalSegment))
                {
                    AddPolygon(
                        world,
                        ScrPalette.Colour(roadColour),
                        depth,
                        RoadOrder,
                        RoadTextures.Textures[_segmentTextures[globalSegment]],
                        s_roadTextureCoords);
                }
                else
                {
                    AddPolygon(world, ScrPalette.Colour(roadColour), depth, RoadOrder);
                }

                // left side (bottom left, top left, next top left, next bottom left)
                uint sideColour = ScrPalette.Colour(piece.SidesColour);
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 2);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 4);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 6);
                AddPolygon(world, sideColour, depth, SideOrder);

                // right side (top right, bottom right, next bottom right, next top right)
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 1);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 3);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 7);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 5);
                AddPolygon(world, sideColour, depth, SideOrder);
            }
        }

        // painter's algorithm: draw the furthest polygons first, and for
        // equal depths the sides before the road
        _polygons.Sort(static (a, b) =>
        {
            int byDepth = b.Depth.CompareTo(a.Depth);
            return byDepth != 0 ? byDepth : a.Order.CompareTo(b.Order);
        });

        foreach (DepthPolygon polygon in _polygons)
        {
            if (polygon.Texture != null)
            {
                _graphics.DrawPolygonTextured(polygon.Points, polygon.TextureCoords!, polygon.Texture);
            }
            else
            {
                _graphics.DrawPolygonFilled(polygon.Points, polygon.Colour);
            }
        }
    }

    // A piece vertex in track units (as the original GetPieceVertex:
    // y coordinates are divided by 4 for display).
    private static Coord3D PieceVertex(TrackPiece piece, int pieceX, int pieceY, int pieceZ, int offset)
    {
        Coord3D coord = piece.Coords[offset];
        return new(coord.X + pieceX, (coord.Y / 4) + pieceY, coord.Z + pieceZ);
    }

    // Whether a road segment is close enough to the player (with wraparound)
    // to draw its road lines.
    private bool IsRoadLined(int globalSegment, int playerGlobalSegment)
    {
        if (playerGlobalSegment < 0)
        {
            return false;
        }

        int distance = Math.Abs(globalSegment - playerGlobalSegment);
        distance = Math.Min(distance, _track.NumSegments - distance);
        return distance <= TexturedSegmentsAroundPlayer;
    }

    private void AddPolygon(in ReadOnlySpan<Coord3D> world, uint colour, long? depth, int order)
        => AddPolygon(world, colour, depth, order, null, default);

    // Adds a polygon with the given depth key (or its own average camera z
    // when null) and a draw order for resolving equal depths, optionally
    // textured (textureCoords pairs with world). The polygon is clipped and
    // drawn one triangle at a time, as the original D3D remake rendered
    // triangles: clipping the whole outline of a twisted quad that straddles
    // the near plane can produce a self-intersecting polygon, which the
    // triangle-fan fill would misfill with large spurious triangles (a
    // clipped triangle is always convex).
    private void AddPolygon(
        in ReadOnlySpan<Coord3D> world,
        uint colour,
        long? depth,
        int order,
        FastBitmap? texture,
        in ReadOnlySpan<Vector2> textureCoords)
    {
        Span<Coord3D> cameraSpace = stackalloc Coord3D[MaxPolygonPoints - 1];
        cameraSpace = cameraSpace[..world.Length];
        long averageDepth = 0;
        for (int i = 0; i < world.Length; i++)
        {
            cameraSpace[i] = _scene.TransformPoint(world[i].X, world[i].Y, world[i].Z);
            averageDepth += cameraSpace[i].Z;
        }

        averageDepth /= world.Length;
        long polygonDepth = depth ?? averageDepth;

        Span<Coord3D> triangle = stackalloc Coord3D[3];
        Span<Vector2> triangleUv = stackalloc Vector2[3];
        Span<Coord3D> clipped = stackalloc Coord3D[4];
        Span<Vector2> clippedUv = stackalloc Vector2[4];
        for (int i = 1; i < cameraSpace.Length - 1; i++)
        {
            triangle[0] = cameraSpace[0];
            triangle[1] = cameraSpace[i];
            triangle[2] = cameraSpace[i + 1];

            int count;
            if (texture == null)
            {
                count = Scene3D.ClipPolygonToNearPlane(triangle, clipped);
            }
            else
            {
                triangleUv[0] = textureCoords[0];
                triangleUv[1] = textureCoords[i];
                triangleUv[2] = textureCoords[i + 1];
                count = Scene3D.ClipPolygonToNearPlane(triangle, triangleUv, clipped, clippedUv);
            }

            if (count < 3)
            {
                continue; // fully behind the near plane
            }

            // emit the clipped result as triangles as well: the integer
            // rounding of the clip points can nudge a long thin clipped
            // quad into a slightly self-intersecting outline
            for (int j = 1; j < count - 1; j++)
            {
                _polygons.Add(new(
                    [
                        _scene.ProjectPoint(clipped[0]),
                        _scene.ProjectPoint(clipped[j]),
                        _scene.ProjectPoint(clipped[j + 1]),
                    ],
                    colour,
                    polygonDepth,
                    order,
                    texture,
                    texture == null ? null : [clippedUv[0], clippedUv[j], clippedUv[j + 1]]));
            }
        }
    }

    private sealed record DepthPolygon(
        Vector2[] Points,
        uint Colour,
        long Depth,
        int Order,
        FastBitmap? Texture,
        Vector2[]? TextureCoords);
}
