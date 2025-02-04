using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.UtilPack
{
    public static class ListExtensions
    {
        #region Shuffle
        /// <summary>
        /// Defines the source of random when using randomness
        /// </summary>
        public enum RandomSortMode
        {
            UnityRandom,
            SystemRandom,
        }

        /// <summary>
        /// Shuffles the list into a random order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="sortMode"></param>
        public static void ShuffleList<T>(this List<T> self, RandomSortMode sortMode = RandomSortMode.UnityRandom)
        {
            switch (sortMode)
            {
                case RandomSortMode.SystemRandom:
                    DoShuffle(self, RandomizeValues(self, new System.Random()));
                    break;
                case RandomSortMode.UnityRandom:
                default:
                    DoShuffle(self, RandomizeValues(self, null));
                    break;
            }
        }

        /// <summary>
        /// Shuffle the list with a provided random.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="random"></param>
        public static void ShuffleList<T>(this List<T> self, System.Random random)
        {
            DoShuffle(self, RandomizeValues(self, random));
        }

        /// <summary>
        /// Runs the shuffle process
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="randomizer"></param>
        private static void DoShuffle<T>(List<T> self, IEnumerator<T> randomizer)
        {
            for (int i = 0; i < self.Count; i++)
            {
                randomizer.MoveNext();
                self[i] = randomizer.Current;
            }
        }

        /// <summary>
        /// Shuffles a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private static IEnumerator<T> RandomizeValues<T>(List<T> self, System.Random random)
        {
            List<T> values = new List<T>(self);
            //Unity random shuffle
            if (random == null)
            {
                for (int i = 0; i < self.Count; i++)
                {
                    int removed = Random.Range(0, values.Count);
                    T item = values[removed];
                    values.RemoveAt(removed);
                    yield return item;
                }
            }
            //System random shuffle with provided random
            else
            {
                for (int i = 0; i < self.Count; i++)
                {
                    int removed = random.Next(values.Count);
                    T item = self[removed];
                    self.RemoveAt(removed);
                    yield return item;
                }
            }
        }
        #endregion
    }
}
