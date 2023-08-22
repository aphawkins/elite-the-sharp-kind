// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Tests
{
    public class VectorMathsTests
    {
        [Fact]
        public void GetInitialMatrix()
        {
            Matrix4x4 matrix = VectorMaths.GetInitialMatrix();
            Matrix4x4 matrixExpected = new(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0);

            Assert.Equal(matrixExpected, matrix);
        }

        [Fact]
        public void RotateVector()
        {
            Matrix4x4 matrix = new(3, 5, 7, 0, 11, 13, 17, 0, 19, 23, 29, 0, 0, 0, 0, 0);
            Matrix4x4 rotated = VectorMaths.Rotate(matrix, 31, 37);
            Matrix4x4 matrixExpected = new(-2539, -4397, -6193, 0, -785, -993, -1273, 0, -29026, -36718, -47072, 0, 0, 0, 0, 0);

            Assert.Equal(matrixExpected, rotated);

            // Would expect these to be equal?
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0, 37, 31);
            Matrix4x4 transpose = Matrix4x4.Transpose(matrix);
            Matrix4x4 rotated1 = Matrix4x4.Transform(transpose, rotation);

            Assert.Equal(rotated, rotated1);
        }
    }
}
