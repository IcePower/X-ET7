/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client
{
	public partial class FUI_LoginPanel: GComponent
	{
		public enum c1_Page
		{
			Page_State1,
			Page_State2,
		}

		public Controller c1;
		public Controller c2;
		public GButton LoginBtn;
		public FUI_InputField1 AccountInput;
		public FUI_InputField1 PasswordInput;
		public const string URL = "ui://rgfb0w498omm0";

		public static FUI_LoginPanel CreateInstance()
		{
			return (FUI_LoginPanel)UIPackage.CreateObject("Login", "LoginPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			c1 = GetControllerAt(0);
			c2 = GetControllerAt(1);
			LoginBtn = (GButton)GetChildAt(1);
			AccountInput = (FUI_InputField1)GetChildAt(2);
			PasswordInput = (FUI_InputField1)GetChildAt(3);
		}
	}
}
