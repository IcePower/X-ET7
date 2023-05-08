using ET.Client.Lobby;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class LobbyPanel: Entity, IAwake
	{
		private FUI_LobbyPanel _fuiLobbyPanel;

		public FUI_LobbyPanel FUILobbyPanel
		{
			get => _fuiLobbyPanel ??= (FUI_LobbyPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
