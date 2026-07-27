// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Moq;

[assembly: CLSCompliant(false)]

namespace Useful.Graphics.Tests;

internal static class MockSetup
{
    internal static Mock<IGraphics> MockGraphics() => new();
}
