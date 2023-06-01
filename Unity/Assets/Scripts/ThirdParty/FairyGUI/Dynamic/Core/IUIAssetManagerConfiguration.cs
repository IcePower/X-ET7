namespace FairyGUI.Dynamic
{
    /// <summary>
    /// UI资源管理器配置
    /// </summary>
    public interface IUIAssetManagerConfiguration
    {
        /// <summary>
        /// UIPackage辅助工具
        /// </summary>
        public IUIPackageHelper PackageHelper { get; }
        
        /// <summary>
        /// UI资源加载器
        /// </summary>
        public IUIAssetLoader AssetLoader { get; }
        
        /// <summary>
        /// 是否立即卸载未使用的UIPackage
        /// </summary>
        public bool UnloadUnusedUIPackageImmediately { get; }
    }
}