using System;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(TestAPanel))]
	public static class TestAPanelSystem
	{
		public static void Awake(this TestAPanel self)
		{

		}
		
		public static void TranslateText(this TestAPanel self, SystemLanguage systemLanguage, Func<string, string, string> translator)
		{
			//本函数自动生成，请勿手动修改
			if (self.Language == systemLanguage)
			{
				return;
			}

			self.FUITestAPanel.n1.text = translator("2kcjlx6nlobh1-n1_lobh", self.FUITestAPanel.n1.text);
			self.FUITestAPanel.HideBtn.text = translator("2kcjlx6nlobh1-n2_lobh", self.FUITestAPanel.HideBtn.text);
			self.FUITestAPanel.OpenTestBBtn.text = translator("2kcjlx6nlobh1-n0_lobh", self.FUITestAPanel.OpenTestBBtn.text);
			self.FUITestAPanel.LanguageCombo.text = translator("2kcjlx6nlobh1-n6_lbin", self.FUITestAPanel.LanguageCombo.text);
			self.FUITestAPanel.n7.text = translator("2kcjlx6nlobh1-n7_lbin", self.FUITestAPanel.n7.text);
			self.FUITestAPanel.n9.text = translator("2kcjlx6nlobh1-n9_lbin", self.FUITestAPanel.n9.text);
			self.FUITestAPanel.n12.text = translator("2kcjlx6nlobh1-n12_lbin", self.FUITestAPanel.n12.text);
			self.FUITestAPanel.n13.text = translator("2kcjlx6nlobh1-n13_lbin", self.FUITestAPanel.n13.text);
			self.FUITestAPanel.n14.text = translator("2kcjlx6nlobh1-n14_lbin", self.FUITestAPanel.n14.text);
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
			self.FUITestAPanel.LanguageCombo.onChanged.Add(() =>
			{
				switch (self.FUITestAPanel.LanguageCombo.selectedIndex)
				{
					case 0:
						self.ClientScene().GetComponent<LocalizeComponent>().SwitchLanguage(SystemLanguage.ChineseSimplified);
						PrintUnitsName();
						break;
					
					case 1:
						self.ClientScene().GetComponent<LocalizeComponent>().SwitchLanguage(SystemLanguage.ChineseTraditional);
						PrintUnitsName();
						break;
					
					case 2:
						self.ClientScene().GetComponent<LocalizeComponent>().SwitchLanguage(SystemLanguage.English);
						PrintUnitsName();
						break;
				}
			});
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
