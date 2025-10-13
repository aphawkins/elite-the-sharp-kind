// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful;

public class UsefulException : Exception
{
    public UsefulException(string message)
        : base(message)
    {
    }

    public UsefulException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UsefulException()
    {
    }
}
