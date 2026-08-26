using System;
using System.Collections;
using System.Collections.Generic;

namespace FeatureFlags.Services
{
    public interface IFeatureFlagService
    {
        IEnumerator InitializeService(IFeatureFlagsToolController toolController);
        
        /// <summary>
        /// Return an object that has all of the flags separated by environment
        /// </summary>
        /// <returns></returns>
        FeatureFlagsFileData GetAllFlags();
        bool IsFlagEnabled(string featureFlag);
    }
}
