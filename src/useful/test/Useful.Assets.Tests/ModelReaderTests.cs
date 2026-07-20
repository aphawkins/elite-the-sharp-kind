// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets.Models;
using Useful.Assets.Palettes;
using Xunit;

namespace Useful.Assets.Tests;

public class ModelReaderTests
{
    [Fact]
    public void ReadPopulatesPerPointFaceNormalsWithoutInflatingModelFaceNormals()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.obj");
        const string obj = """
        v 1 2 3
        v 4 5 6
        v 7 8 9

        vn 0 0 1
        vn 0 1 0

        usemtl White
        f 1 2 3

        l 1 2

        # pn 1 1
        # pn 1 2
        # pn 2 1
        """;
        File.WriteAllText(tempFile, obj);
        TestPalette palette = new() { ["White"] = 0 };

        try
        {
            // Act
            ThreeDModel model = ModelReader.Read(tempFile, palette);

            // Assert: the model-level FaceNormals collection must stay at its original size -
            // regression test for a bug where per-point face-normal references were mistakenly
            // appended to this list instead of the point's own FaceNormals collection.
            Assert.Equal(2, model.FaceNormals.Count);

            // Point 0 references face-normal indices 0 and 1.
            Assert.Equal(2, model.Points[0].FaceNormals.Count);
            Assert.Same(model.FaceNormals[0], model.Points[0].FaceNormals[0]);
            Assert.Same(model.FaceNormals[1], model.Points[0].FaceNormals[1]);

            // Point 1 references face-normal index 0 only.
            Assert.Single(model.Points[1].FaceNormals);
            Assert.Same(model.FaceNormals[0], model.Points[1].FaceNormals[0]);

            // Point 2 has no face-normal references.
            Assert.Empty(model.Points[2].FaceNormals);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    private sealed class TestPalette : Dictionary<string, FastColor>, IPaletteCollection;
}
