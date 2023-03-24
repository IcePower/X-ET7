namespace ET.Client
{
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