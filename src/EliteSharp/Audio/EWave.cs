// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Audio
{
    public class EWave(byte[] bytes)
    {
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Bytes { get; } = bytes;
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
