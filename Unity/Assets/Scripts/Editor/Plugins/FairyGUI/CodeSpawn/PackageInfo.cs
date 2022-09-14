using System.Collections.Generic;

namespace FUIEditor
{
    public class PackageInfo
    {
        public string Id;

        public string Name;

        public string Path;

        public Dictionary<string, PackageComponentInfo> PackageComponentInfos = new Dictionary<string, PackageComponentInfo>();
    }
}