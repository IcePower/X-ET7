using UnityEngine;

namespace FairyGUI.Dynamic
{
    /// <summary>
    /// Resources资源加载器
    /// </summary>
    public sealed class ResourcesUIAssetLoader : IUIAssetLoader
    {
        private readonly string m_PrefixAssetPath;

        public ResourcesUIAssetLoader(string mPrefixAssetPath = null)
        {
            m_PrefixAssetPath = mPrefixAssetPath ?? string.Empty;
        }
        
        public void LoadUIPackageBytesAsync(string packageName, LoadUIPackageBytesCallback callback)
        {
            var assetPath = packageName;
            if (!string.IsNullOrEmpty(m_PrefixAssetPath))
                assetPath = m_PrefixAssetPath + "/" + assetPath;

            Resources.LoadAsync<TextAsset>(assetPath + "_fui").completed += operation =>
            {
                var request = (ResourceRequest)operation;
                if (request.asset == null)
                    callback(null, packageName);
                else
                {
                    var bytes = ((TextAsset)request.asset).bytes;
                    Resources.UnloadAsset(request.asset);
                    
                    callback(bytes, assetPath);
                }
            };
        }

        public void LoadUIPackageBytes(string packageName, out byte[] bytes, out string assetNamePrefix)
        {
            var assetPath = packageName;
            if (!string.IsNullOrEmpty(m_PrefixAssetPath))
                assetPath = m_PrefixAssetPath + "/" + assetPath;

            var asset = Resources.Load<TextAsset>(assetPath + "_fui");
            if (asset == null)
            {
                bytes = null;
                assetNamePrefix = packageName;
            }
            else
            {
                bytes = asset.bytes;
                assetNamePrefix = assetPath;
                Resources.UnloadAsset(asset);
            }
        }

        public void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback)
        {
            Resources.LoadAsync<Texture>(assetName).completed += operation =>
            {
                var request = (ResourceRequest)operation;
                if (request.asset == null)
                    callback(null);
                else
                    callback((Texture)request.asset);
            };
        }

        public void UnloadTexture(Texture texture)
        {
            Resources.UnloadAsset(texture);
        }

        public void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
        {
            Resources.LoadAsync<AudioClip>(assetName).completed += operation =>
            {
                var request = (ResourceRequest)operation;
                if (request.asset == null)
                    callback(null);
                else
                    callback((AudioClip)request.asset);
            };
        }

        public void UnloadAudioClip(AudioClip audioClip)
        {
            Resources.UnloadAsset(audioClip);
        }
    }
}