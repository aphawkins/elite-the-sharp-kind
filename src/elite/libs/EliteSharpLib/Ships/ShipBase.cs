// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;
using Useful;
using Useful.Assets.Models;
using Useful.Graphics;
using Useful.Maths;

namespace EliteSharpLib.Ships;

internal class ShipBase : IShip
{
    // Large enough that Location's contribution to the projected point is
    // negligible, so the result approximates the on-screen vanishing point of
    // the local direction (Model.Points[lasv].Coords) rather than a specific
    // 3D position along it.
    private const float FarAimDistance = 1_000_000f;
    private const int LaserAimSpread = 24;

    // Camera-space depth the ship's faces are clipped against. Ship sizes run
    // to hundreds of units, so this sits effectively on the camera plane -
    // it exists to keep the perspective divide out of the sign flip, not to
    // cull anything a player would otherwise see.
    private const float NearPlane = 1f;

    // Ship faces are small polygons; anything larger falls back to the heap.
    private const int StackFacePoints = 16;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorCyan;
    private readonly FastColor _colorWhite;
    private readonly RNG _rng;
    private int[]? _faceRoot;

    // Reused across frames so drawing a ship doesn't allocate; grown to the
    // model's point count on first use.
    private Vector4[] _pointList = [];
    private Vector3[] _cameraList = [];

    internal ShipBase(IEliteDraw draw, RNG rng)
    {
        _draw = draw;
        _rng = rng;
        Model = ModelReader.None;

        _colorCyan = draw.Palette["Cyan"];
        _colorWhite = draw.Palette["White"];
    }

    private ShipBase(ShipBase other)
    {
        _draw = other._draw;
        _rng = other._rng;
        Model = other.Model;
    }

    public int Acceleration { get; set; }

    public float Bounty { get; set; }

    public int Bravery { get; set; }

    public int Energy { get; set; }

    public int EnergyMax { get; set; }

    public int ExpDelta { get; set; }

    public ShipProperties Flags { get; set; } = ShipProperties.None;

    public int LaserFront { get; set; }

    public int LaserStrength { get; set; }

    public Vector4 Location { get; set; }

    public int LootMax { get; set; }

    public float MinDistance { get; set; }

    public int Missiles { get; set; }

    public int MissilesMax { get; set; }

    public string Name { get; set; } = string.Empty;

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public StockType ScoopedType { get; set; }

    public float Size { get; set; }

    public IObject? Target { get; set; }

    public ShipType Type { get; set; }

    public int VanishPoint { get; set; }

    public float Velocity { get; set; }

    public float VelocityMax { get; set; }

    public ThreeDModel Model { get; set; }

    public IObject Clone()
    {
        ShipBase ship = new(this);
        this.CopyTo(ship);
        return ship;
    }

    /// <summary>
    /// Hacked version of the draw ship routine to display ships...
    /// This needs a lot of tidying...
    /// caveat: it is a work in progress.
    /// A number of features(such as not showing detail at distance) have not yet been implemented.
    /// Check for hidden surface supplied by T.Harte.
    /// </summary>
    public virtual void Draw()
    {
        if (_pointList.Length < Model.Points.Count)
        {
            _pointList = new Vector4[Model.Points.Count];
            _cameraList = new Vector3[Model.Points.Count];
        }

        Vector4[] pointList = _pointList;

        // Transform model points
        TransformModelPoints(Rotmat, pointList);

        // Draw faces
        DrawModelFaces(pointList);

        // Draw firing lasers if needed
        DrawLasers(pointList);
    }

    private void TransformModelPoints(Matrix4x4 transform, Vector4[] pointList)
    {
        for (int i = 0; i < Model.Points.Count; i++)
        {
            Vector4 camera = Vector4.Transform(Model.Points[i].Coords, transform) + Location;
            _cameraList[i] = new(camera.X, camera.Y, camera.Z);
            pointList[i] = ProjectPoint(camera);
        }
    }

    private Vector4 ProjectPoint(Vector4 localCoords, Matrix4x4 transform)
        => ProjectPoint(Vector4.Transform(localCoords, transform) + Location);

