using UnityEngine;

namespace Mimizh.UnityUtilities
{
    public static class GameObjectExtensions
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
        
        public static T OrNull<T> (this T obj) where T : Object => obj ? obj : null;

        public static void DestroyChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildren();
        }
        
        public static void HideInHierarchy(this GameObject gameObject) {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        
        public static void EnableChildren(this GameObject gameObject) {
            gameObject.transform.EnableChildren();
        }
        
        public static void DisableChildren(this GameObject gameObject) {
            gameObject.transform.DisableChildren();
        }
        
        public static void ResetTransformation(this GameObject gameObject) {
            gameObject.transform.Reset();
        }
    }
}