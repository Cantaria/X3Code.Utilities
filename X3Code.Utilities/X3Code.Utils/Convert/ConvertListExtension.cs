using System;
using System.Collections.Generic;
using System.Linq;

namespace X3Code.Utils.Convert;

/// <summary>
/// Useful extension for converting a list
/// </summary>
public static class ConvertListExtension
{
    /// <summary>
    /// Converts a whole list with help of the convertFunction from TSource to TDestination
    /// </summary>
    /// <typeparam name="TSource">The type of the source list that should be converted</typeparam>
    /// <typeparam name="TDestination">The type of the destination that you need</typeparam>
    /// <param name="source">The source list which should be converted</param>
    /// <param name="convertFunction">The function (or extension) which converts a single entity from TSource to TDestination</param>
    /// <returns>The converted list with the destination type</returns>
    public static IEnumerable<TDestination> ConvertList<TSource, TDestination>(this IEnumerable<TSource> source, Func<TSource, TDestination> convertFunction)
    {
        return source.Select(convertFunction);
    }
}