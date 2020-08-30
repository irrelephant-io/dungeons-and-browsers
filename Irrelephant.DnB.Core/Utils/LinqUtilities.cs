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
            where TItem : ICloneable
        {
            return Enumerable
                .Range(0, count)
                .Select(_ => (TItem)((ICloneable)item).Clone());
        }

        public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            foreach (var item in items)
                action(item);

            return items;
        }
    }
}
