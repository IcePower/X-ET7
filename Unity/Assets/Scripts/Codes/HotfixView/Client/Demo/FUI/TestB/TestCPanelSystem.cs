namespace ET.Client
{
	[FriendOf(typeof(TestCPanel))]
	public static class TestCPanelSystem
	{
		public static void Awake(this TestCPanel self)
		{
			self.FUITestCPanel.Loader1.url = "ui://Icon1/Icon1";
			self.FUITestCPanel.Loader2.url = "ui://96tfczmnnt9r1";
		}

		public static void RegisterUIEvent(this TestCPanel self)
		{
			self.FUITestCPanel.CloseBtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().ClosePanel(PanelId.TestCPanel);
			});
		}

		public static void OnShow(this TestCPanel self, Entity contextData = null)
		{
		}

		public static void OnHide(this TestCPanel self)
		{
		}

		public static void BeforeUnload(this TestCPanel self)
		{
		}
	}
}