// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.CompilerServices;

namespace EliteSharp
{
    public static class Guard
    {
        public static void ArgumentNull<T>(
            [ValidatedNotNull] T argument,
            [CallerArgumentExpression(nameof(argument))] string callerArg = "")
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(callerArg);
            }
        }
    }
}
