using FeatureFlags.Model;
using FeatureFlags.Services;
using UnityEngine;

namespace FeatureFlags.Providers
{
    public static class FeatureFlagProvidersFactory
    {
        public static IFeatureFlagService CreateProvider(FeatureFlagsSettings settings)
        {
            FeatureFlagsSettings.Providers providerType = settings.ProviderType;

            switch (providerType)
            {
                case FeatureFlagsSettings.Providers.Api:
                    return new FeatureFlagsServiceFromApi(settings);
                case FeatureFlagsSettings.Providers.PlayerPrefs:
                    return new FeatureFlagsServiceFromPlayerPrefs(settings);
                case FeatureFlagsSettings.Providers.ThirdParty:
                    return new FeatureFlagsServiceFromThirdParty(settings);
                default:
                    Debug.LogError($"Unknown provider type {providerType}");
                    return null;
            }
        }
    }
}
