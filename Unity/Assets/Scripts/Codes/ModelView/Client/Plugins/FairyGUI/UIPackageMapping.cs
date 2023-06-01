using System.Collections.Generic;
using Bright.Serialization;
using FairyGUI.Dynamic;

namespace ET.Client
{
    public sealed class UIPackageMapping : IUIPackageHelper
    {
        private readonly Dictionary<string, string> m_PackageIdToNameMap;

        public UIPackageMapping(byte[] mappingData)
        {
            ByteBuf byteBuf = new ByteBuf(mappingData);
            int count = byteBuf.ReadInt();

            Dictionary<string, string> mappingDict = new Dictionary<string, string>(count);
            for (int i = 0; i < count; i++)
            {
                mappingDict.Add(byteBuf.ReadString(), byteBuf.ReadString());
            }
            
            m_PackageIdToNameMap = mappingDict;
        }
        
        public string GetPackageNameById(string id)
        {
            return m_PackageIdToNameMap.TryGetValue(id, out var packageName) ? packageName : null;
        }
    }
}