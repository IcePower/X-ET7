using System;
using UnityEngine;

namespace ET.Client
{
	public static class LobbyPanelSystem
	{
		public static void Awake(this LobbyPanel self)
		{

		}
		
		public static void RegisterUIEvent(this LobbyPanel self)
		{
			self.FUILobbyPanel.TestABtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().HideAndShowPanelStackAsync(PanelId.LobbyPanel, PanelId.TestAPanel).Coroutine();
			});
			
			self.FUILobbyPanel.EnterMap.AddListnerAsync(self.EnterMap);
		}
		
		public static void TranslateText(this LobbyPanel self, SystemLanguage systemLanguage, Func<string, string, string> translator)
		{
			//本函数自动生成，请勿手动修改
			if (self.Language == systemLanguage)
			{
				return;
			}

			self.FUILobbyPanel.TestABtn.text = translator("ti3ka994t52l0-n2_lobh", self.FUILobbyPanel.TestABtn.text);
			self.FUILobbyPanel.EnterMap.text = translator("ti3ka994t52l0-n3_lobh", self.FUILobbyPanel.EnterMap.text);
		}

		public static void OnShow(this LobbyPanel self, Entity contextData = null)
		{

		}

		public static void OnHide(this LobbyPanel self)
		{

		}

		public static void BeforeUnload(this LobbyPanel self)
		{

		}

		private static async ETTask EnterMap(this LobbyPanel self)
		{
			await EnterMapHelper.EnterMapAsync(self.ClientScene());
			self.ClientScene().GetComponent<FUIComponent>().HidePanel(PanelId.LobbyPanel);
		}
	}
}