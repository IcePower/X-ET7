using System.Collections.Generic;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        /// <summary>
        /// 对UIPackage的引用关系记录数据
        /// </summary>
        public sealed class UIPackageRefInfo
        {
            public string PackageName;
            public uint Version;

            public void Reset()
            {
                PackageName = string.Empty;
                Version = 0;
            }
        }

        /// <summary>
        /// 从对象池中获取引用关系记录实例
        /// </summary>
        private UIPackageRefInfo GetPackageRefInfoFromPool(string packageName, uint version)
        {
            var info = m_PoolUIPackageRefInfos.Count > 0 ? m_PoolUIPackageRefInfos.Dequeue() : new UIPackageRefInfo();
            info.PackageName = packageName;
            info.Version = version;
            return info;
        }

        /// <summary>
        /// 回收引用关系记录
        /// </summary>
        private void ReturnPackageRefInfoToPool(UIPackageRefInfo info)
        {
            info.Reset();
            m_PoolUIPackageRefInfos.Enqueue(info);
        }
        
        private readonly Queue<UIPackageRefInfo> m_PoolUIPackageRefInfos = new Queue<UIPackageRefInfo>();
    }
}