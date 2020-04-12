using System;
using System.Collections.Generic;

namespace X3Code.Utils.Convert
{
    public static class ConvertListExtension
    {
        public static IEnumerable<TDestination> ConvertList<TSource, TDestination>(this IEnumerable<TSource> source, Func<TSource, TDestination> convertFunction)
        {
            var result = new List<TDestination>();
            foreach (var item in source)
            {
                var apiModel = convertFunction(item);
                result.Add(apiModel);
            }
            return result;
        }
    }
}
