using FairyGUI.Utils;

namespace FUIEditor
{
    public class ComponentInfo
    {
        public string PackageId;

        public string Id;

        public string Name;
        
        public string ComponentTypeName;

        public string NameWithoutExtension;
        
        public ComponentType ComponentType;

        public string Url;
        
        public bool Exported;

        public XMLList ControllerList = new XMLList();
        
        public XMLList DisplayList;
    }
}