namespace ET.Client
{
	public class OneComponentAwakeSystem : AwakeSystem<OneComponent, ET.Client.TestB.FUI_OneComponent>
	{
		protected override void Awake(OneComponent self, ET.Client.TestB.FUI_OneComponent fuiOneComponent)
		{
			self.Awake(fuiOneComponent);
		}
	}

	[FriendOf(typeof(OneComponent))]
	public static class OneComponentSystem
	{
		public static void Awake(this OneComponent self, ET.Client.TestB.FUI_OneComponent fuiOneComponent)
		{
			self.FUIOneComponent = fuiOneComponent;
			self.TwoCom = self.AddChild<TwoComponent, ET.Client.Common.FUI_TwoComponent>(self.FUIOneComponent.TwoCom, true);
		}

		public static void RegisterUIEvent(this OneComponent self)
		{
			self.TwoCom.RegisterUIEvent();
		}

		public static void OnShow(this OneComponent self, Entity contextData = null)
		{
			self.TwoCom.OnShow();
		}

		public static void OnHide(this OneComponent self)
		{
			self.TwoCom.OnHide();
		}

		public static void BeforeUnload(this OneComponent self)
		{
			self.TwoCom.BeforeUnload();
		}
	}
}