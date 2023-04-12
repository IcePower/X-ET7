using System;
using System.Collections.Generic;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        public sealed class UIPackageInfo
        {
            /// <summary>
            /// 版本号
            /// </summary>
            public uint Version { get; set; }

            /// <summary>
            /// 包名
            /// </summary>
            public string PackageName { get; set; }

            /// <summary>
            /// UI包实例
            /// </summary>
            public UIPackage UIPackage { get; private set; }

            /// <summary>
            /// 是否加载已完成
            /// </summary>
            public bool IsLoadDone => UIPackage != null;

            /// <summary>
            /// 是否存在引用
            /// </summary>
            public bool IsAnyReference => ReferenceCount > 0 || BeDependentPackageRefInfos.Count > 0;

            /// <summary>
            /// 引用数量
            /// </summary>
            public int ReferenceCount { get; private set; }

            /// <summary>
            /// 依赖的UIPackage关联数据
            /// </summary>
            public List<UIPackageRefInfo> DependencePackageRefInfos { get; } = new List<UIPackageRefInfo>();

            /// <summary>
            /// 被依赖的UIPackage关联数据
            /// </summary>
            public List<UIPackageRefInfo> BeDependentPackageRefInfos { get; } = new List<UIPackageRefInfo>();

            /// <summary>
            /// 添加引用计数
            /// </summary>
            public void AddRef()
            {
                ++ReferenceCount;
            }

            /// <summary>
            /// 移除引用计数
            /// </summary>
            public void RemoveRef()
            {
                --ReferenceCount;
            }

            /// <summary>
            /// 添加回调
            /// </summary>
            public void AddCallback(Action<UIPackage> callback)
            {
                if (callback == null)
                    return;

                if (IsLoadDone)
                    callback.Invoke(UIPackage);
                else
                    m_QueueCallbacks.Enqueue(callback);
            }

            /// <summary>
            /// 触发回调
            /// </summary>
            public void InvokeCallbacks(UIPackage package)
            {
                while (m_QueueCallbacks.Count > 0)
                    m_QueueCallbacks.Dequeue().Invoke(package);
            }

            /// <summary>
            /// 设置UIPackage
            /// </summary>
            public void SetUIPackage(UIPackage package)
            {
                UIPackage = package;
            }

            public void Reset()
            {
                Version = 0;
                PackageName = string.Empty;
                UIPackage = null;
                ReferenceCount = 0;
                DependencePackageRefInfos.Clear();
                BeDependentPackageRefInfos.Clear();
                m_QueueCallbacks.Clear();
            }

            private readonly Queue<Action<UIPackage>> m_QueueCallbacks = new Queue<Action<UIPackage>>();
        }

        /// <summary>
        /// 从对象池中获取UIPackage数据实例
        /// </summary>
        private UIPackageInfo GetPackageInfoFromPool(string packageName)
        {
            var info = m_PoolUIPackageInfos.Count > 0 ? m_PoolUIPackageInfos.Dequeue() : new UIPackageInfo();
            info.Version = ++m_Version;
            info.PackageName = packageName;
            return info;
        }

        /// <summary>
        /// 回收UIPackage数据实例进对象池
        /// </summary>
        private void ReturnPackageInfoToPool(UIPackageInfo info)
        {
            info.Reset();
            m_PoolUIPackageInfos.Enqueue(info);
        }

        private void CheckIfNeedDestroy(UIPackageInfo info)
        {
            // 判断是否要卸载UIPackage
            if (info.IsAnyReference || !UnloadUnusedUIPackageImmediately)
                return;

            DestroyUIPackageInfo(info.PackageName, info.Version);
        }

        private void OnUIPackageAcquire(UIPackage uiPackage, string _)
        {
            var uiPackageInfo = FindUIPackageInfo(uiPackage.name);
            if (uiPackageInfo == null || uiPackageInfo.UIPackage != uiPackage)
                return;

            uiPackageInfo.AddRef();
        }

        private void OnUIPackageRelease(UIPackage uiPackage, string _)
        {
            var uiPackageInfo = FindUIPackageInfo(uiPackage.name);
            if (uiPackageInfo == null || uiPackageInfo.UIPackage != uiPackage)
                return;

            uiPackageInfo.RemoveRef();
            CheckIfNeedDestroy(uiPackageInfo);
        }

        private uint m_Version;
        private readonly Queue<UIPackageInfo> m_PoolUIPackageInfos = new Queue<UIPackageInfo>();
    }
}