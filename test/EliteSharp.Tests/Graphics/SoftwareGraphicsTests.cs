// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Tests.Graphics
{
    public class SoftwareGraphicsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        public void DrawPixelInBounds(float x, float y)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawPixel(new(x, y), EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            void DoAssert(FastBitmap bmp) => Assert.Equal(EliteColors.White, bmp.GetPixel((int)x, (int)y));
        }

        [Theory]
        [InlineData(-9, -9)]
        [InlineData(9, 9)]
        public void DrawPixelOutOfBounds(float x, float y)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, (_) => { });

            // Act
            graphics.DrawPixel(new(x, y), EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
        }

        [Fact]
        public void Clear()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawPixel(new(2, 2), EliteColors.White);
            graphics.Clear();
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp) => Assert.Equal(EliteColors.Black, bmp.GetPixel(2, 2));
        }

        [Fact]
        public void DrawCircleInBounds()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircle(new(2, 2), 2, EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(EliteColors.White, bmp.GetPixel(0, 2));
                Assert.Equal(EliteColors.White, bmp.GetPixel(4, 2));
                Assert.Equal(EliteColors.White, bmp.GetPixel(2, 0));
                Assert.Equal(EliteColors.White, bmp.GetPixel(2, 4));
            }
        }

        [Fact]
        public void DrawCirclePartialInBounds()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircle(new(0, 0), 4, EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(EliteColors.White, bmp.GetPixel(0, 4));
                Assert.Equal(EliteColors.White, bmp.GetPixel(4, 0));
            }
        }

        [Theory]
        [InlineData(2, 2, 3)]
        [InlineData(2, 2, 9)]
        [InlineData(-9, -9, 3)]
        [InlineData(9, 9, 3)]
        public void DrawCircleOutOfBounds(float x, float y, float radius)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircle(new(x, y), radius, EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                for (int screenY = 0; screenY < 5; screenY++)
                {
                    for (int screenX = 0; screenX < 5; screenX++)
                    {
                        Assert.Equal(EliteColors.Black, bmp.GetPixel(screenX, screenY));
                    }
                }
            }
        }

        [Fact]
        public void DrawCircleFilledInBounds()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircleFilled(new(2, 2), 2, EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(EliteColors.White, bmp.GetPixel(0, 2));
                Assert.Equal(EliteColors.White, bmp.GetPixel(4, 2));
                Assert.Equal(EliteColors.White, bmp.GetPixel(2, 0));
                Assert.Equal(EliteColors.White, bmp.GetPixel(2, 4));
                Assert.Equal(EliteColors.White, bmp.GetPixel(2, 2));
            }
        }

        [Theory]
        [InlineData(2, 2, 3, 0xFFFFFFFF)]
        [InlineData(2, 2, 9, 0xFFFFFFFF)]
        [InlineData(-9, -9, 3, 0xFF000000)]
        [InlineData(9, 9, 3, 0xFF000000)]
        public void DrawCircleFilledOutOfBounds(float x, float y, float radius, uint centreColor)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircleFilled(new(x, y), radius, EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            void DoAssert(FastBitmap bmp)
            {
                for (int screenY = 0; screenY < 5; screenY++)
                {
                    for (int screenX = 0; screenX < 5; screenX++)
                    {
                        Assert.Equal(new FastColor(centreColor), bmp.GetPixel(screenX, screenY));
                    }
                }
            }
        }

        [Theory]
        [InlineData(0, 0, 4, 4)]
        [InlineData(0, 4, 4, 0)]
        [InlineData(0, 0, 4, 0)]
        [InlineData(0, 0, 0, 4)]
        public void DrawLineInBounds(float startX, float startY, float endX, float endY)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawLine(new(startX, startY), new(endX, endY), EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(EliteColors.White, bmp.GetPixel((int)startX, (int)startY));
                Assert.Equal(EliteColors.White, bmp.GetPixel((int)endX, (int)endY));
            }
        }

        [Theory]
        [InlineData(0, 0, 5, 5)]
        [InlineData(-1, -1, 5, 5)]
        [InlineData(-1, 5, 5, -1)]
        [InlineData(-1, 2, 5, 2)]
        public void DrawLineOutOfBounds(float startX, float startY, float endX, float endY)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawLine(new(startX, startY), new(endX, endY), EliteColors.White);
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp) => Assert.Equal(EliteColors.White, bmp.GetPixel(2, 2));
        }

        [Theory]
        [InlineData("2x2redtopleft.bmp", 2, 2)]
        public void LoadImage(string filename, int width, int height)
        {
            // Arrange
            using SoftwareGraphics graphics = new(width, height, (_) => { });

            // Act
            graphics.LoadImage(ImageType.EliteText, Path.Combine("Graphics", filename));
            graphics.ScreenUpdate();

            // Assert
        }

        [Theory]
        [InlineData(2, 2, "2x2redtopleft.bmp", 0, 0)]
        [InlineData(4, 4, "2x2redtopleft.bmp", 2, 2)]
        public void DrawImage(int width, int height, string filename, int imageX, int imageY)
        {
            // Arrange
            using SoftwareGraphics graphics = new(width, height, DoAssert);
            graphics.LoadImage(ImageType.EliteText, Path.Combine("Graphics", filename));

            // Act
            graphics.DrawImage(ImageType.EliteText, new(imageX, imageY));
            graphics.ScreenUpdate();

            // Assert
            void DoAssert(FastBitmap bmp) => Assert.Equal(TestColors.OpaqueRed, bmp.GetPixel(imageX, imageY));
        }

        [Theory]
        [InlineData("2x2redtopleft.bmp", 2, 2)]
        public void DrawImageOutOfBounds(string filename, int imageWidth, int imageHeight)
        {
            // Arrange
            using SoftwareGraphics graphics = new(imageWidth, imageHeight, DoAssert);
            graphics.LoadImage(ImageType.EliteText, Path.Combine("Graphics", filename));

            // Act
            graphics.DrawImage(ImageType.EliteText, new(1, 1));
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp) => Assert.Equal(TestColors.OpaqueRed, bmp.GetPixel(1, 1));
        }

        [Theory]
        [InlineData("2x2redtopleft.bmp")]
        public void DrawImageTransparent(string filename)
        {
            // Arrange
            using SoftwareGraphics graphics = new(2, 2, DoAssert);
            graphics.DrawPixel(new(1, 1), TestColors.OpaqueWhite);
            graphics.LoadImage(ImageType.EliteText, Path.Combine("Graphics", filename));

            // Act
            graphics.DrawImage(ImageType.EliteText, new(0, 0));
            graphics.ScreenUpdate();

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(TestColors.OpaqueRed, bmp.GetPixel(0, 0));
                Assert.Equal(TestColors.OpaqueBlack, bmp.GetPixel(0, 1));
                Assert.Equal(TestColors.OpaqueBlack, bmp.GetPixel(1, 0));
                Assert.Equal(TestColors.OpaqueWhite, bmp.GetPixel(1, 1));
            }
        }
    }
}
