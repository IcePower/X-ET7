/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.HotUpdate
{
	public partial class FUI_HotUpdatePanel: GComponent
	{
		public ET.Client.HotUpdate.FUI_ProgressBar1 ProgressBar;
		public const string URL = "ui://2f8jqefflobh0";

		public static FUI_HotUpdatePanel CreateInstance()
		{
			return (FUI_HotUpdatePanel)UIPackage.CreateObject("HotUpdate", "HotUpdatePanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			ProgressBar = (ET.Client.HotUpdate.FUI_ProgressBar1)GetChildAt(1);
		}
	}
}
