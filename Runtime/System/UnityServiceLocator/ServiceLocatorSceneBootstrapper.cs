using UnityEngine;

namespace Mimizh.UnityUtilities
{
    [AddComponentMenu("Service Locator/ServiceLocator Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}