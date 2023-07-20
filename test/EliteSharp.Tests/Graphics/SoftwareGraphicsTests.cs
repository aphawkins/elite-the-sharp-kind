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
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawPixel(new(x, y), Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[(int)x, (int)y]);
        }

        [Theory]
        [InlineData(-9, -9)]
        [InlineData(9, 9)]
        public void DrawPixelOutOfBounds(float x, float y)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawPixel(new(x, y), Colour.White);

            // Assert
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        public void DrawPixelFastInBounds(float x, float y)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawPixelFast(new(x, y), Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[(int)x, (int)y]);
        }

        [Theory]
        [InlineData(-9, -9)]
        [InlineData(9, 9)]
        public void DrawPixelFastOutOfBounds(float x, float y)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawPixelFast(new(x, y), Colour.White);

            // Assert
        }

        [Theory]
        [InlineData(0, 0, 4, 4, Colour.Black)]
        [InlineData(0, 0, 9, 9, Colour.Black)]
        [InlineData(-9, -9, 18, 18, Colour.Black)]
        [InlineData(-9, -9, 9, 9, Colour.White)]
        [InlineData(3, 3, 9, 9, Colour.White)]
        public void ClearArea(float top, float left, float width, float height, Colour centreColour)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawPixel(new(2, 2), Colour.White);
            graphics.ClearArea(new(top, left), width, height);

            // Assert
            Assert.Equal((int)centreColour, buffer[2, 2]);
        }

        [Fact]
        public void DrawCircleInBounds()
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawCircle(new(2, 2), 2, Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[0, 2]);
            Assert.Equal((int)Colour.White, buffer[4, 2]);
            Assert.Equal((int)Colour.White, buffer[2, 0]);
            Assert.Equal((int)Colour.White, buffer[2, 4]);
        }

        [Fact]
        public void DrawCirclePartialInBounds()
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawCircle(new(0, 0), 4, Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[0, 4]);
            Assert.Equal((int)Colour.White, buffer[4, 0]);
        }

        [Theory]
        [InlineData(2, 2, 3)]
        [InlineData(2, 2, 9)]
        [InlineData(-9, -9, 3)]
        [InlineData(9, 9, 3)]
        public void DrawCircleOutOfBounds(float x, float y, float radius)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawCircle(new(x, y), radius, Colour.White);

            // Assert
            for (int bufferY = 0; bufferY < 5; bufferY++)
            {
                for (int bufferX = 0; bufferX < 5; bufferX++)
                {
                    Assert.Equal((int)Colour.Black, buffer[bufferX, bufferY]);
                }
            }
        }

        [Fact]
        public void DrawCircleFilledInBounds()
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawCircleFilled(new(2, 2), 2, Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[0, 2]);
            Assert.Equal((int)Colour.White, buffer[4, 2]);
            Assert.Equal((int)Colour.White, buffer[2, 0]);
            Assert.Equal((int)Colour.White, buffer[2, 4]);
            Assert.Equal((int)Colour.White, buffer[2, 2]);
        }

        [Theory]
        [InlineData(2, 2, 3, Colour.White)]
        [InlineData(2, 2, 9, Colour.White)]
        [InlineData(-9, -9, 3, Colour.Black)]
        [InlineData(9, 9, 3, Colour.Black)]
        public void DrawCircleFilledOutOfBounds(float x, float y, float radius, Colour centreColour)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawCircleFilled(new(x, y), radius, Colour.White);

            // Assert
            for (int bufferY = 0; bufferY < 5; bufferY++)
            {
                for (int bufferX = 0; bufferX < 5; bufferX++)
                {
                    Assert.Equal((int)centreColour, buffer[bufferX, bufferY]);
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
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawLine(new(startX, startY), new(endX, endY), Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[(int)startX, (int)startY]);
            Assert.Equal((int)Colour.White, buffer[(int)endX, (int)endY]);
        }

        [Theory]
        [InlineData(0, 0, 5, 5)]
        [InlineData(-1, -1, 5, 5)]
        [InlineData(-1, 5, 5, -1)]
        [InlineData(-1, 2, 5, 2)]
        public void DrawLineOutOfBounds(float startX, float startY, float endX, float endY)
        {
            // Arrange
            int[,] buffer = new int[5, 5];
            using SoftwareGraphics graphics = new(buffer);

            // Act
            graphics.DrawLine(new(startX, startY), new(endX, endY), Colour.White);

            // Assert
            Assert.Equal((int)Colour.White, buffer[2, 2]);
        }
    }
}
