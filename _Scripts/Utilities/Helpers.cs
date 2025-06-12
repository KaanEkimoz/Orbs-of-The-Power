using System.Linq;
using System;
using System.Text;
using UnityEngine;

namespace com.game.utilities
{
    public static class Helpers
    {
        public static class Text
        {
            public static string Bold(string input)
            {
                return CoverXML(input, "b", "/b");
            }

            public static string Italic(string input)
            {
                return CoverXML(input, "i", "/i");
            }

            public static string Colorize(string input, string colorHexcode)
            {
                return CoverXML(input, $"color={colorHexcode}", "/color");
            }

            public static string CoverXML(string input, string xmlStart, string xmlEnd)
            {
                StringBuilder sb = new();

                sb.Append($"<{xmlStart}>");
                sb.Append(input);
                sb.Append($"<{xmlEnd}>");

                return sb.ToString();
            }
        }

        public static class Math
        {
            public static class Integers
            {
                public static int[] SeperateRandomly(int total, int minCount, int maxCount, System.Random rng = null)
                {
                    rng ??= new System.Random();
                    int count = rng.Next(minCount, maxCount);

                    return SeperateRandomly(total, count, rng);
                }

                // by GPT :^)
                public static int[] SeperateRandomly(int total, int count, System.Random rng = null)
                {
                    if (count < 1 || count > total)
                        throw new ArgumentException("k must be between 1 and total");

                    rng ??= new System.Random();

                    var cuts = new int[count - 1];
                    for (int i = 0; i < count - 1; i++)
                    {
                        int r;
                        do
                        {
                            r = rng.Next(1, total); // [1, total-1]
                        } while (cuts.Take(i).Contains(r));
                        cuts[i] = r;
                    }

                    var points = cuts
                        .OrderBy(x => x)
                        .Prepend(0)
                        .Append(total)
                        .ToArray();

                    var parts = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        parts[i] = points[i + 1] - points[i];
                    }
                    return parts;
                }
            }
        }

        public static class Physics
        {
            public const float DEFAULT_GROUND_CHECK_UP_SHIFT = 10f;

            static float s_groundCheckUpShift = DEFAULT_GROUND_CHECK_UP_SHIFT;
            public static float GroundCheckUpShift
            {
                get
                {
                    return s_groundCheckUpShift;
                }

                set
                {
                    s_groundCheckUpShift = value;
                }
            }

            public static bool TryGetGroundSnappedPosition(Vector3 initialPosition, float radius, LayerMask groundMask, out Vector3 groundSnappedPosition)
            {
                initialPosition.y += s_groundCheckUpShift;
                bool groundFound = UnityEngine.Physics.SphereCast(initialPosition, radius, Vector3.down, out RaycastHit hit, float.MaxValue, groundMask);

                if (groundFound)
                    groundSnappedPosition = hit.point;
                else 
                    groundSnappedPosition = initialPosition;

                s_groundCheckUpShift = DEFAULT_GROUND_CHECK_UP_SHIFT;

                return groundFound;
            }
        }
    }
}
