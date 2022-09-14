using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish a)
        {
            FUIComponent fuiComponent = scene.GetComponent<FUIComponent>();
            fuiComponent.HidePanel(PanelId.LoginPanel);
            await fuiComponent.ShowPanelAsync(PanelId.LobbyPanel);
        }
    }
}

