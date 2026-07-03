// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Tracks;
using Xunit;

namespace StuntCarRacerLib.Tests.Tracks;

public class TrackTests
{
    public static TheoryData<TrackId> AllTracks { get; } =
    [
        TrackId.LittleRamp,
        TrackId.SteppingStones,
        TrackId.HumpBack,
        TrackId.BigRamp,
        TrackId.SkiJump,
        TrackId.DrawBridge,
        TrackId.HighJump,
        TrackId.RollerCoaster,
    ];

    [Theory]
    [MemberData(nameof(AllTracks))]
    public void LoadAllTracksSucceeds(TrackId id)
    {
        // Act
        Track track = Track.Load(id);

        // Assert
        Assert.Equal(id, track.Id);
        Assert.InRange(track.NumPieces, 1, 100);
        Assert.InRange(track.PlayersStartPiece, 0, track.NumPieces - 1);
        Assert.True(track.NumSegments > 0);
    }

    [Theory]
    [MemberData(nameof(AllTracks))]
    public void FirstSegmentNumbersAreCumulative(TrackId id)
    {
        // Act
        Track track = Track.Load(id);

        // Assert
        int firstSegment = 0;
        foreach (TrackPiece piece in track.Pieces)
        {
            Assert.Equal(firstSegment, piece.FirstSegment);
            firstSegment += piece.NumSegments;
        }

        Assert.Equal(firstSegment, track.NumSegments);
    }

    [Theory]
    [MemberData(nameof(AllTracks))]
    public void PiecesJoinUpPerfectly(TrackId id)
    {
        // Act
        Track track = Track.Load(id);

        // Assert - the end coordinates of each piece must equal the start
        // coordinates of the next piece, adjusted for cube position.
        for (int piece = 0; piece < track.NumPieces; piece++)
        {
            TrackPiece current = track.Pieces[piece];
            TrackPiece next = track.Pieces[(piece + 1) % track.NumPieces];

            const int shift = 26 - 14; // LogCubeSize - LogPrecision
            int dx = (next.CubeX - current.CubeX) << shift;
            int dy = (next.CubeY - current.CubeY) << shift;
            int dz = (next.CubeZ - current.CubeZ) << shift;

            for (int i = 0; i < 4; i++)
            {
                Coord3D end = current.Coords[(current.NumSegments * 4) + i];
                Coord3D start = next.Coords[i];
                Assert.Equal(start.X + dx, end.X);
                Assert.Equal(start.Y + dy, end.Y);
                Assert.Equal(start.Z + dz, end.Z);
            }
        }
    }

    [Theory]
    [MemberData(nameof(AllTracks))]
    public void TrackMapLocatesPieces(TrackId id)
    {
        // Act
        Track track = Track.Load(id);

        // Assert - every piece's cube maps back to a piece in the same cube.
        for (int piece = 0; piece < track.NumPieces; piece++)
        {
            TrackPiece current = track.Pieces[piece];
            int mapped = track.GetMapPiece(current.CubeX, current.CubeZ);
            Assert.InRange(mapped, 0, track.NumPieces - 1);
            Assert.Equal(current.CubeX, track.Pieces[mapped].CubeX);
            Assert.Equal(current.CubeZ, track.Pieces[mapped].CubeZ);
        }
    }

    [Fact]
    public void LittleRampMatchesOriginalData()
    {
        // Values taken from the original Track.cpp commented Little Ramp data
        // and the LittleRamp.bin file contents.
        Track track = Track.Load(TrackId.LittleRamp);

        Assert.Equal("Little Ramp", track.Name);
        Assert.Equal(44, track.NumPieces);
        Assert.Equal(15, track.PlayersStartPiece);
        Assert.Equal(15, track.StartLinePiece);
        Assert.Equal(37, track.HalfALapPiece);
        Assert.Equal(34, track.StandardBoost);
        Assert.Equal(47, track.SuperBoost);
        Assert.Equal(398, track.NumSegments);
    }

    [Fact]
    public void LittleRampPieceZeroMatchesOriginalData()
    {
        // Piece 0: x,z position byte 0xcf and angle/template byte 0xa0
        // (template 0 = straight 8, rough angle 0x80 = 180 degrees).
        Track track = Track.Load(TrackId.LittleRamp);
        TrackPiece piece = track.Pieces[0];

        Assert.Equal(15, piece.CubeX);
        Assert.Equal(0, piece.CubeY);
        Assert.Equal(12, piece.CubeZ);
        Assert.Equal(0xa0, piece.AngleAndTemplate);
        Assert.Equal(32768, piece.RoughPieceAngle);
        Assert.False(piece.OppositeDirection);
        Assert.False(piece.CurveToLeft);
        Assert.Equal(0x00, piece.Type);
        Assert.Equal(8, piece.NumSegments);
        Assert.Equal(128, piece.LengthReduction);
        Assert.Equal(0x20, piece.SteeringAmount);
        Assert.Equal(0, piece.FirstSegment);
    }

    [Fact]
    public void LittleRampPieceTwelveIsReversedCurve()
    {
        // Piece 12: angle/template byte 0x57 - template 7 (curve left 9),
        // rough angle 0x40 = 90 degrees, bit 4 set = opposite direction.
        Track track = Track.Load(TrackId.LittleRamp);
        TrackPiece piece = track.Pieces[12];

        Assert.Equal(0x57, piece.AngleAndTemplate);
        Assert.Equal(16384, piece.RoughPieceAngle);
        Assert.True(piece.OppositeDirection);
        Assert.True(piece.CurveToLeft);
        Assert.Equal(0xc0, piece.Type);
        Assert.Equal(9, piece.NumSegments);
    }

    [Fact]
    public void BottomCoordinatesAreAtTrackBottom()
    {
        Track track = Track.Load(TrackId.LittleRamp);

        // All bottom coords (except the joined-up final four, which take the
        // next piece's values) sit at track bottom level.
        foreach (TrackPiece piece in track.Pieces)
        {
            for (int i = 0; i < piece.NumSegments; i++)
            {
                Assert.Equal(0, piece.Coords[(i * 4) + 2].Y);
                Assert.Equal(0, piece.Coords[(i * 4) + 3].Y);
            }
        }
    }

    [Fact]
    public void LoadWithNullStreamThrows()
        => Assert.Throws<ArgumentNullException>(() => Track.Load(TrackId.LittleRamp, null!));

    [Fact]
    public void RoadColoursAreBaseColours()
    {
        Track track = Track.Load(TrackId.RollerCoaster);

        foreach (TrackPiece piece in track.Pieces)
        {
            for (int i = 0; i < piece.NumSegments; i++)
            {
                // black, dark or light road surface
                Assert.Contains(piece.RoadColours[i], new byte[] { 26, 27, 28 });
            }
        }
    }
}
