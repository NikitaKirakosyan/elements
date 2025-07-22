using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any() || collection.All(item => item == null);
    }
    
    public static TSource GetRandomElement<TSource>(this IEnumerable<TSource> source)
    {
        if(source.IsNullOrEmpty())
            return default;
        
        var count = source.Count();
        
        if(count == 0)
            return default;
        
        var randomIndex = new Random().Next(0, count);
        return source.ElementAt(randomIndex);
    }
    
    public static List<TSource> GetRandomElements<TSource>(this IEnumerable<TSource> source, int count)
    {
        if(source.IsNullOrEmpty() || count == 0)
            return null;
        
        var sourceCount = source.Count();
        
        if(sourceCount == 0)
            return null;
        
        var freeIndexes = new List<int>();
        for(var i = 0; i < source.Count(); i++)
            freeIndexes.Add(i);
        
        var newSource = new List<TSource>();
        for(var i = 0; i < count; i++)
        {
            var randomIndex = freeIndexes.GetRandomElement();
            freeIndexes.Remove(randomIndex);
            newSource.Add(source.ElementAt(randomIndex));
        }
        
        return newSource;
    }
}