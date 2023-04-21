/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestB
{
	public partial class FUI_TestBPanel: GComponent
	{
		public GTextField n0;
		public ET.Client.Common.FUI_CommonBtn CloseBtn;
		public const string URL = "ui://296l7tjhlobh0";

		public static FUI_TestBPanel CreateInstance()
		{
			return (FUI_TestBPanel)UIPackage.CreateObject("TestB", "TestBPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GTextField)GetChildAt(0);
			CloseBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(1);
		}
	}
}
