// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.SDL;

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
