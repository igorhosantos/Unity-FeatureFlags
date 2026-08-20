using System;
using System.Collections.Generic;

namespace FeatureFlags.Services
{
    public interface IFeatureFlagService
    {
        Dictionary<string,bool> GetAllFeatureFlagsStatus();
        void SetFeatureFlagOverride(string featureFlag, Func<bool> isEnabledGetter);
        void RemoveFeatureFlagOverride(string featureFlag);
        bool IsFeatureEnabled(string featureFlag);
    }
}
