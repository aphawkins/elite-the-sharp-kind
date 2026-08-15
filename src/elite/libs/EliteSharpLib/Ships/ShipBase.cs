// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;
using SharpKind;
using SharpKind.Assets.Models;
using SharpKind.Graphics;
using SharpKind.Graphics.Rendering;

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

    // The frustum's far plane. Space removes a ship once it is further out
    // than this (Space.cs), so the plane records the range the game already
    // has rather than imposing a new one - a frustum has six planes and this
    // is the sixth, not a new decision about what the player can see.
    private const float FarPlane = 57344f;

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
    private readonly RNG _rng;

    // What the model's shape implies. Derived whenever the model is set, so
    // the two always agree.
    private ModelGeometry _geometry = ModelGeometry.For(ModelReader.None);

    // Reused across frames so drawing a ship doesn't allocate; grown to the
    // model's point count on first use.
    private Vector4[] _pointList = [];
    private Vector3[] _cameraList = [];

    internal ShipBase(IEliteDraw draw, RNG rng)
    {
        _draw = draw;
        _rng = rng;
        Model = ModelReader.None;
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

    public ThreeDModel Model
    {
        get;

        set
        {
            field = value;
            _geometry = ModelGeometry.For(value);
        }
    }

    // Gets the model's normal at each corner of each face. A Gouraud fill
    // interpolates between these; a flat one has no use for them.
    internal IReadOnlyList<IReadOnlyList<Vector3>> CornerNormals => _geometry.CornerNormals;

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
        // Nothing of this ship can be on screen, so none of it is worth
        // transforming: the whole model, its faces and its laser go together.
        if (!IsWithinView())
        {
            return;
        }

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

    // The ship as a bounding sphere at its own origin, against the frustum the
    // viewport sees. The radius comes from the model rather than the
    // hand-authored Size - Size is the collision radius squared (see Combat)
    // and need not agree with the geometry, and a cull that rejects a ship the
    // model would have drawn is a hole in the hull. Rotating a model about its
    // origin cannot move it outside the sphere, so the one test covers every
    // orientation. Conservative, so a ship with any part on screen always
    // draws.
    private bool IsWithinView()
    {
        ViewFrustum frustum = ViewFrustum.FromViewport(
            _draw.Projector,
            _draw.Layout.ViewportLeft,
            _draw.Layout.ViewportTop,
            _draw.Layout.ViewportWidth,
            _draw.Layout.ViewportHeight,
            NearPlane,
            FarPlane);

        return frustum.Intersects(new(Location.X, Location.Y, Location.Z), _geometry.BoundingRadius);
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

        Vector2 screen = _draw.Projector.Project(vec.X, vec.Y, vec.Z);
        vec.X = screen.X;
        vec.Y = screen.Y;

        return vec;
    }

    private Vector2 ProjectCameraPoint(Vector3 cameraPoint) => _draw.Projector.Project(cameraPoint);

    private void DrawModelFaces(Vector4[] pointList)
    {
        int maxPoints = 0;
        for (int i = 0; i < Model.Faces.Count; i++)
        {
            maxPoints = Math.Max(maxPoints, Model.Faces[i].Points.Count);
        }

        Span<Vector3> face = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints] : new Vector3[maxPoints];
        Span<Vector3> clipped = maxPoints <= StackFacePoints ? stackalloc Vector3[StackFacePoints + 1] : new Vector3[maxPoints + 1];
        Span<FastColor> corners = maxPoints <= StackFacePoints ? stackalloc FastColor[StackFacePoints] : new FastColor[maxPoints];
        Span<FastColor> clippedCorners = maxPoints <= StackFacePoints
            ? stackalloc FastColor[StackFacePoints + 1]
            : new FastColor[maxPoints + 1];

        for (int i = 0; i < Model.Faces.Count; i++)
        {
            if (!IsFacingCamera(i, out Vector3 cameraNormal))
            {
                continue;
            }

            if (ShadesCorners(i))
            {
                DrawShadedFace(i, pointList, face, clipped, corners, clippedCorners);
                continue;
            }

            DrawFlatFace(i, cameraNormal, pointList, face, clipped);
        }
    }

    // One face as a single colour: the model's own, as this rendition's
    // lighting leaves it.
    private void DrawFlatFace(
        int faceIndex,
        Vector3 cameraNormal,
        Vector4[] pointList,
        in Span<Vector3> cameraPoints,
        in Span<Vector3> clipped)
    {
        float bias = _geometry.FaceRoots[faceIndex] == faceIndex ? 1f : DecalDepthBias;
        Vector2[]? poly_list = BuildFacePolygon(
            Model.Faces[faceIndex],
            pointList,
            cameraPoints,
            clipped,
            bias,
            out float[] depths);

        if (poly_list != null)
        {
            FastColor color = _draw.ShadeFace(Model.Faces[faceIndex].Color, cameraNormal, _geometry.FullyLit);
            _draw.DrawPolygonFilled(poly_list, depths, color, FaceMeanZ(_geometry.FaceRoots[faceIndex], pointList));
        }
    }

    // One face shaded at each corner, for the fill to blend between.
    private void DrawShadedFace(
        int faceIndex,
        Vector4[] pointList,
        in Span<Vector3> cameraPoints,
        in Span<Vector3> clipped,
        in Span<FastColor> cornerColours,
        in Span<FastColor> clippedColours)
    {
        Vector2[]? polygon = BuildShadedFacePolygon(
            faceIndex,
            cameraPoints,
            clipped,
            cornerColours,
            clippedColours,
            out float[] depths,
            out FastColor[] colours);

        if (polygon != null)
        {
            _draw.DrawPolygonFilled(polygon, depths, colours, FaceMeanZ(faceIndex, pointList));
        }
    }

    // A face blends across itself only when the draw shades per corner and
    // the face has corners of its own to shade. A decal or detail line was
    // left out of the smoothing, so its corner normals are zero: it lies in
    // one plane on top of another and fills flat, which is what it should do
    // and what the depth bias in the flat path below assumes.
    private bool ShadesCorners(int faceIndex)
        => _draw.ShadesPerVertex
            && CornerNormals[faceIndex].Count >= 3
            && CornerNormals[faceIndex][0] != Vector3.Zero;

    // Backface cull in camera space, against the face's own model-space
    // normal rotated into view. Doing it here rather than on the projected
    // outline keeps the decision off the near-plane depth clamp, which
    // produces meaningless X/Y for a face straddling the camera plane -
    // and no later clip can undo a cull already taken.
    // cameraNormal is the rotated normal the cull already had to compute, handed
    // back so lighting need not repeat the transform. It is Vector3.Zero for a
    // face with no normal of its own - a detail line, culled below against the
    // faces it lies on - which has no single direction to light and so takes
    // the model's flat colour.
    private bool IsFacingCamera(int faceIndex, out Vector3 cameraNormal)
    {
        Face face = Model.Faces[faceIndex];
        Vector3 surfacePoint = _cameraList[face.PointIndices[0]];
        Vector3 normal = _geometry.FaceNormals[faceIndex];

        if (normal == Vector3.Zero)
        {
            cameraNormal = Vector3.Zero;
            return AnySharedVertexNormalFacesCamera(face, surfacePoint);
        }

        cameraNormal = RotateToCamera(normal);
        return Vector3.Dot(cameraNormal, surfacePoint) <= 0;
    }

    // A model-space normal in view. Lighting wants the rotated vector itself
    // and not just which side of the camera it falls, so the rotation is its
    // own step.
    private Vector3 RotateToCamera(Vector3 normal)
    {
        Vector4 rotated = Vector4.Transform(new Vector4(normal, 0), Rotmat);
        return new(rotated.X, rotated.Y, rotated.Z);
    }

    // True when a model-space normal, rotated into view, turns towards the
    // camera at the given camera-space point on the surface.
    private bool FacesCamera(Vector3 normal, Vector3 surfacePoint)
        => Vector3.Dot(RotateToCamera(normal), surfacePoint) <= 0;

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

    // As BuildFacePolygon, shading each corner and carrying the colours
    // through the clip so a corner the clipper invents gets the colour the
    // face had where the near plane cut it. No depth bias: only a face that
    // roots to itself gets here, and nothing sits in its plane to tie with.
    private Vector2[]? BuildShadedFacePolygon(
        int faceIndex,
        in Span<Vector3> cameraPoints,
        in Span<Vector3> clipped,
        in Span<FastColor> cornerColours,
        in Span<FastColor> clippedColours,
        out float[] depths,
        out FastColor[] colours)
    {
        Face face = Model.Faces[faceIndex];
        IReadOnlyList<Vector3> cornerNormals = CornerNormals[faceIndex];
        int numPoints = face.Points.Count;

        for (int j = 0; j < numPoints; j++)
        {
            cameraPoints[j] = _cameraList[face.PointIndices[j]];
            cornerColours[j] = _draw.ShadeVertex(face.Color, RotateToCamera(cornerNormals[j]), _geometry.FullyLit);
        }

        int count = NearPlaneClip.Clip(
            cameraPoints[..numPoints],
            cornerColours[..numPoints],
            NearPlane,
            clipped,
            clippedColours);

        if (count < 3)
        {
            depths = [];
            colours = [];
            return null;
        }

        Vector2[] polygon = new Vector2[count];
        depths = new float[count];
        colours = new FastColor[count];
        for (int j = 0; j < count; j++)
        {
            polygon[j] = ProjectCameraPoint(clipped[j]);
            depths[j] = clipped[j].Z;
            colours[j] = clippedColours[j];
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

        // A Viper's beam is the colour a Viper is on the scanner: both come
        // from the rendition's one definition of what a police ship looks like.
        FastColor color = _draw.Ships.For(Type == ShipType.Viper ? ShipClass.Police : ShipClass.Default);

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
