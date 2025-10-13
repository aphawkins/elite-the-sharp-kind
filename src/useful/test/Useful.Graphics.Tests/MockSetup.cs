// 'Useful Libraries' - Andy Hawkins 2025.

using Moq;

[assembly: CLSCompliant(false)]

namespace Useful.Graphics.Tests;

internal static class MockSetup
{
    internal static Mock<IGraphics> MockGraphics()
    {
        Mock<IGraphics> graphicsMoq = new();
        graphicsMoq.Setup(x => x.Scale).Returns(2);
        return graphicsMoq;
    }
}
