// 'Useful Libraries' - Andy Hawkins 2025.

using System.Globalization;
using System.Numerics;
using Useful.Assets.Palettes;

namespace Useful.Assets.Models;

/// <summary>
/// Reads ship geometry stored as Wavefront OBJ. Vertices ('v'), face-normal-pool entries
/// ('vn'), materials ('usemtl'), faces ('f', position indices only) and edges ('l') use
/// standard OBJ syntax. A ship's points can be linked to face-normal-pool entries that no
/// face itself references (used only by the explosion debris effect); since plain OBJ has
/// no syntax for that link, it is carried as '# pn &lt;point&gt; &lt;normal&gt;' comment lines
/// (1-based indices), which any standard OBJ tool will simply ignore.
/// </summary>
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

        string[] modelLines = File.ReadAllLines(modelName);

        List<Point> points = [];
        List<FaceNormal> faceNormals = [];
        List<Face> faces = [];
        List<Line> lines = [];
        List<(int PointIndex, int NormalIndex)> pointNormalLinks = [];
        string currentMaterial = string.Empty;

        foreach (string modelLine in modelLines)
        {
            string[] tokens = modelLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                continue;
            }

            switch (tokens[0])
            {
                case "v":
                    points.Add(new()
                    {
                        Coords = ParseVector(tokens),
                        FaceNormals = [],
                    });
                    break;

                case "vn":
                    faceNormals.Add(new()
                    {
                        Direction = ParseVector(tokens),
                        Visible = false,
                    });
                    break;

                case "usemtl":
                    currentMaterial = tokens[1];
                    break;

                case "f":
                    int[] pointIndices = [.. tokens.Skip(1).Select(t => int.Parse(t, CultureInfo.InvariantCulture) - 1)];
                    faces.Add(new()
                    {
                        Color = palette[currentMaterial],
                        Points = [.. pointIndices.Select(index => points[index])],
                        PointIndices = pointIndices,
                    });
                    break;

                case "l":
                    lines.Add(new()
                    {
                        StartPoint = points[int.Parse(tokens[1], CultureInfo.InvariantCulture) - 1],
                        EndPoint = points[int.Parse(tokens[2], CultureInfo.InvariantCulture) - 1],
                    });
                    break;

                case "#" when tokens.Length == 4 && tokens[1] == "pn":
                    pointNormalLinks.Add((
                        int.Parse(tokens[2], CultureInfo.InvariantCulture) - 1,
                        int.Parse(tokens[3], CultureInfo.InvariantCulture) - 1));
                    break;
            }
        }

        foreach ((int pointIndex, int normalIndex) in pointNormalLinks)
        {
            points[pointIndex].FaceNormals.Add(faceNormals[normalIndex]);
        }

        return points.Count == 0 && faces.Count == 0
            ? None
            : new()
            {
                FaceNormals = faceNormals,
                Faces = faces,
                Lines = lines,
                Points = points,
            };
    }

    private static Vector4 ParseVector(string[] tokens) => new(
        float.Parse(tokens[1], CultureInfo.InvariantCulture),
        float.Parse(tokens[2], CultureInfo.InvariantCulture),
        float.Parse(tokens[3], CultureInfo.InvariantCulture),
        0);
}
