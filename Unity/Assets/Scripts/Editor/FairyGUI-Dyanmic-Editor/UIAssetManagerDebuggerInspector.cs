using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FairyGUI.Dynamic.Editor
{
    [CustomEditor(typeof(UIAssetManager.Debugger))]
    public class UIAssetManagerDebuggerInspector : UnityEditor.Editor
    {
        private UIAssetManager.Debugger m_Debugger;

        private void OnEnable()
        {
            m_Debugger = (UIAssetManager.Debugger)target;
        }

        public override void OnInspectorGUI()
        {
            var dict = m_Debugger.GetUIPackageRefCounts();

            m_UIPackageNames.Clear();
            m_UIPackageNames.AddRange(dict.Keys);
            m_UIPackageNames.Sort();

            foreach (var packageName in m_UIPackageNames)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("packageName", packageName);
                EditorGUILayout.LabelField("referenceCount", dict[packageName].ToString());
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Unload Unused UI Packages"))
                m_Debugger.UnloadUnusedUIPackages();

            if (GUILayout.Button("Unload All UI Packages"))
                m_Debugger.UnloadAllUIPackages();

            EditorUtility.SetDirty(target);
        }

        private readonly List<string> m_UIPackageNames = new List<string>();
    }
}