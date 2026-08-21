using System;
using FeatureFlags.Data;


namespace FeatureFlags
{
    public interface IFeatureFlagsToolController
    {
        bool EnablingUsageToggle { get; set; }
        bool UseLocalVersion { get; set; }
        
        FeatureFlagsFileData FetchLocalData();
        FeatureFlagsFileData FetchDataFromProvider();

        bool UpdateLocalFromProvider();
        bool OverrideLocalFeatureFlag(BackendEnvironment env, string flagId, bool newValue);
    }

    [Serializable]
    public class FeatureFlagsAppState
    {
        public bool EnablingUsageToggle;
        public bool UseLocalVersion;
        public FeatureFlagsFileData FeatureFlagsFileData;
    }
}
