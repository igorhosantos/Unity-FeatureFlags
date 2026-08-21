using System;
using System.Collections;
using System.Collections.Generic;

namespace FeatureFlags.Services
{
    public interface IFeatureFlagService
    {
        IEnumerator InitializeService(IFeatureFlagsToolController toolController);
        Dictionary<string,bool> GetAllFlags();
        bool IsFlagEnabled(string featureFlag);
    }
}
