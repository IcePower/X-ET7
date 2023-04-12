using UnityEngine;

namespace FairyGUI.Dynamic
{
    public delegate void LoadUIPackageCallback(byte[] bytes, string assetNamePrefix);

    public delegate void LoadTextureCallback(Texture texture);

    public delegate void LoadAudioClipCallback(AudioClip audioClip);

    /// <summary>
    /// UI资源加载器接口
    /// </summary>
    public interface IUIAssetLoader
    {
        /// <summary>
        /// 异步加载UIPackage文化的二进制数据
        /// </summary>
        void LoadUIPackageAsync(string packageName, LoadUIPackageCallback callback);

        /// <summary>
        /// 异步加载Texture资源
        /// </summary>
        void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback);

        /// <summary>
        /// 释放Texture资源
        /// </summary>
        void ReleaseTexture(Texture texture);

        /// <summary>
        /// 异步加载AudioClip资源
        /// </summary>
        void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback);

        /// <summary>
        /// 释放AudioCip资源
        /// </summary>
        void ReleaseAudioClip(AudioClip audioClip);
    }
}