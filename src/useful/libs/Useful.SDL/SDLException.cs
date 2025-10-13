// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.SDL;

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
