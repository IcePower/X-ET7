using ET.Client.Common;

namespace ET.Client
{
	[ChildOf]
	public class TwoComponent: Entity, IAwake<FUI_TwoComponent>
	{
		public FUI_TwoComponent FUITwoComponent { get; set; }
	}
}
