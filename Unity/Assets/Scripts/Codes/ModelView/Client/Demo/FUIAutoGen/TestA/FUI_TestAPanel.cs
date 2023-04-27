/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.TestA
{
	public partial class FUI_TestAPanel: GComponent
	{
		public Controller c1;
		public GTextField n1;
		public ET.Client.Common.FUI_CommonBtn HideBtn;
		public ET.Client.Common.FUI_CommonBtn OpenTestBBtn;
		public ET.Client.Common.FUI_ComboBox1 LanguageCombo;
		public ET.Client.Common.FUI_Label1 n7;
		public ET.Client.Common.FUI_ProgressBar1 n8;
		public GTextField n9;
		public GTextField n10;
		public GTextField n12;
		public GTextField n13;
		public GTextField n14;
		public ET.Client.TestA.FUI_Button1 n15;
		public GButton n19;
		public GButton n18;
		public GRichTextField n23;
		public const string URL = "ui://2kcjlx6nlobh1";

		public static FUI_TestAPanel CreateInstance()
		{
			return (FUI_TestAPanel)UIPackage.CreateObject("TestA", "TestAPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			c1 = GetControllerAt(0);
			n1 = (GTextField)GetChildAt(0);
			HideBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(1);
			OpenTestBBtn = (ET.Client.Common.FUI_CommonBtn)GetChildAt(2);
			LanguageCombo = (ET.Client.Common.FUI_ComboBox1)GetChildAt(3);
			n7 = (ET.Client.Common.FUI_Label1)GetChildAt(4);
			n8 = (ET.Client.Common.FUI_ProgressBar1)GetChildAt(5);
			n9 = (GTextField)GetChildAt(6);
			n10 = (GTextField)GetChildAt(7);
			n12 = (GTextField)GetChildAt(8);
			n13 = (GTextField)GetChildAt(9);
			n14 = (GTextField)GetChildAt(10);
			n15 = (ET.Client.TestA.FUI_Button1)GetChildAt(11);
			n19 = (GButton)GetChildAt(12);
			n18 = (GButton)GetChildAt(13);
			n23 = (GRichTextField)GetChildAt(14);
		}
	}
}
