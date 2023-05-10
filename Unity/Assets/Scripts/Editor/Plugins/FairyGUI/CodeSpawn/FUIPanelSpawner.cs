using System;
using System.IO;
using System.Text;
using ET;
using UnityEngine;

namespace FUIEditor
{
    public static class FUIPanelSpawner
    {
        public static void SpawnSubPanel(string packageName, ComponentInfo componentInfo)
        {
            string fileDir = "{0}/{1}".Fmt(FUICodeSpawner.ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string componentName = componentInfo.NameWithoutExtension;
            string nameSpace = componentInfo.NameSpace;
            string filePath = "{0}/{1}.cs".Fmt(fileDir, componentName);
            if (File.Exists(filePath))
            {
                // Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For SubComponent {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("\t[ChildOf]");
            sb.AppendLine($"\tpublic class {componentName}: Entity, IAwake<FUI_{componentName}>");
            sb.AppendLine("\t{");
            
            // 子组件
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported)
                {
                    return;
                }

                if (variableInfo.ComponentInfo?.PanelType != PanelType.Common)
                {
                    return;
                }

                int index = variableInfo.TypeName.IndexOf("FUI_", StringComparison.Ordinal);
                string comName = variableInfo.TypeName.Substring(index + 4);
                sb.AppendLine($"\t\tpublic {comName} {variableInfo.VariableName} {{get; set;}}");
            });
            
            sb.AppendLine($"\t\tpublic FUI_{componentName} FUI{componentName} {{ get; set; }}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            File.WriteAllText(filePath, sb.ToString());
        }
        
        public static void SpawnPanel(string packageName, ComponentInfo componentInfo)
        {
            string nameSpace = componentInfo.NameSpace;
            string panelName = componentInfo.NameWithoutExtension;
            
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
            
            // 子组件
            componentInfo.VariableInfos.ForEach(variableInfo =>
            {
                if (!variableInfo.IsExported)
                {
                    return;
                }

                if (variableInfo.ComponentInfo?.PanelType != PanelType.Common)
                {
                    return;
                }

                int index = variableInfo.TypeName.IndexOf("FUI_", StringComparison.Ordinal);
                string comName = variableInfo.TypeName.Substring(index + 4);
                sb.AppendLine($"\t\tpublic {comName} {variableInfo.VariableName} {{get; set;}}\n");
            });
            
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