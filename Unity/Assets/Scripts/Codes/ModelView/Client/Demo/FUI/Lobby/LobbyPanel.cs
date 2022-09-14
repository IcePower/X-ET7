namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class LobbyPanel: Entity, IAwake
	{
		public FUI_LobbyPanel FUILobbyPanel
		{
			get => (FUI_LobbyPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
