using System;
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
        public const string FolderPath = "Assets/Resources";
        public const string FolderName = "FeatureFlags";
        public const string FileName = "localfeatureflagsfile";
        public static string FilePath = $"{FolderPath}/{FolderName}/{FileName}.json";

        public enum Providers
        {
            PlayerPrefs = 0,
            Api = 1,
        }

        [SerializeField] private Providers providerType;
        public Providers ProviderType => providerType;
    }
}
