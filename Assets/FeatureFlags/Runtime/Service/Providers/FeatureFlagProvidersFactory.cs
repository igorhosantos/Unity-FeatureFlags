
using UnityEngine;
using FeatureFlags.Model;
using FeatureFlags.Services;
using FeatureFlags.Model;

namespace FeatureFlags.Providers
{
    public static class FeatureFlagProvidersFactory
    {
        public static IFeatureFlagService CreateProvider(FeatureFlagsSettings settings)
        {
            FeatureFlagsSettings.Providers providerType = settings.ProviderType;

            return new FeatureFlagsServiceFromPlayerPrefs();
        }
    }
}
