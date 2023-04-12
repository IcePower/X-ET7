using System;
using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public sealed partial class UIAssetManager : IDisposable
    {
        /// <summary>
        /// 是否立即卸载引用计数为0的UIPackage
        /// </summary>
        public bool UnloadUnusedUIPackageImmediately { get; set; } = true;
        
        private readonly IUIAssetLoader m_AssetLoader;
        private readonly IUIPackageHelper m_UIPackageHelper;

        public UIAssetManager(IUIAssetLoader assetLoader, IUIPackageHelper uiPackageHelper)
        {
            m_AssetLoader = assetLoader;
            m_UIPackageHelper = uiPackageHelper;
            
            NTexture.CustomDestroyMethod = DestroyTexture;
            NAudioClip.CustomDestroyMethod = DestroyAudioClip;
            UIPackage.OnPackageAcquire += OnUIPackageAcquire;
            UIPackage.OnPackageRelease += OnUIPackageRelease;
            UIPanel.GetPackageFunc = GetPackageFunc;

#if UNITY_EDITOR
            Debugger.CreateDebugger(this);
#endif
        }

        public void Dispose()
        {
            UnloadAllUIPackages();
            
            foreach (var texture in m_LoadedTextures)
                m_AssetLoader.ReleaseTexture(texture);
            m_LoadedTextures.Clear();
            
            foreach (var audioClip in m_LoadedAudioClips)
                m_AssetLoader.ReleaseAudioClip(audioClip);
            m_LoadedAudioClips.Clear();
            
            m_NTextureAssetRefInfos.Clear();
            m_NAudioClipAssetRefInfos.Clear();
            
            m_StrHashSetBuffer.Clear();
            m_DictUIPackageInfos.Clear();
            
            m_Version = 0;
            m_PoolUIPackageInfos.Clear();
            m_PoolUIPackageRefInfos.Clear();
            
            m_Buffer.Clear();
            
#if UNITY_EDITOR
            Debugger.DestroyDebugger();
#endif
            
            NTexture.CustomDestroyMethod -= DestroyTexture;
            NAudioClip.CustomDestroyMethod -= DestroyAudioClip;
            UIPackage.OnPackageAcquire -= OnUIPackageAcquire;
            UIPackage.OnPackageRelease -= OnUIPackageRelease;
            UIPanel.GetPackageFunc -= GetPackageFunc;
        }

        /// <summary>
        /// 加载指定的UIPackage 不会增加引用计数
        /// </summary>
        public void LoadUIPackageAsync(string packageName, Action<UIPackage> callback = null)
        {
            var info = FindOrCreateUIPackageInfo(packageName);
            if (callback == null)
                return;
            
            info.AddCallback(callback);
        }

        /// <summary>
        /// 加载指定的UIPackage 并让引用计数+1
        /// </summary>
        public void LoadUIPackageAsyncAndAddRef(string packageName, Action<UIPackage> callback = null)
        {
            var info = FindOrCreateUIPackageInfo(packageName);
            
            info.AddRef();
            
            if (callback == null)
                return;
            
            info.AddCallback(callback);
        }

        /// <summary>
        /// 通过id加载指定的UIPackage 不会增加引用计数
        /// </summary>
        public void LoadUIPackageAsyncById(string id, Action<UIPackage> callback = null)
        {
            var packageName = m_UIPackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
            {
                callback?.Invoke(null);
                return;
            }

            LoadUIPackageAsync(packageName, callback);
        }

        /// <summary>
        /// 通过id加载指定的UIPackage 并让引用计数+1
        /// </summary>
        public void LoadUIPackageAsyncAndAddRefById(string id, Action<UIPackage> callback = null)
        {
            var packageName = m_UIPackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
            {
                callback?.Invoke(null);
                return;
            }
            
            LoadUIPackageAsyncAndAddRef(packageName, callback);
        }

        /// <summary>
        /// 令指定的UIPackage 引用次数减一
        /// </summary>
        public void ReleaseUIPackage(string packageName)
        {
            var info = FindUIPackageInfo(packageName);
            if (info == null)
                return;
            
            info.RemoveRef();
            CheckIfNeedDestroy(info);
        }

        /// <summary>
        /// 通过id令指定的UIPackage 引用次数减一
        /// </summary>
        public void ReleaseUIPackageById(string id)
        {
            var packageName = m_UIPackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
                return;

            ReleaseUIPackage(packageName);
        }
        
        /// <summary>
        /// 卸载引用计数为0的UIPackage
        /// </summary>
        public void UnloadUnusedUIPackages()
        {
            // 一直遍历到无包可卸载为止
            while (true)
            {
                foreach (var (key, info) in m_DictUIPackageInfos)
                {
                    if (info.IsAnyReference)
                        continue;
                
                    m_Buffer.Enqueue(key);
                }
                
                if (m_Buffer.Count == 0)
                    break;

                while (m_Buffer.Count > 0)
                    DestroyUIPackageInfo(m_Buffer.Dequeue());
            }
        }
        
        /// <summary>
        /// 强制卸载指定的UI包 无视引用次数
        /// </summary>
        public void UnloadUIPackageForce(string packageName)
        {
            DestroyUIPackageInfo(packageName);
        }
        
        /// <summary>
        /// 通过id强制卸载指定的UI包 无视引用次数
        /// </summary>
        public void UnloadUIPackageForceById(string id)
        {
            var packageName = m_UIPackageHelper.GetPackageNameById(id);
            if (string.IsNullOrEmpty(packageName))
                return;

            UnloadUIPackageForce(packageName);
        }
        
        /// <summary>
        /// 强制卸载所有UI包 无视引用次数
        /// </summary>
        public void UnloadAllUIPackages()
        {
            foreach (var info in m_DictUIPackageInfos.Keys)
                m_Buffer.Enqueue(info);
            
            while (m_Buffer.Count > 0)
                DestroyUIPackageInfo(m_Buffer.Dequeue());
        }

        private void GetPackageFunc(string packagePath, string packageName, Action<UIPackage> onComplete)
        {
            LoadUIPackageAsync(packageName, onComplete);
        }
        
        private readonly Queue<string> m_Buffer = new Queue<string>();
    }
}
