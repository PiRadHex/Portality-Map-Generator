using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PiRadHex
{
    namespace Shuffle
    {
        /// <summary>
        /// Provides methods for shuffling elements within a list.
        /// </summary>
        public static class ShuffleList
        {
            /// <summary>
            /// Shuffles the elements within the specified list using the Fisher-Yates shuffle algorithm.
            /// </summary>
            /// <typeparam name="T">The type of elements in the list.</typeparam>
            /// <param name="list">The list to be shuffled.</param>
            public static void FisherYates<T>(ref List<T> list)
            {
                // Fisher-Yates shuffle algorithm
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int randomIndex = Random.Range(0, i + 1);
                    T temp = list[i];
                    list[i] = list[randomIndex];
                    list[randomIndex] = temp;
                }
            }
        }
    }


}
