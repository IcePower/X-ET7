using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [ObjectSystem]
    public class FUIEntityAwakeSystem : AwakeSystem<FUIEntity>
    {
        protected override void Awake(FUIEntity self)
        {
            self.PanelCoreData = self.AddChild<PanelCoreData>();
            self.Language = LocalizeComponentSystem.DefaultLanguage;
        }
    }
    
    [ObjectSystem]
    public class FUIEntityDestroySystem : DestroySystem<FUIEntity>
    {
        protected override void Destroy(FUIEntity self)
        {
            self.PanelCoreData?.Dispose();
            self.PanelId = PanelId.Invalid;
            if (self.GComponent != null)
            {
                self.GComponent.Dispose();
                self.GComponent = null;
            }

            self.Language = LocalizeComponentSystem.DefaultLanguage;
        }
    }

    public static class FUIEntitySystem
    {
        public static void SetRoot(this FUIEntity self, GComponent rootGComponent)
        {
            if(self.GComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} GComponent is null!!!");
                return;
            }
            if(rootGComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} rootGComponent is null!!!");
                return;
            }
            rootGComponent.AddChild(self.GComponent);
        }
    }
}