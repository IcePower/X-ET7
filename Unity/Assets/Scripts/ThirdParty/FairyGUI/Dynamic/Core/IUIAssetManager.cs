using System;

namespace FairyGUI.Dynamic
{
    /// <summary>
    /// UI资源管理器 用于统一维护UIPackage与其他UI资源的加载与卸载
    /// </summary>
    public interface IUIAssetManager : IDisposable
    {
        /// <summary>
        /// 通过配置实例初始化管理器
        /// </summary>
        void Initialize(IUIAssetManagerConfiguration configuration);
    }
}