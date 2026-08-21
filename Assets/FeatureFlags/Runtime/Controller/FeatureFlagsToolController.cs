using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Exception = System.Exception;
using FeatureFlags.Data;

namespace FeatureFlags
{
    /// <summary>
    /// Class responsible to isolate all the logic from Feature Flag Tool Window
    /// </summary>
    public class FeatureFlagsToolController: IFeatureFlagsToolController
    {
        
        private FeatureFlagsAppState _currentAppState = new FeatureFlagsAppState();

        public FeatureFlagsFileData FetchLocalData()
        {
            throw new NotImplementedException();
        }

        public FeatureFlagsFileData FetchDataFromProvider()
        {
            throw new NotImplementedException();
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
            var flagsData = new FeatureFlagsFileData();
            
           /* // Get the list of flags to populate
            var flagsToPopulate = new List<string>();
            if (_fallbackFeatureFlags != null)
            {
                flagsToPopulate.AddRange(_fallbackFeatureFlags);
            }
            
            // Remove duplicates
            flagsToPopulate = flagsToPopulate.Distinct().ToList();
            
            // Create default flag dictionaries for both environments
            var defaultFlags = flagsToPopulate.ToDictionary(flag => flag, flag => false);
            
            flagsData.SetDataPerEnvironment(BackendEnvironment.Dev, new Dictionary<string, bool>(defaultFlags));
            flagsData.SetDataPerEnvironment(BackendEnvironment.Prod, new Dictionary<string, bool>(defaultFlags));*/
            
            return flagsData;
        }

        public FeatureFlagsToolController()
        {
            FetchLocalFeatureFlags();
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
                CreateOrUpdateLocalData(newState);
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
                CreateOrUpdateLocalData(newState);
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
        public FeatureFlagsFileData FetchLocalFeatureFlags()
        {
            return CreateFeatureFlagsWithDefaults();
            /*
            // For API-only mode, initialize with defaults and skip file loading
            if (IsApiOnlyMode)
            {
                var defaultFeatureFlagsData = CreateFeatureFlagsWithDefaults();
                _currentAppState = new FeatureFlagsAppState()
                {
                    EnablingUsageToggle = false,
                    UseLocalVersion = false,
                    FeatureFlagsFileData = defaultFeatureFlagsData
                };
                return defaultFeatureFlagsData;
            }

            try
            {
                TextAsset file = Resources.Load<TextAsset>($"{FeatureFlagsUtils.FolderName}/{FeatureFlagsUtils.FileName}");
                if (file == null)
                {
                    return CreateFeatureFlagsWithDefaults();
                }

                _currentAppState = JsonConvert.DeserializeObject<FeatureFlagsAppState>(file.text);
                return _currentAppState.FeatureFlagsFileData;

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return CreateFeatureFlagsWithDefaults();
            }*/
        }

        /// <summary>
        /// It will create or update the local file based on the BE data
        /// </summary>
        /// <returns></returns>
        public FeatureFlagsFileData FetchApiFeatureFlags(List<string> availableFlags, bool requestDev, bool requestProd)
        {
            try
            {

                return new FeatureFlagsFileData();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            return CreateFeatureFlagsWithDefaults();
        }

        private Dictionary<string,bool> ProcessFlagsByEnvironment(Dictionary<string, object> data)
        {
            var dictio = data
                .ToDictionary( p=> p.Key, p=> (bool)p.Value);

            return dictio;
        }


        private bool CreateOrUpdateLocalData(FeatureFlagsAppState newAppState)
        {
            _currentAppState = newAppState;
            return CreateOrUpdateLocalData(_currentAppState.FeatureFlagsFileData);
        }

        private bool CreateOrUpdateLocalData(FeatureFlagsFileData fileData)
        {
            return false;
            /*
#if UNITY_EDITOR
            // Skip file operations entirely in API-only mode
            if (IsApiOnlyMode)
            {
                return false;
            }

            try
            {
                var currentState = new FeatureFlagsAppState()
                {
                    EnablingUsageToggle = this.EnablingUsageToggle,
                    UseLocalVersion = this.UseLocalVersion,
                    FeatureFlagsFileData = fileData,
                };

                if (!AssetDatabase.IsValidFolder($"{FeatureFlagsUtils.FolderPath}/{FeatureFlagsUtils.FolderName}"))
                {
                    AssetDatabase.CreateFolder(FeatureFlagsUtils.FolderPath, FeatureFlagsUtils.FolderName);
                }

                var serializedState = JsonConvert.SerializeObject(currentState, Formatting.Indented);
                public string FilePath = $"{FolderPath}/{FolderName}/{FileName}.json";
                await System.IO.File.WriteAllTextAsync($"{FilePath}", serializedState);

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
*/
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

            CreateOrUpdateLocalData(newState);
        }
    }
}
