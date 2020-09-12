using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Irrelephant.DnB.Client.Tests
{
    public static class AssertExtensions
    {
        public static async Task Eventually(Func<Task> assertion)
        {
            var exceptions = new List<Exception>();
            var attempt = 0;
            while (attempt < 5)
            {
                try
                {
                    await assertion();
                    return;
                }
                catch (Exception e)
                {
                    await Task.Delay((int)Math.Pow(2, attempt) * 100);
                    exceptions.Add(e);
                    attempt++;
                }
            }
            throw new AggregateException(exceptions);
        }

        public static Task Eventually(Action assertion)
        {
            return Eventually(() => {
                assertion();
                return Task.CompletedTask;
            });
        }
    }
}
