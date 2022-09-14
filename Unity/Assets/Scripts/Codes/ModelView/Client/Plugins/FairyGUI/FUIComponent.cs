using System.Collections.Generic;

namespace ET.Client
{
    public interface IFUILogic
    {
        
    }
    
    [ComponentOf(typeof(Scene))]
    [ChildOf(typeof(FUIEntity))]
    public class FUIComponent : Entity,IAwake,IDestroy
    {
        // public HashSet<PanelId> LoadingPanels = new HashSet<PanelId>();
        
        public List<PanelId> VisiblePanelsQueue = new List<PanelId>(10);
        
        public Dictionary<int, FUIEntity> AllPanelsDic = new Dictionary<int, FUIEntity>(10);
        
        public List<PanelId> FUIEntitylistCached = new List<PanelId>(10);
        
        public Dictionary<int, FUIEntity> VisiblePanelsDic = new Dictionary<int, FUIEntity>(10);
        
        public Stack<PanelId> HidePanelsStack = new Stack<PanelId>(10);

        // 每个 UIPakcage 对应的 Asset 地址。
        public MultiMap<string, string> UIPackageLocations = new MultiMap<string, string>();
    }
}