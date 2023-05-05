using ET.Client.Common;

namespace ET.Client
{
	[ChildOf]
	public class InputField1: Entity, IAwake<FUI_InputField1>
	{
		public FUI_InputField1 FUIInputField1 { get; set; }
	}
}
