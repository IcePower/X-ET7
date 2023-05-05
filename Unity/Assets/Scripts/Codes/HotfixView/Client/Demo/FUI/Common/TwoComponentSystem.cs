namespace ET.Client
{
	public class TwoComponentAwakeSystem : AwakeSystem<TwoComponent, ET.Client.Common.FUI_TwoComponent>
	{
		protected override void Awake(TwoComponent self, ET.Client.Common.FUI_TwoComponent fuiTwoComponent)
		{
			self.Awake(fuiTwoComponent);
		}
	}

	[FriendOf(typeof(TwoComponent))]
	public static class TwoComponentSystem
	{
		public static void Awake(this TwoComponent self, ET.Client.Common.FUI_TwoComponent fuiTwoComponent)
		{
			self.FUITwoComponent = fuiTwoComponent;
		}

		public static void RegisterUIEvent(this TwoComponent self)
		{
			self.FUITwoComponent.OneBtn.AddListner(() =>
			{
				Log.Info("TwoComponentSystem OneBtn Click");
			});
		}

		public static void OnShow(this TwoComponent self, Entity contextData = null)
		{
		}

		public static void OnHide(this TwoComponent self)
		{
		}

		public static void BeforeUnload(this TwoComponent self)
		{
		}
	}
}