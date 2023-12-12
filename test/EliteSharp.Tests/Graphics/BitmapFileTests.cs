// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Tests.Graphics
{
    public class BitmapFileTests
    {
        [Theory]
        [InlineData("2x2blacktopleft.bmp", 2, 2)]
        public void LoadBitmapOrientation(string filename, int width, int height)
        {
            // Arrange
            string path = Path.Combine("Graphics", filename);

            // Act
            FastBitmap bitmap = BitmapFile.Read(path);

            // Assert
            Assert.Equal(width, bitmap.Width);
            Assert.Equal(height, bitmap.Height);
            Assert.Equal(32, bitmap.BitsPerPixel);
            Assert.Equal(TestColors.Black, bitmap.GetPixel(0, 0));
            Assert.Equal(TestColors.TransparentBlack, bitmap.GetPixel(0, 1));
            Assert.Equal(TestColors.TransparentBlack, bitmap.GetPixel(1, 0));
            Assert.Equal(TestColors.TransparentBlack, bitmap.GetPixel(1, 1));
        }
    }
}
