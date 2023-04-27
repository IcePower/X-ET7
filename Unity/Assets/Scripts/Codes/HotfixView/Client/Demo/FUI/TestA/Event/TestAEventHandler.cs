using System;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(PanelCoreData))]
	[FriendOf(typeof(FUIEntity))]
	[FUIEvent(PanelId.TestAPanel, "TestA", "TestAPanel")]
	public class TestAEventHandler: IFUIEventHandler
	{
		public void OnInitPanelCoreData(FUIEntity fuiEntity)
		{
			fuiEntity.PanelCoreData.panelType = UIPanelType.Normal;
		}

		public void OnInitComponent(FUIEntity fuiEntity)
		{
			TestAPanel panel = fuiEntity.AddComponent<TestAPanel>();
			panel.Awake();
		}

		public void OnRegisterUIEvent(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestAPanel>().RegisterUIEvent();
		}

		public void OnShow(FUIEntity fuiEntity, Entity contextData = null)
		{
			fuiEntity.GetComponent<TestAPanel>().OnShow(contextData);
		}

		public void OnHide(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestAPanel>().OnHide();
		}

		public void BeforeUnload(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<TestAPanel>().BeforeUnload();
		}
	}
}