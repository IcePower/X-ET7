using System;
using UnityEngine;

namespace ET.Client
{
	public static class TestBPanelSystem
	{
		public static void Awake(this TestBPanel self)
		{

		}

		public static void RegisterUIEvent(this TestBPanel self)
		{
			self.FUITestBPanel.CloseBtn.AddListner(() =>
			{
				self.ClientScene().GetComponent<FUIComponent>().ClosePanel(PanelId.TestBPanel);
			});
		}

		public static void OnShow(this TestBPanel self, Entity contextData = null)
		{

		}

		public static void OnHide(this TestBPanel self)
		{

		}

		public static void BeforeUnload(this TestBPanel self)
		{

		}
	}
}