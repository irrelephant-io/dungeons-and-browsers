using System;
using System.Collections;
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
    }
}
