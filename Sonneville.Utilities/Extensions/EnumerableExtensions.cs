using System;
using System.Collections.Generic;

namespace Sonneville.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="System.Linq.Enumerable.Zip{TFirst,TSecond,TResult}"/> which stops once reaching the end of one of the sequences, <see cref="ZipAll{TFirst,TSecond,TResult}"/> continues until both sequences are exhausted, passing default values for <see cref="TFirst"/> or <see cref="TSecond"/> as appropriate.
        /// </remarks>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains merged elements of two input sequences.
        /// </returns>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            var e1 = first.GetEnumerator();
            try
            {
                var e2 = second.GetEnumerator();
                try
                {
                    bool e1HasMore, e2HasMore = false;
                    while ((e1HasMore = e1.MoveNext()) && (e2HasMore = e2.MoveNext()))
                    {
                        if (e1HasMore && e2HasMore)
                        {
                            yield return resultSelector(e1.Current, e2.Current);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (e1HasMore)
                    {
                        do
                        {
                            yield return resultSelector(e1.Current, default(TSecond));
                        } while (e1.MoveNext());
                    }
                    if (e2HasMore)
                    {
                        while (e2.MoveNext())
                            yield return resultSelector(default(TFirst), e2.Current);
                    }
                }
                finally
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    e2?.Dispose();
                }
            }
            finally
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                e1?.Dispose();
            }
        }
    }
}