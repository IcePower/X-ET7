/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestB
{
	public partial class FUI_TestCPanel: GComponent
	{
		public ET.Client.Common.FUI_CommonBtn CloseBtn;
		public GLoader Loader1;
		public GLoader Loader2;
		public const string URL = "ui://296l7tjhkhvd3";

		public static FUI_TestCPanel CreateInstance()
		{
			return (FUI_TestCPanel)UIPackage.CreateObject("TestB", "TestCPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			CloseBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(1);
			Loader1 = (GLoader)GetChildAt(2);
			Loader2 = (GLoader)GetChildAt(3);
		}
	}
}
