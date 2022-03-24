using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.Internal.Utilities
{
    internal static class ScriptableObjectUtils
    {
        public static bool TryCreateNewAsset(Type type, out ScriptableObject scriptableObject)
        {
            scriptableObject = ScriptableObject.CreateInstance(type);

            var dest = EditorUtility.SaveFilePanel($"Save {type.Name} as", Application.dataPath,
                "New " + type.Name, "asset");

            var assetsIndex = dest.IndexOf("Assets/", StringComparison.Ordinal);

            if (assetsIndex >= 0) 
                dest = dest.Substring(assetsIndex);

            if (string.IsNullOrEmpty(dest))
            {
                Object.DestroyImmediate(scriptableObject);
                return false;
            }

            try
            {
                AssetDatabase.CreateAsset(scriptableObject, dest);
            }
            catch (UnityException)
            {
                Object.DestroyImmediate(scriptableObject);
                return false;
            }
            
            AssetDatabase.Refresh();
            return true;
        }
    }
}