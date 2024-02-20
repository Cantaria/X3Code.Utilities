using System.Collections.Generic;
using System.Linq;

namespace X3Code.Utils.Extensions;

/// <summary>
/// Contains little helpers for list
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// Return true, if the list is null or contains no elements.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        if (source == null || !source.Any()) return true;

        return false;
    }
}