/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestA
{
	public partial class FUI_TestAPanel: GComponent
	{
		public ET.Client.Common.FUI_CommonBtn HideBtn;
		public ET.Client.Common.FUI_CommonBtn OpenTestBBtn;
		public const string URL = "ui://2kcjlx6nlobh1";

		public static FUI_TestAPanel CreateInstance()
		{
			return (FUI_TestAPanel)UIPackage.CreateObject("TestA", "TestAPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			HideBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(1);
			OpenTestBBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(2);
		}
	}
}
