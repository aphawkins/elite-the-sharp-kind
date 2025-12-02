// 'Useful Libraries' - Andy Hawkins 2025.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public sealed record Point
{
    ////public Point(Vector3 coords, int distance, FaceNormal face1, FaceNormal face2, FaceNormal face3, FaceNormal face4)
    ////{
    ////    Coords = coords;
    ////    Distance = distance;
    ////    Face1 = face1;
    ////    Face2 = face2;
    ////    Face3 = face3;
    ////    Face4 = face4;
    ////}

    public int Distance { get; set; }

    public required Collection<FaceNormal> FaceNormals { get; set; }

    public Vector4 Coords { get; set; }
}
