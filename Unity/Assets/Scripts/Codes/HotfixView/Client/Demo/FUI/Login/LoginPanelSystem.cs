using System;
using UnityEngine;

namespace ET.Client
{
	public static class LoginPanelSystem
	{
		public static void Awake(this LoginPanel self)
		{

		}

		public static void RegisterUIEvent(this LoginPanel self)
		{
			self.FUILoginPanel.LoginBtn.AddListnerAsync(self.Login);
		}
		
		public static void TranslateText(this LoginPanel self, SystemLanguage systemLanguage, Func<string, string, string> translator)
		{
			//本函数自动生成，请勿手动修改
			if (self.Language == systemLanguage)
			{
				return;
			}

			self.FUILoginPanel.LoginBtn.text = translator("rgfb0w498omm0-n27_lobh", self.FUILoginPanel.LoginBtn.text);
		}

		public static void OnShow(this LoginPanel self, Entity contextData = null)
		{
			var context = (LoginPanel_ContextData)contextData;
			Log.Info(context.Data);
		}

		public static void OnHide(this LoginPanel self)
		{

		}

		public static void BeforeUnload(this LoginPanel self)
		{

		}

		private static async ETTask Login(this LoginPanel self)
		{
			await LoginHelper.Login(self.DomainScene(), self.FUILoginPanel.AccountInput.Input.text, self.FUILoginPanel.PasswordInput.Input.text);
		}
	}
}