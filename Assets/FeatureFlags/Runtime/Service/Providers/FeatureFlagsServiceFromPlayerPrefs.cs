using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FeatureFlags.Services;

namespace FeatureFlags.Providers
{
    public class FeatureFlagsServiceFromPlayerPrefs: IFeatureFlagService
    {
        public FeatureFlagsServiceFromPlayerPrefs(FeatureFlagsToolBehavior ffToolBehavior, bool prodOverride = false)
        {
           
        }

        public FeatureFlagsServiceFromPlayerPrefs()
        {

        }

        public Dictionary<string, bool> GetAllFeatureFlagsStatus()
        {
            return null;
        }

        public void SetFeatureFlagOverride(string featureFlag, Func<bool> isEnabledGetter)
        {
            
        }

        public void RemoveFeatureFlagOverride(string featureFlag)
        {
            
        }

        public bool IsFeatureEnabled(string featureFlag)
        {
            return false;
        }
    }
}
