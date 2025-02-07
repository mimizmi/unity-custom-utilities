using UnityEngine;

namespace Mimizh.UnityUtilities
{
    public static class NumberExtensions
    {
        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
    }
}