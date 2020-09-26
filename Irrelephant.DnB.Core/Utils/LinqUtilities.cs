using Irrelephant.DnB.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irrelephant.DnB.Core.Utils
{
    public static class LinqUtilities
    {

        public static IEnumerable<TItem> ArrayOf<TItem>(this TItem item)
        {
            return new[] { item };
        }

        public static IEnumerable<TItem> Copies<TItem>(this TItem item, int count)
            where TItem : ICopyable<TItem>
        {
            return Enumerable
                .Range(0, count)
                .Select(_ => item.Copy())
                .ToArray();
        }

        public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            var itemArray = items as TItem[] ?? items.ToArray();
            foreach (var item in itemArray)
            {
                action(item);
            }

            return itemArray;
        }

        private static readonly Random Rng = new Random();

        public static IEnumerable<TItem> Shuffle<TItem>(this IEnumerable<TItem> items)
        {
            return items.OrderBy(rng => Rng.Next());
        }

        public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TItem, TKey, TValue>(
            this IEnumerable<TItem> items,
            Func<TItem, TKey> keySelector,
            Func<TItem, Task<TValue>> valueSelector)
        {
            var enumerable = items as TItem[] ?? items.ToArray();
            var values = await Task.WhenAll(enumerable.Select(valueSelector));
            var keys = enumerable.Select(keySelector);

            return keys
                .Zip(values, (k, v) => new KeyValuePair<TKey, TValue>(k, v))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
