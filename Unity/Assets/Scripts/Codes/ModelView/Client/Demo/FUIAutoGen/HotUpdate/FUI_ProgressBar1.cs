/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.HotUpdate
{
	public partial class FUI_ProgressBar1: GProgressBar
	{
		public enum c1Page
		{
			a,
			b,
		}

		public Controller c1;
		public const string URL = "ui://2f8jqefflobh1";

		public static FUI_ProgressBar1 CreateInstance()
		{
			return (FUI_ProgressBar1)UIPackage.CreateObject("HotUpdate", "ProgressBar1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			c1 = GetControllerAt(0);
		}
	}
}
