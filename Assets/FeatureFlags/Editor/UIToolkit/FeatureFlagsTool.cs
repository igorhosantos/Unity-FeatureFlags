using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using FeatureFlags;
using FeatureFlags.Data;

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
        private ObjectField _dataInfoPathField;
        private Toggle _enablingUsageToggle;
        private Toggle _usingLocalFlagsToggle;

        // Controllers and Logic
        private FeatureFlagsToolBehavior _ffToolBehavior;
        private Dictionary<string, Action> _actions;
        private readonly List<Toggle> _records = new List<Toggle>();
        private readonly List<VisualElement> _containers = new List<VisualElement>();
        private FeatureFlagsDataInfo _listPartnerDataInfo;


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
            _ffToolBehavior = new FeatureFlagsToolBehavior();

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FeatureFlags/Editor/UIToolkit/FeatureFlagsTool.uxml");
            _rootFromUxml = visualTree.Instantiate();
            rootVisualElement.Add(_rootFromUxml);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FeatureFlags/Editor/UIToolkit/FeatureFlagsTool.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _actions = new Dictionary<string, Action>
            {
                {"Fetch Local Feature Flags", FetchAllFlagsFromLocal},
                {"Fetch Backend Feature Flags", FetchAllFlagsApi},
                {"Update Local FFs From Backend FFs", UpdateLocalFromBackend},
            };


            VisualElement buttons = _rootFromUxml.Q<VisualElement>("Options");

            _actionList = buttons.Q<ScrollView>("ActionList");
            _contentOptions = _rootFromUxml.Q<VisualElement>("Contents");
            _contentList = _contentOptions.Q<ScrollView>("ResultScroll");
            _logs = _contentOptions.Q<Label>("Logs");

            VisualElement toggleContent = _rootFromUxml.Q<VisualElement>("EnablingTool");
            _enablingUsageToggle = toggleContent.Q<Toggle>("EnablingToggle");
            _enablingUsageToggle.value = _ffToolBehavior.EnablingUsageToggle;

            _usingLocalFlagsToggle = toggleContent.Q<Toggle>("UsingLocalToggle");
            _usingLocalFlagsToggle.value = _ffToolBehavior.UseLocalVersion;

            _enablingUsageToggle.RegisterValueChangedCallback(OnEnablingUsageChange);
            _usingLocalFlagsToggle.RegisterValueChangedCallback(OnUsingLocalChange);

            SetupDataInfo();

            AddActions();
            
            //show app version as a readonly
            VisualElement appVersionContent = _rootFromUxml.Q<VisualElement>("AppVersion");
            Label appVersionLabel = appVersionContent.Q<Label>("AppVersionLabel");
            appVersionLabel.text = $"Current App Version: {Application.version}";
        }

        private void SetupDataInfo()
        {
            ProcessDataInfo();
        }

        /// <summary>
        /// setting up the path for getting the data info
        /// </summary>
        private void ProcessDataInfo()
        {
            _logs.text = string.Empty;
            ColorUtility.TryParseHtmlString("#00ff00", out Color successColor);
            ColorUtility.TryParseHtmlString("#ff0000", out Color failedColor);

            try
            {
                //try to access the local saved version if exists
                _ffToolBehavior.FetchFlagsDataInfo();
                _listPartnerDataInfo = _ffToolBehavior.ListPartnerDataInfo;

                if(_listPartnerDataInfo != null)
                {
                    _logs.text = $"Data Info Successfully loaded";
                    _logs.style.color = new StyleColor(successColor);
                    _dataInfoPathField.value = _listPartnerDataInfo;
                    return;
                }

                _listPartnerDataInfo = (FeatureFlagsDataInfo)_dataInfoPathField.value;
                if(_listPartnerDataInfo != null)
                {
                    _logs.text = $"Data Info Successfully loaded";
                    _logs.style.color = new StyleColor(successColor);
                    return;
                }

                _logs.text = $"Invalid Path for Party Data Info";
                _dataInfoPathField.value = null;
            }
            catch (Exception e)
            {
                _logs.text = $"Invalid Path for Party Data Info \n {e}";
                _dataInfoPathField.value = null;
            }

            _logs.style.color = new StyleColor(failedColor);
        }

        private void OnEnablingUsageChange(ChangeEvent<bool> evt)
        {
            _ffToolBehavior.EnablingUsageToggle = evt.newValue;
        }
        private void OnUsingLocalChange(ChangeEvent<bool> evt)
        {
            _ffToolBehavior.UseLocalVersion = evt.newValue;
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
            var result = _ffToolBehavior.UpdateLocalFromBackend();
             _logs.text = result ? "Updated Successfully!" : "Error during update, check the logs";
        }

        private void ProcessContent(bool isLocal = false)
        {
            RemoveAllContent();

            var finalList = new List<string>();
            finalList.AddRange(_listPartnerDataInfo.Data);
           
            FeatureFlagsFileData response = isLocal ?
                _ffToolBehavior.FetchLocalFeatureFlags() :
                _ffToolBehavior.FetchApiFeatureFlags(finalList, requestDev:true, requestProd:true);
            
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

            //align environment visual elements horizontaly
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
            _ffToolBehavior.OverrideLocalFeatureFlag(env, flagId, newValue);
        }

    }
}
