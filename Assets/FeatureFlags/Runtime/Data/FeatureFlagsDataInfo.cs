using System.Collections.Generic;
using UnityEngine;

namespace FeatureFlags
{
    /// <summary>
    /// A collection of sensitive data info of the feature flags
    /// </summary>
    [CreateAssetMenu(fileName = "FeatureFlagsDataInfo", menuName = "Feature Flags/FeatureFlagsDataInfo")]
    public class FeatureFlagsDataInfo : ScriptableObject
    {
        [SerializeField] private List<string> _data;
        public List<string> Data => _data;
    }
}
