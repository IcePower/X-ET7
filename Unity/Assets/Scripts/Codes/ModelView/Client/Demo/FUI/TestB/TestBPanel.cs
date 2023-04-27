using ET.Client.TestB;
using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class TestBPanel: Entity, IAwake
	{
		private FUI_TestBPanel _fuiTestBPanel;

		public FUI_TestBPanel FUITestBPanel
		{
			get => _fuiTestBPanel ??= (FUI_TestBPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
