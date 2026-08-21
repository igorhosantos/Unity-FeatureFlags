using System.Collections;
using System.Collections.Generic;
using FeatureFlags.Model;
using FeatureFlags.Services;

namespace FeatureFlags.Providers
{
    public class FeatureFlagsServiceFromPlayerPrefs: IFeatureFlagService
    {
        private IFeatureFlagsToolController _toolController;
        private FeatureFlagsSettings _settings;
        private readonly Dictionary<string, bool> _flags = new Dictionary<string, bool>();
        public FeatureFlagsServiceFromPlayerPrefs(FeatureFlagsSettings settings)
        {
           _settings = settings;
        }

        public IEnumerator InitializeService(IFeatureFlagsToolController toolController)
        {
            _toolController = toolController;
            yield return null;
        }

        public Dictionary<string, bool> GetAllFlags()
        {
            //_flags = _toolController;
            return _flags;
        }

        public bool IsFlagEnabled(string featureFlag)
        {
            if (_flags.TryGetValue(featureFlag, out bool value))
            {
                return value;
            }
            
            return false;
        }
    }
}
