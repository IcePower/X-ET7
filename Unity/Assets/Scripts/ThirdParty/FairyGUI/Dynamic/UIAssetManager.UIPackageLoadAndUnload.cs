using System;
using System.Collections.Generic;
using System.Linq;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        /// <summary>
        /// 查找指定包名的UIPackageInfo 若不存在，则创建新的实例并安徽
        /// </summary>
        private UIPackageInfo FindOrCreateUIPackageInfo(string packageName)
        {
            return m_DictUIPackageInfos.TryGetValue(packageName, out var info) ? info : CreateUIPackageInfo(packageName);
        }
        
        /// <summary>
        /// 创建新的UIPackageInfo实例 这将触发对应UIPackage的加载
        /// </summary>
        private UIPackageInfo CreateUIPackageInfo(string packageName)
        {
            var info = GetPackageInfoFromPool(packageName);
            m_DictUIPackageInfos.Add(packageName, info);

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            var version = info.Version;

            m_AssetLoader.LoadUIPackageAsync(packageName, (bytes, prefix) => { OnUIPackageDataLoadFinished(packageName, version, bytes, prefix); });

            return info;
        }

        /// <summary>
        /// 查找指定包名的UIPackageInfo 若不存在，则返回null
        /// </summary>
        private UIPackageInfo FindUIPackageInfo(string packageName, uint version = 0)
        {
            if (!m_DictUIPackageInfos.TryGetValue(packageName, out var info))
                return null;

            // 存在版本校验的需求 且目标版本与当前不一致 则认为不存在该包
            if (version > 0 && info.Version != version)
                return null;

            return info;
        }

        /// <summary>
        /// 通过关联数据查找对应的UIPackageInfo 若不存在，则返回null
        /// </summary>
        private UIPackageInfo FindUIPackageInfoByRefInfo(UIPackageRefInfo refInfo)
        {
            return FindUIPackageInfo(refInfo.PackageName, refInfo.Version);
        }

        /// <summary>
        /// 销毁指定包名的UIPackageInfo实例 这将触发对应UIPackage的卸载
        /// </summary>
        private void DestroyUIPackageInfo(string packageName, uint version = 0)
        {
            var info = FindUIPackageInfo(packageName, version);
            if (info == null)
                return;

            m_DictUIPackageInfos.Remove(packageName);
            
            // 已经确认要卸载该包体，则可以提前清理其他包对该包的依赖关系
            foreach (var refInfo in info.BeDependentPackageRefInfos)
                ReturnPackageRefInfoToPool(refInfo);
            info.BeDependentPackageRefInfos.Clear();

            // 解除对依赖包的引用
            foreach (var refInfo in info.DependencePackageRefInfos)
            {
                var dependencePackageInfo = FindUIPackageInfoByRefInfo(refInfo);
                ReturnPackageRefInfoToPool(refInfo);
                
                if (dependencePackageInfo == null)
                    continue;

                RemoveBeDependentRelation(dependencePackageInfo, info.PackageName, info.Version);
            }
            info.DependencePackageRefInfos.Clear();
            
            // 卸载对应的UIPackage
            if (UIPackage.GetByName(packageName) != null)
                UIPackage.RemovePackage(packageName);
            
            // 让等待中的回调收到加载失败的结果
            info.InvokeCallbacks(null);
            
            // 回收进对象池
            ReturnPackageInfoToPool(info);
        }

        /// <summary>
        /// 接触被他人依赖的关联关系
        /// </summary>
        private void RemoveBeDependentRelation(UIPackageInfo info, string packageName, uint version)
        {
            var index = info.BeDependentPackageRefInfos.FindIndex(item => item.PackageName == packageName && item.Version == version);
            // 未找到引用关系
            if (index < 0)
                return;

            // 断开两个UIPackage的依赖关系
            var refInfo = info.BeDependentPackageRefInfos[index];
            info.BeDependentPackageRefInfos.RemoveAt(index);
            ReturnPackageRefInfoToPool(refInfo);
                
            CheckIfNeedDestroy(info);
        }
        
        /// <summary>
        /// UIPackage数据加载完成回调
        /// </summary>
        private void OnUIPackageDataLoadFinished(string packageName, uint version, byte[] bytes, string assetNamePrefix)
        {
            var info = FindUIPackageInfo(packageName, version);

            if (info == null)
                return;

            var package = bytes != null && bytes.Length > 0 ? UIPackage.AddPackage(bytes, assetNamePrefix, LoadResourceAsync) : null;
            if (package == null)
            {
                // 加载失败 销毁对应包体数据
                DestroyUIPackageInfo(packageName);
                return;
            }

            // 加载成功 解析依赖的包
            m_StrHashSetBuffer.Clear();
            foreach (var dependency in package.dependencies)
            {
                if (!dependency.TryGetValue("name", out var dependencePackageName))
                    continue;
                
                if (string.IsNullOrEmpty(dependencePackageName))
                    continue;
                
                if (string.Equals(dependencePackageName, packageName))
                    continue;

                m_StrHashSetBuffer.Add(dependencePackageName);
            }

            var remainCount = m_StrHashSetBuffer.Count;
            
            if (remainCount == 0)
            {
                // 没有依赖的包，直接设置结果
                info.SetUIPackage(package);
                // 并触发回调
                info.InvokeCallbacks(package);
                return;
            }
            
            foreach (var dependencePackageName in m_StrHashSetBuffer)
            {
                // 加载依赖的包
                var dependencePackageInfo = FindOrCreateUIPackageInfo(dependencePackageName);
                var dependenceVersion = dependencePackageInfo.Version;
                
                // 建立二者的关联关系
                info.DependencePackageRefInfos.Add(GetPackageRefInfoFromPool(dependencePackageName, dependenceVersion));
                dependencePackageInfo.BeDependentPackageRefInfos.Add(GetPackageRefInfoFromPool(packageName, version));
                
                dependencePackageInfo.AddCallback(_ =>
                {
                    // 做一轮基础数据校验
                    var dependencePackageInfoNew = FindUIPackageInfo(dependencePackageName, dependenceVersion);
                    if (dependencePackageInfoNew == null)
                        return;

                    var beDependentPackageInfo = FindUIPackageInfo(packageName, version);
                    if (beDependentPackageInfo == null)
                    {
                        // 依赖自己的Package已经被卸载了 解除关联
                        RemoveBeDependentRelation(dependencePackageInfoNew, packageName, version);
                        return;
                    }
                    
                    --remainCount;
                    if (remainCount != 0) 
                        return;
                    
                    // 依赖的包都加载完成 设置结果
                    info.SetUIPackage(package);
                    // 并触发回调
                    info.InvokeCallbacks(package);
                });
            }
        }

        private readonly HashSet<string> m_StrHashSetBuffer = new HashSet<string>();
        private readonly Dictionary<string, UIPackageInfo> m_DictUIPackageInfos = new Dictionary<string, UIPackageInfo>();
    }
}