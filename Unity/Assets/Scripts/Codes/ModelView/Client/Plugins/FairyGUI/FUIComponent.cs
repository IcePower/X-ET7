using System.Collections.Generic;
using FairyGUI.Dynamic;

namespace ET.Client
{
    public interface IFUILogic
    {
        
    }
    
    [ComponentOf(typeof(Scene))]
    public class FUIComponent : Entity,IAwake,IDestroy
    {
        // public HashSet<PanelId> LoadingPanels = new HashSet<PanelId>();
        
        public List<PanelId> VisiblePanelsQueue = new List<PanelId>(10);
        
        public Dictionary<int, FUIEntity> AllPanelsDic = new Dictionary<int, FUIEntity>(10);
        
        public List<PanelId> FUIEntitylistCached = new List<PanelId>(10);
        
        public Dictionary<int, FUIEntity> VisiblePanelsDic = new Dictionary<int, FUIEntity>(10);
        
        public Stack<PanelId> HidePanelsStack = new Stack<PanelId>(10);
    }
}