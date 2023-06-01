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
            var packageRef = FindUIPackageRef(packageName);
            if (packageRef == null)
                return;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            // 在加载前添加引用 防止加载过程中UIPackage引用为0被卸载
            packageRef.AddRef();

            if (type == typeof(Texture))
            {
                m_AssetLoader.LoadTextureAsync(packageName, name, extension, asset =>
                {
                    var newPackageRef = FindUIPackageRef(packageName);
                    if (newPackageRef != packageRef)
                    {
                        // 如果加载完成后UIPackage引用不是当前引用 则卸载资源
                        DestroyTexture(asset);
                        return;
                    }

                    if (asset == null)
                    {
                        // 加载失败 归还引用
                        packageRef.RemoveRef();
                        return;
                    }

                    // 加载成功
                    m_LoadedTextures.Add(asset);

                    item.owner.SetItemAsset(item, asset, DestroyMethod.Custom);
                    item.texture.onRelease -= OnTextureRelease;
                    item.texture.onRelease += OnTextureRelease;
                    m_NTextureAssetRefInfos[item.texture.GetHashCode()] = packageRef;
                });
            }
            else if (type == typeof(AudioClip))
            {
                m_AssetLoader.LoadAudioClipAsync(packageName, name, extension, asset =>
                {
                    var newPackageRef = FindUIPackageRef(packageName);
                    if (newPackageRef != packageRef)
                    {
                        // 如果加载完成后UIPackage引用不是当前引用 则卸载资源
                        DestroyAudioClip(asset);
                        return;
                    }

                    if (asset == null)
                    {
                        // 加载失败 归还引用
                        packageRef.RemoveRef();
                        return;
                    }

                    // 加载成功
                    m_LoadedAudioClips.Add(asset);

                    item.owner.SetItemAsset(item, asset, DestroyMethod.Custom);
                    item.audioClip.onRelease -= OnAudioClipRelease;
                    item.audioClip.onRelease += OnAudioClipRelease;
                    m_NAudioClipAssetRefInfos[item.audioClip.GetHashCode()] = packageRef;
                });
            }
            else
            {
                // 暂不支持的类型 归还引用
                packageRef.RemoveRef();
            }
        }

        private void OnTextureRelease(NTexture nTexture)
        {
            nTexture.onRelease -= OnTextureRelease;

            var hashCode = nTexture.GetHashCode();
            if (!m_NTextureAssetRefInfos.TryGetValue(hashCode, out var refInfo))
                return;

            m_NTextureAssetRefInfos.Remove(hashCode);

            var packageRef = FindUIPackageRef(refInfo.Name);
            if (packageRef != refInfo)
                return;

            // 归还引用
            packageRef.RemoveRef();
        }

        private void OnAudioClipRelease(NAudioClip nAudioClip)
        {
            nAudioClip.onRelease -= OnAudioClipRelease;

            var hashCode = nAudioClip.GetHashCode();
            if (!m_NAudioClipAssetRefInfos.TryGetValue(hashCode, out var refInfo))
                return;

            m_NAudioClipAssetRefInfos.Remove(hashCode);

            var packageRef = FindUIPackageRef(refInfo.Name);
            if (packageRef != refInfo)
                return;

            // 归还引用
            packageRef.RemoveRef();
        }

        private readonly Dictionary<int, UIPackageRef> m_NTextureAssetRefInfos = new Dictionary<int, UIPackageRef>();
        private readonly Dictionary<int, UIPackageRef> m_NAudioClipAssetRefInfos = new Dictionary<int, UIPackageRef>();
    }
}