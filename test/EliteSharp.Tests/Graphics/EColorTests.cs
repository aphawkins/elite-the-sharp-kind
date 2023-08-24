// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Tests.Graphics
{
    public class EColorTests
    {
        [Fact]
        public void EColorEquals()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(EColors.DarkerGrey.Equals(EColors.DarkerGrey));
            Assert.Equal(EColors.DarkerGrey, EColors.DarkerGrey);
            Assert.Equal(EColors.DarkerGrey, new EColor(0x606060));
        }
    }
}
