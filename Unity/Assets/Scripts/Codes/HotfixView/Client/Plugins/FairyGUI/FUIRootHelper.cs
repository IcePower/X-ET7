using System;
using ET.Client;
using FairyGUI;

namespace ET.Client
{
    [FriendOf(typeof(GlobalComponent))]
    public static class FUIRootHelper
    {
        public static void Init()
        {
          
        }
        
        public static GComponent GetTargetRoot(UIPanelType type)
        {
            if (type == UIPanelType.Normal)
            {
                return GlobalComponent.Instance.NormalGRoot;
            }
            else if (type == UIPanelType.Fixed)
            {
                return GlobalComponent.Instance.FixedGRoot;
            }
            else if (type == UIPanelType.PopUp)
            {
                return GlobalComponent.Instance.PopUpGRoot;
            }
            else if (type == UIPanelType.Other)
            {
                return GlobalComponent.Instance.OtherGRoot;
            }

            Log.Error("uiroot type is error: " + type.ToString());
            return null;
        }
    }
}