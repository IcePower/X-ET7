using System.IO;
using System.Text;

namespace FUIEditor
{
    public static class FUIBinderSpawner
    {
        public static void SpawnCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
    
            sb.AppendLine("");
            sb.AppendLine($"namespace {FUICodeSpawner.NameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static class FUIBinder");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic static void BindAll()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tUIObjectFactory.Clear();");
            sb.AppendLine("\t\t\t");
    
            foreach (PackageInfo packageInfo in FUICodeSpawner.PackageInfos.Values)
            {
                sb.AppendLine($"\t\t\t{packageInfo.Name}Binder.BindAll();");
            }
    
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
    
            string filePath = $"../Unity/Assets/Scripts/Codes/ModelView/Client/Demo/FUIAutoGen/FUIBinder.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}