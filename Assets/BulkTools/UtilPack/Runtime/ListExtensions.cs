using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.UtilPack
{
    public static class ListExtensions
    {
        #region Shuffle
        public enum RandomSortMode
        {
            UnityRandom,
            SystemRandom,
        }

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

        public static void ShuffleList<T>(this List<T> self, System.Random random)
        {
            DoShuffle(self, RandomizeValues(self, random));
        }

        private static void DoShuffle<T>(List<T> self, IEnumerator<T> randomizer)
        {
            for (int i = 0; i < self.Count; i++)
            {
                randomizer.MoveNext();
                self[i] = randomizer.Current;
            }
        }

        private static IEnumerator<T> RandomizeValues<T>(List<T> self, System.Random random)
        {
            List<T> values = new List<T>(self);
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
