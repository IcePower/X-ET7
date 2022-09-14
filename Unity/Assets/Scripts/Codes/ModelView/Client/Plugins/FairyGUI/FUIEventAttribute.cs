using System;

namespace ET.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FUIEventAttribute: BaseAttribute
    {
        public PanelId PanelId
        {
            get;
        }

        public PanelInfo PanelInfo
        {
            get;
        }

        public FUIEventAttribute(PanelId panelId, string packageName, string componentName)
        {
            this.PanelId = panelId;
            this.PanelInfo = new PanelInfo() { PanelId = panelId, PackageName = packageName, ComponentName = componentName };
        }
    }
}