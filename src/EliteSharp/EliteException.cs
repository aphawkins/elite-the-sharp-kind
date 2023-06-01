// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp
{
    public sealed class EliteException : Exception
    {
        public EliteException(string message)
            : base(message)
        {
        }

        public EliteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EliteException()
        {
        }
    }
}
