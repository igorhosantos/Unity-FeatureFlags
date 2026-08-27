using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif
using UnityEngine;
using Exception = System.Exception;
using FeatureFlags.Data;
using FeatureFlags.Model;
using FeatureFlags.Providers;
using FeatureFlags.Services;


namespace FeatureFlags
{
    /// <summary>
    /// Class responsible to isolate all the logic from Feature Flag Tool Window
    /// </summary>
    public class FeatureFlagsToolController: IFeatureFlagsToolController
    {
        private FeatureFlagsAppState _currentAppState = new FeatureFlagsAppState();
        private FeatureFlagsSettings _settings;
        private IFeatureFlagService _service;

        public void Initialize(FeatureFlagsSettings settings, IFeatureFlagService service = null)
        {
            _settings = settings;
            if (service == null) _service = FeatureFlagProvidersFactory.CreateProvider(settings);
        }
        
        /// <summary>
        ///  Check the settings path configuration to access local manifest if exist
        ///  If not it will create the first manifest and set the default file
        /// </summary>
        /// <returns></returns>
        public FeatureFlagsFileData FetchLocalData()
        {
            var result = TryFetchLocalFeatureFlags();
            return result;
        }

        public FeatureFlagsFileData FetchDataFromProvider()
        {
            var flagData = _service.GetAllFlags();
            return flagData;
        }

        public bool UpdateLocalFromProvider()
        {
            throw new NotImplementedException();
        }

        bool IFeatureFlagsToolController.OverrideLocalFeatureFlag(BackendEnvironment env, string flagId, bool newValue)
        {
            throw new NotImplementedException();
        }

        public FeatureFlagsFileData FeatureFlagsFileData { get; }
        
        /// <summary>
        /// Creates a FeatureFlagsFileData with fallback feature flags set to default values
        /// </summary>
        private FeatureFlagsFileData CreateFeatureFlagsWithDefaults()
        {
            var flagData = _service.GetAllFlags();
            return flagData;
        }

        public FeatureFlagsToolController()
        {
            
        }
        

        public bool EnablingUsageToggle
        {
            get
            {
                return _currentAppState.EnablingUsageToggle;
            }
            set
            {
                var newState = new FeatureFlagsAppState()
                {
                    EnablingUsageToggle = value,
                    UseLocalVersion = _currentAppState.UseLocalVersion,
                    FeatureFlagsFileData = _currentAppState.FeatureFlagsFileData,
                };

                _currentAppState = newState;
                UpdateLocalData(newState);
            }
        }

        public bool UseLocalVersion
        {
            get
            {
                return _currentAppState.UseLocalVersion;
            }
            set
            {
                var newState = new FeatureFlagsAppState()
                {
                    EnablingUsageToggle = _currentAppState.EnablingUsageToggle,
                    UseLocalVersion = value,
                    FeatureFlagsFileData = _currentAppState.FeatureFlagsFileData,
                };

                _currentAppState = newState;
                UpdateLocalData(newState);
            }
        }

        /// <summary>
        /// It will try to access the file that already exist,otherwise return and error
        /// </summary>
        /// <returns></returns>
        public bool UpdateLocalFromBackend()
        {
            return false;
            /*
#if UNITY_EDITOR
            try
            {
                await FetchFlagsDataInfo();

                //considering the list from shared feature flag as well
                var finalList = new List<string>();
                finalList.AddRange(_listPartnerDataInfo.Data);
                finalList.AddRange(SharedFeatureFlags.GetList());

                FeatureFlagsFileData apiVersion = await FetchApiFeatureFlags(finalList, requestDev:true, requestProd:true);

                if (!AssetDatabase.IsValidFolder($"{FeatureFlagsUtils.FolderPath}/{FeatureFlagsUtils.FolderName}"))
                {
                    AssetDatabase.CreateFolder(FeatureFlagsUtils.FolderPath, FeatureFlagsUtils.FolderName);
                }

                var serialized = JsonConvert.SerializeObject(apiVersion, Formatting.Indented);

                await System.IO.File.WriteAllTextAsync($"{FeatureFlagsUtils.FilePath}", serialized);
                await CreateOrUpdateLocalData(apiVersion);

                return true;

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
#else
            return false;
#endif
*/
        }

        /// <summary>
        /// It will try to access the file that already exist,otherwise return and error
        /// </summary>
        /// <returns></returns>
        private FeatureFlagsFileData TryFetchLocalFeatureFlags()
        {
            try
            {
                TextAsset file = Resources.Load<TextAsset>($"{_settings.FolderName}/{_settings.FileName}");
                if (file == null)
                {
                    var defaults = CreateFeatureFlagsWithDefaults();
                    UpdateLocalData(defaults);
                    return defaults;
                }

                _currentAppState = JsonConvert.DeserializeObject<FeatureFlagsAppState>(file.text);
                return _currentAppState.FeatureFlagsFileData;

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                var defaults = CreateFeatureFlagsWithDefaults();
                UpdateLocalData(defaults);
                return defaults;
            }
        }
        
        private Dictionary<string,bool> ProcessFlagsByEnvironment(Dictionary<string, object> data)
        {
            var dictio = data
                .ToDictionary( p=> p.Key, p=> (bool)p.Value);

            return dictio;
        }
        
        private bool UpdateLocalData(FeatureFlagsAppState newAppState)
        {
            _currentAppState = newAppState;
            return UpdateLocalData(_currentAppState.FeatureFlagsFileData);
        }

        private bool UpdateLocalData(FeatureFlagsFileData fileData)
        {
#if UNITY_EDITOR
            try
            {
                var currentState = new FeatureFlagsAppState()
                {
                    EnablingUsageToggle = this.EnablingUsageToggle,
                    UseLocalVersion = this.UseLocalVersion,
                    FeatureFlagsFileData = fileData,
                };

                if (!AssetDatabase.IsValidFolder($"{_settings.FolderPath}/{_settings.FolderName}"))
                {
                    AssetDatabase.CreateFolder(_settings.FolderPath, _settings.FolderName);
                }

                var serializedState = JsonConvert.SerializeObject(currentState, Formatting.Indented);
                var filePath = $"{_settings.FolderPath}/{_settings.FolderName}/{_settings.FileName}.json";
                System.IO.File.WriteAllText($"{filePath}", serializedState);

                AssetDatabase.Refresh();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
#else
            return false;
#endif

        }

        public void OverrideLocalFeatureFlag(BackendEnvironment env, string flagId, bool newValue)
        {
            FeatureFlagsFileData newFileData = _currentAppState.FeatureFlagsFileData;
            newFileData.Data[env][flagId] = newValue;

            var newState = new FeatureFlagsAppState()
            {
                EnablingUsageToggle = _currentAppState.EnablingUsageToggle,
                UseLocalVersion = _currentAppState.UseLocalVersion,
                FeatureFlagsFileData = newFileData,
            };

            UpdateLocalData(newState);
        }
    }
}
