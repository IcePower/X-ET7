using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ET;
using FairyGUI.Utils;
using UnityEngine;
using CsvHelper;


namespace FUIEditor
{
    public static class FUILocalizeHandler
    {
        // 本工具导出的 CSV 文件路径
        private static readonly string CSVPath = $"{Application.dataPath}/Config/Excel/Datas/LocalizeConfig_FUI.csv";

        // 增加语言的话，在这里 new LanguageConfig。
        private static readonly LanguageConfig[] LanguageConfigs = new[]
        {
            new LanguageConfig { name = "TextCHS", comment = "简体中文" },
            new LanguageConfig { name = "TextCHT", comment = "繁体中文" },
            new LanguageConfig { name = "TextEN", comment = "英文" },
        };
        
        private class LanguageConfig
        {
            public string name;
            public string comment;
        }
        
        private class I18NItem
        {
            public string key;
            public string name;

            public readonly Dictionary<string, string> TextDict = new Dictionary<string, string>();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(key);
                sb.Append(",");
                sb.Append(name);
                foreach (LanguageConfig languageConfig in LanguageConfigs)
                {
                    sb.Append(",");
                    sb.Append(TextDict[languageConfig.name]);
                }

                return sb.ToString();
            }
        }
        
        public static void Localize(string xmlPath)
        {
            // 解析旧的csv文件，防止已翻译的内容被覆盖。
            Dictionary<string, I18NItem> oldI18NItemDict = ParseCSV(CSVPath);

            XmlToCSV(xmlPath, oldI18NItemDict);
        }
        
        /// <summary>
        /// 解析旧的csv文件
        /// </summary>
        private static Dictionary<string, I18NItem> ParseCSV(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                return null;
            }
            
            Dictionary<string, I18NItem>  i18NItemDict = new Dictionary<string, I18NItem>();

            using StreamReader reader = new StreamReader(csvPath);
            using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    if (csv.GetField("##").StartsWith("##"))
                    {
                        continue;
                    }
                    
                    I18NItem i18NItem = new I18NItem();
                    i18NItem.key = csv.GetField("key");
                    i18NItem.name = csv.GetField("name");
                    foreach (LanguageConfig languageConfig in LanguageConfigs)
                    {
                        i18NItem.TextDict.Add(languageConfig.name, csv.GetField(languageConfig.name));
                    }

                    i18NItemDict.Add(i18NItem.key, i18NItem);
                }
            }

            return i18NItemDict;
        }

        /// <summary>
        /// 将 XML 转为 CSV
        /// </summary>
        private static void XmlToCSV(string xmlPath, Dictionary<string, I18NItem> oldI18NItemDict)
        {
            if (!File.Exists(xmlPath))
            {
                Log.Error("FairyGUI 没有导出多语言文件!");
                return;
            }
            
            StringBuilder sb = new StringBuilder();
            
            // 第一行
            sb.Append("##,key,name");
            for (int i = 0; i < LanguageConfigs.Length; i++)
            {
                LanguageConfig languageConfig = LanguageConfigs[i];
                sb.Append($",{languageConfig.name}");
            }
            sb.AppendLine();
            
            // 第二行
            sb.Append("##type,string,string");
            for (int i = 0; i < LanguageConfigs.Length; i++)
            {
                sb.Append($",string");
            }
            sb.AppendLine();
            
            // 第三行
            sb.Append("##comment,,控件名，方便查找");
            for (int i = 0; i < LanguageConfigs.Length; i++)
            {
                LanguageConfig languageConfig = LanguageConfigs[i];
                sb.Append($",{languageConfig.comment}");
            }
            sb.AppendLine();

            // 内容
            XML xml = new XML(File.ReadAllText(xmlPath));
            for (int index = 0; index < xml.elements.Count; index++)
            {
                XML element = xml.elements[index];

                if (element.name != "string")
                {
                    // 注释
                    string comment = element.text.Substring(5, element.text.Length - 9);
                    sb.AppendLine($"##,{comment},,,,");
                }
                else
                {
                    string key = element.GetAttribute("name");
                    string name = element.GetAttribute("mz");
                    string text = EncodeString(element.text);

                    sb.Append($",{key},{name},{text}");

                    if (oldI18NItemDict == null || !oldI18NItemDict.TryGetValue(key, out I18NItem i18NItem))
                    {
                        // 之前没有的全新的字段
                        for (int i = 1; i < LanguageConfigs.Length; i++)
                        {
                            sb.Append(",");
                        }
                    }
                    else
                    {
                        for (int i = 1; i < LanguageConfigs.Length; i++)
                        {
                            LanguageConfig languageConfig = LanguageConfigs[i];
                            if (i18NItem.TextDict.TryGetValue(languageConfig.name, out string value))
                            {
                                // 保留之前的内容
                                sb.Append($",{EncodeString(value)}");
                            }
                            else
                            {
                                // 新增的语言
                                sb.Append(",");
                            }
                        }
                    }
                    sb.AppendLine();
                }
            }
            
            using FileStream fs = new FileStream(CSVPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(sb.ToString());
        }
   
        private static readonly string[] ESCAPES = new string[]
        {
            "\"", "\"\"",
            "\n", "\\n",
            "\t", "\\t",
            "\r", "\\r",
        };
        
        private static string EncodeString(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < ESCAPES.Length; i += 2)
            {
                sb.Replace(ESCAPES[i], ESCAPES[i + 1]);
            }

            string value = sb.ToString();
            if (value.Contains(",") || value.Contains("\""))
            {
                return $"\"{value}\"";
            }
            
            return sb.ToString();
        }
    }
}