using UnityEngine;
using UnityEngine.Serialization;

namespace Mimizh.UnityUtilities.FlyweightFactory
{
    [CreateAssetMenu(fileName = "FlyweightSettings", menuName = "Flyweight/Flyweight Settings")]
    public abstract class FlyweightSettings : ScriptableObject
    {
        public GameObject prefab;
        public FlyweightType type;
        public virtual Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name;
            
            var flyweight = go.AddComponent<Flyweight>();
            flyweight.settings = this;
            return flyweight;
        }
        public virtual void OnGet(Flyweight flyweight) => flyweight.gameObject.SetActive(true);
        public virtual void OnRelease(Flyweight flyweight) => flyweight.gameObject.SetActive(false);
        public virtual void OnDestroyPoolObject(Flyweight flyweight) => Destroy(flyweight.gameObject);
    }

    public enum FlyweightType
    {
        OneWay,
        TwoWay,
    }
}