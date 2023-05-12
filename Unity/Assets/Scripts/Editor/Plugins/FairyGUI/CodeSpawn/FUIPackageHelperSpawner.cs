using System.IO;
using System.Text;

namespace FUIEditor
{
    public static class FUIPackageHelperSpawner
    {
        public static void SpawnUIPackageHelper()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace FairyGUI.Dynamic");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic class UIPackageHelper : IUIPackageHelper");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tprivate static readonly Dictionary<string, string> IdToNameDict = new Dictionary<string, string>()");
            sb.AppendLine("\t\t{");
            foreach (PackageInfo packageInfo in FUICodeSpawner.PackageInfos.Values)
            {
                sb.AppendLine($"\t\t\t[\"{packageInfo.Id}\"] = \"{packageInfo.Name}\",");
            }
            sb.AppendLine("\t\t};");
            sb.AppendLine();
            sb.AppendLine("\t\tpublic string GetPackageNameById(string id)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif (IdToNameDict.TryGetValue(id, out var name))");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\treturn name;");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine();
            sb.AppendLine("\t\t\treturn null;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            string filePath = $"../Unity/Assets/Scripts/ThirdParty/FairyGUI/Dynamic/UIPackageHelper.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}