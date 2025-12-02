// 'Useful Libraries' - Andy Hawkins 2025.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Useful.Assets.Models.Serialization;
using Useful.Assets.Palettes;

namespace Useful.Assets.Models;

public static class ModelReader
{
    public static ThreeDModel None => new()
    {
        FaceNormals = Array.Empty<FaceNormal>(),
        Faces = Array.Empty<Face>(),
        Lines = Array.Empty<Line>(),
        Points = Array.Empty<Point>(),
    };

    public static ThreeDModel Read(string modelName, IPaletteCollection palette)
    {
        Guard.ArgumentNull(modelName);
        Guard.ArgumentNull(palette);

        string modelJson = File.ReadAllText(modelName);
        GeometryData? geometry = JsonSerializer.Deserialize<GeometryData>(modelJson);
        if (geometry is null)
        {
            return None;
        }

        Debug.Assert(geometry.FaceNormals is not null, "geometry.FaceNormals is not null");
        //// Debug.Assert(geometry.FaceNormals.Count == 4, "geometry.FaceNormals count must be 4");
        //// Debug.Assert(geometry.Faces is not null, "geometry.Faces is not null");
        //// Debug.Assert(geometry.Faces.Count == 8, "geometry.Faces count must be 8");
        Debug.Assert(geometry.Lines is not null, "geometry.Lines is not null");
        Debug.Assert(geometry.Points is not null, "geometry.Points is not null");
        ////Debug.Assert(geometry.Points.Count == 4, "geometry.FaceNormals count must be 4");

        IList<FaceNormal> faceNormals = [.. geometry.FaceNormals
            .Select(fn => new FaceNormal()
            {
                Distance = fn[0],
                Direction = new(fn[1], fn[2], fn[3], 0),
                Visible = false,
            })];

        IList<Point> points = [];
        foreach (List<int> point in geometry.Points)
        {
            Vector4 coords = new(point[0], point[1], point[2], 0);
            int distance = point[3];
            Collection<FaceNormal> pointFaceNormals = [];
            for (int i = 4; i < point.Count; i++)
            {
                //// Debug.Assert(point[i] >= 0 && point[i] < faceNormals.Count, "Face normal index is within range");
                if (point[i] >= 0 && point[i] < faceNormals.Count)
                {
                    faceNormals.Add(faceNormals[point[i]]);
                }
            }

            points.Add(new()
            {
                Coords = coords,
                Distance = distance,
                FaceNormals = pointFaceNormals,
            });
        }

        IList<Face> faces = [];
        foreach (JsonElement face in geometry.Faces)
        {
            uint color = palette[face[0].GetString() ?? string.Empty];
            Vector4 normal = new(face[1].GetInt32(), face[2].GetInt32(), face[3].GetInt32(), 0);

            List<Point> facePoints = [];
            for (int i = 4; i < face.GetArrayLength(); i++)
            {
                facePoints.Add(points[face[i].GetInt32()]);
            }

            faces.Add(new Face()
            {
                Color = color,
                Normal = normal,
                Points = facePoints,
            });
        }

        List<Line> lines = [];
        foreach (List<int> line in geometry.Lines)
        {
            Collection<FaceNormal> lineFaceNormals = [];
            for (int i = 1; i <= 2; i++)
            {
                if (line[i] >= 0 && line[i] < faceNormals.Count)
                {
                    lineFaceNormals.Add(faceNormals[line[i]]);
                }
            }

            lines.Add(new Line()
            {
                Distance = line[0],
                FaceNormals = lineFaceNormals,
                StartPoint = points[line[3]],
                EndPoint = points[line[4]],
            });
        }

        return new()
        {
            FaceNormals = faceNormals,
            Faces = faces,
            Lines = lines,
            Points = points,
        };
    }
}
