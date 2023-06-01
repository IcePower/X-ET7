using System;
using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public sealed partial class UIAssetManager : IUIAssetManager
    {
        public void Initialize(IUIAssetManagerConfiguration configuration)
        {
            if (m_Initialized)
                throw new Exception("UIAssetManager has been initialized!");

            m_Initialized = true;
            m_PackageHelper = configuration.PackageHelper;
            m_AssetLoader = configuration.AssetLoader;
            m_UnloadUnusedUIPackageImmediately = configuration.UnloadUnusedUIPackageImmediately;

            NTexture.CustomDestroyMethod += DestroyTexture;
            NAudioClip.CustomDestroyMethod = DestroyAudioClip;
            UIPackage.OnPackageAcquire += OnUIPackageAcquire;
            UIPackage.OnPackageRelease += OnUIPackageRelease;
            UIPackage.GetUIPackageByIdFunc = GetUIPackageByIdFunc;
            UIPackage.GetUIPackageByNameFunc = GetUIPackageByNameFunc;
            UIPackage.GetUIPackageAsyncByIdHandler = GetUIPackageAsyncById;
            UIPackage.GetUIPackageAsyncByNameHandler = GetUIPackageAsyncByName;
            UIPackage.RemoveAllPackagesHandler = UnloadAllUIPackages;
            UIPackage.RemoveUnusedPackagesHandler = UnloadUnusedUIPackages;
            UIPanel.GetPackageFunc = GetPackageFunc;

#if UNITY_EDITOR
            Debugger.CreateDebugger(this);
#endif
        }

        public void Dispose()
        {
            if (!m_Initialized)
                throw new Exception("UIAssetManager has not been initialized!");

            m_Initialized = false;
            
#if UNITY_EDITOR
            Debugger.DestroyDebugger();
#endif

            UnloadAllUIPackages();

            foreach (var texture in m_LoadedTextures)
                m_AssetLoader.UnloadTexture(texture);
            m_LoadedTextures.Clear();

            foreach (var audioClip in m_LoadedAudioClips)
                m_AssetLoader.UnloadAudioClip(audioClip);
            m_LoadedAudioClips.Clear();

            m_NTextureAssetRefInfos.Clear();
            m_NAudioClipAssetRefInfos.Clear();

            NTexture.CustomDestroyMethod -= DestroyTexture;
            NAudioClip.CustomDestroyMethod -= DestroyAudioClip;
            UIPackage.OnPackageAcquire -= OnUIPackageAcquire;
            UIPackage.OnPackageRelease -= OnUIPackageRelease;
            UIPackage.GetUIPackageByIdFunc -= GetUIPackageByIdFunc;
            UIPackage.GetUIPackageByNameFunc -= GetUIPackageByNameFunc;
            UIPackage.RemoveAllPackagesHandler -= UnloadAllUIPackages;
            UIPackage.RemoveUnusedPackagesHandler -= UnloadUnusedUIPackages;
            UIPanel.GetPackageFunc -= GetPackageFunc;
        }

        private UIPackage GetUIPackageByNameFunc(string name)
        {
            return FindOrCreateUIPackage(name);
        }

        private UIPackage GetUIPackageByIdFunc(string id)
        {
            if (m_PackageHelper == null)
                throw new Exception("请设置PackageHelper");

            var packageName = m_PackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
            {
                // 获取packageName失败
                Debug.Log($"获取packageName失败: {packageName}");
                return null;
            }
            
            return FindOrCreateUIPackage(packageName);
        }

        private void GetUIPackageAsyncByName(string arg1, UIPackage.GetUIPackageAsyncCallback arg2)
        {
            FindOrCreateUIPackageAsync(arg1, arg2);
        }

        private void GetUIPackageAsyncById(string id, UIPackage.GetUIPackageAsyncCallback callback)
        {
            if (m_PackageHelper == null)
                throw new Exception("请设置PackageHelper");
            
            var packageName = m_PackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
            {
                // 获取packageName失败
                Debug.Log($"获取packageName失败: {packageName}");
                callback(null);
                return;
            }
            
            FindOrCreateUIPackageAsync(packageName, callback);
        }

        private void UnloadAllUIPackages()
        {
            foreach (var packageRef in m_UIPackageRefs.Values)
                UIPackage.RemovePackage(packageRef.Name);

            m_UIPackageRefs.Clear();
        }

        private void UnloadUnusedUIPackages()
        {
            m_RemoveBuffer.Clear();

            foreach (var packageRef in m_UIPackageRefs.Values)
            {
                if (packageRef.RefCount > 0)
                    continue;

                m_RemoveBuffer.Add(packageRef.Name);
                UIPackage.RemovePackage(packageRef.Name);
            }

            foreach (var packageName in m_RemoveBuffer)
                m_UIPackageRefs.Remove(packageName);
        }

        private void GetPackageFunc(string packagePath, string packageName, UIPackage.GetUIPackageAsyncCallback onComplete)
        {
            FindOrCreateUIPackageAsync(packageName, onComplete);
        }

        private UIPackage FindOrCreateUIPackage(string packageName)
        {
            var packageRef = FindUIPackageRef(packageName);
            if (packageRef != null)
                return packageRef.UIPackage;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_AssetLoader.LoadUIPackageBytes(packageName, out var bytes, out var assetNamePrefix);
            if (bytes == null)
            {
                Debug.LogError($"加载UIPackage失败: {packageName}");
                return null;
            }
            
            return AddUIPackage(packageName, bytes, assetNamePrefix);
        }

        private void FindOrCreateUIPackageAsync(string packageName, UIPackage.GetUIPackageAsyncCallback onComplete)
        {
            var packageRef = FindUIPackageRef(packageName);
            if (packageRef != null)
            {
                onComplete?.Invoke(packageRef.UIPackage);
                return;
            }

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_AssetLoader.LoadUIPackageBytesAsync(packageName, (bytes, assetNamePrefix) =>
            {
                if (bytes == null)
                {
                    Debug.LogError($"加载UIPackage失败: {packageName}");
                    onComplete?.Invoke(null);
                    return;
                }
                
                var uiPackage = AddUIPackage(packageName, bytes, assetNamePrefix);
                onComplete?.Invoke(uiPackage);
            });
        }


        private bool m_Initialized;
        private IUIPackageHelper m_PackageHelper;
        private IUIAssetLoader m_AssetLoader;
        private bool m_UnloadUnusedUIPackageImmediately;

        private readonly List<Texture> m_LoadedTextures = new List<Texture>();
        private readonly List<AudioClip> m_LoadedAudioClips = new List<AudioClip>();

        private readonly HashSet<string> m_RemoveBuffer = new HashSet<string>();
    }
}