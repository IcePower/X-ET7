namespace ET.Client
{
	[FriendOf(typeof(TestCPanel))]
	public static class TestCPanelSystem
	{
		public static void Awake(this TestCPanel self)
		{
			self.FUITestCPanel.Loader1.url = "ui://Icon1/Icon1";
			self.FUITestCPanel.Loader2.url = "ui://96tfczmnnt9r1";
			self.FUITestCPanel.Loader3.url = "ui://Icon3/IconCom";
		}

		public static void RegisterUIEvent(this TestCPanel self)
		{
			self.FUITestCPanel.CloseBtn.AddListner(() =>
			{
				var fuiCom = self.ClientScene().GetComponent<FUIComponent>();

				var testCPanelcontextData = (TestCPanel_ContextData)self.GetParent<FUIEntity>().ContextData;
				var contextData = fuiCom.AddChild<TestBPanel_ContextData>(true);
				contextData.Data = testCPanelcontextData.Data;
				
				fuiCom.ClosePanel(PanelId.TestCPanel, contextData);
			});
		}

		public static void OnShow(this TestCPanel self, Entity contextData = null)
		{
			TestCPanel_ContextData data = contextData as TestCPanel_ContextData;
			Log.Info(data.Data);
		}

		public static void OnHide(this TestCPanel self)
		{
		}

		public static void BeforeUnload(this TestCPanel self)
		{
		}
	}
}