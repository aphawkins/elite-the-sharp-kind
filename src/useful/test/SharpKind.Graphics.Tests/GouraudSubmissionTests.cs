// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Moq;
using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Tests;

// A caller submits per-vertex colours without first asking whether the
// strategy in force can blend them, so each strategy has to do something
// sensible with a Gouraud face.
public class GouraudSubmissionTests
{
    private static readonly Vector2[] s_points = [new(0, 0), new(10, 0), new(5, 10)];
    private static readonly float[] s_depths = [100f, 100f, 100f];

    private static readonly FastColor[] s_corners =
    [
        new(255, 0, 0, 0),
        new(255, 255, 255, 255),
        new(255, 255, 255, 255),
    ];

    [Fact]
    public void TheZBufferStrategyPassesTheCornersToTheFill()
    {
        Mock<IGraphics> graphics = MockSetup.MockGraphics();
        ZBufferRenderer renderer = new(graphics.Object);

        renderer.StartFrame();
        renderer.Submit(s_points, s_depths, s_corners, 100f, null);
        renderer.EndFrame();

        graphics.Verify(
            x => x.DrawPolygonFilledDepth(s_points, It.IsAny<float[]>(), It.Is<FastColor[]>(c => c.Length == 3), null),
            Times.Once);

        graphics.Verify(
            x => x.DrawPolygonFilledDepth(It.IsAny<Vector2[]>(), It.IsAny<float[]>(), It.IsAny<FastColor>(), It.IsAny<IColourQuantiser?>()),
            Times.Never);
    }

    // The corners are copied on submission, so a caller reusing its buffer
    // between faces cannot rewrite a face already queued for this frame.
    [Fact]
    public void TheZBufferStrategyCopiesTheCorners()
    {
        Mock<IGraphics> graphics = MockSetup.MockGraphics();
        FastColor[]? drawn = null;
        graphics
            .Setup(x => x.DrawPolygonFilledDepth(
                It.IsAny<Vector2[]>(),
                It.IsAny<float[]>(),
                It.IsAny<FastColor[]>(),
                It.IsAny<IColourQuantiser?>()))
            .Callback<Vector2[], float[], FastColor[], IColourQuantiser?>((_, _, colours, _) => drawn = colours);

        ZBufferRenderer renderer = new(graphics.Object);
        FastColor[] reused = [.. s_corners];

        renderer.StartFrame();
        renderer.Submit(s_points, s_depths, reused, 100f, null);
        reused[0] = new(255, 7, 7, 7);
        renderer.EndFrame();

        Assert.NotNull(drawn);
        Assert.Equal(s_corners[0], drawn[0]);
    }

    // A plain fill has no per-pixel hook, so the face stands down to the one
    // colour nearest what blending would have given.
    [Fact]
    public void ThePaintersStrategyFlattensTheFace()
    {
        Mock<IGraphics> graphics = MockSetup.MockGraphics();
        PainterRenderer renderer = new(graphics.Object);

        renderer.StartFrame();
        renderer.Submit(s_points, s_depths, s_corners, 100f, null);
        renderer.EndFrame();

        graphics.Verify(
            x => x.DrawPolygonFilled(s_points, VertexColours.Mean(s_corners), null),
            Times.Once);
    }

    // Two points are a detail line, which has no interior to blend across.
    [Fact]
    public void ATwoPointFaceIsDrawnAsALine()
    {
        Mock<IGraphics> graphics = MockSetup.MockGraphics();
        ZBufferRenderer renderer = new(graphics.Object);

        Vector2[] line = [new(0, 0), new(10, 0)];
        FastColor[] ends = [new(255, 0, 0, 0), new(255, 255, 255, 255)];

        renderer.StartFrame();
        renderer.Submit(line, [100f, 100f], ends, 100f, null);
        renderer.EndFrame();

        graphics.Verify(
            x => x.DrawLineDepth(
                line[0],
                line[1],
                It.IsAny<float>(),
                It.IsAny<float>(),
                VertexColours.Mean(ends),
                0),
            Times.Once);
    }
}
