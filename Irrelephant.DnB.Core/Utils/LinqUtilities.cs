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
            foreach (var item in items)
                action(item);

            return items;
        }

        private static readonly Random _rng = new Random();

        public static IEnumerable<TItem> Shuffle<TItem>(this IEnumerable<TItem> items)
        {
            return items.OrderBy(rng => _rng.Next());
        }
    }
}
