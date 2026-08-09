// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text;
using Xunit;

namespace Useful.Security.Cryptography.Tests;

public class VigenereTests
{
    public static TheoryData<string, string, string> Data => new()
    {
        { "ATTACKATDAWN", "LXFOPVEFRNHR", "LEMON" },
    };

    [Fact]
    public void CtorSettings()
    {
        Vigenere cipher = new(new VigenereSettings() { Keyword = "Hello" });
        Assert.Equal("HELLO", cipher.Settings.Keyword);
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("ATTACKATDAWN", "LxFoPvEfRnHr", "LEMON")]
    [InlineData("ATTACKATDAWN", "ATTACKatDAWN", "")]
    public void Decrypt(string plaintext, string ciphertext, string keyword)
    {
        Vigenere cipher = new(new VigenereSettings());
        cipher.Settings.Keyword = keyword;
        Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
    }

    [Theory]
    [MemberData(nameof(Data))]
    [InlineData("AtTaCkAtDaWn", "LXFOPVEFRNHR", "LEMON")]
    [InlineData("ATTACKatDAWN", "ATTACKATDAWN", "")]
    public void Encrypt(string plaintext, string ciphertext, string keyword)
    {
        Vigenere cipher = new(new VigenereSettings());
        cipher.Settings.Keyword = keyword;
        Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
    }

    [Fact]
    public void GenerateSettings()
    {
        Vigenere cipher = new(new VigenereSettings());

        const int testsCount = 5;
        string[] keywords = new string[testsCount];
        for (int i = 0; i < testsCount; i++)
        {
            cipher.GenerateSettings();
            keywords[i] = cipher.Settings.Keyword;
        }

        Assert.True(keywords.Distinct().Count() > 1);
        Assert.All(keywords, keyword => Assert.Matches("^[A-Z]{1,25}$", keyword));
    }

    [Theory]
    [InlineData("LEM0N")] // Digit
    [InlineData("LEM ON")] // Space
    [InlineData("LEM-ON")] // Punctuation
    public void KeywordInvalidCharacters(string keyword)
        => Assert.Throws<ArgumentException>("Keyword", () => new VigenereSettings() { Keyword = keyword });

    [Fact]
    public void KeywordTooLong()
        => Assert.Throws<ArgumentOutOfRangeException>(
            "Keyword",
            () => new VigenereSettings() { Keyword = new string('A', 26) });

    [Theory]
    [InlineData("ATTACK AT DAWN!", "LXFOPV EF RNHR!", "LEMON")] // Non-letters pass through and don't consume the keyword
    public void EncryptSkipsNonLetters(string plaintext, string ciphertext, string keyword)
    {
        ArgumentNullException.ThrowIfNull(plaintext);

        Vigenere cipher = new(new VigenereSettings() { Keyword = keyword });
        Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
        Assert.Equal(plaintext.ToUpperInvariant(), cipher.Decrypt(ciphertext));
    }

    [Fact]
    public void Name()
    {
        Vigenere cipher = new(new VigenereSettings());
        Assert.Equal("Vigenere", cipher.CipherName);
        Assert.Equal("Vigenere", cipher.ToString());
    }

    [Fact]
    public void SinghCodeBook()
    {
        string ciphertext = new StringBuilder()
            .Append("KQOWEFV JPUJ UUNUKGL MEK JINMWU XFQMKJBGW ")
            .Append("RLFNFGHU DWU UMBSVLPS NCMUEK QCTESWR EEK OYSS ")
            .Append("IWC TUAXYOT APXPLWPNT CGOJBGFQHT DW XIZAYG ")
            .Append("FF NSXCSE YNCTSSPN TUJ NYT GGWZGRWU UNEJU ")
            .Append("U QEAPY MEK QHUIDU XFPGUYT SMT FFS HNUOCZGM ")
            .Append("RUW EYT RGKM EE DCTVR ECFBDJQCUS WV BPNLGOY ")
            .Append("LSKMTEFV JJTWWMFMWPN MEMTM HRSPXFS SKFFS TNUOCZGM ")
            .Append("DOEOY EEK CPJRGPM URSKHFR S EIUE VGOY ")
            .Append("CW XIZAYGOS AANY DOEOY JL WUN HAMEBF EL XYVLW ")
            .Append("NOJ NSIOFRW UC CESW KVID GMU CGOCRUW GN MAAF ")
            .Append("FVN SIUDE KQH CEU CPFC MP VSUDG AVEMNY ")
            .Append("MAMVLF MAOY FN TQCUAFV FJNXKLNE IWC WODCCU ")
            .Append("LW RIFTW GMU SWOVMATNY BU HTCOCW FYT NMGYT ")
            .Append("QMK BBNLG FB TWOJFTW GN TE JKN EE DCLDHWT ")
            .Append("VBUVGFBIJG ")
            .Append("YYIDG MVR DG MPL SW GJLAGO EEK JOFEK ")
            .Append("NY NOL RIVR WVUHE IWUURW GMU TJCDBN ")
            .Append("KGM BIDGM EEYGUOT DGGQEUJYOT VG GBRUJYS")
            .ToString();

        string plaintext = new StringBuilder()
            .Append("SOUVENT POUR SAMUSER LES HOMMES DEQUIPAGE ")
            .Append("PRENNENT DES ALBATROS VASTES OISEAUX DES MERS ")
            .Append("QUI SUIVENT INDOLENTS COMPAGNONS DE VOYAGE ")
            .Append("LE NAVIRE GLISSANT SUR LES GOUFFRES AMERS ")
            .Append("A PEINE LES ONTILS DEPOSES SUR LES PLANCHES ")
            .Append("QUE CES ROIS DE LAZUR MALADROITS ET HONTEUX ")
            .Append("LAISSENT PITEUSEMENT LEURS GRANDES AILES BLANCHES ")
            .Append("COMME DES AVIRONS TRAINER A COTE DEUX ")
            .Append("CE VOYAGEUR AILE COMME IL EST GAUCHE ET VEULE ")
            .Append("LUI NAGUERE SI BEAU QUIL EST COMIQUE ET LAID ")
            .Append("LUN AGACE SON BEC AVEC UN BRULE GUEULE ")
            .Append("LAUTRE MIME EN BOITANT LINFIRME QUI VOLAIT ")
            .Append("LE POETE EST SEMBLABLE AU PRINCE DES NUEES ")
            .Append("QUI HANTE LA TEMPETE ET SE RIT DE LARCHER ")
            .Append("BAUDELAIRE ")
            .Append("EXILE SUR LE SOL AU MILIEU DES HUEES ")
            .Append("LE MOT POUR ETAGE QUATRE EST TRAJAN ")
            .Append("SES AILES DEGEANT LEMPECHENT DE MARCHER")
            .ToString();

        Vigenere cipher = new(
            new VigenereSettings()
            {
                Keyword = "SCUBA",
            });
        Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
        Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
    }
}
