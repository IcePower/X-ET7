/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using System.Collections.Generic;

namespace FairyGUI.Dynamic
{
	public class UIPackageHelper : IUIPackageHelper
	{
		private static readonly Dictionary<string, string> IdToNameDict = new Dictionary<string, string>()
		{
			["f2boiu4i"] = "Common",
			["2f8jqeff"] = "HotUpdate",
			["9gkqq49y"] = "Icon1",
			["96tfczmn"] = "Icon2",
			["9cdyueiu"] = "Icon3",
			["9bg9r3vf"] = "Icon4",
			["ti3ka994"] = "Lobby",
			["rgfb0w49"] = "Login",
			["2kcjlx6n"] = "TestA",
			["296l7tjh"] = "TestB",
		};

		public string GetPackageNameById(string id)
		{
			if (IdToNameDict.TryGetValue(id, out var name))
			{
				return name;
			}

			return null;
		}
	}
}
