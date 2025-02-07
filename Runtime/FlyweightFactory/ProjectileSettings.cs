using UnityEngine;

namespace Mimizh.UnityUtilities.FlyweightFactory
{
    [CreateAssetMenu(fileName = "ProjectileSettings", menuName = "Flyweight/Projectile Settings")]
    public class ProjectileSettings : FlyweightSettings
    {
        public float despawnDelay;
        public float speed;
        public float damage;
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name;
            
            var projectile = go.AddComponent<Projectile>();
            projectile.settings = this;
            return projectile;
        }
    }
}