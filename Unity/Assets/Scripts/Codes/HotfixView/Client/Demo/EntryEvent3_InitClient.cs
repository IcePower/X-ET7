using System;
using System.IO;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<ET.EventType.EntryEvent3>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            // 加载配置
            
            Root.Instance.Scene.AddComponent<GlobalComponent>();
            
            Root.Instance.Scene.AddComponent<FsmDispatcherComponent>();

            Scene clientScene = await SceneFactory.CreateClientScene(1, "Game");

            FUIComponent fuiComponent = clientScene.GetComponent<FUIComponent>();

            // 热更流程
            await ResComponent.Instance.InitResourceAsync(clientScene);
            
            // 加载 Packages
            await FUIPackageLoader.LoadPackagesAsync(fuiComponent);
            
            await clientScene.GetComponent<FUIComponent>().ShowPanelAsync(PanelId.LoginPanel);

            await EventSystem.Instance.PublishAsync(clientScene, new EventType.AppStartInitFinish());
        }
    }
}