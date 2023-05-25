using ET.Client.TestB;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class TestBPanel: Entity, IAwake
	{
		public OneComponent Com1 {get; set;}

		public TwoComponent Com2 {get; set;}

		private FUI_TestBPanel _fuiTestBPanel;

		public FUI_TestBPanel FUITestBPanel
		{
			get => _fuiTestBPanel ??= (FUI_TestBPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
	
	[ChildOf]
	public class TestBPanel_ContextData: Entity, IAwake
	{
		/// <summary>
		/// 测试数据
		/// </summary>
		public string Data { get; set; }
	}
}
