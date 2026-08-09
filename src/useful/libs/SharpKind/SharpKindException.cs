// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind;

public class SharpKindException : Exception
{
    public SharpKindException(string message)
        : base(message)
    {
    }

    public SharpKindException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SharpKindException()
    {
    }
}
