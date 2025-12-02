// 'Useful Libraries' - Andy Hawkins 2025.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Useful.Assets.Models;

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]

public sealed class Line
{
    ////internal Line(int distance, FaceNormal face1, FaceNormal face2, Point startPoint, Point endPoint)
    ////{
    ////    Distance = distance;
    ////    Face1 = face1;
    ////    Face2 = face2;
    ////    StartPoint = startPoint;
    ////    EndPoint = endPoint;
    ////}

    public int Distance { get; set; }

    public required Point EndPoint { get; set; }

    public required Collection<FaceNormal> FaceNormals { get; set; }

    public required Point StartPoint { get; set; }
}
