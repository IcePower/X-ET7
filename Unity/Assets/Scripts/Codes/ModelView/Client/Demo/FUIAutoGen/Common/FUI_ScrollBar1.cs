/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Common
{
	public partial class FUI_ScrollBar1: GScrollBar
	{
		public GGraph n0;
		public const string URL = "ui://f2boiu4iab9en";

		public static FUI_ScrollBar1 CreateInstance()
		{
			return (FUI_ScrollBar1)UIPackage.CreateObject("Common", "ScrollBar1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GGraph)GetChildAt(0);
		}
	}
}
