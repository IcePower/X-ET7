/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Test
{
	public partial class FUI_TestPanel: GComponent
	{
		public GImage bgg;
		public const string URL = "ui://k8mgj3munkb31";

		public static FUI_TestPanel CreateInstance()
		{
			return (FUI_TestPanel)UIPackage.CreateObject("Test", "TestPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			bgg = (GImage)GetChildAt(0);
		}
	}
}
