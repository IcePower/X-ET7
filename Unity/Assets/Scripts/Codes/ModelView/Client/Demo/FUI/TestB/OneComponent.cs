using ET.Client.TestB;

namespace ET.Client
{
	[ChildOf]
	public class OneComponent: Entity, IAwake<FUI_OneComponent>
	{
		public TwoComponent TwoCom {get; set;}
		public FUI_OneComponent FUIOneComponent { get; set; }
	}
}
