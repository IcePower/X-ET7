namespace ET.Client
{
	[FriendOf(typeof(PanelCoreData))]
	[FriendOf(typeof(FUIEntity))]
	[FUIEvent(PanelId.TestPanel, "Test", "TestPanel")]
	public class TestEventHandler: IFUIEventHandler
	{
		public void OnInitPanelCoreData(FUIEntity fuiEntity)
		{
			fuiEntity.PanelCoreData.panelType = UIPanelType.Normal;
		}

		public void OnInitComponent(FUIEntity fuiEntity)
		{
			TestPanel panel = fuiEntity.AddComponent<TestPanel>();
			panel.Awake();
		}

		public void OnRegisterUIEvent(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestPanel>().RegisterUIEvent();
		}

		public void OnShow(FUIEntity fuiEntity, Entity contexData = null)
		{
			fuiEntity.GetComponent<TestPanel>().OnShow(contexData);
		}

		public void OnHide(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestPanel>().OnHide();
		}

		public void BeforeUnload(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestPanel>().BeforeUnload();
		}
	}
}