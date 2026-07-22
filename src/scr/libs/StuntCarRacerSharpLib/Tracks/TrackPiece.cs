// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerSharpLib.Tracks;

public sealed class TrackPiece
{
    internal TrackPiece(int numSegments)
    {
        NumSegments = numSegments;
        Coords = new Coord3D[(numSegments + 1) * 4];
        RoadColours = new byte[Track.MaxSegments];
    }

    // Piece's cube within the 16x16 track map.
    public int CubeX { get; internal set; }

    public int CubeY { get; internal set; }

    public int CubeZ { get; internal set; }

    // 0, 90, 180, 270 degrees, stored in internal angle format (65536 = 360 degrees).
    public int RoughPieceAngle { get; internal set; }

    // Normal travel direction along piece.
    public bool OppositeDirection { get; internal set; }

    public bool CurveToLeft { get; internal set; }

    // near.section.byte1 - 0x00 straight, 0x40 diagonal (45 degrees),
    // 0x80 curve right, 0xc0 curve left.
    public int Type { get; internal set; }

    public int LengthReduction { get; internal set; }

    public int SteeringAmount { get; internal set; }

    public int NumSegments { get; }

    // Piece's first segment number within the whole track.
    public int FirstSegment { get; internal set; }

    // Initial roadLinesColour offset (0 or 1).
    public int InitialColour { get; internal set; }

    // Raw road.section.angle.and.piece byte, kept for car behaviour.
    public byte AngleAndTemplate { get; internal set; }

    // Colour index for individual segment road surfaces.
    public IList<byte> RoadColours { get; }

    public byte SidesColour { get; internal set; }

    // Unrotated coordinates: four per segment boundary
    // (top left, top right, bottom left, bottom right).
    public IList<Coord3D> Coords { get; }

    // Overall y shifts from the track data, retained for the draw bridge animation.
    internal int LeftYShift { get; set; }

    internal int RightYShift { get; set; }
}
