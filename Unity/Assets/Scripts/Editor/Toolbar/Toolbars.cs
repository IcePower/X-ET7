using System.IO;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace ET
{
    [InitializeOnLoad]
    public static class Toolbars
    {
        private static readonly Texture FolderIcon = EditorGUIUtility.IconContent(@"Folder Icon").image;
        private static readonly GUIContent PersistentDataPath;
        private static readonly GUIContent ProjectPath;

        private static readonly GUILayoutOption Width = GUILayout.Width(100f);
        private static readonly GUILayoutOption Height = GUILayout.Height(23f);
        
        static Toolbars()
        {
            ProjectPath = new GUIContent("Project", FolderIcon, "找到项目目录");
            PersistentDataPath = new GUIContent("Persistent", FolderIcon, "找到持久化数据目录");
            
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        }

        private static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ProjectPath, Width, Height))
            {
                EditorUtility.RevealInFinder(Path.GetDirectoryName(Application.dataPath));
            }
            
            if (GUILayout.Button(PersistentDataPath, Width, Height))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }

        private static void OnRightToolbarGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}