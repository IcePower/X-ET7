using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSystemSpawner
    {
        public static void SpawnPanelSystem(string packageName, ComponentInfo componentInfo)
        {
            string panelName = "{0}Panel".Fmt(packageName);

            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            StringBuilder sb = new StringBuilder();
            string filePath = "{0}/{1}System.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                return;
            }
            
            Debug.Log("Spawn Code For PanelSystem {0}".Fmt(filePath));
            
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine($"[FriendOf(typeof({panelName}))]");
            sb.AppendFormat("\tpublic static class {0}System", panelName);
            sb.AppendLine();
            sb.AppendLine("\t{");
            
            sb.AppendFormat("\t\tpublic static void Awake(this {0} self)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void RegisterUIEvent(this {0} self)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void OnShow(this {0} self, Entity contextData = null)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void OnHide(this {0} self)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void BeforeUnload(this {0} self)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t}");
            sb.Append("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}