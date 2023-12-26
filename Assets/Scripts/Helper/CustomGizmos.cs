using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRadHex
{
    namespace CustomGizmos
    {
        /// <summary>
        /// Helper class containing methods for drawing custom Gizmos.
        /// </summary>
        public static class CustomGizmos
        {
            /// <summary>
            /// Draws a filled triangle using Gizmos.
            /// </summary>
            /// <param name="position">The position of the triangle.</param>
            /// <param name="direction">The direction the triangle points to.</param>
            /// <param name="size">The overall size of the height/altitude of the triangle.</param>
            /// <param name="baseLength">The length of the base of the triangle.</param>
            /// <param name="fillNum">The level of fill for the triangle.</param>
            /// <param name="fillerLineWidth">The width of each filler line.</param>
            /// <param name="fillerLineHeight">The height of the filler lines.</param>
            public static void DrawTriangle(Vector3 position, Vector3 direction, float size = 1f, float baseLength = 0.4f, int fillNum = 6, float fillerLineWidth = 0.02f, float fillerLineHeight = 0.001f)
            {
                int fillerLines = Mathf.CeilToInt(Mathf.Pow(2, fillNum));
                for (int i = 0; i < fillerLines; i++)
                {
                    Gizmos.DrawCube(position + i * direction * size / fillerLines, new Vector3(fillerLineWidth, fillerLineHeight, baseLength - i / (fillerLines / baseLength)));
                }

            }
        }
    }
    

}

