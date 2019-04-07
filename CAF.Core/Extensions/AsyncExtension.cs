#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    public static class AsyncExtension
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }
}