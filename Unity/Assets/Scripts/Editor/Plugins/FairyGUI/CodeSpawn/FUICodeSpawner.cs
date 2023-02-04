using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ET;
using FairyGUI.Utils;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using FileMode = System.IO.FileMode;

namespace FUIEditor
{
    public static class FUICodeSpawner
    {
        // 名字空间
        public static string NameSpace = "ET.Client";
        
        // 类名前缀
        public static string ClassNamePrefix = "FUI_";
        
        // 代码生成路径
        private const string FUIAutoGenDir = "../Unity/Assets/Scripts/Codes/ModelView/Client/Demo/FUIAutoGen";
        private const string ModelViewCodeDir = "../Unity/Assets/Scripts/Codes/ModelView/Client/Demo/FUI";
        private const string HotfixViewCodeDir = "../Unity/Assets/Scripts/Codes/HotfixView/Client/Demo/FUI";

        // 不生成使用默认名称的成员
        private static readonly bool IgnoreDefaultVariableName = true;
        
        public static readonly Dictionary<string, PackageInfo> PackageInfos = new Dictionary<string, PackageInfo>();

        public static readonly Dictionary<string, ComponentInfo> ComponentInfos = new Dictionary<string, ComponentInfo>();
        
        private static readonly MultiDictionary<string, string, ComponentInfo> ExportedComponentInfos = new MultiDictionary<string, string, ComponentInfo>();

        private static readonly HashSet<string> ExtralExportURLs = new HashSet<string>();

        private static readonly Dictionary<ComponentType, string> ComponentTypeToClassType = new Dictionary<ComponentType, string>()
        {
            {ComponentType.Component, "GComponent"},
            {ComponentType.Button, "GButton"},
            {ComponentType.ComboBox, "GComboBox"},
            {ComponentType.Label, "GLabel"},
            {ComponentType.ProgressBar, "GProgressBar"},
            {ComponentType.ScrollBar, "GScrollBar"},
            {ComponentType.Slider, "GSlider"},
            {ComponentType.Tree, "GTree"}
        };

        public static void FUICodeSpawn()
        {
            ParseAndSpawnCode();

            AssetDatabase.Refresh();
        }

        public static void ParseAndSpawnCode()
        {
            ParseAllPackages();
            AfterParseAllPackages();
            SpawnCode();
        }

        private static void ParseAllPackages()
        {
            PackageInfos.Clear();
            ComponentInfos.Clear();
            ExportedComponentInfos.Clear();
            ExtralExportURLs.Clear();

            string fuiAssetsDir = Application.dataPath + "/../../FGUIProject/assets";
            string[] packageDirs = Directory.GetDirectories(fuiAssetsDir);
            foreach (var packageDir in packageDirs)
            {
                PackageInfo packageInfo = ParsePackage(packageDir);
                PackageInfos.Add(packageInfo.Id, packageInfo);
            }
        }

        private static PackageInfo ParsePackage(string packageDir)
        {
            PackageInfo packageInfo = new PackageInfo();

            packageInfo.Path = packageDir;
            packageInfo.Name = Path.GetFileName(packageDir);
                
            XML xml = new XML(File.ReadAllText(packageDir + "/package.xml"));
            packageInfo.Id = xml.GetAttribute("id");

            if (xml.elements[0].name != "resources" || xml.elements[1].name != "publish")
            {
                throw new Exception("package.xml 格式不对！");
            }
            
            foreach (XML element in xml.elements[0].elements)
            {
                if (element.name != "component")
                {
                    continue;
                }
                
                PackageComponentInfo packageComponentInfo = new PackageComponentInfo();
                packageComponentInfo.Id = element.GetAttribute("id");
                packageComponentInfo.Name = element.GetAttribute("name");
                packageComponentInfo.Path = "{0}{1}{2}".Fmt(packageDir, element.GetAttribute("path"), packageComponentInfo.Name);
                packageComponentInfo.Exported = element.GetAttribute("exported") == "true";
                
                packageInfo.PackageComponentInfos.Add(packageComponentInfo.Name, packageComponentInfo);

                ComponentInfo componentInfo = ParseComponent(packageInfo, packageComponentInfo);
                string key = "{0}/{1}".Fmt(componentInfo.PackageId, componentInfo.Id);
                ComponentInfos.Add(key, componentInfo);
            }

            return packageInfo;
        }

