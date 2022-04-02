using UnityEditor;
using UnityEngine;

namespace Gadget.Editor.Internal.Utilities
{
    internal static class InfoBoxUtils
    {
        public static float GetInfoBoxHeight(string text)
        {
            return GUI.skin.box.CalcHeight(new GUIContent(text),
                EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth * 2);
        }       
    }
}