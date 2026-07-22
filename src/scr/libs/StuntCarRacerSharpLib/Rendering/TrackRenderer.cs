// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerSharpLib.Tracks;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerSharpLib.Rendering;

// Draws the track as flat-shaded polygons: one road surface and two side
// surfaces per segment (as the original CreateUpdatePieceInVBMode1), depth
// tested per pixel as the original's Direct3D z-buffer (the remake drew the
// track with D3DRS_ZENABLE on, in arbitrary order). Road segments near the
// player are textured with the road-line strips.
public sealed class TrackRenderer
{
    private const int MaxPolygonPoints = 5; // quad plus one clip point

    // Road segments this far around the player draw with road lines (the
    // original TEXTURED_SEGMENTS_AROUND_PLAYER).
    private const int TexturedSegmentsAroundPlayer = 11;

    // The road quad's texture coordinates: u = 0 on the left edge, 1 on the
    // right, matching the original StorePieceTriangle (tv was always 0).
    // Order pairs with the road quad below (left, next left, next right, right).
    private static readonly Vector2[] s_roadTextureCoords =
        [new(0, 0), new(0, 0), new(1, 0), new(1, 0)];

    private readonly Track _track;

    private readonly IGraphics _graphics;

    private readonly ScrPalette _palette;

    private readonly Scene3D _scene = new();

    private readonly RoadTextures _roadTextures;

    private readonly int[] _segmentTextures;

    public TrackRenderer(Track track, IGraphics graphics, ScrPalette palette, RoadTextures roadTextures)
    {
        Guard.ArgumentNull(track);
        Guard.ArgumentNull(graphics);
        Guard.ArgumentNull(palette);
        Guard.ArgumentNull(roadTextures);

        _track = track;
        _graphics = graphics;
        _palette = palette;
        _roadTextures = roadTextures;
        _segmentTextures = RoadTextures.SegmentTextures(track);
    }

    public void Draw(SceneCamera camera) => Draw(camera, null);

    public void Draw(SceneCamera camera, IEnumerable<WorldPolygon>? extraPolygons)
        => Draw(camera, extraPolygons, -1, 0);

    // Draws the track plus optional extra world polygons (e.g. the opponent),
    // all depth-tested per pixel so draw order does not matter, except that
    // a segment's road draws after its side walls so the road wins the
    // shared edges (the original drew sides first with a LESSEQUAL z test).
    // Road segments near the player's position draw their road lines
    // (playerPiece -1 disables the road lines entirely).
    public void Draw(SceneCamera camera, IEnumerable<WorldPolygon>? extraPolygons, int playerPiece, int playerSegment)
    {
        _scene.SetView(camera, _graphics.ScreenWidth, _graphics.ScreenHeight);
        _graphics.ClearDepth();

        int playerGlobalSegment = playerPiece < 0
            ? -1
            : _track.Pieces[playerPiece].FirstSegment + playerSegment;

        if (extraPolygons != null)
        {
            foreach (WorldPolygon polygon in extraPolygons)
            {
                DrawWorldPolygon(polygon.Points, polygon.Colour, null, default);
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

                // left side (top left, bottom left, next bottom left, next
                // top left - the original triangles 0,2,6 and 0,6,4)
                FastColor sideColour = _palette.Colour(piece.SidesColour);
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 2);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 6);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 4);
                DrawWorldPolygon(world, sideColour, null, default);

                // right side (next top right, next bottom right, bottom
                // right, top right - the original triangles 5,7,3 and 5,3,1)
                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 5);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 7);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 3);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 1);
                DrawWorldPolygon(world, sideColour, null, default);

                // road surface (top left, next top left, next top right, top
                // right - the original triangles 0,4,5 and 0,5,1)
                byte roadColour = piece.RoadColours[segment];
                if (pieceIndex == _track.StartLinePiece && segment == piece.NumSegments - 1)
                {
                    // set colour to white for the start line
                    roadColour = Track.ScrBaseColour + 15;
                }

                world[0] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset);
                world[1] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 4);
                world[2] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 5);
                world[3] = PieceVertex(piece, pieceX, pieceY, pieceZ, offset + 1);

                int globalSegment = piece.FirstSegment + segment;
                if (IsRoadLined(globalSegment, playerGlobalSegment))
                {
                    DrawWorldPolygon(
                        world,
                        _palette.Colour(roadColour),
                        _roadTextures.Textures[_segmentTextures[globalSegment]],
                        s_roadTextureCoords);
                }
                else
                {
                    DrawWorldPolygon(world, _palette.Colour(roadColour), null, default);
                }
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

    // Transforms, clips and projects a world polygon and draws it depth
    // tested, optionally textured (textureCoords pairs with world). The
    // polygon is clipped one triangle at a time: clipping the whole outline
    // of a twisted quad that straddles the near plane can produce a
    // self-intersecting polygon, whereas a clipped triangle is always convex.
    private void DrawWorldPolygon(
        in ReadOnlySpan<Coord3D> world,
        uint colour,
        FastBitmap? texture,
        in ReadOnlySpan<Vector2> textureCoords)
    {
        Span<Vector3> cameraSpace = stackalloc Vector3[MaxPolygonPoints - 1];
        cameraSpace = cameraSpace[..world.Length];
        for (int i = 0; i < world.Length; i++)
        {
            Coord3D transformed = _scene.TransformPoint(world[i].X, world[i].Y, world[i].Z);
            cameraSpace[i] = new(transformed.X, transformed.Y, transformed.Z);
        }

        Span<Vector3> triangle = stackalloc Vector3[3];
        Span<Vector2> triangleUv = stackalloc Vector2[3];
        Span<Vector3> clipped = stackalloc Vector3[4];
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

            // emit the clipped result as triangles as well, so rounding on
            // a long thin clipped quad can never present the fan fill with
            // a concave outline
            for (int j = 1; j < count - 1; j++)
            {
                Vector2[] points =
                [
                    _scene.ProjectPoint(clipped[0]),
                    _scene.ProjectPoint(clipped[j]),
                    _scene.ProjectPoint(clipped[j + 1]),
                ];
                float[] depths = [clipped[0].Z, clipped[j].Z, clipped[j + 1].Z];

                if (texture != null)
                {
                    _graphics.DrawPolygonTexturedDepth(
                        points,
                        depths,
                        [clippedUv[0], clippedUv[j], clippedUv[j + 1]],
                        texture);
                }
                else
                {
                    _graphics.DrawPolygonFilledDepth(points, depths, colour);
                }
            }
        }
    }
}
