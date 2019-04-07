#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using CompositeApplicationFramework.Collections;
using CompositeApplicationFramework.Utility;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Extensions
{

    #region Dependencies

    #endregion

    public static class CollectionExtensions
    {
        public static TransformedCollection<TSource, TTarget> Transform<TSource, TTarget>(
            this ICollection<TSource> sourceCollection,
            Func<TSource, TTarget> setup,
            Action<TTarget> teardown = null)
        {
            return new TransformedCollection<TSource, TTarget>(sourceCollection, setup, teardown);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Calls the provided action on each item, providing the item and its index into the source.
        /// </summary>
        public static void CountForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            source.ForEach(item => action(item, i++));
        }

        [System.Diagnostics.Contracts.Pure]
        public static IEnumerable<TTarget> CountSelect<TSource, TTarget>(this IEnumerable<TSource> source,
            Func<TSource, int, TTarget> func)
        {
            var i = 0;
            return source.Select(item => func(item, i++));
        }

        /// <summary>
        ///     Returns true if all items in the list are unique using
        ///     <see cref="EqualityComparer{T}.Default">EqualityComparer&lt;T&gt;.Default</see>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if <param name="source"/> is null.</exception>
        public static bool AllUnique<T>(this IList<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var comparer = EqualityComparer<T>.Default;

            return source.TrueForAllPairs((a, b) => !comparer.Equals(a, b));
        }

        /// <summary>
        ///     Returns true if <paramref name="compare"/> returns
        ///     true for every pair of items in <paramref name="source"/>.
        /// </summary>
        public static bool TrueForAllPairs<T>(this IList<T> source, Func<T, T, bool> compare)
        {
            for (var i = 0; i < source.Count; i++)
            {
                for (var j = i + 1; j < source.Count; j++)
                {
                    if (!compare(source[i], source[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns true if all items in <paramref name="source"/> exist
        ///     in <paramref name="set"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if <param name="source"/> or <param name="set"/> are null.</exception>
        [System.Diagnostics.Contracts.Pure]
        public static bool AllExistIn<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> set)
        {
            return source.All(set.Contains);
        }

        /// <summary>
        ///     Returns true if <paramref name="source"/> has no items in it; otherwise, false.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     If an <see cref="ICollection{TSource}"/> is provided,
        ///     <see cref="ICollection{TSource}.Count"/> is used.
        /// </para>
        /// <para>
        ///     Yes, this does basically the same thing as the
        ///     <see cref="System.Linq.Enumerable.Any{TSource}(IEnumerable{TSource})"/>
        ///     extention. The differences: 'IsEmpty' is easier to remember and it leverages
        ///     <see cref="ICollection{TSource}.Count">ICollection.Count</see> if it exists.
        /// </para>
        /// </remarks>
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            var sources = source as ICollection<TSource>;
            if (sources != null)
            {
                return sources.Count == 0;
            }
            using (var enumerator = source.GetEnumerator())
            {
                return !enumerator.MoveNext();
            }
        }

        /// <summary>
        ///     Returns the index of the first item in <paramref name="source"/>
        ///     for which <paramref name="predicate"/> returns true. If none, -1.
        /// </summary>
        /// <param name="source">The source enumerable.</param>
        /// <param name="predicate">The function to evaluate on each element.</param>
        [System.Diagnostics.Contracts.Pure]
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        ///     Returns a new <see cref="ReadOnlyCollection{T}"/> using the
        ///     contents of <paramref name="source"/>.
        /// </summary>
        /// <remarks>
        ///     The contents of <paramref name="source"/> are copied to
        ///     an array to ensure the contents of the returned value
        ///     don't mutate.
        /// </remarks>
        public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
        {
            return new ReadOnlyCollection<TSource>(source.ToArray());
        }

        /// <summary>
        ///     Performs the specified <paramref name="action"/>
        ///     on each element of the specified <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to which is applied the specified <paramref name="action"/>.</param>
        /// <param name="action">The action applied to each element in <paramref name="source"/>.</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Removes the last element from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list from which to remove the last element.</param>
        /// <returns>The last element.</returns>
        /// <remarks><paramref name="source"/> must have at least one element and allow changes.</remarks>
        public static TSource RemoveLast<TSource>(this IList<TSource> source)
        {
            var item = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);
            return item;
        }

        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext()) yield break;

                for (var value = e.Current; e.MoveNext(); value = e.Current)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        ///     If <paramref name="source"/> is null, return an empty <see cref="IEnumerable{TSource}"/>;
        ///     otherwise, return <paramref name="source"/>.
        /// </summary>
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }

        /// <summary>
        ///     Recursively projects each nested element to an <see cref="IEnumerable{TSource}"/>
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="recursiveSelector">A transform to apply to each element.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TSource}"/> whose elements are the
        ///     result of recursively invoking the recursive transform function
        ///     on each element and nested element of the input sequence.
        /// </returns>
        /// <remarks>This is a depth-first traversal. Be careful if you're using this to find something
        /// shallow in a deep tree.</remarks>
        public static IEnumerable<TSource> SelectRecursive<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> recursiveSelector)
        {
            var stack = new Stack<IEnumerator<TSource>>();
            stack.Push(source.GetEnumerator());

            try
            {
                while (stack.Any())
                {
                    if (stack.Peek().MoveNext())
                    {
                        var current = stack.Peek().Current;

                        yield return current;

                        stack.Push(recursiveSelector(current).GetEnumerator());
                    }
                    else
                    {
                        stack.Pop().Dispose();
                    }
                }
            }
            finally
            {
                while (stack.Any())
                {
                    stack.Pop().Dispose();
                }
            }
        } //*** SelectRecursive

        public static IList<TTo> ToList<TFrom, TTo>(this IEnumerable<TFrom> source) where TFrom : TTo
        {
            return source.Cast<TTo>().ToList();
        }

        public static T Random<T>(this IList<T> source)
        {
            return source[Util.Rnd.Next(source.Count)];
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
        {
            return source.Distinct(comparer.ToEqualityComparer());
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items.AsEnumerable());
        }

        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return dictionary.Contains(new KeyValuePair<TKey, TValue>(key, value));
        }

        public static bool CountAtLeast<T>(this IEnumerable<T> source, int count)
        {
            var collection = source as ICollection<T>;
            if (collection != null)
            {
                return collection.Count >= count;
            }
            using (var enumerator = source.GetEnumerator())
            {
                while (count > 0)
                {
                    if (enumerator.MoveNext())
                    {
                        count--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static IEnumerable<TSource> Except<TSource, TOther>(this IEnumerable<TSource> source,
            IEnumerable<TOther> other, Func<TSource, TOther, bool> comparer)
        {
            return from item in source
                where !other.Any(x => comparer(item, x))
                select item;
        }

        public static IEnumerable<TSource> Intersect<TSource, TOther>(this IEnumerable<TSource> source,
            IEnumerable<TOther> other, Func<TSource, TOther, bool> comparer)
        {
            return from item in source
                where other.Any(x => comparer(item, x))
                select item;
        }

        public static INotifyCollectionChanged AsINotifyPropertyChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return source;
        }

        /// <summary>
        /// Creates an <see cref="ObservableCollection{T}"/> from the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source elements.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create the <see cref="ObservableCollection{T}"/> from.</param>
        /// <returns>An <see cref="ObservableCollection{T}"/> that contains elements from the input sequence.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }      

        public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source");
            return source.ToDictionary(p => p.Key, p => p.Value);
        }

        public static bool TryGetTypedValue<TOutput, TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            out TOutput value) where TOutput : TValue
        {
            TValue val;
            if (dictionary.TryGetValue(key, out val))
            {
                if (val is TOutput)
                {
                    value = (TOutput) val;
                    return true;
                }
            }
            value = default(TOutput);
            return false;
        }

        public static TValue EnsureItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            [NotNull] Func<TValue> valueFactory)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value)) return value;
            value = valueFactory();
            dictionary.Add(key, value);
            return value;
        }
    }
}