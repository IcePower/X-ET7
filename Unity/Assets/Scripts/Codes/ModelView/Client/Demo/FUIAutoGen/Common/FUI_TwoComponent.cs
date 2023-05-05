/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Common
{
	public partial class FUI_TwoComponent: GComponent
	{
		public GTextField title;
		public ET.Client.Common.FUI_CommonBtn OneBtn;
		public const string URL = "ui://f2boiu4iw6rg3";

		public static FUI_TwoComponent CreateInstance()
		{
			return (FUI_TwoComponent)UIPackage.CreateObject("Common", "TwoComponent");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(1);
			OneBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(2);
		}
	}
}
