using System;
using System.Collections;
using System.Collections.Generic;
using FeatureFlags.Model;
using FeatureFlags.Services;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace FeatureFlags.Providers
{
    public class FeatureFlagsServiceFromPlayerPrefs: IFeatureFlagService
    {
        private IFeatureFlagsToolController _toolController;
        private FeatureFlagsSettings _settings;
        private FeatureFlagsDataInfo _dataInfo;
        private FeatureFlagsFileData _flagsData;
        public FeatureFlagsServiceFromPlayerPrefs(FeatureFlagsSettings settings)
        {
           _settings = settings;
           _dataInfo = settings.FeatureFlagsDataInfo;
        }

        public IEnumerator InitializeService(IFeatureFlagsToolController toolController)
        {
            _toolController = toolController;
            _flagsData = GetAllFlags();
            yield return null; 
        }

        public FeatureFlagsFileData GetAllFlags()
        { 
            try
            {
                var jsonFile = PlayerPrefs.GetString(_settings.FeatureId);

                if (string.IsNullOrEmpty(jsonFile))
                {
                    var serializedDefaultFlags = JsonConvert.SerializeObject(_toolController.LocalFlags);
                    PlayerPrefs.SetString(_settings.FeatureId, serializedDefaultFlags);
                    jsonFile = PlayerPrefs.GetString(_settings.FeatureId);
                }

                var fileData = JsonConvert.DeserializeObject<FeatureFlagsFileData>(jsonFile);
                return fileData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on getting all flags: {e.Message}");
                return null;
            }
        }

        public bool IsFlagEnabled(string featureFlag)
        {
            //check local file 
            if (_toolController.UseLocalVersion)
            {
                return _toolController.LocalFlags.Data[_settings.Environment].GetValueOrDefault(featureFlag, false);
            }
            
            return _flagsData.Data[_settings.Environment].GetValueOrDefault(featureFlag, false);
        }
    }
}
