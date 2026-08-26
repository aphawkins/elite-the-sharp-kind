// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind;

namespace EliteSharpLib;

/// <summary>
/// The entropy the drawing uses: laser-aim shimmer and the scatter of an
/// explosion's debris. A separate stream from <see cref="RNG"/>, which is
/// the game's.
/// </summary>
/// <remarks>
/// <para>
/// These used to come out of <see cref="RNG"/> along with the encounter
/// rolls, the bounties and the tactics, and that made the simulation depend
/// on what happened to be on screen. Every one of these draws sits behind a
/// visibility test - <c>ShipBase.DrawLasers</c> returns before its two draws
/// when the laser mount faces away, and the explosion scatters sixteen
/// blocks around each <i>visible</i> projected point - so turning the camera
/// away from an explosion changed how many numbers were taken and therefore
/// which encounters arrived later.
/// </para>
/// <para>
/// It is a distinct type rather than a second <see cref="RNG"/> registration
/// so the two cannot be confused at a constructor: a class that draws takes
/// this, a class that plays the game takes the other, and nothing has to
/// remember which of two identical parameters it was handed.
/// </para>
/// </remarks>
internal sealed class RenderRandom(Random random) : IRandomSource
{
    private readonly RandomSource _source = new(random);

    public int NextInt() => _source.NextInt();

    public int Random(int toExclusive) => _source.Random(toExclusive);

    public int Random(int fromInclusive, int toExclusive) => _source.Random(fromInclusive, toExclusive);

    public bool TrueOrFalse() => _source.TrueOrFalse();

    public int GaussianRandom(int min, int max) => _source.GaussianRandom(min, max);
}