    // Points behind the near plane still have to yield something for the
    // backface-winding test and the laser aim, so those keep the original's
    // depth clamp; the faces themselves are clipped properly before drawing.
    private Vector4 ProjectPoint(Vector4 cameraCoords)
    {
        Vector4 vec = cameraCoords;

        if (vec.Z <= 0)
        {
            vec.Z = 1;
        }

        vec.X = _draw.Layout.ViewportCentre.X + (vec.X * _draw.Focus / vec.Z);
        vec.Y = _draw.Layout.ViewportCentre.Y - (vec.Y * _draw.Focus / vec.Z);

        return vec;
    }

    private Vector2 ProjectCameraPoint(Vector3 cameraPoint) => new(
        _draw.Layout.ViewportCentre.X + (cameraPoint.X * _draw.Focus / cameraPoint.Z),
        _draw.Layout.ViewportCentre.Y - (cameraPoint.Y * _draw.Focus / cameraPoint.Z));

    private void DrawModelFaces(Vector4[] pointList)
    {
        _faceRoot ??= FindFaceRoots();

        int maxPoints = 0;
        for (int i = 0; i < Model.Faces.Count; i++)
        {
            maxPoints = Math.Max(maxPoints, Model.Faces[i].Points.Count);
        }

        Span<Vector3> face = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints] : new Vector3[maxPoints];
        Span<Vector3> clipped = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints + 1] : new Vector3[maxPoints + 1];

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            int point0 = Model.Faces[i].PointIndices[0];
            int point1 = Model.Faces[i].PointIndices[1];
            int point2 = Model.Faces[i].Points.Count > 2
                ? Model.Faces[i].PointIndices[2]
                : Model.Faces[i].PointIndices[0];

            if (((pointList[point0].X - pointList[point1].X) * (pointList[point2].Y - pointList[point1].Y)) <=
                ((pointList[point0].Y - pointList[point1].Y) * (pointList[point2].X - pointList[point1].X)))
            {
                Vector2[]? poly_list = BuildFacePolygon(Model.Faces[i], pointList, face, clipped);
                if (poly_list != null)
                {
                    _draw.DrawPolygonFilled(poly_list, Model.Faces[i].Color, FaceMeanZ(_faceRoot[i], pointList));
                }
            }
        }
    }

    // The face's screen outline, clipped to the near plane, or null when the
    // face lies entirely behind it.
    private Vector2[]? BuildFacePolygon(
        Face face,
        Vector4[] pointList,
        in Span<Vector3> cameraPoints,
        in Span<Vector3> clipped)
    {
        int numPoints = face.Points.Count;

        // A 2-point detail line is not a polygon - the cyclic clipper would
        // walk its single edge twice - so it keeps the clamped projection.
        if (numPoints < 3)
        {
            Vector2[] line = new Vector2[numPoints];
            for (int j = 0; j < numPoints; j++)
            {
                int index = face.PointIndices[j];
                line[j] = new(pointList[index].X, pointList[index].Y);
            }

            return line;
        }

        for (int j = 0; j < numPoints; j++)
        {
            cameraPoints[j] = _cameraList[face.PointIndices[j]];
        }

        int count = NearPlaneClip.Clip(cameraPoints[..numPoints], NearPlane, clipped);
        if (count < 3)
        {
            return null;
        }

        Vector2[] polygon = new Vector2[count];
        for (int j = 0; j < count; j++)
        {
            polygon[j] = ProjectCameraPoint(clipped[j]);
        }

        return polygon;
    }

    // The whole-face depth key: the mean Z of the face's transformed
    // points. Decals and detail lines use their root face's key so they
    // tie exactly with the surface they sit on and draw over it.
    private float FaceMeanZ(int faceIndex, Vector4[] pointList)
    {
        Face face = Model.Faces[faceIndex];
        float z = 0;
        for (int j = 0; j < face.Points.Count; j++)
        {
            z += pointList[face.PointIndices[j]].Z;
        }

        return z / face.Points.Count;
    }

    // For each face, the face it sits on: decal faces (cockpit windows,
    // engine plates) and 2-point detail lines lie exactly in the plane of
    // an earlier, larger face. They must render over that base face, so
    // they share its depth key. Faces on no earlier plane root to
    // themselves. Computed once per instance from the model geometry.
    private int[] FindFaceRoots()
    {
        int[] roots = new int[Model.Faces.Count];
        List<(Vector4 Normal, float Offset, int Index)> planes = [];

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            roots[i] = i;

            Face face = Model.Faces[i];
            foreach ((Vector4 normal, float offset, int index) in planes)
            {
                bool onPlane = true;
                for (int j = 0; j < face.Points.Count && onPlane; j++)
                {
                    onPlane = MathF.Abs(VectorMaths.VectorDotProduct(normal, face.Points[j].Coords) - offset) < 0.1f;
                }

                if (onPlane)
                {
                    roots[i] = roots[index];
                    break;
                }
            }

            if (face.Points.Count >= 3)
            {
                Vector4 edge1 = face.Points[1].Coords - face.Points[0].Coords;
                Vector4 edge2 = face.Points[2].Coords - face.Points[0].Coords;
                Vector3 cross = Vector3.Cross(new(edge1.X, edge1.Y, edge1.Z), new(edge2.X, edge2.Y, edge2.Z));
                if (cross.LengthSquared() > 0)
                {
                    cross = Vector3.Normalize(cross);
                    Vector4 normal = new(cross, 0);
                    planes.Add((normal, VectorMaths.VectorDotProduct(normal, face.Points[0].Coords), i));
                }
            }
        }

        return roots;
    }

    private void DrawLasers(Vector4[] pointList)
    {
        if (!Flags.HasFlag(ShipProperties.Firing))
        {
            return;
        }

        int lasv = LaserFront;
        FastColor color = (Type == ShipType.Viper) ? _colorCyan : _colorWhite;

        Vector2 mount = new(pointList[lasv].X, pointList[lasv].Y);

        // Aim along the ship's real firing direction - the vector from its local
        // origin through the laser mount (the nose) - projected a long way out so
        // it approximates where that direction vanishes on screen, plus a small
        // random spread so repeated shots aren't visually identical. The previous
        // code picked a screen-edge X by which side the ship was on and a Y
        // uniformly random over the whole view, ignoring the ship's firing angle.
        Vector4 aimPoint = ProjectPoint(Model.Points[lasv].Coords * FarAimDistance, Rotmat);
        float aimX = aimPoint.X + _rng.Random(-LaserAimSpread, LaserAimSpread);
        float aimY = aimPoint.Y + _rng.Random(-LaserAimSpread, LaserAimSpread);
        Vector2 direction = new Vector2(aimX, aimY) - mount;

        Vector2 endPoint = ProjectToViewBoundary(mount, direction);

        _draw.DrawPolygonFilled([mount, endPoint], color, pointList[lasv].Z);
    }

    // Finds where a ray from origin along direction leaves the view rectangle,
    // so the laser is clipped to the actual viewport rather than a hardcoded
    // screen size.
    private Vector2 ProjectToViewBoundary(Vector2 origin, Vector2 direction)
    {
        float exitDistance = float.PositiveInfinity;

        if (direction.X > 0)
        {
            exitDistance = MathF.Min(exitDistance, (_draw.Layout.ViewportRight - origin.X) / direction.X);
        }
        else if (direction.X < 0)
        {
            exitDistance = MathF.Min(exitDistance, (_draw.Layout.ViewportLeft - origin.X) / direction.X);
        }

        if (direction.Y > 0)
        {
            exitDistance = MathF.Min(exitDistance, (_draw.Layout.ViewportBottom - origin.Y) / direction.Y);
        }
        else if (direction.Y < 0)
        {
            exitDistance = MathF.Min(exitDistance, (_draw.Layout.ViewportTop - origin.Y) / direction.Y);
        }

        return !float.IsFinite(exitDistance) || exitDistance <= 0
            ? origin
            : origin + (direction * exitDistance);
    }
}
