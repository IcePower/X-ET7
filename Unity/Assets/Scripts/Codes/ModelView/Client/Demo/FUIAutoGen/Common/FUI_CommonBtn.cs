/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Common
{
	public partial class FUI_CommonBtn: GButton
	{
		public GLoader image;
		public const string URL = "ui://f2boiu4ilobhq";

		public static FUI_CommonBtn CreateInstance()
		{
			return (FUI_CommonBtn)UIPackage.CreateObject("Common", "CommonBtn");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			image = (GLoader)GetChildAt(0);
		}
	}
}
