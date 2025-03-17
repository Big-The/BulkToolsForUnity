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
                    self.ShuffleList(new System.Random());
                    break;
                case RandomSortMode.UnityRandom:
                default:
                    self.ShuffleList(null);
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
            //Unity random shuffle
            if (random == null)
            {
                int n = self.Count;
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    T value = self[k];
                    self[k] = self[n];
                    self[n] = value;
                }
            }
            //System random shuffle with provided random
            else
            {
                int n = self.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    T value = self[k];
                    self[k] = self[n];
                    self[n] = value;
                }
            }
        }
        #endregion
    }
}
