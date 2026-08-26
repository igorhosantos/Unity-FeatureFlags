using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FeatureFlags.Data;
using FeatureFlags.Model;
using FeatureFlags.Services;

namespace FeatureFlags.Providers
{
    public class FeatureFlagsServiceFromPlayerPrefs: IFeatureFlagService
    {
        private IFeatureFlagsToolController _toolController;
        private FeatureFlagsSettings _settings;
        private FeatureFlagsDataInfo _dataInfo;
        private readonly Dictionary<string, bool> _flags = new Dictionary<string, bool>();
        public FeatureFlagsServiceFromPlayerPrefs(FeatureFlagsSettings settings)
        {
           _settings = settings;
           _dataInfo = settings.FeatureFlagsDataInfo;
        }

        public IEnumerator InitializeService(IFeatureFlagsToolController toolController)
        {
            _toolController = toolController;
            yield return null;
        }

        public FeatureFlagsFileData GetAllFlags()
        {
           var fileData = new FeatureFlagsFileData();
           
           var dictio = _dataInfo.Data.ToDictionary(kvp => kvp, kvp => false);
           
           fileData.SetDataPerEnvironment(BackendEnvironment.Dev, dictio);
           fileData.SetDataPerEnvironment(BackendEnvironment.Prod, dictio);
           
           return fileData;
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
