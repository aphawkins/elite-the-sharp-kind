// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using SharpKind.Assets.Models;

namespace SharpKind.Graphics.Rendering;

/// <summary>
/// Placing a model's geometry somewhere: every point moved onto a frame, and
/// a face's own points gathered from the result.
/// </summary>
/// <remarks>
/// The step every renderer of a <see cref="ThreeDModel"/> starts with,
/// whatever it does next - one game carries on into a camera-space cull and a
/// near-plane clip, another hands the points straight to a depth-sorted world.
/// Rotation and translation are separate because the models carry
/// <c>w = 0</c> coordinates, which a matrix's translation row would not reach.
/// </remarks>
public static class MeshTransform
{
    /// <summary>
    /// Every point of the model, rotated onto the given frame and moved to the
    /// given origin.
    /// </summary>
    /// <param name="model">The model whose points to place.</param>
    /// <param name="rotation">
    /// The frame to turn the model onto - its rows are where the model's own
    /// x, y and z axes end up.
    /// </param>
    /// <param name="translation">Where the model's origin ends up.</param>
    /// <param name="destination">
    /// Receives one point per model point, in the model's own order. Must have
    /// room for all of them.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is too short for the model.
    /// </exception>
    public static void TransformPoints(
        ThreeDModel model,
        Matrix4x4 rotation,
        Vector3 translation,
        in Span<Vector3> destination)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (destination.Length < model.Points.Count)
        {
            throw new ArgumentException("Room is needed for every point of the model.", nameof(destination));
        }

        for (int i = 0; i < model.Points.Count; i++)
        {
            Vector4 placed = Vector4.Transform(model.Points[i].Coords, rotation);
            destination[i] = new Vector3(placed.X, placed.Y, placed.Z) + translation;
        }
    }

    /// <summary>
    /// One face's own points, gathered from points already placed by
    /// <see cref="TransformPoints"/>.
    /// </summary>
    /// <param name="model">The model the face belongs to.</param>
    /// <param name="faceIndex">Which of its faces to gather.</param>
    /// <param name="points">The model's placed points.</param>
    /// <param name="destination">
    /// Receives the face's points, in the order the face lists them. Must have
    /// room for all of them.
    /// </param>
    /// <returns>How many points the face has.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is too short for the face.
    /// </exception>
    public static int FacePoints(
        ThreeDModel model,
        int faceIndex,
        in ReadOnlySpan<Vector3> points,
        in Span<Vector3> destination)
    {
        ArgumentNullException.ThrowIfNull(model);

        IList<int> indices = model.Faces[faceIndex].PointIndices;

        if (destination.Length < indices.Count)
        {
            throw new ArgumentException("Room is needed for every point of the face.", nameof(destination));
        }

        for (int i = 0; i < indices.Count; i++)
        {
            destination[i] = points[indices[i]];
        }

        return indices.Count;
    }

    /// <summary>
    /// The most points any one of the model's faces has - what a caller sizes
    /// a single reused face buffer by.
    /// </summary>
    /// <param name="model">The model to measure.</param>
    /// <returns>Its largest face's point count, or zero if it has no faces.</returns>
    public static int MaxFacePoints(ThreeDModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        int most = 0;
        for (int i = 0; i < model.Faces.Count; i++)
        {
            most = Math.Max(most, model.Faces[i].PointIndices.Count);
        }

        return most;
    }
}
