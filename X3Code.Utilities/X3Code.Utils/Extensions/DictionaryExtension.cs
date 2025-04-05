using System;
using System.Collections.Generic;

namespace X3Code.Utils.Extensions;

/// <summary>
/// Contains some little helpers for Dictionaries
/// </summary>
public static class DictionaryExtension
{
    #region Overloads for Add-Method
    
    /// <summary>
    /// Tries to add a new key/value pair to the specified Dictionary from a KeyValuePair.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The dictionary to add the key/value pair to.</param>
    /// <param name="keyValuePair">The KeyValuePair containing the key/value pair to add.</param>
    /// <returns>True if the add is successful, false if the key already exists in the dictionary.</returns>
    public static bool Add<TKey, TValue>(this Dictionary<TKey, TValue> source, KeyValuePair<TKey, TValue> keyValuePair) where TKey : notnull
    {
        return source.TryAdd(keyValuePair.Key, keyValuePair.Value);
    }

    /// <summary>
    /// Tries to add a new key/value pair to the specified Dictionary from a tuple.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The dictionary to add the key/value pair to.</param>
    /// <param name="keyValueTuple">The tuple containing the key/value pair to add.</param>
    /// <returns>True if the add is successful, false if the key already exists in the dictionary.</returns>
    public static bool Add<TKey, TValue>(this Dictionary<TKey, TValue> source, (TKey, TValue) keyValueTuple) where TKey : notnull
    {
        return source.TryAdd(keyValueTuple.Item1, keyValueTuple.Item2);
    }
    
    #endregion
    
    #region add functionality

    /// <summary>
    /// Converts an IEnumerable of values to a Dictionary, using a specified key selector function, and optionally tracks duplicates.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the resulting dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the IEnumerable and resulting dictionary.</typeparam>
    /// <param name="values">The IEnumerable of values to convert to a dictionary.</param>
    /// <param name="keySelector">A function to extract the key for each value.</param>
    /// <param name="duplicates">An output list that contains any duplicate values if <paramref name="ignoreDuplicates"/> is true.</param>
    /// <param name="ignoreDuplicates">Determines whether duplicates should be ignored. If false, an exception is thrown when duplicates are encountered.</param>
    /// <returns>A dictionary containing the values as values and their selected keys.</returns>
    /// <exception cref="Exception">Thrown when a duplicate key is encountered and <paramref name="ignoreDuplicates"/> is false.</exception>
    public static Dictionary<TKey, TValue> ConvertToDictionary<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector, out List<TValue> duplicates, bool ignoreDuplicates = true) where TKey : notnull where TValue : class
    {
        duplicates = [];
        var result = new Dictionary<TKey, TValue>();
    
        foreach (var value in values)
        {
            var key = keySelector(value);
            var success = result.TryAdd(key, value);
            if (!success)
            {
                if (ignoreDuplicates)
                {
                    duplicates.Add(value);   
                }
                else
                {
                    throw new Exception($"Duplicate key [{key}] found.");
                }
            }
        }

        return result;
    }
    
    #endregion
}