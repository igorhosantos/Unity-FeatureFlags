using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using FeatureFlags.Data;
using FeatureFlags.Model;

namespace FeatureFlags.Editor.Tool
{
    public class FeatureFlagsTool : EditorWindow
    {
        // View/Visual Elements
        private VisualElement _rootFromUxml;
        private DropdownField _levelState;
        private ScrollView _actionList;
        private VisualElement _contentOptions;
        private ScrollView _contentList;
        private Label _logs;
        private Label _contentName;
        private ScrollView _splitContents;
        private ObjectField _settingsField;
        private Toggle _enablingUsageToggle;
        private Toggle _usingLocalFlagsToggle;

        // Controllers and Logic
        private IFeatureFlagsToolController _controller;
        private Dictionary<string, Action> _actions;
        private readonly List<Toggle> _records = new List<Toggle>();
        private readonly List<VisualElement> _containers = new List<VisualElement>();
       

        [MenuItem("Feature Flags/Open Panel")]
        public static void ShowMyEditor()
        {
            EditorWindow wnd = GetWindow<FeatureFlagsTool>();
            wnd.titleContent = new GUIContent("Feature Flags Tool");

            wnd.minSize = new Vector2(450, 200);
            wnd.maxSize = new Vector2(1920, 720);
        }

        public void CreateGUI()
        {
            _controller = new FeatureFlagsToolController();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FeatureFlags/Editor/UIToolkit/FeatureFlagsTool.uxml");
            _rootFromUxml = visualTree.Instantiate();
            rootVisualElement.Add(_rootFromUxml);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FeatureFlags/Editor/UIToolkit/FeatureFlagsTool.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _actions = new Dictionary<string, Action>
            {
                {"Get Flags From Provider", FetchAllFlagsApi},
                {"Get Local Flags", FetchAllFlagsFromLocal},
                {"Update Local Flags From Provider", UpdateLocalFromBackend},
            };


            VisualElement buttons = _rootFromUxml.Q<VisualElement>("Options");

            _actionList = buttons.Q<ScrollView>("ActionList");
            _contentOptions = _rootFromUxml.Q<VisualElement>("Contents");
            _contentList = _contentOptions.Q<ScrollView>("ResultScroll");
            _logs = _contentOptions.Q<Label>("Logs");

            VisualElement toggleContent = _rootFromUxml.Q<VisualElement>("EnablingTool");
            _enablingUsageToggle = toggleContent.Q<Toggle>("EnablingToggle");
            _enablingUsageToggle.value = _controller.EnablingUsageToggle;

            _usingLocalFlagsToggle = toggleContent.Q<Toggle>("UsingLocalToggle");
            _usingLocalFlagsToggle.value = _controller.UseLocalVersion;

            _enablingUsageToggle.RegisterValueChangedCallback(OnEnablingUsageChange);
            _usingLocalFlagsToggle.RegisterValueChangedCallback(OnUsingLocalChange);
            
            //show app version as a readonly
            VisualElement appVersionContent = _rootFromUxml.Q<VisualElement>("AppVersion");
            Label appVersionLabel = appVersionContent.Q<Label>("AppVersionLabel");
            appVersionLabel.text = $"Current App Version: {Application.version}";
            
            VisualElement dataInfoContainer = _rootFromUxml.Q<VisualElement>("DataInfoContainer");
            _settingsField = dataInfoContainer.Q<ObjectField>("ObjectFieldDataInfo");
            
            Button button  = dataInfoContainer.Q<Button>("DataInfoButton");
            button.clicked += SetupDataInfo;
            
            SetupDataInfo();
            AddActions();
        }

        private void SetupDataInfo()
        {
            ProcessSettingsFile();
            //ProcessLocalDataInfo();
            //ProcessDataFromProvider();
        }

        private void ProcessSettingsFile()
        { 
            _logs.text = string.Empty;
            ColorUtility.TryParseHtmlString("#00ff00", out Color successColor);
            ColorUtility.TryParseHtmlString("#ff0000", out Color failedColor);

            try
            {
                //find one settings file already set
                var scriptableObjectFile = (FeatureFlagsSettingsScriptableObject)_settingsField.value;
                if(scriptableObjectFile != null)
                {
                    _logs.text = $"ProcessSettingsFile Successfully";
                    _logs.style.color = new StyleColor(successColor);
                    _controller.Initialize(scriptableObjectFile.Settings);
                    return;
                }
                
                //try to search a valid settings file in the project
                var allAvailableSettingsGuids = AssetDatabase.FindAssets($"t:{nameof(FeatureFlagsSettingsScriptableObject)}");
                var allAvailableSettings = allAvailableSettingsGuids
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<FeatureFlagsSettingsScriptableObject>)
                    .Where(asset => asset != null)
                    .ToArray();

                var searchedSettings = allAvailableSettings.First();
                
                if(searchedSettings != null)
                {
                    _logs.text = $"ProcessSettingsFile Successfully";
                    _logs.style.color = new StyleColor(successColor);
                    _settingsField.value = searchedSettings;
                    _controller.Initialize(searchedSettings.Settings);
                    return;
                }

                _logs.text = $"Failed during ProcessSettingsFile";
            }
            catch (Exception e)
            {
                _logs.text = $"Failed during ProcessSettingsFile: {e}";
            }

            _logs.style.color = new StyleColor(failedColor);
            
        }

