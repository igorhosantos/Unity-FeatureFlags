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
    public class FeatureFlagsToolBehavior
    {
        private const string _dataPath = "Party/Data";
        private FeatureFlagsAppState _currentAppState = new FeatureFlagsAppState();
        private FeatureFlagsDataInfo _listPartnerDataInfo;
        public FeatureFlagsDataInfo ListPartnerDataInfo => _listPartnerDataInfo;
        private string _userId = string.Empty;
        private bool _usePartnerListData;
        private List<string> _fallbackFeatureFlags;

        /// <summary>
        /// Indicates whether this instance is configured for API-only operation (no local files)
        /// </summary>
        private bool IsApiOnlyMode => _fallbackFeatureFlags != null && _fallbackFeatureFlags.Count > 0;

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

        public FeatureFlagsToolBehavior(bool usePartnerListData = true)
        {
            Initialize(null, usePartnerListData);
        }

        public FeatureFlagsToolBehavior(List<string> fallbackFeatureFlags)
        {
            Initialize(fallbackFeatureFlags, false);
        }

        private void Initialize(List<string> fallbackFeatureFlags, bool usePartnerListData)
        {
            _fallbackFeatureFlags = fallbackFeatureFlags;
            _usePartnerListData = usePartnerListData;

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

                // Only attempt file operations if not in API-only mode
                if (!IsApiOnlyMode)
                {
                    CreateOrUpdateLocalData(newState);
                }
            }
        }

        /// <summary>
        ///  This Behavior will fetch all the flags that comes from:
        ///  - Flags Data Info (Scriptable Object
        ///  - GeniesPartyFeatureFlags (Flags that we're using only on Genies Party)
        /// </summary>
        /// <returns></returns>
        public List<string> FetchFlagsDataInfo()
        {
            try
            {
                // For API-only mode, use fallback feature flags
                if (IsApiOnlyMode)
                {
                    var apiOnlyList = new List<string>();
                    if (_fallbackFeatureFlags != null)
                    {
                        apiOnlyList.AddRange(_fallbackFeatureFlags);
                    }
                    return apiOnlyList;
                }

                if (_listPartnerDataInfo != null)
                {
                    var currentList = new List<string>();
                    if (_listPartnerDataInfo.Data != null)
                    {
                        currentList.AddRange(_listPartnerDataInfo.Data);
                    }
                    
                    return currentList;
                }

                if (_usePartnerListData)
                {
                    FeatureFlagsDataInfo[] dataFiles = Resources.LoadAll<FeatureFlagsDataInfo>(_dataPath);

                    if (dataFiles == null || dataFiles.Length == 0)
                    {
                        Debug.LogError($"ListPartnerDataInfo not found it");
  
                        return new List<string>();
                    }

                    _listPartnerDataInfo = dataFiles.FirstOrDefault(d => d.name.Contains("Flag"));

                    if (_listPartnerDataInfo == null)
                    {
                        Debug.LogError($"Invalid data info for Feature Flags");
                        return new List<string>();
                    }
                }

                //considering the list from shared feature flag as well
                var finalList = new List<string>();
                if (_listPartnerDataInfo != null && _listPartnerDataInfo.Data != null)
                {
                    finalList.AddRange(_listPartnerDataInfo.Data);
                }
                
                return finalList;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new List<string>();
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

                // Only attempt file operations if not in API-only mode
                if (!IsApiOnlyMode)
                {
                    CreateOrUpdateLocalData(newState);
                }
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
                await System.IO.File.WriteAllTextAsync($"{FeatureFlagsUtils.FilePath}", serializedState);

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

    [Serializable]
    public class FeatureFlagsAppState
    {
        public bool EnablingUsageToggle;
        public bool UseLocalVersion;
        public FeatureFlagsFileData FeatureFlagsFileData;
    }
}
