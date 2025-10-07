// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
