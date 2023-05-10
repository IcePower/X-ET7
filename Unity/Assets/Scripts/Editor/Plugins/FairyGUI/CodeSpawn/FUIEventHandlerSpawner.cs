using System.IO;
using System.Text;
using ET;

namespace FUIEditor
{
    public static class FUIEventHandlerSpawner
    {
        public static void SpawnEventHandler(string packageName, ComponentInfo componentInfo)
        {
            string panelName = componentInfo.NameWithoutExtension;
            
            string fileDir = "{0}/{1}/Event".Fmt(FUICodeSpawner.HotfixViewCodeDir, packageName);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            
            string filePath = "{0}/{1}EventHandler.cs".Fmt(fileDir, packageName);
            // Debug.Log("Spawn EventHandler {0}".Fmt(filePath));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
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
            sb.AppendFormat("\t\tpublic void OnShow(FUIEntity fuiEntity, Entity contextData = null)");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tfuiEntity.GetComponent<{0}>().OnShow(contextData);", panelName);
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

    }
}