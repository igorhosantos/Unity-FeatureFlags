using System.Collections;
using System.Collections.Generic;
using FeatureFlags.Model;
using FeatureFlags.Services;

namespace FeatureFlags.Providers
{
    public class FeatureFlagsServiceFromThirdParty: IFeatureFlagService
    {
        public FeatureFlagsServiceFromThirdParty(FeatureFlagsSettings settings)
        {
           
        }

        public IEnumerator InitializeService(IFeatureFlagsToolController toolController)
        {
            throw new System.NotImplementedException();
        }

        public FeatureFlagsFileData GetAllFlags()
        {
            throw new System.NotImplementedException();
        }

        public bool IsFlagEnabled(string featureFlag)
        {
            throw new System.NotImplementedException();
        }
    }
}
