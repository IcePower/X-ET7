using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSystemSpawner
    {
        public static void SpawnPanelSystem(string packageName)
        {
            string panelName = "{0}Panel".Fmt(packageName);

            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            string filePath = "{0}/{1}System.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For PanelSystem {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
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
            sb.AppendFormat("\t\tpublic static void OnShow(this {0} self, Entity contexData = null)", panelName);
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