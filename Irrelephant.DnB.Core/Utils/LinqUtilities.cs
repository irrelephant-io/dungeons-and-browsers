using Irrelephant.DnB.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
