// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Graphics;

// Sutherland-Hodgman clipping of a camera-space polygon against the near
// plane. Both games need it: geometry crossing the camera plane projects to
// garbage otherwise, since the perspective divide flips sign through z = 0.
// The plane distance is a parameter because the two games work in different
// camera units.
public static class NearPlaneClip
{
    // Returns the number of resulting points written to the output span,
    // which needs room for one more point than the input.
    public static int Clip(in ReadOnlySpan<Vector3> input, float nearPlane, in Span<Vector3> output)
    {
        Span<Vector2> textureCoords = stackalloc Vector2[input.Length];
        Span<Vector2> clippedTextureCoords = stackalloc Vector2[input.Length + 1];
        return Clip(input, textureCoords, nearPlane, output, clippedTextureCoords);
    }

    // As above, interpolating a texture coordinate per point through the clip
    // (textureCoords pairs with input, outputTextureCoords with output).
    public static int Clip(
        in ReadOnlySpan<Vector3> input,
        in ReadOnlySpan<Vector2> textureCoords,
        float nearPlane,
        in Span<Vector3> output,
        in Span<Vector2> outputTextureCoords)
    {
        int count = 0;

        for (int i = 0; i < input.Length; i++)
        {
            int nextIndex = (i + 1) % input.Length;
            Vector3 current = input[i];
            Vector3 next = input[nextIndex];

            bool currentInside = current.Z >= nearPlane;
            bool nextInside = next.Z >= nearPlane;

            if (currentInside)
            {
                outputTextureCoords[count] = textureCoords[i];
                output[count++] = current;
            }

            if (currentInside != nextInside)
            {
                float t = (nearPlane - current.Z) / (next.Z - current.Z);
                outputTextureCoords[count] = Vector2.Lerp(textureCoords[i], textureCoords[nextIndex], t);
                output[count++] = new(
                    current.X + ((next.X - current.X) * t),
                    current.Y + ((next.Y - current.Y) * t),
                    nearPlane);
            }
        }

        return count;
    }
}
