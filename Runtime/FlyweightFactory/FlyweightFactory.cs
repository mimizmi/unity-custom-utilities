using System.Collections.Generic;
using Mimizh.UnityUtilities;
using UnityEngine;
using UnityEngine.Pool;

namespace Mimizh.UnityUtilities.FlyweightFactory
{
    public class FlyweightFactory : PersistentSingleton<FlyweightFactory>
    {
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        private readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> _pools = new();
        
        public static Flyweight Spawn(FlyweightSettings s) => instance.GetPoolFor(s)?.Get();

        public static void ReturnToPool(Flyweight flyweight) =>
            instance.GetPoolFor(flyweight.settings)?.Release(flyweight);

        IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings)
        {
            IObjectPool<Flyweight> pool;
            
            if (_pools.TryGetValue(settings.type, out pool)) return pool;

            pool = new ObjectPool<Flyweight>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            
            _pools.Add(settings.type, pool);
            return pool;
        }
    }
}