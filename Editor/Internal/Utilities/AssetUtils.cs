using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InspectorEssentials.Editor.Internal.Utilities
{
    internal static class AssetUtils
    {
        public static bool TryCreateScriptableObject(Type type, out ScriptableObject scriptableObject)
        {
            scriptableObject = ScriptableObject.CreateInstance(type);

            return TryCreateAsset(type, scriptableObject, "asset");
        }
        
        public static bool TryCreateScriptableObject<T>(out T scriptableObject) where T : ScriptableObject
        {
            scriptableObject = ScriptableObject.CreateInstance(typeof(T)) as T;

            return TryCreateAsset(scriptableObject, "asset");
        }

        public static bool TryCreateMaterial(out Material material)
        {
            material = new Material(Shader.Find("Standard"));

            return TryCreateAsset(material, "mat");
        }

        private static bool TryCreateAsset(Type type, Object asset, string extension)
        {
            var dest = EditorUtility.SaveFilePanel($"Save {type.Name} as", Application.dataPath,
                "New " + type.Name, extension);

            var assetsIndex = dest.IndexOf("Assets/", StringComparison.Ordinal);

            if (assetsIndex >= 0) 
                dest = dest.Substring(assetsIndex);

            if (string.IsNullOrEmpty(dest))
            {
                Object.DestroyImmediate(asset);
                return false;
            }

            try
            {
                AssetDatabase.CreateAsset(asset, dest);
            }
            catch (UnityException)
            {
                Object.DestroyImmediate(asset);
                return false;
            }
            
            AssetDatabase.Refresh();
            return true;
        }

        private static bool TryCreateAsset<T>(T asset, string extension) where T : Object
        {
            return TryCreateAsset(typeof(T), asset, extension);
        }
    }
}