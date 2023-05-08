using System;
using System.Collections.Generic;
using ET;
using FairyGUI.Utils;
using UnityEngine;

namespace FUIEditor
{
    public class VariableInfo
    {
        public string TypeName { get; set; }

        public string PackageId { get; set; }

        public ComponentInfo ComponentInfo { get; set; }
        
        public string VariableName { get; set; }
        
        // 是否是默认的名称，比如 n0, n1
        public bool IsDefaultName { get; set; }
        
        // 是否是内部指定的名称，比如 title, icon
        public bool IsAppointName { get; set; }

        public string LanguageKey { get; set; }
        
        public bool IsExported { get; set; }

        public XML displayXML { get; set; }
    }
    
    public enum PanelType
    {
        None,
        Main,
        Common
    }
    
    public class ComponentInfo
    {
        public string NameSpace { get; private set; } = "";

        public string PackageId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
        
        public PanelType PanelType { get; set; }
        
        public string ComponentTypeName { get; private set; }

        public string NameWithoutExtension { get; set; }
        
        public ComponentType ComponentType { get; set; }

        public string ComponentClassName { get; private set; }

        public string Url { get; set; }
        
        // 编辑器里设置为导出
        public bool Exported { get; set; }

        public XMLList ControllerList  { get; set; } = new XMLList();
        
        public XMLList DisplayList { get; set; }

        // 最终是否需要导出类
        public bool NeedExportClass { get; private set; }
        
        public List<VariableInfo> VariableInfos { get; set; } = new List<VariableInfo>();
        
        private bool HasCustomVariableName;
        
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

        public void CheckCanExport(HashSet<string> ExtralExportURLs, bool IgnoreDefaultVariableName)
        {
            bool needExportClass = true;
            
            if (!Exported && !ExtralExportURLs.Contains(Url))
            {
                needExportClass = false;
            }

            GatherVariable();

            if (this.PanelType != PanelType.None)
            {
                needExportClass = true;
            }
            else
            {
                if (VariableInfos.Count == 0)
                {
                    needExportClass = false;
                }

                // 如果没有控制器，且忽略默认变量名，且没有自定义变量名，那么不需要导出类
                if (ControllerList.Count == 0 && IgnoreDefaultVariableName && !HasCustomVariableName)
                {
                    needExportClass = false;
                }
            }

            NeedExportClass = needExportClass;

            ComponentClassName = ComponentTypeToClassType[ComponentType];

            if (needExportClass)
            {
                NameSpace = "{0}.{1}".Fmt(FUICodeSpawner.NameSpace, FUICodeSpawner.PackageInfos[PackageId].Name);
                ComponentTypeName = "{0}{1}".Fmt(FUICodeSpawner.ClassNamePrefix, NameWithoutExtension);
            }
            else
            {
                ComponentTypeName = ComponentClassName;
            }
        }

        public void SetVariableInfoTypeName()
        {
            for (int index = 0; index < this.VariableInfos.Count; index++)
            {
                VariableInfo variableInfo = this.VariableInfos[index];
                variableInfo.TypeName = GetTypeNameByDisplayXML(this.PackageId, variableInfo.displayXML);

                string packageId = variableInfo.displayXML.GetAttribute("pkg");
                if (string.IsNullOrEmpty(packageId))
                {
                    packageId = this.PackageId;
                }
            
                string key = "{0}/{1}".Fmt(packageId, variableInfo.displayXML.GetAttribute("src"));

                if (FUICodeSpawner.ComponentInfos.TryGetValue(key, out ComponentInfo componentInfo))
                {
                    if (componentInfo.PanelType == PanelType.Common)
                    {
                        variableInfo.ComponentInfo = componentInfo;
                    }
                }
            }
        }
        
        private void GatherVariable()
        {
            if (DisplayList == null)
            {
                return;
            }
            
            foreach (XML displayXML in DisplayList)
            {
                string variableName = displayXML.GetAttribute("name");

                bool isAppointName = IsAppointName(variableName, ComponentType);

                bool isDefaultName = displayXML.GetAttribute("id").StartsWith(variableName);

                string packageId = displayXML.GetAttribute("pkg");
                if (string.IsNullOrEmpty(packageId))
                {
                    packageId = PackageId;
                }
                
                VariableInfos.Add(new VariableInfo()
                {
                    VariableName = variableName,
                    IsDefaultName = isDefaultName,
                    IsAppointName = isAppointName,
                    displayXML = displayXML,
                    PackageId = packageId,
                });

                if (!isDefaultName && !isAppointName)
                {
                    HasCustomVariableName = true;
                }
            }
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
                    return false;
                case ComponentType.Button: 
                case ComponentType.ComboBox:
                case ComponentType.Label:
                    if (variableName == "title" || variableName == "dragArea" || variableName == "closeButton")
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
        
        private static string GetTypeNameByDisplayXML(string parentPackageId, XML displayXML)
        {
            string typeName = string.Empty;

            if (displayXML.name == "component")
            {
                string packageId = displayXML.GetAttribute("pkg");
                if (string.IsNullOrEmpty(packageId))
                {
                    packageId = parentPackageId;
                }
                
                string key = "{0}/{1}".Fmt(packageId, displayXML.GetAttribute("src"));
                ComponentInfo displayComponentInfo = FUICodeSpawner.ComponentInfos[key];
                if (displayComponentInfo == null)
                {
                    throw new Exception("没找到对应类型：{0}".Fmt(displayXML.GetAttribute("src")));
                }

                if (string.IsNullOrEmpty(displayComponentInfo.NameSpace))
                {
                    typeName = displayComponentInfo.ComponentTypeName;
                }
                else
                {
                    typeName = "{0}.{1}".Fmt(displayComponentInfo.NameSpace, displayComponentInfo.ComponentTypeName);
                }
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