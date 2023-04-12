using UnityEngine;

namespace FairyGUI.Dynamic
{
    public sealed class ResourcesUIAssetLoader : IUIAssetLoader
    {
        private readonly string m_PrefixAssetPath;

        public ResourcesUIAssetLoader(string mPrefixAssetPath = null)
        {
            m_PrefixAssetPath = mPrefixAssetPath ?? string.Empty;
        }

        public void LoadUIPackageAsync(string packageName, LoadUIPackageCallback callback)
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

        public void ReleaseTexture(Texture texture)
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

        public void ReleaseAudioClip(AudioClip audioClip)
        {
            Resources.UnloadAsset(audioClip);
        }
    }
}