using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSpawner
    {
        public static void SpawnPanel(string packageName, string nameSpace)
        {
            string panelName = "{0}Panel".Fmt(packageName);
            
            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For Panel {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\t[ComponentOf(typeof(FUIEntity))]");
            sb.AppendFormat("\tpublic class {0}: Entity, IAwake", panelName);
            sb.AppendLine();
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic SystemLanguage Language {get; set;}");
            sb.AppendLine();
            sb.AppendLine("\t\tprivate FUI_{0} _fui{0};".Fmt(panelName));
            sb.AppendLine();
            sb.AppendLine("\t\tpublic FUI_{0} FUI{0}".Fmt(panelName));
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tget => _fui{0} ??= (FUI_{0})this.GetParent<FUIEntity>().GComponent;".Fmt(panelName));
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

    }
}