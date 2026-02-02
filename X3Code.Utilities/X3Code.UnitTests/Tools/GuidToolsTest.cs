using System;
using X3Code.Utils.Tools;
using Xunit;

namespace X3Code.UnitTests.Tools;

public class GuidToolsTest
{
    [Theory]
    [InlineData('1', "11111111-1111-1111-1111-111111111111")]
    [InlineData('a', "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")]
    [InlineData('f', "ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public void GuidEmptyWith_ShouldReplaceAllZeros(char character, string expectedGuid)
    {
        var result = GuidTools.GuidEmptyWith(character);
        Assert.Equal(new Guid(expectedGuid), result);
        Assert.Equal(expectedGuid, result.ToString());
    }

    [Fact]
    public void GuidEmptyWith_ShouldThrowIfInvalidHexCharacter()
    {
        // GUIDs only support 0-9, a-f. Replacing with 'g' should result in a FormatException when creating the Guid.
        Assert.Throws<FormatException>(() => GuidTools.GuidEmptyWith('g'));
    }

    [Theory]
    [InlineData('1', 0, "11111111-0000-0000-0000-000000000000")]
    [InlineData('1', 1, "00000000-1111-0000-0000-000000000000")]
    [InlineData('1', 2, "00000000-0000-1111-0000-000000000000")]
    [InlineData('1', 3, "00000000-0000-0000-1111-000000000000")]
    [InlineData('1', 4, "00000000-0000-0000-0000-111111111111")]
    public void EmptyGuidWithChangedBlock_ShouldReplaceSpecificBlock(char replacement, int blockIndex, string expectedGuid)
    {
        var result = GuidTools.EmptyGuidWithChangedBlock(replacement, blockIndex);
        Assert.Equal(new Guid(expectedGuid), result);
        Assert.Equal(expectedGuid, result.ToString());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    [InlineData(10)]
    public void EmptyGuidWithChangedBlock_ShouldThrowIfIndexOutOfRange(int blockIndex)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GuidTools.EmptyGuidWithChangedBlock('1', blockIndex));
    }

    [Fact]
    public void EmptyGuidWithChangedBlock_ShouldThrowIfReplacementIsInvalid()
    {
        // Replacing a block with non-hex characters should throw FormatException when Guid is instantiated
        Assert.Throws<FormatException>(() => GuidTools.EmptyGuidWithChangedBlock('g', 1));
    }

    [Theory]
    [InlineData("12345678", 0, "12345678-0000-0000-0000-000000000000")]
    [InlineData("1234", 1, "00000000-1234-0000-0000-000000000000")]
    [InlineData("abcd", 2, "00000000-0000-abcd-0000-000000000000")]
    [InlineData("beef", 3, "00000000-0000-0000-beef-000000000000")]
    [InlineData("1234567890ab", 4, "00000000-0000-0000-0000-1234567890ab")]
    public void ReplaceGuidBlockWith_ShouldReplaceSpecificBlock(string replacement, int blockIndex, string expectedGuid)
    {
        var result = Guid.Empty.ReplaceGuidBlockWith(replacement, blockIndex);
        Assert.Equal(new Guid(expectedGuid), result);
        Assert.Equal(expectedGuid, result.ToString());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void ReplaceGuidBlockWith_ShouldThrowIfIndexOutOfRange(int blockIndex)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guid.Empty.ReplaceGuidBlockWith("1234", blockIndex));
    }

    [Theory]
    [InlineData("123", 1)] // Too short
    [InlineData("12345", 1)] // Too long
    [InlineData("1234567", 0)] // Too short for block 0
    public void ReplaceGuidBlockWith_ShouldThrowIfLengthMismatch(string replacement, int blockIndex)
    {
        Assert.Throws<Exception>(() => Guid.Empty.ReplaceGuidBlockWith(replacement, blockIndex));
    }

    [Fact]
    public void ReplaceGuidBlockWith_ShouldThrowIfReplacementIsInvalid()
    {
        Assert.Throws<FormatException>(() => Guid.Empty.ReplaceGuidBlockWith("gggg", 1));
    }
}
