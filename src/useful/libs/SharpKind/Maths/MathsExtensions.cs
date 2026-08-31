// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Maths;

public static class MathsExtensions
{
    public static bool IsOdd(this int value) => value % 2 != 0;

    public static bool IsOdd(this float value) => ((int)value).IsOdd();
}
