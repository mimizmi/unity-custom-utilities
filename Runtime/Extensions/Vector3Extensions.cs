using UnityEngine;

namespace Mimizh.UnityUtilities
{
    public static class Vector3Extensions
    {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }

        public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)
        {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            float minRadius2 = minRadius * minRadius;
            float maxRadius2 = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadius2 - minRadius2) + minRadius2);
            Vector3 position = Vector3.zero.With(x : dir.x, z : dir.y) * distance;
            return origin + position;
        }
    }
}