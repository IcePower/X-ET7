using ET.Client.Test;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class TestPanel: Entity, IAwake
	{
		private FUI_TestPanel _fuiTestPanel;

		public FUI_TestPanel FUITestPanel
		{
			get => _fuiTestPanel ??= (FUI_TestPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
