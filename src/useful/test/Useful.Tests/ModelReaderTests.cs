// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets.Models;
using Useful.Assets.Palettes;
using Xunit;

namespace Useful.Tests;

public class ModelReaderTests
{
    [Fact]
    public void ReadPopulatesPerPointFaceNormalsWithoutInflatingModelFaceNormals()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
        const string json = /*lang=json,strict*/ """
        {
          "FaceNormals": [
            [ 1, 0, 0, 1 ],
            [ 2, 0, 1, 0 ]
          ],
          "Faces": [
            [ "White", 0, 0, 1, 0, 1, 2 ]
          ],
          "Lines": [
            [ 1, 0, 1, 0, 1 ]
          ],
          "Points": [
            [ 1, 2, 3, 10, 0, 1 ],
            [ 4, 5, 6, 20, 0 ],
            [ 7, 8, 9, 30 ]
          ]
        }
        """;
        File.WriteAllText(tempFile, json);
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

    private sealed class TestPalette : Dictionary<string, uint>, IPaletteCollection;
}
