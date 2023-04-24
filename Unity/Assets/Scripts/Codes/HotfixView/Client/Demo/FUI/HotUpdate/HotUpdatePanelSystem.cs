using System;
using ET.EventType;
using UnityEngine;

namespace ET.Client
{
	[Event(SceneType.Client)]
	public class OnPatchDownloadProgressEvent: AEvent<Scene, OnPatchDownloadProgress>
	{
		protected override async ETTask Run(Scene scene, OnPatchDownloadProgress a)
		{
			HotUpdatePanel hotupdatePanel = scene.GetComponent<FUIComponent>().GetPanelLogic<HotUpdatePanel>();
			hotupdatePanel?.OnPatchDownloadProgress(a.TotalDownloadCount, a.CurrentDownloadCount, a.TotalDownloadSizeBytes, a.CurrentDownloadSizeBytes);

			await ETTask.CompletedTask;
		}
	}
	
	[Event(SceneType.Client)]
	public class OnPatchDownlodFailedEvent: AEvent<Scene, OnPatchDownlodFailed>
	{
		protected override async ETTask Run(Scene scene, OnPatchDownlodFailed a)
		{
			Log.Error($"下载资源失败: {a.FileName} {a.Error}");
			await ETTask.CompletedTask;
		}
	}
	
	public static class HotUpdatePanelSystem
	{
		public static void Awake(this HotUpdatePanel self)
		{

		}

		public static void TranslateText(this HotUpdatePanel self, SystemLanguage systemLanguage, Func<string, string, string> translator)
		{
			//本函数自动生成，请勿手动修改
			if (self.Language == systemLanguage)
			{
				return;
			}

			self.FUIHotUpdatePanel.n0.text = translator("2f8jqefflobh0-n0_lobh", self.FUIHotUpdatePanel.n0.text);
		}

		public static void RegisterUIEvent(this HotUpdatePanel self)
		{

		}

		public static void OnShow(this HotUpdatePanel self, Entity contexData = null)
		{

		}

		public static void OnHide(this HotUpdatePanel self)
		{

		}

		public static void BeforeUnload(this HotUpdatePanel self)
		{

		}

		public static void OnPatchDownloadProgress(this HotUpdatePanel self, int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
		{
			self.FUIHotUpdatePanel.ProgressBar.value = 100.0f * currentDownloadBytes / totalDownloadBytes;
		}
	}
}