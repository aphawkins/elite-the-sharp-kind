// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Immutable;

namespace EliteSharp.Missions.Abstractions;

/// <summary>
/// One screen of a mission's message sequence. A single screen draws every
/// mission's briefings, so it lays itself out from what is here - whether there
/// is a headline, how many paragraphs there are, whether somebody is pictured,
/// whether a ship is posing behind the text - and never from which mission or
/// which stage it came.
/// <para>
/// The parts are set by name, and each is checked as it is set, so a briefing
/// that has been built is a briefing that can be drawn - including one built
/// with <c>with</c> from another.
/// </para>
/// </summary>
public sealed record MissionBriefing
{
    /// <summary>
    /// Gets the message, split where it should be blocked on screen. There is
    /// always at least one paragraph: a briefing with nothing to say is a
    /// screen the commander stares at. It is an immutable array so that a
    /// mission cannot go on editing a briefing it has already handed over.
    /// </summary>
    /// <exception cref="ArgumentException">There are no paragraphs, or one of them is blank.</exception>
    public required ImmutableArray<string> Paragraphs
    {
        get;

        init
        {
            if (value.IsDefaultOrEmpty)
            {
                throw new ArgumentException("A briefing with nothing to say is not a briefing.", nameof(value));
            }

            if (value.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("A blank paragraph is a gap on the screen.", nameof(value));
            }

            field = value;
        }
    }

    /// <summary>
    /// Gets the line set above the message, or null when the message stands on
    /// its own. The screen sets a headline in the larger font, so having one is
    /// what tells a congratulation from a plain message.
    /// </summary>
    /// <exception cref="ArgumentException">The headline is blank rather than absent.</exception>
    public string? Headline
    {
        get;

        init
        {
            if (value is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
            }

            field = value;
        }
    }

    /// <summary>
    /// Gets who is speaking, drawn beside the message, or null when nobody is
    /// shown.
    /// </summary>
    public MissionPortrait? Portrait { get; init; }

    /// <summary>
    /// Gets the ship posing behind the text, by the name the game's ship list
    /// knows it by, or null for none - how a briefing about a particular ship
    /// shows the commander what to look for. The game spawns it as it shows the
    /// briefing, so a briefing the game never shows spawns nothing.
    /// </summary>
    /// <exception cref="ArgumentException">The ship name is blank rather than absent.</exception>
    public string? ShipName
    {
        get;

        init
        {
            if (value is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
            }

            field = value;
        }
    }

    /// <summary>
    /// Compares two briefings by what they say. The compiler's own comparison
    /// would compare the paragraphs by reference, which would make two
    /// briefings with the same words unequal and leave this record without the
    /// value semantics it exists for.
    /// </summary>
    /// <param name="other">The briefing to compare with.</param>
    /// <returns>True when both briefings would draw the same screen.</returns>
    public bool Equals(MissionBriefing? other)
        => other is not null
            && Portrait == other.Portrait
            && string.Equals(Headline, other.Headline, StringComparison.Ordinal)
            && string.Equals(ShipName, other.ShipName, StringComparison.Ordinal)
            && Paragraphs.SequenceEqual(other.Paragraphs, StringComparer.Ordinal);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(Headline);
        hash.Add(Portrait);
        hash.Add(ShipName);

        foreach (string paragraph in Paragraphs)
        {
            hash.Add(paragraph);
        }

        return hash.ToHashCode();
    }
}
