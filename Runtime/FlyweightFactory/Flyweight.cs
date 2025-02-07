using UnityEngine;
using UnityEngine.Serialization;

namespace Mimizh.UnityUtilities.FlyweightFactory
{
    public abstract class Flyweight : MonoBehaviour
    {
        public FlyweightSettings settings;
    }

    public class Projectile : Flyweight
    {
        private new ProjectileSettings settings => (ProjectileSettings)base.settings;
    }
}

