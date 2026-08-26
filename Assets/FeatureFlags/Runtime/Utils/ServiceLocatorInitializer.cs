using FeatureFlags;
using FeatureFlags.Model;
using FeatureFlags.Providers;
using FeatureFlags.Services;
using UnityEngine;

namespace Utils
{
    public class ServiceLocatorInitializer: MonoBehaviour
    {
        [SerializeField] private FeatureFlagsSettingsScriptableObject  featureFlagsSettings;
        private void Awake()
        {
            var provider = FeatureFlagProvidersFactory.CreateProvider(featureFlagsSettings.Settings);
            IFeatureFlagsToolController toolController = new FeatureFlagsToolController();
            
            toolController.Initialize(featureFlagsSettings.Settings, provider);
            StartCoroutine(provider.InitializeService(toolController));

            ServiceLocator.Register<IFeatureFlagService>(provider);
        }
    }
}
