namespace ET.Client
{
	public static class LobbyPanelSystem
	{
		public static void RegisterUIEvent(this LobbyPanel self)
		{
			self.FUILobbyPanel.EnterMap.AddListnerAsync(self.EnterMap);
		}

		public static void OnShow(this LobbyPanel self)
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