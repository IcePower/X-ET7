using ET.Client.Login;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class LoginPanel: Entity, IAwake
	{
		private FUI_LoginPanel _fuiLoginPanel;

		public FUI_LoginPanel FUILoginPanel
		{
			get => _fuiLoginPanel ??= (FUI_LoginPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
