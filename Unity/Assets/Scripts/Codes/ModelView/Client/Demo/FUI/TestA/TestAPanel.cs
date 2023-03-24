using ET.Client.TestA;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class TestAPanel: Entity, IAwake
	{
		private FUI_TestAPanel _fuiTestAPanel;

		public FUI_TestAPanel FUITestAPanel
		{
			get => _fuiTestAPanel ??= (FUI_TestAPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
