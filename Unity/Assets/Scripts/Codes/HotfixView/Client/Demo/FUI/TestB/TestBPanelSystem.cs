using System;
using UnityEngine;

namespace ET.Client
{
	public static class TestBPanelSystem
	{
		public static void Awake(this TestBPanel self)
		{

		}

		public static void TranslateText(this TestBPanel self, SystemLanguage systemLanguage, Func<string, string, string> translator)
		{
			//本函数自动生成，请勿手动修改
			if (self.Language == systemLanguage)
			{
				return;
			}

			self.FUITestBPanel.n0.text = translator("296l7tjhlobh0-n0_lobh", self.FUITestBPanel.n0.text);
			self.FUITestBPanel.CloseBtn.text = translator("296l7tjhlobh0-n1_lobh", self.FUITestBPanel.CloseBtn.text);
		}

		public static void RegisterUIEvent(this TestBPanel self)
		{
			self.FUITestBPanel.CloseBtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().ClosePanel(PanelId.TestBPanel);
			});
		}

		public static void OnShow(this TestBPanel self, Entity contextData = null)
		{

		}

		public static void OnHide(this TestBPanel self)
		{

		}

		public static void BeforeUnload(this TestBPanel self)
		{

		}
	}
}