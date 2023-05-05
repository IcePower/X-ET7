/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestB
{
	public partial class FUI_TestBPanel: GComponent
	{
		public ET.Client.Common.FUI_CommonBtn CloseBtn;
		public ET.Client.TestB.FUI_OneComponent Com1;
		public ET.Client.Common.FUI_TwoComponent Com2;
		public const string URL = "ui://296l7tjhlobh0";

		public static FUI_TestBPanel CreateInstance()
		{
			return (FUI_TestBPanel)UIPackage.CreateObject("TestB", "TestBPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			CloseBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(1);
			Com1 = (ET.Client.TestB.FUI_OneComponent)GetChildAt(2);
			Com2 = (ET.Client.Common.FUI_TwoComponent)GetChildAt(3);
		}
	}
}
