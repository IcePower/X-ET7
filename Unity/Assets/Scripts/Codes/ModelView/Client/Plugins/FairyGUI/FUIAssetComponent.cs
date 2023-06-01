using System.Collections.Generic;
using FairyGUI.Dynamic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class FUIAssetComponent : Entity, IAwake<bool>, IDestroy, IUIAssetManagerConfiguration
    {
        public UIAssetManager UIAssetManager;

        public Dictionary<int, string> Locations;
        
        public IUIPackageHelper PackageHelper { get; set; }
        
        public IUIAssetLoader AssetLoader { get;  set;}
        
        public bool UnloadUnusedUIPackageImmediately { get; set; }
    }
}