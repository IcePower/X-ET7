using System;
using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        private void LoadResourceAsync(string name, string extension, Type type, PackageItem item)
        {
            var packageName = item.owner.name;
            var info = FindUIPackageInfo(packageName);
            if (info == null)
                return;

            var version = info.Version;

            if (type == typeof(Texture))
            {
                if (m_AssetLoader == null)
                    throw new Exception("请设置AssetLoader");

                // 在加载前添加引用 防止加载过程中UIPackage引用为0被卸载
                info.AddRef();

                m_AssetLoader.LoadTextureAsync(packageName, name, extension, texture =>
                {
                    var newInfo = FindUIPackageInfo(packageName, version);
                    if (newInfo == null)
                    {
                        // 对应UIPackage不存在  直接释放该资源
                        DestroyTexture(texture);
                        return;
                    }

                    if (texture == null)
                    {
                        // 加载失败 归还引用
                        info.RemoveRef();
                        CheckIfNeedDestroy(info);
                        return;
                    }
                    
                    m_LoadedTextures.Add(texture);

                    item.owner.SetItemAsset(item, texture, DestroyMethod.Custom);
                    item.texture.onRelease -= OnTextureRelease;
                    item.texture.onRelease += OnTextureRelease;
                    m_NTextureAssetRefInfos[item.texture.GetHashCode()] = GetPackageRefInfoFromPool(packageName, version);
                });
            }
            else if (type == typeof(AudioClip))
            {
                if (m_AssetLoader == null)
                    throw new Exception("请设置AssetLoader");

                // 在加载前添加引用 防止加载过程中UIPackage引用为0被卸载
                info.AddRef();

                m_AssetLoader.LoadAudioClipAsync(packageName, name, extension, audioClip =>
                {
                    var newInfo = FindUIPackageInfo(packageName, version);
                    if (newInfo == null)
                    {
                        // 对应UIPackage不存在  直接释放该资源
                        DestroyAudioClip(audioClip);
                        return;
                    }

                    if (audioClip == null)
                    {
                        // 加载失败 归还引用
                        info.RemoveRef();
                        CheckIfNeedDestroy(info);
                        return;
                    }
                    
                    m_LoadedAudioClips.Add(audioClip);

                    item.owner.SetItemAsset(item, audioClip, DestroyMethod.Custom);
                    item.audioClip.onRelease -= OnAudioClipRelease;
                    item.audioClip.onRelease += OnAudioClipRelease;
                    m_NAudioClipAssetRefInfos[item.audioClip.GetHashCode()] = GetPackageRefInfoFromPool(packageName, version);
                });
            }
        }
        private void OnTextureRelease(NTexture nTexture)
        {
            nTexture.onRelease -= OnTextureRelease;

            var hashCode = nTexture.GetHashCode();
            if (!m_NTextureAssetRefInfos.TryGetValue(hashCode, out var refInfo))
                return;

            m_NTextureAssetRefInfos.Remove(hashCode);

            var uiPackageInfo = FindUIPackageInfoByRefInfo(refInfo);
            ReturnPackageRefInfoToPool(refInfo);

            if (uiPackageInfo == null) 
                return;
            
            uiPackageInfo.RemoveRef();
            CheckIfNeedDestroy(uiPackageInfo);
        }

        private void OnAudioClipRelease(NAudioClip nAudioClip)
        {
            nAudioClip.onRelease -= OnAudioClipRelease;

            var hashCode = nAudioClip.GetHashCode();
            if (!m_NAudioClipAssetRefInfos.TryGetValue(hashCode, out var refInfo))
                return;

            m_NAudioClipAssetRefInfos.Remove(hashCode);

            var uiPackageInfo = FindUIPackageInfoByRefInfo(refInfo);
            ReturnPackageRefInfoToPool(refInfo);
            
            if (uiPackageInfo == null) 
                return;
            
            uiPackageInfo.RemoveRef();
            CheckIfNeedDestroy(uiPackageInfo);
        }

        private void DestroyTexture(Texture texture)
        {
            if (texture == null)
                return;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_LoadedTextures.Remove(texture);
            m_AssetLoader.ReleaseTexture(texture);
        }

        private void DestroyAudioClip(AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_LoadedAudioClips.Remove(audioClip);
            m_AssetLoader.ReleaseAudioClip(audioClip);
        }

        private readonly List<Texture> m_LoadedTextures = new List<Texture>();
        private readonly List<AudioClip> m_LoadedAudioClips = new List<AudioClip>();

        private readonly Dictionary<int, UIPackageRefInfo> m_NTextureAssetRefInfos = new Dictionary<int, UIPackageRefInfo>();
        private readonly Dictionary<int, UIPackageRefInfo> m_NAudioClipAssetRefInfos = new Dictionary<int, UIPackageRefInfo>();
    }
}