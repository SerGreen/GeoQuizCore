using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoQuizCore.Models.Shared
{
    public static class ExtensionMethods
    {
        private static Random rnd = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rnd.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }
        public static IEnumerable<T> Shuffle2<T>(this IEnumerable<T> list)
        {
            IList<T> copy = new List<T>(list);
            Random rnd = new Random();
            for (int i = copy.Count() - 1; i >= 0; i--)
            {
                int index = rnd.Next(0, copy.Count());
                yield return copy.ElementAt(index);
                copy.RemoveAt(index);
            }
        }

        public static IEnumerable<T> Shuffle3<T>(this IEnumerable<T> source)
        {
            T[] elements = source.ToArray();
            for (int i = 0; i < rnd.Next(5, 10); i++)
            {
                int swapA = rnd.Next(elements.Length);
                int swapB = rnd.Next(elements.Length);
                if (swapA != swapB)
                {
                    T swap = elements[swapA];
                    elements[swapA] = elements[swapB];
                    elements[swapB] = swap;
                }
            }

            return elements as IEnumerable<T>;
        }

        /// <summary>
        /// Inserts spaces in capitalized CamelCase.
        /// 'FooBarTar' => 'Foo Bar Tar'
        /// </summary>
        public static string ToSentence(this string input)
        {
            return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new char[] { ' ', c } : new char[] { c }).ToArray());
        }
    }
}