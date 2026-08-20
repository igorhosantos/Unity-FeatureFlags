using UnityEngine;

namespace FeatureFlags.Model
{
    [CreateAssetMenu(fileName = "FeatureFlagsSettingsScriptableObject",
        menuName = "FeatureFlags/FeatureFlagsSettingsScriptableObject")]
    public class FeatureFlagsSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private FeatureFlagsSettings settings;
        public FeatureFlagsSettings Settings => settings;
    }

    public class FeatureFlagsSettings
    {
        public const string FolderPath = "Assets/Genies/Resources";
        public const string FolderName = "FeatureFlags";
        public const string FileName = "localfeatureflagsfile";
        public static string FilePath = $"{FolderPath}/{FolderName}/{FileName}.json";

        public enum Providers
        {
            PlayerPrefs = 0,
            Api = 1,
        }

        public Providers ProviderType;
    }
}
