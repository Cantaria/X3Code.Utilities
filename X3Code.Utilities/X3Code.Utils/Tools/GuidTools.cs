using System;

namespace X3Code.Utils.Tools;

/// <summary>
/// A utility class for working with GUIDs (Globally Unique Identifiers).
/// Provides methods for creating modified versions of Guid.Empty. Like "00000000-0000-1111-0000-000000000000"
/// These kinds of guids are often used as identifiers in systems that require unique, persistent values.
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
    public static Guid EmptyGuidWithChangedBlock(char character, int blockIndex)
    {
        var blocks = Guid.Empty.ToString().Split('-');
        if (blockIndex >= blocks.Length) throw new ArgumentOutOfRangeException(nameof(blockIndex));
        if (blockIndex < 0) throw new ArgumentOutOfRangeException(nameof(blockIndex));
        
        blocks[blockIndex] = blocks[blockIndex].Replace("0", character.ToString());
        return new Guid(string.Join("-", blocks));
    }

    /// <summary>
    /// Replaces the specified block of the GUID with a given replacement string and returns a new GUID instance.
    /// </summary>
    /// <param name="source">
    /// The original <see cref="Guid"/> instance to modify.
    /// </param>
    /// <param name="replacement">
    /// The string to replace the specified block of the GUID. The replacement string must have the same length as the original block.
    /// </param>
    /// <param name="blockIndex">
    /// The index of the block in the GUID to replace. Must be within the valid range of block indices for a GUID.
    /// </param>
    /// <returns>
    /// A new <see cref="Guid"/> instance with the specified block replaced by the replacement string.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the specified block index is not within the valid range of block indices for a GUID.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown if the length of the replacement string does not match the length of the original block being replaced.
    /// </exception>
    public static Guid ReplaceGuidBlockWith(this Guid source, string replacement, int blockIndex)
    {
        var blocks = source.ToString().Split('-');
        if (blockIndex >= blocks.Length || blockIndex < 0) throw new ArgumentOutOfRangeException(nameof(blockIndex));

        if (replacement.Length != blocks[blockIndex].Length) throw new Exception("The replacement text must have the same length as the original");
        blocks[blockIndex] = replacement;
        return new Guid(string.Join("-", blocks));
    }
}