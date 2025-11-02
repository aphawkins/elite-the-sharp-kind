// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Moq;
using Useful.Graphics;

[assembly: CLSCompliant(false)]

namespace EliteSharpLib.Tests;

internal static class MockSetup
{
    internal static Mock<IEliteDraw> MockDraw()
    {
        Mock<IEliteDraw> drawMoq = new();
        drawMoq.Setup(x => x.Graphics).Returns(MockGraphics().Object);
        drawMoq.Setup(x => x.Left).Returns(0);
        drawMoq.Setup(x => x.Right).Returns(511);
        drawMoq.Setup(x => x.Top).Returns(0);
        drawMoq.Setup(x => x.Bottom).Returns(511);
        drawMoq.Setup(x => x.Centre).Returns(new Vector2(255, 255));
        return drawMoq;
    }

    internal static Mock<IGraphics> MockGraphics()
    {
        Mock<IGraphics> graphicsMoq = new();
        graphicsMoq.Setup(x => x.Scale).Returns(2);
        return graphicsMoq;
    }
}
