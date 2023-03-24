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