using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;
using FairyGUI.Utils;

namespace FUIEditor
{
    public static class FUIComponentSpawner
    {
        private static readonly List<string> ControllerNames = new List<string>();
        private static readonly Dictionary<string, List<string>> ControllerPageNames = new Dictionary<string, List<string>>();
        public static void SpawnComponent(ComponentInfo componentInfo)
        {
            if (!componentInfo.NeedExportClass)
            {
                return;
            }
                
            GatherController(componentInfo);

            FUICodeSpawner.ExportedComponentInfos.Add(componentInfo.PackageId, componentInfo.Id, componentInfo);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine("using FairyGUI.Utils;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", componentInfo.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic partial class {0}{1}: {2}", FUICodeSpawner.ClassNamePrefix, componentInfo.NameWithoutExtension, componentInfo.ComponentClassName);
            sb.AppendLine();
            sb.AppendLine("\t{");

            foreach (var kv in ControllerPageNames)
            {
                sb.AppendFormat("\t\tpublic enum {0}Page", kv.Key);
                sb.AppendLine();
                sb.AppendLine("\t\t{");
                foreach (string pageName in kv.Value)
                {
                    sb.AppendFormat("\t\t\t{0},", pageName);
                    sb.AppendLine();
                }
                sb.AppendLine("\t\t}");
                sb.AppendLine();
            }
            
            for (int i = 0; i < ControllerNames.Count; i++)
            {
                sb.AppendFormat("\t\tpublic Controller {0};", ControllerNames[i]);
                sb.AppendLine();
            }
            
            // 去掉 typeName 为空的变量
            List<VariableInfo> variableInfos = new List<VariableInfo>();
            for (int i = 0; i < componentInfo.VariableInfos.Count; i++)
            {
                if(!string.IsNullOrEmpty(componentInfo.VariableInfos[i].TypeName))
                {
                    variableInfos.Add(componentInfo.VariableInfos[i]);
                }
            }

            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (!IsExportVariable(variableInfos[i]))
                {
                    continue;
                }

                sb.AppendFormat("\t\tpublic {0} {1};", variableInfos[i].TypeName, variableInfos[i].VariableName);
                sb.AppendLine();
            }

            sb.AppendFormat("\t\tpublic const string URL = \"{0}\";", componentInfo.Url);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("\t\tpublic static {0}{1} CreateInstance()", FUICodeSpawner.ClassNamePrefix, componentInfo.NameWithoutExtension);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\treturn ({0}{1})UIPackage.CreateObject(\"{2}\", \"{1}\");", FUICodeSpawner.ClassNamePrefix, componentInfo.NameWithoutExtension, FUICodeSpawner.PackageInfos[componentInfo.PackageId].Name);
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            
            sb.AppendLine("\t\tpublic override void ConstructFromXML(XML xml)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.ConstructFromXML(xml);");
            sb.AppendLine();

            for (int i = 0; i < ControllerNames.Count; i++)
            {
                sb.AppendFormat("\t\t\t{0} = GetControllerAt({1});", ControllerNames[i], i);
                sb.AppendLine();
            }
            
            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (!IsExportVariable(variableInfos[i]))
                {
                    continue;
                }
                
                sb.AppendFormat("\t\t\t{0} = ({1})GetChildAt({2});", variableInfos[i].VariableName, variableInfos[i].TypeName, i);
                sb.AppendLine();
            }
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            string dir = "{0}/{1}".Fmt(FUICodeSpawner.FUIAutoGenDir, FUICodeSpawner.PackageInfos[componentInfo.PackageId].Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}{2}.cs".Fmt(dir, FUICodeSpawner.ClassNamePrefix, componentInfo.NameWithoutExtension);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        private static bool IsExportVariable(VariableInfo variableInfo)
        {
            if (FUICodeSpawner.IgnoreDefaultVariableName && variableInfo.IsDefaultName)
            {
                variableInfo.IsExported = false;
                return false;
            }

            if (variableInfo.IsAppointName)
            {
                variableInfo.IsExported = false;
                return false;
            }

            string typeName = variableInfo.TypeName;
            if (string.IsNullOrEmpty(typeName))
            {
                variableInfo.IsExported = false;
                return false;
            }

            variableInfo.IsExported = true;
            return true;
        }

        private static void GatherController(ComponentInfo componentInfo)
        {
            ControllerNames.Clear();
            ControllerPageNames.Clear();
            foreach (XML controllerXML in componentInfo.ControllerList)
            {
                string controllerName = controllerXML.GetAttribute("name");
                if (!CheckControllerName(controllerName, componentInfo.ComponentType))
                {
                    continue;
                }

                ControllerNames.Add(controllerName);

                List<string> pageNames = new List<string>();
                string[] pages = controllerXML.GetAttribute("pages").Split(',');
                for (int i = 0; i < pages.Length; i++)
                {
                    string page = pages[i];
                    if (i % 2 == 1)
                    {
                        if (!string.IsNullOrEmpty(page))
                        {
                            pageNames.Add(page);
                        }
                    }
                }

                if (pageNames.Count == pages.Length / 2)
                {
                    ControllerPageNames.Add(controllerName, pageNames);
                }
            }
        }

        private static bool CheckControllerName(string controllerName, ComponentType componentType)
        {
            if (componentType == ComponentType.Button || componentType == ComponentType.ComboBox)
            {
                return controllerName != "button";
            }

            return true;
        }
    }
}