        /// <summary>
        /// setting up the path for getting the data info
        /// </summary>
        private void ProcessLocalDataInfo()
        {
            _logs.text = string.Empty;
            ColorUtility.TryParseHtmlString("#00ff00", out Color successColor);
            ColorUtility.TryParseHtmlString("#ff0000", out Color failedColor);

            try
            {
                //try to access the local saved version if exists
                var localData = _controller.FetchLocalData();
            
                if(localData != null)
                {
                    _logs.text = $"Local Data Successfully Loaded";
                    _logs.style.color = new StyleColor(successColor);
                    return;
                }

                _logs.text = $"Invalid Local Data";
            }
            catch (Exception e)
            {
                _logs.text = $"Invalid Local Data \n {e}";
            }

            _logs.style.color = new StyleColor(failedColor);
        }

        /// <summary>
        /// setting up the path for getting the data info
        /// </summary>
        private void ProcessDataFromProvider()
        {
            _logs.text = string.Empty;
            ColorUtility.TryParseHtmlString("#00ff00", out Color successColor);
            ColorUtility.TryParseHtmlString("#ff0000", out Color failedColor);

            try
            {
                //try to access the local saved version if exists
                var localData = _controller.FetchDataFromProvider();
            
                if(localData != null)
                {
                    _logs.text = $"Local Data Successfully Loaded";
                    _logs.style.color = new StyleColor(successColor);
                    return;
                }

                _logs.text = $"Invalid Local Data";
            }
            catch (Exception e)
            {
                _logs.text = $"Invalid Local Data \n {e}";
            }

            _logs.style.color = new StyleColor(failedColor);
        }
        
        private void OnEnablingUsageChange(ChangeEvent<bool> evt)
        {
            _controller.EnablingUsageToggle = evt.newValue;
        }
        private void OnUsingLocalChange(ChangeEvent<bool> evt)
        {
            _controller.UseLocalVersion = evt.newValue;
        }

        private void AddActions()
        {
            foreach (var action in _actions)
            {
                var button = AddActionButton(action.Key, action.Value);
                _actionList.Add(button);

            }
        }

        private void RemoveAllContent()
        {
            for (int i = 0; i < _containers.Count; i++)
            {
                _contentList.Remove(_containers[i]);
            }

            _containers.Clear();

        }

        private Button AddActionButton(string text, Action action)
        {
            var button = new Button(action)
            {
                text = text,
            };
            return button;
        }


        private void FetchAllFlagsFromLocal()
        {
            _logs.text = "Current tags from local file:";
            ProcessContent(true);
        }

        private void FetchAllFlagsApi()
        {
            _logs.text = "Current tags from Backend API:";
            ProcessContent(false);
        }

        private void UpdateLocalFromBackend()
        {
            _logs.text = string.Empty;
            RemoveAllContent();
            ProcessUpdateLocalFromBackend();
        }

        private void ProcessUpdateLocalFromBackend()
        {
            var result = _controller.UpdateLocalFromProvider();
             _logs.text = result ? "Updated Successfully!" : "Error during update, check the logs";
        }

        private void ProcessContent(bool isLocal = false)
        {
            RemoveAllContent();
            
            FeatureFlagsFileData response = isLocal ? _controller.FetchLocalData() : _controller.FetchDataFromProvider();
            
            if (response == null)
            {
                _logs.text = isLocal ? $"Can't fetch local data, call {nameof(UpdateLocalFromBackend)} first" : "Invalid data from Api";
                return;
            }

            if (response.Data.TryGetValue(BackendEnvironment.Dev, out Dictionary<string, bool> flagsDev))
            {
                SetContentPerEnvironment(isLocal,BackendEnvironment.Dev, flagsDev);
            }
            else
            {
                _logs.text += "\n Flags from Dev are not available";
            }

            if (response.Data.TryGetValue(BackendEnvironment.Prod, out Dictionary<string, bool> flagsProd))
            {
                SetContentPerEnvironment(isLocal,BackendEnvironment.Prod, flagsProd);
            }
            else
            {
                _logs.text += "\n Flags from Prod are not available";
            }

        }

        private void SetContentPerEnvironment(bool isLocal, BackendEnvironment environmentId, Dictionary<string, bool> flags)
        {
            //set the container per environment
            var container = new ScrollView();
            var textTitle = new Label(environmentId.ToString());

            //align environment visual elements horizontally
            container.style.width = new StyleLength(250);
            container.mode = ScrollViewMode.Vertical;

            container.Add(textTitle);
            _containers.Add(container);

            foreach (var keyValuePair in flags)
            {
                var toggle = new Toggle
                {
                    text = $"[{environmentId}]: {keyValuePair.Key}",
                    value = keyValuePair.Value,
                };

                toggle.SetEnabled(isLocal);

                if (isLocal)
                {
                    toggle.RegisterValueChangedCallback(
                        new EventCallback<ChangeEvent<bool>>(evt =>
                        {
                            NotifyLocalChange(environmentId, keyValuePair.Key, evt.newValue);
                        }));
                }

                container.Add(toggle);
                _records.Add(toggle);
            }

            _contentList.Add(container);
        }

        private void NotifyLocalChange(BackendEnvironment env, string flagId, bool newValue)
        {
            Debug.Log($"Override from toggle: {env} - {flagId} - {newValue}");
            var result = _controller.OverrideLocalFeatureFlag(env, flagId, newValue);
        }

    }
}
