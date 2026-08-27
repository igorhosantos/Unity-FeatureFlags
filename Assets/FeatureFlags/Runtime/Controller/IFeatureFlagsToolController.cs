using System;
using FeatureFlags.Data;
using FeatureFlags.Model;
using FeatureFlags.Services;


namespace FeatureFlags
{
    public interface IFeatureFlagsToolController
    {
        void Initialize(FeatureFlagsSettings settings, IFeatureFlagService service = null);
        bool EnablingUsageToggle { get; set; }
        bool UseLocalVersion { get; set; }
        
        FeatureFlagsFileData FetchLocalData();
        FeatureFlagsFileData FetchDataFromProvider();

        bool UpdateLocalFromProvider();
        bool OverrideLocalFeatureFlag(BackendEnvironment env, string flagId, bool newValue);

        FeatureFlagsFileData LocalFlags { get; }
    }

    [Serializable]
    public class FeatureFlagsAppState
    {
        public bool EnablingUsageToggle;
        public bool UseLocalVersion;
        public FeatureFlagsFileData FeatureFlagsFileData;
    }
}
