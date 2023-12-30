// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Tests.Graphics
{
    public class FastColorTests
    {
        [Fact]
        public void FastColorEquals()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(BaseColors.TransparentWhite.Equals(BaseColors.TransparentWhite));
            Assert.Equal(BaseColors.TransparentWhite, BaseColors.TransparentWhite);
            Assert.Equal(BaseColors.TransparentWhite, new FastColor(0x00FFFFFF));
        }
    }
}
