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
        private const string NameSpace = "ET.Client";
        
        // 类名前缀
        private const string ClassNamePrefix = "FUI_";
        
        // 代码生成路径
        private const string FUIAutoGenDir = "../Unity/Assets/Scripts/Codes/ModelView/Client/Demo/FUIAutoGen";
        private const string ModelViewCodeDir = "../Unity/Assets/Scripts/Codes/ModelView/Client/Demo/FUI";
        private const string HotfixViewCodeDir = "../Unity/Assets/Scripts/Codes/HotfixView/Client/Demo/FUI";

        // 不生成使用默认名称的成员
        private static readonly bool IgnoreDefaultVariableName = true;
        
        private static readonly Dictionary<string, PackageInfo> PackageInfos = new Dictionary<string, PackageInfo>();

        private static readonly Dictionary<string, ComponentInfo> ComponentInfos = new Dictionary<string, ComponentInfo>();
        
        private static readonly MultiDictionary<string, string, ComponentInfo> ExportedComponentInfos = new MultiDictionary<string, string, ComponentInfo>();

        private static readonly HashSet<string> ExtralExportURLs = new HashSet<string>();
        
        private static readonly Dictionary<ObjectType, string> ObjectTypeToClassType = new Dictionary<ObjectType, string>()
        {
            {ObjectType.graph, "GGraph"},
            {ObjectType.group, "GGroup"},
            {ObjectType.image, "GImage"},
            {ObjectType.loader, "GLoader"},
            {ObjectType.loader3D, "GLoader3D"},
            {ObjectType.movieclip, "GMovieClip"},
            {ObjectType.textfield, "GTextField"},
            {ObjectType.textinput, "GTextInput"},
            {ObjectType.richtext, "GRichTextField"},
            {ObjectType.list, "GList"}
        };
        
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
                ComponentInfos.Add(componentInfo.Id, componentInfo);
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

            // 下面获取组件的真实类型
            bool hasNoAppointName = false; // 有不是约定的名字
            bool hasCustomName = false; // 有自定义的名字
            foreach (XML displayXML in componentInfo.DisplayList)
            {
                string variableName = displayXML.GetAttribute("name");

                bool isAppointName = IsAppointName(variableName, componentInfo.ComponentType);
                if (isAppointName)
                {
                    continue;
                }

                hasNoAppointName = true;

                bool isDefaultName = displayXML.GetAttribute("id").StartsWith(variableName);
                if (!isDefaultName)
                {
                    hasCustomName = true;
                }
            }

            if (hasCustomName || (hasNoAppointName && !IgnoreDefaultVariableName))
            {
                componentInfo.ComponentTypeName = "{0}{1}".Fmt(ClassNamePrefix, componentInfo.NameWithoutExtension);
            }
            else
            {
                componentInfo.ComponentTypeName = ComponentTypeToClassType[componentInfo.ComponentType];
            }

            return componentInfo;
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
                if (packageInfo.PackageComponentInfos.ContainsKey(panelName))
                {
                    SpawnCodeForPanel(packageInfo.Name);
                    SpawnCodeForPanelSystem(packageInfo.Name);
                    SpawnEventHandler(packageInfo.Name);
                }
            }
        }

        private static void SpawnCodeForPanelId()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/\n");
            sb.AppendFormat("namespace {0}\n", NameSpace);
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
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/\n");
            sb.AppendFormat("namespace {0}\n", NameSpace);
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

        private static void SpawnCodeForPanel(string packageName)
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
            sb.AppendFormat("namespace {0}\n", NameSpace);
            sb.AppendLine("{");
            sb.AppendLine("\t[ComponentOf(typeof(FUIEntity))]");
            sb.AppendFormat("\tpublic class {0}: Entity, IAwake\n", panelName);
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
            sb.AppendFormat("namespace {0}\n", NameSpace);
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic static class {0}System\n", panelName);
            sb.AppendLine("\t{");
            
            sb.AppendFormat("\t\tpublic static void RegisterUIEvent(this {0} self)\n", panelName);
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void OnShow(this {0} self)\n", panelName);
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void OnHide(this {0} self)\n", panelName);
            sb.AppendLine("\t\t{");
            sb.AppendLine();
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic static void BeforeUnload(this {0} self)\n", panelName);
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
            sb.AppendFormat("namespace {0}\n", NameSpace);
            sb.AppendLine("{");
            sb.AppendLine("\t[FriendOf(typeof(PanelCoreData))]");
            sb.AppendLine("\t[FriendOf(typeof(FUIEntity))]");
            sb.AppendFormat("\t[FUIEvent(PanelId.{0}, \"{1}\", \"{0}\")]\n", panelName, packageName);
            sb.AppendFormat("\tpublic class {0}EventHandler: IFUIEventHandler\n", packageName);
            sb.AppendLine("\t{");
            
            sb.AppendFormat("\t\tpublic void OnInitPanelCoreData(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tfuiEntity.PanelCoreData.panelType = UIPanelType.Normal;");
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnInitComponent(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.AddComponent<{0}>();\n", panelName);
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnRegisterUIEvent(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().RegisterUIEvent();\n", panelName);
            sb.AppendLine("\t\t}");
            
            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnShow(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().OnShow();\n", panelName);
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void OnHide(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().OnHide();\n", panelName);
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendFormat("\t\tpublic void BeforeUnload(FUIEntity fuiEntity)\n");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().BeforeUnload();\n", panelName);
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
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/\n");
            
            sb.AppendLine("using FairyGUI;\n");
            sb.AppendFormat("namespace {0}\n", NameSpace);
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic class {0}Binder\n", packageInfo.Name);
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic static void BindAll()");
            sb.AppendLine("\t\t{");

            foreach (ComponentInfo componentInfo in componentInfos.Values)
            {
                if (!componentInfo.Exported && !ExtralExportURLs.Contains(componentInfo.Url))
                {
                    continue;
                }
                
                sb.AppendFormat("\t\t\tUIObjectFactory.SetPackageItemExtension({0}{1}.URL, typeof({0}{1}));\n", ClassNamePrefix, componentInfo.NameWithoutExtension);
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

        private class VariableInfo
        {
            public string TypeName;
            public string VariableName;
            public bool IsParentName;
            public bool IsDefaultName;
        }
        private static readonly List<VariableInfo> VariableInfos = new List<VariableInfo>();
        private static readonly List<string> ControllerNames = new List<string>();
        private static readonly Dictionary<string, List<string>> ControllerPageNames = new Dictionary<string, List<string>>();
        private static void SpawnCodeForComponent(ComponentInfo componentInfo)
        {
            if (!componentInfo.Exported && !ExtralExportURLs.Contains(componentInfo.Url))
            {
                return;
            }

            GatherVariable(componentInfo);

            if (VariableInfos.Count == 0)
            {
                return;
            }
                
            GatherController(componentInfo);

            ExportedComponentInfos.Add(componentInfo.PackageId, componentInfo.Id, componentInfo);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/\n");
            sb.AppendLine("using FairyGUI;");
            sb.AppendLine("using FairyGUI.Utils;\n");
            sb.AppendFormat("namespace {0}\n", NameSpace);
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic partial class {0}{1}: GComponent\n", ClassNamePrefix, componentInfo.NameWithoutExtension);
            sb.AppendLine("\t{");

            foreach (var kv in ControllerPageNames)
            {
                sb.AppendFormat("\t\tpublic enum {0}_Page\n", kv.Key);
                sb.AppendLine("\t\t{");
                foreach (string pageName in kv.Value)
                {
                    sb.AppendFormat("\t\t\tPage_{0},\n", pageName);
                }
                sb.AppendLine("\t\t}\n");
            }
            
            for (int i = 0; i < ControllerNames.Count; i++)
            {
                sb.AppendFormat("\t\tpublic Controller {0};\n", ControllerNames[i]);
            }
            
            for (int i = 0; i < VariableInfos.Count; i++)
            {
                if (IgnoreDefaultVariableName && VariableInfos[i].IsDefaultName)
                {
                    continue;
                }
                
                string typeName = VariableInfos[i].TypeName;
                string variableName = VariableInfos[i].VariableName;
                sb.AppendFormat("\t\tpublic {0} {1};\n", typeName, variableName);
            }

            sb.AppendFormat("\t\tpublic const string URL = \"{0}\";\n\n", componentInfo.Url);

            sb.AppendFormat("\t\tpublic static {0}{1} CreateInstance()\n", ClassNamePrefix, componentInfo.NameWithoutExtension);
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\treturn ({0}{1})UIPackage.CreateObject(\"{2}\", \"{1}\");\n", ClassNamePrefix, componentInfo.NameWithoutExtension, PackageInfos[componentInfo.PackageId].Name);
            sb.AppendLine("\t\t}\n");
            
            sb.AppendLine("\t\tpublic override void ConstructFromXML(XML xml)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tbase.ConstructFromXML(xml);\n");

            for (int i = 0; i < ControllerNames.Count; i++)
            {
                sb.AppendFormat("\t\t\t{0} = GetControllerAt({1});\n", ControllerNames[i], i);
            }
            
            for (int i = 0; i < VariableInfos.Count; i++)
            {
                if (IgnoreDefaultVariableName && VariableInfos[i].IsDefaultName)
                {
                    continue;
                }
                sb.AppendFormat("\t\t\t{0} = ({1})GetChildAt({2});\n", VariableInfos[i].VariableName, VariableInfos[i].TypeName, i);
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

        private static void GatherVariable(ComponentInfo componentInfo)
        {
            VariableInfos.Clear();

            foreach (XML displayXML in componentInfo.DisplayList)
            {
                string variableName = displayXML.GetAttribute("name");

                if (IsAppointName(variableName, componentInfo.ComponentType))
                {
                    continue;
                }

                string typeName = GetTypeNameByDisplayXML(displayXML);
                if (string.IsNullOrEmpty(typeName))
                {
                    continue;
                }

                bool isDefaultName = displayXML.GetAttribute("id").StartsWith(variableName);

                VariableInfos.Add(new VariableInfo()
                {
                    TypeName = typeName,
                    VariableName = variableName,
                    IsDefaultName = isDefaultName
                });
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
        
        private static bool IsAppointName(string variableName, ComponentType componentType)
        {
            if (variableName == "icon" || variableName == "text")
            {
                return true;
            }

            switch (componentType)
            {
                case ComponentType.Component:
                    break;
                case ComponentType.Button:
                case ComponentType.ComboBox:
                case ComponentType.Label:
                    if (variableName == "title")
                    {
                        return true;
                    }
                    break;
                case ComponentType.ProgressBar:
                    if (variableName == "bar" || variableName == "bar_v" || variableName == "ani")
                    {
                        return true;
                    }
                    break;
                case ComponentType.ScrollBar:
                    if (variableName == "arrow1" || variableName == "arrow2" || variableName == "grip" || variableName == "bar")
                    {
                        return true;
                    }
                    break;
                case ComponentType.Slider:
                    if (variableName == "bar" || variableName == "bar_v" || variableName == "grip" || variableName == "ani")
                    {
                        return true;
                    }
                    break;
                default:
                    throw new Exception("没有处理这种类型: {0}".Fmt(componentType));
            }

            return false;
        }
        
        private static string GetTypeNameByDisplayXML(XML displayXML)
        {
            string typeName = string.Empty;

            if (displayXML.name == "component")
            {
                ComponentInfo displayComponentInfo = ComponentInfos[displayXML.GetAttribute("src")];
                if (displayComponentInfo == null)
                {
                    throw new Exception("没找到对应类型：{0}".Fmt(displayXML.GetAttribute("src")));
                }

                typeName = displayComponentInfo.ComponentTypeName;
            }
            else if (displayXML.name == "text")
            {
                ObjectType objectType = displayXML.GetAttribute("input") == "true" ? ObjectType.textinput : ObjectType.textfield;
                typeName = ObjectTypeToClassType[objectType];
            }
            else if (displayXML.name == "group") 
            {
                if (displayXML.GetAttribute("advanced") != "true")
                {
                    return typeName;
                }

                ObjectType objectType = EnumHelper.FromString<ObjectType>(displayXML.name);
                typeName = ObjectTypeToClassType[objectType];
            }
            else
            {
                ObjectType objectType = EnumHelper.FromString<ObjectType>(displayXML.name);

                try
                {
                    typeName = ObjectTypeToClassType[objectType];
                }
                catch (Exception e)
                {
                    Debug.LogError($"{objectType}没找到！");
                    Debug.LogError(e);
                }
            }

            return typeName;
        }
    }
}











