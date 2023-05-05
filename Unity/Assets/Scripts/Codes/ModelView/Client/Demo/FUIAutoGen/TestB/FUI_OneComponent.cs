/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestB
{
	public partial class FUI_OneComponent: GComponent
	{
		public GTextField title;
		public GButton OneBtn;
		public ET.Client.Common.FUI_TwoComponent TwoCom;
		public const string URL = "ui://296l7tjhw6rg1";

		public static FUI_OneComponent CreateInstance()
		{
			return (FUI_OneComponent)UIPackage.CreateObject("TestB", "OneComponent");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextField)GetChildAt(1);
			OneBtn = (GButton)GetChildAt(2);
			TwoCom = (ET.Client.Common.FUI_TwoComponent)GetChildAt(3);
		}
	}
}
