using ET.Client.Lobby;
using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class LobbyPanel: Entity, IAwake
	{
		public SystemLanguage Language { get; set; }

		public FUI_LobbyPanel FUILobbyPanel
		{
			get => (FUI_LobbyPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
