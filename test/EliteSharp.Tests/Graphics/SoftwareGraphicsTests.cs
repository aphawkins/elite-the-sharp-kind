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
            graphics.DrawPixel(new(x, y), FastColors.White);
            graphics.ScreenUpdate();

            // Assert
            void DoAssert(FastBitmap bmp) => Assert.Equal(FastColors.White, bmp.GetPixel((int)x, (int)y));
        }

        [Theory]
        [InlineData(-9, -9)]
        [InlineData(9, 9)]
        public void DrawPixelOutOfBounds(float x, float y)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, (_) => { });

            // Act
            graphics.DrawPixel(new(x, y), FastColors.White);

            // Assert
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        public void DrawPixelFastInBounds(float x, float y)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawPixelFast(new(x, y), FastColors.White);

            // Assert
            void DoAssert(FastBitmap bmp) => Assert.Equal(FastColors.White, bmp.GetPixel((int)x, (int)y));
        }

        [Theory]
        [InlineData(-9, -9)]
        [InlineData(9, 9)]
        public void DrawPixelFastOutOfBounds(float x, float y)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, (_) => { });

            // Act
            graphics.DrawPixelFast(new(x, y), FastColors.White);

            // Assert
        }

        [Fact]
        public void Clear()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawPixel(new(2, 2), FastColors.White);
            graphics.Clear();

            // Assert
            static void DoAssert(FastBitmap bmp) => Assert.Equal(FastColors.Black, bmp.GetPixel(2, 2));
        }

        [Fact]
        public void DrawCircleInBounds()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircle(new(2, 2), 2, FastColors.White);

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(FastColors.White, bmp.GetPixel(0, 2));
                Assert.Equal(FastColors.White, bmp.GetPixel(4, 2));
                Assert.Equal(FastColors.White, bmp.GetPixel(2, 0));
                Assert.Equal(FastColors.White, bmp.GetPixel(2, 4));
            }
        }

        [Fact]
        public void DrawCirclePartialInBounds()
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircle(new(0, 0), 4, FastColors.White);

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(FastColors.White, bmp.GetPixel(0, 4));
                Assert.Equal(FastColors.White, bmp.GetPixel(4, 0));
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
            graphics.DrawCircle(new(x, y), radius, FastColors.White);

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                for (int screenY = 0; screenY < 5; screenY++)
                {
                    for (int screenX = 0; screenX < 5; screenX++)
                    {
                        Assert.Equal(FastColors.Black, bmp.GetPixel(screenX, screenY));
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
            graphics.DrawCircleFilled(new(2, 2), 2, FastColors.White);

            // Assert
            static void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(FastColors.White, bmp.GetPixel(0, 2));
                Assert.Equal(FastColors.White, bmp.GetPixel(4, 2));
                Assert.Equal(FastColors.White, bmp.GetPixel(2, 0));
                Assert.Equal(FastColors.White, bmp.GetPixel(2, 4));
                Assert.Equal(FastColors.White, bmp.GetPixel(2, 2));
            }
        }

        [Theory]
        [InlineData(2, 2, 3, 0xFFFFFF)]
        [InlineData(2, 2, 9, 0xFFFFFF)]
        [InlineData(-9, -9, 3, 0x000000)]
        [InlineData(9, 9, 3, 0x000000)]
        public void DrawCircleFilledOutOfBounds(float x, float y, float radius, int centreColour)
        {
            // Arrange
            using SoftwareGraphics graphics = new(5, 5, DoAssert);

            // Act
            graphics.DrawCircleFilled(new(x, y), radius, FastColors.White);

            // Assert
            void DoAssert(FastBitmap bmp)
            {
                for (int screenY = 0; screenY < 5; screenY++)
                {
                    for (int screenX = 0; screenX < 5; screenX++)
                    {
                        Assert.Equal(new FastColor(centreColour), bmp.GetPixel(screenX, screenY));
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
            graphics.DrawLine(new(startX, startY), new(endX, endY), FastColors.White);

            // Assert
            void DoAssert(FastBitmap bmp)
            {
                Assert.Equal(FastColors.White, bmp.GetPixel((int)startX, (int)startY));
                Assert.Equal(FastColors.White, bmp.GetPixel((int)endX, (int)endY));
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
            graphics.DrawLine(new(startX, startY), new(endX, endY), FastColors.White);

            // Assert
            static void DoAssert(FastBitmap bmp) => Assert.Equal(FastColors.White, bmp.GetPixel(2, 2));
        }
    }
}
