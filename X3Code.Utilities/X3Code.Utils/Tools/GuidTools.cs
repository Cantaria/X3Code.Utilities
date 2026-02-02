using System;

namespace X3Code.Utils.Tools;

/// <summary>
/// A utility class for working with GUIDs (Globally Unique Identifiers).
/// Provides methods for creating modified versions of Guid.Empty.
/// </summary>
public static class GuidTools
{
    /// <summary>
    /// Creates a new GUID based on the empty GUID, replacing all occurrences of the character '0'
    /// with the specified character.
    /// </summary>
    /// <param name="character">
    /// The character to replace all occurrences of '0' in the empty GUID.
    /// </param>
    /// <returns>
    /// A new <see cref="Guid"/> instance where all '0' characters in the empty GUID
    /// have been replaced with the specified character.
    /// </returns>
    public static Guid GuidEmptyWith(char character)
    {
        var result = Guid.Empty.ToString();
        return new Guid(result.Replace("0", character.ToString()));
    }

    /// <summary>
    /// Creates a new GUID based on an empty GUID, with all occurrences of the character '0'
    /// in the specified block replaced with the provided character.
    /// </summary>
    /// <param name="character">
    /// The character to replace occurrences of '0' in the specified block of the empty GUID.
    /// </param>
    /// <param name="blockIndex">
    /// The index of the block within the GUID where replacements will occur.
    /// Must be within the range of available blocks (0 to 4).
    /// </param>
    /// <returns>
    /// A new <see cref="Guid"/> with the specified modifications applied to the targeted block.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="blockIndex"/> is less than 0 or greater than the number of blocks in the GUID.
    /// </exception>
    public static Guid EmptyGuidWithChangedBlock(string character, int blockIndex)
    {
        var blocks = Guid.Empty.ToString().Split('-');
        if (blockIndex >= blocks.Length) throw new ArgumentOutOfRangeException(nameof(blockIndex));
        if (blockIndex < 0) throw new ArgumentOutOfRangeException(nameof(blockIndex));
        
        blocks[blockIndex] = blocks[blockIndex].Replace("0", character);
        return new Guid(string.Join("-", blocks));
    }
}