using UnityEngine;

namespace FairyGUI.Dynamic
{
    /// <summary>
    /// UIPackage二进制数据加载回调
    /// </summary>
    public delegate void LoadUIPackageBytesCallback(byte[] bytes, string assetNamePrefix);

    /// <summary>
    /// Texture资源加载回调
    /// </summary>
    public delegate void LoadTextureCallback(Texture texture);

    /// <summary>
    /// AudioClip资源加载回调
    /// </summary>
    public delegate void LoadAudioClipCallback(AudioClip audioClip);

    /// <summary>
    /// UI资源加载器接口 由外部实现接口后传给管理器进行使用
    /// </summary>
    public interface IUIAssetLoader
    {
        /// <summary>
        /// 异步加载UIPackage二进制数据
        /// </summary>
        void LoadUIPackageBytesAsync(string packageName, LoadUIPackageBytesCallback callback);
        
        /// <summary>
        /// 同步加载UIPackage二进制数据 通过bytes返回数据 assetNamePrefix返回资源前缀
        /// </summary>
        void LoadUIPackageBytes(string packageName, out byte[] bytes, out string assetNamePrefix);

        /// <summary>
        /// 异步加载Texture资源
        /// </summary>
        void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback);

        /// <summary>
        /// 卸载Texture资源
        /// </summary>
        void UnloadTexture(Texture texture);

        /// <summary>
        /// 异步加载AudioClip资源
        /// </summary>
        void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback);

        /// <summary>
        /// 卸载AudioCip资源
        /// </summary>
        void UnloadAudioClip(AudioClip audioClip);
    }
}