using ET.Client.TestB;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class TestCPanel: Entity, IAwake
	{
		private FUI_TestCPanel _fuiTestCPanel;

		public FUI_TestCPanel FUITestCPanel
		{
			get => _fuiTestCPanel ??= (FUI_TestCPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
	
	[ChildOf]
	public class TestCPanel_ContextData: Entity, IAwake
	{
		/// <summary>
		/// 测试数据
		/// </summary>
		public string Data { get; set; }
	}
}
