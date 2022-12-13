/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Lobby
{
	public partial class FUI_LobbyPanel: GComponent
	{
		public GImage bg;
		public GButton EnterGame;
		public const string URL = "ui://ti3ka994t52l0";

		public static FUI_LobbyPanel CreateInstance()
		{
			return (FUI_LobbyPanel)UIPackage.CreateObject("Lobby", "LobbyPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			bg = (GImage)GetChildAt(0);
			EnterGame = (GButton)GetChildAt(1);
		}
	}
}
