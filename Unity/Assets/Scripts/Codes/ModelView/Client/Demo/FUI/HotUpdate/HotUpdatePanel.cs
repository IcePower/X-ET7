using ET.Client.HotUpdate;
using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class HotUpdatePanel: Entity, IAwake
	{
		private FUI_HotUpdatePanel _fuiHotUpdatePanel;

		public FUI_HotUpdatePanel FUIHotUpdatePanel
		{
			get => _fuiHotUpdatePanel ??= (FUI_HotUpdatePanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
