using UnityEngine;

namespace FairyGUI.Dynamic
{
    /// <summary>
    /// 自定义委托的IUIAssetLoader派生类 为不方便实现接口的情况提供功能支持
    /// </summary>
    public sealed class DelegateUIAssetLoader : IUIAssetLoader
    {
        public delegate void LoadUIPackageAsyncHandler(string packageName, LoadUIPackageCallback callback);

        public delegate void LoadTextureAsyncHandler(string packageName, string assetName, string extension, LoadTextureCallback callback);

        public delegate void ReleaseTextureHandler(Texture texture);

        public delegate void LoadAudioClipAsyncHandler(string packageName, string assetName, string extension, LoadAudioClipCallback callback);

        public delegate void ReleaseAudioClipHandler(AudioClip audioClip);

        public DelegateUIAssetLoader(LoadUIPackageAsyncHandler mLoadUIPackageAsyncHandler, LoadTextureAsyncHandler mLoadTextureAsyncHandler, ReleaseTextureHandler mReleaseTextureHandler, LoadAudioClipAsyncHandler mLoadAudioClipAsyncHandler, ReleaseAudioClipHandler mReleaseAudioClipHandler)
        {
            m_LoadUIPackageAsyncHandler = mLoadUIPackageAsyncHandler;
            m_LoadTextureAsyncHandler = mLoadTextureAsyncHandler;
            m_ReleaseTextureHandler = mReleaseTextureHandler;
            m_LoadAudioClipAsyncHandler = mLoadAudioClipAsyncHandler;
            m_ReleaseAudioClipHandler = mReleaseAudioClipHandler;
        }

        public void LoadUIPackageAsync(string packageName, LoadUIPackageCallback callback)
        {
            m_LoadUIPackageAsyncHandler.Invoke(packageName, callback);
        }

        public void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback)
        {
            m_LoadTextureAsyncHandler.Invoke(packageName, assetName, extension, callback);
        }

        public void ReleaseTexture(Texture texture)
        {
            m_ReleaseTextureHandler.Invoke(texture);
        }

        public void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
        {
            m_LoadAudioClipAsyncHandler.Invoke(packageName, assetName, extension, callback);
        }

        public void ReleaseAudioClip(AudioClip audioClip)
        {
            m_ReleaseAudioClipHandler.Invoke(audioClip);
        }

        private readonly LoadUIPackageAsyncHandler m_LoadUIPackageAsyncHandler;
        private readonly LoadTextureAsyncHandler m_LoadTextureAsyncHandler;
        private readonly ReleaseTextureHandler m_ReleaseTextureHandler;
        private readonly LoadAudioClipAsyncHandler m_LoadAudioClipAsyncHandler;
        private readonly ReleaseAudioClipHandler m_ReleaseAudioClipHandler;
    }
}