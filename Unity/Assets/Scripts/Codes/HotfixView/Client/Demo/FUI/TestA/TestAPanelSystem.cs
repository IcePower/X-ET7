using System;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(TestAPanel))]
	public static class TestAPanelSystem
	{
		public static void Awake(this TestAPanel self)
		{

		}

		public static void RegisterUIEvent(this TestAPanel self)
		{
			self.FUITestAPanel.OpenTestBBtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().HideAndShowPanelStackAsync(PanelId.TestAPanel, PanelId.TestBPanel).Coroutine();
			});
			
			self.FUITestAPanel.HideBtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().HidePanel(PanelId.TestAPanel);
			});
			
			self.FUITestAPanel.LanguageCombo.items = new string[] {"简体中文", "繁體中文", "English"};
			self.FUITestAPanel.LanguageCombo.selectedIndex = 0;
			self.FUITestAPanel.LanguageCombo.onChanged.Add(() =>
			{
				switch (self.FUITestAPanel.LanguageCombo.selectedIndex)
				{
					case 0:
						self.SwitchLanguage(SystemLanguage.ChineseSimplified);
						PrintUnitsName();
						break;
					
					case 1:
						self.SwitchLanguage(SystemLanguage.ChineseTraditional);
						PrintUnitsName();
						break;
					
					case 2:
						self.SwitchLanguage(SystemLanguage.English);
						PrintUnitsName();
						break;
				}
			});
		}
		
		private static void SwitchLanguage(this TestAPanel self, SystemLanguage language)
		{
			self.ClientScene().GetComponent<LocalizeComponent>().SwitchLanguage(language);
		}

		private static void PrintUnitsName()
		{
			var dataMap = UnitConfigCategory.Instance.GetAll();
			foreach (var kv in dataMap)
			{
				Log.Info($"id: {kv.Key}, name: {kv.Value.Name}");
			}
		}

		public static void OnShow(this TestAPanel self, Entity contexData = null)
		{

		}

		public static void OnHide(this TestAPanel self)
		{

		}

		public static void BeforeUnload(this TestAPanel self)
		{

		}
	}
}
