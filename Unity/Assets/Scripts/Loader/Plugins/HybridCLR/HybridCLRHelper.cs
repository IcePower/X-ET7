using System.Collections.Generic;
using HybridCLR;
using UnityEngine;

namespace ET
{
    public static class HybridCLRHelper
    {
        public static void Load()
        {
            string[] addresses = MonoResComponent.Instance.GetAddressesByTag("aotdlls");
            foreach (string address in addresses)
            {
                byte[] bytes = MonoResComponent.Instance.LoadRawFile(address);
                RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.Consistent);
            }
        }
    }
}