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

    // Decal faces (cockpit windows, engine plates) lie exactly in the plane
    // of the hull face they sit on, so per-vertex depth makes them tie with
    // it pixel for pixel - and the rasteriser interpolates inverse depth
    // along a scanline from floored pixel positions, so the tie comes out
    // inexact and the hull speckles through. Pulling the decal nearer by a
    // fraction of its depth settles it; relative rather than absolute so it
    // holds at any range. 0.1% was measured too small to cover the
    // interpolation error on a near edge-on decal, 1% covers it and stays
    // far inside the front-to-back spread of a single ship, so a decal
    // can't punch through a face genuinely in front of it.
    private const float DecalDepthBias = 0.99f;

    private readonly IEliteDraw _draw;
    private readonly FastColor _colorCyan;
    private readonly FastColor _colorWhite;
    private readonly RNG _rng;
    private int[]? _faceRoot;
    private Vector3[]? _faceNormal;

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
    // laser aim, so those keep the original's depth clamp; the faces
    // themselves are culled in camera space and clipped properly before
    // drawing, so the clamp no longer feeds any visibility decision.
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
        if (_faceRoot == null)
        {
            (_faceRoot, _faceNormal) = FindFaceRoots();
        }

        int maxPoints = 0;
        for (int i = 0; i < Model.Faces.Count; i++)
        {
            maxPoints = Math.Max(maxPoints, Model.Faces[i].Points.Count);
        }

        Span<Vector3> face = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints] : new Vector3[maxPoints];
        Span<Vector3> clipped = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints + 1] : new Vector3[maxPoints + 1];

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            if (IsFacingCamera(i))
            {
                float bias = _faceRoot[i] == i ? 1f : DecalDepthBias;
                Vector2[]? poly_list = BuildFacePolygon(Model.Faces[i], pointList, face, clipped, bias, out float[] depths);
                if (poly_list != null)
                {
                    _draw.DrawPolygonFilled(poly_list, depths, Model.Faces[i].Color, FaceMeanZ(_faceRoot[i], pointList));
                }
            }
        }
    }

    // Backface cull in camera space, against the face's own model-space
    // normal rotated into view. Doing it here rather than on the projected
    // outline keeps the decision off the near-plane depth clamp, which
    // produces meaningless X/Y for a face straddling the camera plane -
    // and no later clip can undo a cull already taken.
    private bool IsFacingCamera(int faceIndex)
    {
        Face face = Model.Faces[faceIndex];
        Vector3 surfacePoint = _cameraList[face.PointIndices[0]];
        Vector3 normal = _faceNormal![faceIndex];

        return normal != Vector3.Zero
            ? FacesCamera(normal, surfacePoint)
            : AnySharedVertexNormalFacesCamera(face, surfacePoint);
    }

    // True when a model-space normal, rotated into view, turns towards the
    // camera at the given camera-space point on the surface.
    private bool FacesCamera(Vector3 normal, Vector3 surfacePoint)
    {
        Vector4 rotated = Vector4.Transform(new Vector4(normal, 0), Rotmat);
        return ((rotated.X * surfacePoint.X) + (rotated.Y * surfacePoint.Y) + (rotated.Z * surfacePoint.Z)) <= 0;
    }

    // A detail line lying on no other face's plane has no normal of its own,
    // so it cannot be culled the way a face is. The model still records, per
    // vertex, the normals of the faces that vertex belongs to; the faces the
    // line runs along are those shared by every one of its ends, and the line
    // is visible when any of them is - which is how the original decided a
    // line's visibility. A line sharing none (a model carrying no such data)
    // has nothing to cull against and draws, as before.
    private bool AnySharedVertexNormalFacesCamera(Face face, Vector3 surfacePoint)
    {
        // Identity, not equality: the reader hands every vertex on a face the
        // same pooled FaceNormal instance, and two distinct faces can carry
        // numerically equal normals.
        static bool SharesNormal(Point point, FaceNormal normal)
        {
            foreach (FaceNormal candidate in point.FaceNormals)
            {
                if (ReferenceEquals(candidate, normal))
                {
                    return true;
                }
            }

            return false;
        }

        bool anyShared = false;

        foreach (FaceNormal candidate in face.Points[0].FaceNormals)
        {
            bool sharedByAll = true;
            for (int j = 1; j < face.Points.Count && sharedByAll; j++)
            {
                sharedByAll = SharesNormal(face.Points[j], candidate);
            }

            if (!sharedByAll)
            {
                continue;
            }

            anyShared = true;
            Vector4 direction = candidate.Direction;
            if (FacesCamera(new(direction.X, direction.Y, direction.Z), surfacePoint))
            {
                return true;
            }
        }

        return !anyShared;
    }

    // A single vertex is visible when any face it belongs to is. A vertex
    // the model records no normals for has nothing to cull against.
    private bool IsPointFacingCamera(int pointIndex)
    {
        Vector3 surfacePoint = _cameraList[pointIndex];
        bool any = false;

        foreach (FaceNormal normal in Model.Points[pointIndex].FaceNormals)
        {
            any = true;
            Vector4 direction = normal.Direction;
            if (FacesCamera(new(direction.X, direction.Y, direction.Z), surfacePoint))
            {
                return true;
            }
        }

        return !any;
    }

    // The face's screen outline, clipped to the near plane, with the
    // camera-space depth of each of its points; null when the face lies
    // entirely behind the near plane. depthBias scales those depths, to
    // settle a decal against the face it sits on.
    private Vector2[]? BuildFacePolygon(
        Face face,
        Vector4[] pointList,
        in Span<Vector3> cameraPoints,
        in Span<Vector3> clipped,
        float depthBias,
        out float[] depths)
    {
        int numPoints = face.Points.Count;

        // A 2-point detail line is not a polygon - the cyclic clipper would
        // walk its single edge twice - so it keeps the clamped projection,
        // whose Z is the camera depth except for points behind the camera.
        // It takes the same bias as a decal: a detail line lies on a hull
        // face just as a decal panel does.
        if (numPoints < 3)
        {
            Vector2[] line = new Vector2[numPoints];
            depths = new float[numPoints];
            for (int j = 0; j < numPoints; j++)
            {
                int index = face.PointIndices[j];
                line[j] = new(pointList[index].X, pointList[index].Y);
                depths[j] = pointList[index].Z * depthBias;
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
            depths = [];
            return null;
        }

        Vector2[] polygon = new Vector2[count];
        depths = new float[count];
        for (int j = 0; j < count; j++)
        {
            polygon[j] = ProjectCameraPoint(clipped[j]);
            depths[j] = clipped[j].Z * depthBias;
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
    //
    // Also returns each face's model-space normal, for the backface cull. A
    // detail line has no normal of its own, so it takes its root face's -
    // which is what makes far-side detail cull with the hull it sits on.
    private (int[] Roots, Vector3[] Normals) FindFaceRoots()
    {
        int[] roots = new int[Model.Faces.Count];
        Vector3[] normals = new Vector3[Model.Faces.Count];
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
                    normals[i] = cross;
                    Vector4 normal = new(cross, 0);
                    planes.Add((normal, VectorMaths.VectorDotProduct(normal, face.Points[0].Coords), i));
                }
            }
            else if (roots[i] != i)
            {
                normals[i] = normals[roots[i]];
            }
        }

        return (roots, normals);
    }

    private void DrawLasers(Vector4[] pointList)
    {
        if (!Flags.HasFlag(ShipProperties.Firing))
        {
            return;
        }

        int lasv = LaserFront;

        // The bolt springs from a mount on the hull, so it is only visible
        // when that part of the hull is. Without this the laser bypasses the
        // face cull entirely and a ship firing away from us draws its bolt
        // straight through its own hull - which the depth test hides in
        // z-buffered mode but wireframe, having no depth buffer at all,
        // cannot.
        if (!IsPointFacingCamera(lasv))
        {
            return;
        }

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

        // The bolt emerges from the mount on the hull surface, and its far
        // end is a screen-space boundary point with no camera-space depth of
        // its own, so the whole line tests at the mount's depth - biased
        // nearer, like a decal, so the hull it springs from cannot swallow
        // it. Anything genuinely in front of the firing ship still hides it.
        float mountZ = pointList[lasv].Z * DecalDepthBias;
        _draw.DrawPolygonFilled([mount, endPoint], [mountZ, mountZ], color, mountZ);
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