        private static ComponentInfo ParseComponent(PackageInfo packageInfo, PackageComponentInfo packageComponentInfo)
        {
            ComponentInfo componentInfo = new ComponentInfo();
            componentInfo.PackageId = packageInfo.Id;
            componentInfo.Id = packageComponentInfo.Id;
            componentInfo.Name = packageComponentInfo.Name;
            componentInfo.NameWithoutExtension = Path.GetFileNameWithoutExtension(packageComponentInfo.Name);
            componentInfo.Url = "ui://{0}{1}".Fmt(packageInfo.Id, packageComponentInfo.Id);
            componentInfo.Exported = packageComponentInfo.Exported;
            componentInfo.ComponentType = ComponentType.Component;

            XML xml = new XML(File.ReadAllText(packageComponentInfo.Path));
            foreach (XML element in xml.elements)
            {
                if (element.name == "displayList")
                {
                    componentInfo.DisplayList = element.elements;
                }
                else if (element.name == "controller")
                {
                    componentInfo.ControllerList.Add(element);
                }
                else if (element.name == "relation")
                { 
                    
                }
                else
                {
                    ComponentType type = EnumHelper.FromString<ComponentType>(element.name);
                    if (type == ComponentType.None)
                    {
                        Debug.LogError("{0}类型没有处理！".Fmt(element.name));
                        continue;
                    } 
                    
                    if (type == ComponentType.ComboBox)
                    {
                        ExtralExportURLs.Add(element.GetAttribute("dropdown"));
                    }
                    
                    componentInfo.ComponentType = type;
                }
            }

            return componentInfo;
        }
        
        // 检查哪些组件可以导出。需要在 ParseAllPackages 后执行，因为需要有全部 package 的信息。
        private static void AfterParseAllPackages()
        {
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.CheckCanExport(ExtralExportURLs, IgnoreDefaultVariableName);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                componentInfo.SetVariableInfoTypeName();
            }
        }
        
        private static void SpawnCode()
        {
            if (Directory.Exists(FUIAutoGenDir))
            {
                Directory.Delete(FUIAutoGenDir, true);
            }
            
            foreach (ComponentInfo componentInfo in ComponentInfos.Values)
            {
                SpawnCodeForComponent(componentInfo);
            }
            
            foreach (var kv in ExportedComponentInfos)
            {
                SpawnCodeForBinder(PackageInfos[kv.Key], kv.Value);
            }

            SpawnCodeForInit();
            
            SpawnCodeForPanelId();

            foreach (PackageInfo packageInfo in PackageInfos.Values)
            {
                string panelName = "{0}Panel.xml".Fmt(packageInfo.Name);
                if (packageInfo.PackageComponentInfos.TryGetValue(panelName, out var packageComponentInfo))
                {
                    string componentId = $"{packageInfo.Id}/{packageComponentInfo.Id}";
                    if (ComponentInfos.TryGetValue(componentId, out var componentInfo))
                    {
                        SpawnCodeForPanel(packageInfo.Name, componentInfo.NameSpace);
                        SpawnCodeForPanelSystem(packageInfo.Name);
                        SpawnEventHandler(packageInfo.Name);
                    }
                }
            }
        }

        private static void SpawnCodeForPanelId()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum PanelId");
            sb.AppendLine("\t{");

            sb.AppendLine("\t\tInvalid = 0,");
            
            foreach (PackageInfo packageInfo in PackageInfos.Values)
            {
                string panelName = "{0}Panel.xml".Fmt(packageInfo.Name);
                if (packageInfo.PackageComponentInfos.ContainsKey(panelName))
                {
                    sb.AppendLine("\t\t{0}Panel,".Fmt(packageInfo.Name));
                }
            }
            
            sb.AppendLine("\t}"); 
            sb.AppendLine("}");
            
            string filePath = "{0}/PanelId.cs".Fmt(ModelViewCodeDir);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
        
