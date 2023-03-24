﻿using UnityEngine;
using UnityEditor;
using System.IO;

namespace YooAsset.Editor
{
	public class DefaultFilterRule
	{
		/// <summary>
		/// 忽略的文件类型
		/// </summary>
		public static readonly string[] IgnoreFileExtensions = { "", ".so", ".dll", ".cs", ".js", ".boo", ".meta", ".cginc", ".hlsl" };
	}

	[DisplayName("收集所有资源")]
	public class CollectAll : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			return true;
		}
	}

	[DisplayName("收集场景")]
	public class CollectScene : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			return Path.GetExtension(data.AssetPath) == ".unity";
		}
	}

	[DisplayName("收集预制体")]
	public class CollectPrefab : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			return Path.GetExtension(data.AssetPath) == ".prefab";
		}
	}

	[DisplayName("收集精灵类型的纹理")]
	public class CollectSprite : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath);
			if (mainAssetType == typeof(Texture2D))
			{
				var texImporter = AssetImporter.GetAtPath(data.AssetPath) as TextureImporter;
				if (texImporter != null && texImporter.textureType == TextureImporterType.Sprite)
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

	[DisplayName("收集着色器变种集合")]
	public class CollectShaderVariants : IFilterRule
	{
		public bool IsCollectAsset(FilterRuleData data)
		{
			return Path.GetExtension(data.AssetPath) == ".shadervariants";
		}
	}
}