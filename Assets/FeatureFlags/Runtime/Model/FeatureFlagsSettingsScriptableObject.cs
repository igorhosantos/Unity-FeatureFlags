using System;
using FeatureFlags.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace FeatureFlags.Model
{
    [CreateAssetMenu(fileName = "FeatureFlagsSettingsScriptableObject",
        menuName = "Feature Flags/FeatureFlagsSettingsScriptableObject")]
    public class FeatureFlagsSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private FeatureFlagsSettings settings;
        public FeatureFlagsSettings Settings => settings;
    }

    [Serializable]
    public class FeatureFlagsSettings
    {
        public string FeatureId = "featureflags";
        public string FolderPath = "Assets/Resources";
        public string FolderName = "FeatureFlags";
        public string FileName = "featureflag_manifest.json";
        public enum Providers
        {
            PlayerPrefs = 0,
            Api = 1,
            ThirdParty = 2,
        }

        [SerializeField] private BackendEnvironment environment;
        [SerializeField] private Providers providerType;
        
        [Header("(CanBeNull) List of FlagIds")]
        [SerializeField][CanBeNull] private FeatureFlagsDataInfo featureFlagsDataInfo;
        public Providers ProviderType => providerType;
        public FeatureFlagsDataInfo FeatureFlagsDataInfo => featureFlagsDataInfo;
        public BackendEnvironment Environment => environment;
    }
}
