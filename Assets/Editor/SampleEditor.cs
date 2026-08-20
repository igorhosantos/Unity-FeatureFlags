using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

//just for compilation purpose 
namespace Editor
{
    public static class SampleEditor
    {
        [MenuItem("Sample/Show Simple Log")]
        public static void ShowSimpleLog()
        {
            Debug.Log("Show Simple Log");
        }
    }
}
#endif
