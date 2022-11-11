using System;
using System.Collections.Generic;
using ET;
using FairyGUI.Utils;
using UnityEngine;

namespace FUIEditor
{
    public class VariableInfo
    {
        public string TypeName;
        
        public string VariableName;
        
        public bool IsDefaultName;
        
        public XML displayXML;
    }
    
    public class ComponentInfo
    {
        public string NameSpace = "";
        
        public string PackageId;

        public string Id;

        public string Name;
        
        public string ComponentTypeName;

        public string NameWithoutExtension;
        
        public ComponentType ComponentType;

        public string ComponentClassName;

        public string Url;
        
        // 编辑器里设置为导出
        public bool Exported;

        public XMLList ControllerList = new XMLList();
        
        public XMLList DisplayList;

        // 最终是否需要导出类
        public bool NeedExportClass;
        
        public List<VariableInfo> VariableInfos = new List<VariableInfo>();
        
        public bool HasCustomVariableName;
        
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

            if (VariableInfos.Count == 0)
            {
                needExportClass = false;
            }

            if (!HasCustomVariableName && IgnoreDefaultVariableName)
            {
                needExportClass = false;
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
            foreach (var variableInfo in VariableInfos)
            {
                variableInfo.TypeName = GetTypeNameByDisplayXML(PackageId, variableInfo.displayXML);
            }
        }
        
        public void GatherVariable()
        {
            foreach (XML displayXML in DisplayList)
            {
                string variableName = displayXML.GetAttribute("name");

                if (IsAppointName(variableName, ComponentType))
                {
                    continue;
                }

                bool isDefaultName = displayXML.GetAttribute("id").StartsWith(variableName);

                VariableInfos.Add(new VariableInfo()
                {
                    VariableName = variableName,
                    IsDefaultName = isDefaultName,
                    displayXML = displayXML
                });

                if (!isDefaultName)
                {
                    HasCustomVariableName = true;
                }
            }
        }
        
        private bool IsAppointName(string variableName, ComponentType componentType)
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
                    typeName = "{0}".Fmt(displayComponentInfo.ComponentTypeName);
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