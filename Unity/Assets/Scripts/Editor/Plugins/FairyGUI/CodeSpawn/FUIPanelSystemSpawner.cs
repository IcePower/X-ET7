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
                ReSpawnCode(componentInfo, sb, panelName, filePath);
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
            SpawnTranslateText(componentInfo, sb, panelName);
            sb.AppendLine();
            
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

        private static void ReSpawnCode(ComponentInfo componentInfo, StringBuilder sb, string panelName, string filePath)
        {
            // 文件已经存在的话，重新生成 TranslateText 方法
            SpawnTranslateText(componentInfo, sb, panelName);

            string fileContent = File.ReadAllText(filePath);
            int startPos = fileContent.IndexOf("\t\tpublic static void TranslateText", StringComparison.Ordinal);
            if (startPos != -1)
            {
                // 已有 TranslateText 方法，替换掉
                int endPos = fileContent.IndexOf("}", startPos, StringComparison.Ordinal);
                endPos = fileContent.IndexOf("}", endPos + 1, StringComparison.Ordinal);
                string oldCode = fileContent.Substring(startPos, endPos - startPos + 1);
                string newCode = fileContent.Replace(oldCode, sb.ToString());
                File.WriteAllText(filePath, newCode);
            }
            else
            {
                // 没有 TranslateText 方法，插入到 RegisterUIEvent 方法之前
                startPos = fileContent.IndexOf("\t\tpublic static void RegisterUIEvent", StringComparison.Ordinal);
                if (startPos == -1)
                {
                    throw new Exception("没找到 RegisterUIEvent 函数，生成 TranslateText 函数失败！");
                }

                string newCode = fileContent.Insert(startPos, $"{sb.ToString()}\n\n");
                File.WriteAllText(filePath, $"{newCode}");
            }
        }

        private static void SpawnTranslateText(ComponentInfo componentInfo, StringBuilder sb, string panelName)
        {
            sb.AppendFormat("\t\tpublic static void TranslateText(this {0} self, SystemLanguage systemLanguage, Func<string, string, string> translator)", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t//本函数自动生成，请勿手动修改");
            sb.AppendLine("\t\t\tif (self.Language == systemLanguage)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\treturn;");
            sb.AppendLine("\t\t\t}\n");

            // 去掉 typeName 为空的变量
            List<VariableInfo> variableInfos = new List<VariableInfo>();
            for (int i = 0; i < componentInfo.VariableInfos.Count; i++)
            {
                if (!string.IsNullOrEmpty(componentInfo.VariableInfos[i].TypeName))
                {
                    variableInfos.Add(componentInfo.VariableInfos[i]);
                }
            }

            for (int i = 0; i < variableInfos.Count; i++)
            {
                if (string.IsNullOrEmpty(variableInfos[i].LanguageKey))
                {
                    continue;
                }

                string textVariableName = $"self.FUI{panelName}.{variableInfos[i].VariableName}.text";
                sb.AppendFormat($"\t\t\t{textVariableName} = translator(\"{variableInfos[i].LanguageKey}\", {textVariableName});");
                sb.AppendLine();
            }

            sb.Append("\t\t}");
        }
    }
}