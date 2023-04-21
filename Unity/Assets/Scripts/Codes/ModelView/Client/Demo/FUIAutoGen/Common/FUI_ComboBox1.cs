/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.Common
{
	public partial class FUI_ComboBox1: GComboBox
	{
		public GGraph n0;
		public GGraph n1;
		public GGraph n2;
		public GLoader TestLoader;
		public const string URL = "ui://f2boiu4iab9ej";

		public static FUI_ComboBox1 CreateInstance()
		{
			return (FUI_ComboBox1)UIPackage.CreateObject("Common", "ComboBox1");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GGraph)GetChildAt(0);
			n1 = (GGraph)GetChildAt(1);
			n2 = (GGraph)GetChildAt(2);
			TestLoader = (GLoader)GetChildAt(4);
		}
	}
}
