using System;
using System.Collections.Generic;
using UnityEditor;

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
            var dict = m_Debugger.GetUIPackageInfoDict();
            
            m_List.Clear();
            m_List.AddRange(dict.Values);
            m_List.Sort(SortByName);

            foreach (var info in m_List)
            {
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.LabelField("packageName", info.PackageName);
                EditorGUILayout.LabelField("referenceCount", info.ReferenceCount.ToString());
                EditorGUILayout.LabelField("referenceCountByOtherPackage", info.BeDependentPackageRefInfos.Count.ToString());
                EditorGUILayout.LabelField("isLoadDone", info.IsLoadDone.ToString());
                
                EditorGUILayout.EndVertical();
            }
            
            EditorUtility.SetDirty(target);
        }

        private int SortByName(UIAssetManager.UIPackageInfo x, UIAssetManager.UIPackageInfo y)
        {
            return string.Compare(x.PackageName, y.PackageName, StringComparison.Ordinal);
        }

        private readonly List<UIAssetManager.UIPackageInfo> m_List = new List<UIAssetManager.UIPackageInfo>();
    }
}