        private static void SpawnCodeForInit()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static class FUIPackageLoader");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic static async ETTask LoadPackagesAsync(FUIComponent fuiComponent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tusing (ListComponent<ETTask> tasks = ListComponent<ETTask>.Create())");
            sb.AppendLine("\t\t\t{");
            foreach (var kv in ExportedComponentInfos)
            {
                sb.AppendLine("\t\t\t\ttasks.Add(fuiComponent.AddPackageAsync(\"{0}\"));".Fmt(PackageInfos[kv.Key].Name));
            }
            sb.AppendLine();
            sb.AppendLine("\t\t\t\tawait ETTaskHelper.WaitAll(tasks);");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine();

            foreach (var kv in ExportedComponentInfos)
            {
                sb.AppendLine("\t\t\t{0}Binder.BindAll();".Fmt(PackageInfos[kv.Key].Name));
            }
            
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            string filePath = "{0}/FUIPackageLoader.cs".Fmt(HotfixViewCodeDir);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        private static void SpawnCodeForPanel(string packageName, string nameSpace)
        {
            string panelName = "{0}Panel".Fmt(packageName);
            
            string fileDir = "{0}/{1}".Fmt(ModelViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For Panel {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"using {nameSpace};");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\t[ComponentOf(typeof(FUIEntity))]");
            sb.AppendFormat("\tpublic class {0}: Entity, IAwake", panelName);
            sb.AppendLine();
            sb.AppendLine("\t{");
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

        private static void SpawnCodeForPanelSystem(string packageName)
        {
            string panelName = "{0}Panel".Fmt(packageName);

            string fileDir = "{0}/{1}".Fmt(HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            string filePath = "{0}/{1}System.cs".Fmt(fileDir, panelName);
            if (File.Exists(filePath))
            {
                Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn Code For PanelSystem {0}".Fmt(filePath));
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("namespace {0}", NameSpace);
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

        private static void SpawnEventHandler(string packageName)
        {
            string panelName = "{0}Panel".Fmt(packageName);
            
            string fileDir = "{0}/{1}/Event".Fmt(HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}EventHandler.cs".Fmt(fileDir, packageName);
            if (File.Exists(filePath))
            {
                Debug.Log("{0} 已经存在".Fmt(filePath));
                return;
            }
            
            Debug.Log("Spawn EventHandler {0}".Fmt(filePath));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("namespace {0}", NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\t[FriendOf(typeof(PanelCoreData))]");
            sb.AppendLine("\t[FriendOf(typeof(FUIEntity))]");
            sb.AppendFormat("\t[FUIEvent(PanelId.{0}, \"{1}\", \"{0}\")]", panelName, packageName);
            sb.AppendLine();
            sb.AppendFormat("\tpublic class {0}EventHandler: IFUIEventHandler", packageName);
            sb.AppendLine();
            sb.AppendLine("\t{");
            
            sb.AppendFormat("\t\tpublic void OnInitPanelCoreData(FUIEntity fuiEntity)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tfuiEntity.PanelCoreData.panelType = UIPanelType.Normal;");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnInitComponent(FUIEntity fuiEntity)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\t{0} panel = fuiEntity.AddComponent<{0}>();", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t\tpanel.Awake();");
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnRegisterUIEvent(FUIEntity fuiEntity)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().RegisterUIEvent();", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnShow(FUIEntity fuiEntity, Entity contexData = null)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().OnShow(contexData);", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnHide(FUIEntity fuiEntity)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().OnHide();", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void BeforeUnload(FUIEntity fuiEntity)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().BeforeUnload();", panelName);
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine("\t}");
            sb.Append("}");
            
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
        
        private static void SpawnCodeForBinder(PackageInfo packageInfo, Dictionary<string, ComponentInfo> componentInfos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", NameSpace);
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
            
            string dir = "{0}/{1}".Fmt(FUIAutoGenDir, packageInfo.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}Binder.cs".Fmt(dir, packageInfo.Name);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }

        private static readonly List<string> ControllerNames = new List<string>();
        private static readonly Dictionary<string, List<string>> ControllerPageNames = new Dictionary<string, List<string>>();
        private static void SpawnCodeForComponent(ComponentInfo componentInfo)
        {
            if (!componentInfo.NeedExportClass)
            {
                return;
            }
                
            GatherController(componentInfo);

            ExportedComponentInfos.Add(componentInfo.PackageId, componentInfo.Id, componentInfo);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine("using FairyGUI.Utils;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", componentInfo.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic partial class {0}{1}: {2}", ClassNamePrefix, componentInfo.NameWithoutExtension, componentInfo.ComponentClassName);
            sb.AppendLine();
            sb.AppendLine("\t{");

            foreach (var kv in ControllerPageNames)
            {
                sb.AppendFormat("\t\tpublic enum {0}_Page", kv.Key);
                sb.AppendLine();
                sb.AppendLine("\t\t{");
                foreach (string pageName in kv.Value)
                {
                    sb.AppendFormat("\t\t\tPage_{0},", pageName);
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
                if (IgnoreDefaultVariableName && variableInfos[i].IsDefaultName)
                {
                    continue;
                }
                
                string typeName = variableInfos[i].TypeName;
                if(string.IsNullOrEmpty(typeName))
                {
                    continue;
                }
                
                string variableName = variableInfos[i].VariableName;
                sb.AppendFormat("\t\tpublic {0} {1};", typeName, variableName);
                sb.AppendLine();
            }

            sb.AppendFormat("\t\tpublic const string URL = \"{0}\";", componentInfo.Url);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("\t\tpublic static {0}{1} CreateInstance()", ClassNamePrefix, componentInfo.NameWithoutExtension);
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\treturn ({0}{1})UIPackage.CreateObject(\"{2}\", \"{1}\");", ClassNamePrefix, componentInfo.NameWithoutExtension, PackageInfos[componentInfo.PackageId].Name);
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
                if (IgnoreDefaultVariableName && variableInfos[i].IsDefaultName)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(variableInfos[i].TypeName))
                {
                    continue;
                }
                
                sb.AppendFormat("\t\t\t{0} = ({1})GetChildAt({2});", variableInfos[i].VariableName, variableInfos[i].TypeName, i);
                sb.AppendLine();
            }
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            string dir = "{0}/{1}".Fmt(FUIAutoGenDir, PackageInfos[componentInfo.PackageId].Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            string filePath = "{0}/{1}{2}.cs".Fmt(dir, ClassNamePrefix, componentInfo.NameWithoutExtension);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
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











