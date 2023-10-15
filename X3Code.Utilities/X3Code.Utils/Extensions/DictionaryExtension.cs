using System.Collections.Generic;

namespace X3Code.Utils.Extensions;

/// <summary>
/// Contains some little helpers for Dictionaries
/// </summary>
public static class DictionaryExtension
{
    /// <summary>
    /// Tries to add a new key/value pair to the specified Dictionary from a KeyValuePair.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The dictionary to add the key/value pair to.</param>
    /// <param name="keyValuePair">The KeyValuePair containing the key/value pair to add.</param>
    /// <returns>True if the add is successful, false if the key already exists in the dictionary.</returns>
    public static bool Add<TKey, TValue>(this Dictionary<TKey, TValue> source, KeyValuePair<TKey, TValue> keyValuePair)
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
    public static bool Add<TKey, TValue>(this Dictionary<TKey, TValue> source, (TKey, TValue) keyValueTuple)
    {
        return source.TryAdd(keyValueTuple.Item1, keyValueTuple.Item2);
    }
}