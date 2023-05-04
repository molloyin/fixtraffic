using System;
using UnityEngine;

namespace Utils
{
    public static class Tools
    {
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            float t = (value - from1) / (to1 - from1);
            return Mathf.Pow(1 - t, 3) * from2 +
                   3 * Mathf.Pow(1 - t, 2) * t * from2 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * to2 +
                   Mathf.Pow(t, 3) * to2;
        }
    }
}