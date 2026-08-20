using FeatureFlags.Model;
using UnityEngine;

namespace Utils
{
    public class ServiceLocatorInitializer: MonoBehaviour
    {
        [SerializeField] private FeatureFlagsSettingsScriptableObject  featureFlagsSettings;
        private void Awake()
        {
            //IAssetSpawnerService assetSpawnerService = new AssetSpawnerService(assetSpawnerSettings.Settings);
            //StartCoroutine(assetSpawnerService.InitializeService());
            //ServiceLocator.Register(assetSpawnerService);
        }
    }
}
