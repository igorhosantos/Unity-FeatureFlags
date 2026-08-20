using System;
using UnityEngine;

namespace FeatureFlags
{
    [Serializable]
    public class FeatureData
    {
        public string featureFlag;
        public bool enabled;

#if UNITY_EDITOR
        public bool shouldOverrideRemote;
#endif
    }
    
    public class FeatureConfig : ScriptableObject
    {
        public FeatureData[] features;
    }
}
