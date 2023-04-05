using System.Collections.Generic;
using FairyGUI.Dynamic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class FUIAssetComponent : Entity, IAwake, IDestroy
    {
        public UIAssetManager UIAssetManager;

        public IUIAssetLoader UIAssetLoader;

        public Dictionary<int, string> Locations;
    }
}