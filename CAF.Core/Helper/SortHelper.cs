#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompositeApplicationFramework.Extensions;

#endregion

#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#else

#endif

namespace CompositeApplicationFramework.Helper
{
    public static class SortHelper
    {
        public static bool QuickSort<T>(this IList<T> list, Func<T, T, int> comparer)
        {
            return list.QuickSort(comparer.ToComparer());
        }

        public static bool QuickSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            return list.QuickSort(comparison.ToComparer());
        }

        public static bool QuickSort<T>(this IList<T> list)
        {
            return list.QuickSort(Comparer<T>.Default);
        }

        public static bool QuickSort<T>(this IList<T> list, IComparer<T> comparer)
        {
            if (list.Count <= 1) return false;
            try
            {
                return QuickSort(list, 0, list.Count - 1, comparer);
            }
            catch (IndexOutOfRangeException ioore)
            {
                throw new ArgumentException("Bogus IComparer", ioore);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("IComparer Failed", exception);
            }
        }

        private static bool QuickSort<T>(IList<T> keys, int left, int right, IComparer<T> comparer)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(keys != null);
            Debug.Assert(left >= 0);
            Debug.Assert(left < keys.Count);
            Debug.Assert(right >= 0);
            Debug.Assert(right < keys.Count);

            var change = false;
            do
            {
                var a = left;
                var b = right;
                var num3 = a + ((b - a) >> 1);
                change = SwapIfGreaterWithItems(keys, comparer, a, num3) || change;
                change = SwapIfGreaterWithItems(keys, comparer, a, b) || change;
                change = SwapIfGreaterWithItems(keys, comparer, num3, b) || change;
                var y = keys[num3];
                do
                {
                    while (comparer.Compare(keys[a], y) < 0)
                    {
                        a++;
                    }
                    while (comparer.Compare(y, keys[b]) < 0)
                    {
                        b--;
                    }
                    if (a > b)
                    {
                        break;
                    }
                    if (a < b)
                    {
                        var local2 = keys[a];
                        keys[a] = keys[b];
                        keys[b] = local2;
                        change = true;
                    }
                    a++;
                    b--;
                } while (a <= b);
                if ((b - left) <= (right - a))
                {
                    if (left < b)
                    {
                        change = QuickSort(keys, left, b, comparer) || change;
                    }
                    left = a;
                }
                else
                {
                    if (a < right)
                    {
                        change = QuickSort(keys, a, right, comparer) || change;
                    }
                    right = b;
                }
            } while (left < right);

            return change;
        }

        private static bool SwapIfGreaterWithItems<T>(IList<T> keys, IComparer<T> comparer, int a, int b)
        {
            if ((a == b) || (comparer.Compare(keys[a], keys[b]) <= 0)) return false;
            var local = keys[a];
            keys[a] = keys[b];
            keys[b] = local;
            return true;
        }
    }
}