/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestA
{
	public partial class FUI_Button1: GButton
	{
		public GGraph n0;
		public GGraph n1;
		public GGraph n2;
		public GTextField Text1;
		public GTextField Text2;
		public const string URL = "ui://2kcjlx6nw6rg2";

		public static FUI_Button1 CreateInstance()
		{
			return (FUI_Button1)UIPackage.CreateObject("TestA", "Button1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GGraph)GetChildAt(0);
			n1 = (GGraph)GetChildAt(1);
			n2 = (GGraph)GetChildAt(2);
			Text1 = (GTextField)GetChildAt(3);
			Text2 = (GTextField)GetChildAt(4);
		}
	}
}
