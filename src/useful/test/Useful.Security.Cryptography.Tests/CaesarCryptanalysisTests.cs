// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class CaesarCryptanalysisTests
{
    [Theory]
    [InlineData("AAAAAAAABBCCCDDDDEEEEEEEEEEEEEFFGGHHHHHHIIIIIIIKLLLLMMNNNNNNNOOOOOOOOPPRRRRRRSSSSSSSSSTTTTTTTTTUUUVWWYY", 0)]
    [InlineData("YMJHFJXFWHNUMJWNXTSJTKYMJJFWQNJXYPSTBSFSIXNRUQJXYHNUMJWX", 5)] // http://practicalcryptography.com/cryptanalysis/stochastic-searching/cryptanalysis-caesar-cipher/
    [InlineData("MHILY LZA ZBHL XBPZXBL MVYABUHL HWWPBZ JSHBKPBZ JHLJBZ KPJABT HYJHUBT LZA ULBAYVU", 7)] // Singh Code Book
    [InlineData("QFM", 12)]
    public void Crack(string ciphertext, int shift)
    {
        (int bestShift, IDictionary<int, string> _) = CaesarCryptanalysis.Crack(ciphertext);
        Assert.Equal(shift, bestShift);
    }
}
