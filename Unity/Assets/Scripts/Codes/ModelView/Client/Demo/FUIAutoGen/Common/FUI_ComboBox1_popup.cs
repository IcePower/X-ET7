/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Common
{
	public partial class FUI_ComboBox1_popup: GComponent
	{
		public GList list;
		public const string URL = "ui://f2boiu4iab9ei";

		public static FUI_ComboBox1_popup CreateInstance()
		{
			return (FUI_ComboBox1_popup)UIPackage.CreateObject("Common", "ComboBox1_popup");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(1);
		}
	}
}
