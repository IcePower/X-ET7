/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestA
{
	public partial class FUI_Button2: GButton
	{
		public GGraph n0;
		public GGraph n1;
		public GGraph n2;
		public const string URL = "ui://2kcjlx6nw6rg3";

		public static FUI_Button2 CreateInstance()
		{
			return (FUI_Button2)UIPackage.CreateObject("TestA", "Button2");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GGraph)GetChildAt(0);
			n1 = (GGraph)GetChildAt(1);
			n2 = (GGraph)GetChildAt(2);
		}
	}
}
