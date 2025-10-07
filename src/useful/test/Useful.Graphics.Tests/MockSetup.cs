// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
