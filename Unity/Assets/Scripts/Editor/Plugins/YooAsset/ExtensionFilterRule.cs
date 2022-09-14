using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace ET
{
    public class CollectBytes : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            return Path.GetExtension(data.AssetPath) == ".bytes";
        }
    }
    
    public class CollectAtlas : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
            if(mainAssetType == typeof(Texture2D))
            {
                var texImporter = AssetImporter.GetAtPath(data.AssetPath) as TextureImporter;
                if (texImporter != null && texImporter.textureType == TextureImporterType.Default)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}