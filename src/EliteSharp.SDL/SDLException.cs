// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.SDL
{
    public sealed class SDLException : Exception
    {
        public SDLException(string message)
            : base(message)
        {
        }

        public SDLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SDLException()
        {
        }
    }
}
