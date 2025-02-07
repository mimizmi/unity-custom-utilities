using UnityEngine;

namespace Mimizh.UnityUtilities
{
    /// <summary>
    /// Destroy any old singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        public static bool HasInstance => instance != null;
        
        public float InitializationTime { get; private set; }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "Auto Created";
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);
            
            T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T old in oldInstances)
            {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
                    Destroy(old.gameObject);
            }

            if (instance == null)
            {
                instance = this as T;
            }
        }
    }
}