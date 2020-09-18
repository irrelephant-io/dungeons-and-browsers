using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Irrelephant.DnB.Core.Utils
{
    public static class AsyncUtilities
    {
        public static async Task Sequentially<TItem>(this IEnumerable<TItem> items, Func<TItem, Task> mapperFunction)
        {
            foreach (var item in items)
            {
                await mapperFunction(item);
            }
        }

        public static async Task<IEnumerable<TResult>> Sequentially<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Task<TResult>> mapperFunction)
        {
            var results = new List<TResult>();
            foreach (var item in items)
            {
                var result = await mapperFunction(item);
                results.Add(result);
            }
            return results;
        }
    }
}
