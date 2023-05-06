using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;

namespace FUIEditor
{
    public static class FUIBinderSpawner
    {
        public static void SpawnFUIBinder(List<PackageInfo> exportedPackageInfos)
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
    
            foreach (PackageInfo packageInfo in exportedPackageInfos)
            {
                sb.AppendLine($"\t\t\t{packageInfo.Name}Binder.BindAll();");
            }
    
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
    
            string filePath = $"{FUICodeSpawner.FUIAutoGenDir}/FUIBinder.cs";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
        
        public static void SpawnCodeForPanelBinder(PackageInfo packageInfo, Dictionary<string, ComponentInfo> componentInfos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic class {0}Binder", packageInfo.Name);
            sb.AppendLine();
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic static void BindAll()");
            sb.AppendLine("\t\t{");

            foreach (ComponentInfo componentInfo in componentInfos.Values)
            {
                sb.AppendFormat("\t\t\tUIObjectFactory.SetPackageItemExtension({0}.{1}.URL, typeof({0}.{1}));", componentInfo.NameSpace, componentInfo.ComponentTypeName);
                sb.AppendLine();
            }
            
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            string dir = "{0}/{1}".Fmt(FUICodeSpawner.FUIAutoGenDir, packageInfo.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}Binder.cs".Fmt(dir, packageInfo.Name);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}