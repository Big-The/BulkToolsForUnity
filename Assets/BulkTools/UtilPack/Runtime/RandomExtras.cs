using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.UtilPack
{
    public static class RandomExtras
    {
        public static int[] RandomSequence(int min, int max, int size, bool allUniqe)
        {
            if (min > max)
            {
                Debug.LogError("Max can not be smaller then min.");
                return new int[size];
            }
            if (allUniqe && max - min < size)
            {
                Debug.LogError("Range is not big enough.");
                return new int[size];
            }

            if (max - min < size / 2)
            {
                return RandomSequencePicker(min, max, size);
            }

            List<int> results = new List<int>();
            for (int i = 0; i < size; i++)
            {
                do
                {
                    int temp = Random.Range(min, max);
                    if (!results.Contains(temp))
                    {
                        results.Add(temp);
                        break;
                    }
                } while (allUniqe);
            }
            return results.ToArray();
        }
        private static int[] RandomSequencePicker(int min, int max, int size)
        {
            List<int> ints = new List<int>();
            for (int i = min; i < max; i++)
            {
                ints.Add(i);
            }
            int[] results = new int[size];
            for (int i = 0; i < size; i++)
            {
                int intsIndex = Random.Range(min, max);
                results[i] = ints[intsIndex];
                ints.Remove(intsIndex);
            }
            return ints.ToArray();
        }

        public static int WeightedRandom(IEnumerable<float> weights)
        {
            float total = 0;
            int weightsLength = 0;
            foreach (int weight in weights)
            {
                weightsLength++;
                total += weight;
            }
            if (total == 0)
            {
                return -1;
            }
            float target = total * Random.value;

            int index = 0;
            float curVal = 0;
            foreach (float weight in weights)
            {
                if (index < weightsLength - 1)
                {
                    curVal += weight;
                    if (curVal > target)
                    {
                        return index;
                    }
                }
                index++;
            }
            //for just incase floating points cause a weird value
            if (index >= weightsLength)
            {
                index = weightsLength - 1;
            }
            return index;
        }

        /// <summary>
        /// Picks a random value from the list based on the weights
        /// Warning: This does not check if the weights length is the same as the values length and can therefore have index out of bounds exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedRandomPick<T>(List<T> values, IEnumerable<float> weights)
        {
            return values[WeightedRandom(weights)];
        }

        /// <summary>
        /// Picks a random value from the array based on the weights
        /// Warning: This does not check if the weights length is the same as the values length and can therefore have index out of bounds exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static T WeightedRandomPick<T>(T[] values, IEnumerable<float> weights)
        {
            return values[WeightedRandom(weights)];
        }
    }
}