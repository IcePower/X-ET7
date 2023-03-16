using System;

namespace ET.Client
{
    [ObjectSystem]
    public class FUIEventComponentLoadSystem : LoadSystem<FUIEventComponent>
    {
        protected override void Load(FUIEventComponent self)
        {
            FUIEventComponent.Instance = self;
            self.Load();
        }
    }
    
    [ObjectSystem]
    public class FUIEventComponentAwakeSystem : AwakeSystem<FUIEventComponent>
    {
        protected override void Awake(FUIEventComponent self)
        {
            FUIEventComponent.Instance = self;
            self.Load();
        }
    }
    
    [ObjectSystem]
    public class FUIEventComponentDestroySystem : DestroySystem<FUIEventComponent>
    {
        protected override void Destroy(FUIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.PanelIdInfoDict.Clear();
            self.PanelTypeInfoDict.Clear();
            self.isClicked = false;
            FUIEventComponent.Instance = null;
        }
    }
    
    [FriendOf(typeof(FUIEventComponent))]
    public static class FUIEventComponentSystem
    {
        public static void Load(this FUIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.PanelIdInfoDict.Clear();
            self.PanelTypeInfoDict.Clear();
            
            foreach (Type v in EventSystem.Instance.GetTypes(typeof(FUIEventAttribute)))
            {
                FUIEventAttribute attr = v.GetCustomAttributes(typeof(FUIEventAttribute), false)[0] as FUIEventAttribute;
                self.UIEventHandlers.Add(attr.PanelId, Activator.CreateInstance(v) as IFUIEventHandler);
                self.PanelIdInfoDict.Add(attr.PanelId, attr.PanelInfo);
                self.PanelTypeInfoDict.Add(attr.PanelId.ToString(), attr.PanelInfo);
            }
        }
        
        public static IFUIEventHandler GetUIEventHandler(this FUIEventComponent self, PanelId panelId)
        {
            if (self.UIEventHandlers.TryGetValue(panelId, out IFUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"panelId : {panelId} is not have any uiEvent");
            return null;
        }

        public static PanelInfo GetPanelInfo(this FUIEventComponent self, PanelId panelId)
        {
            if (self.PanelIdInfoDict.TryGetValue(panelId, out PanelInfo panelInfo))
            {
                return panelInfo;
            }
            Log.Error($"panelId : {panelId} is not have any panelInfo");
            return default;
        }
    }
}