using System.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<ET.EventType.EntryEvent3>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            Root.Instance.Scene.AddComponent<GlobalComponent>();
            
            Root.Instance.Scene.AddComponent<FsmDispatcherComponent>();

            Scene clientScene = await SceneFactory.CreateClientScene(1, "Game");

            // 预加载Packages
            await PreloadPackagesAsync(clientScene);

            // 热更流程
            await HotUpdateAsync(clientScene);
        }
        
        /// <summary>
        /// 预加载Packages
        /// </summary>
        /// <param name="fuiComponent"></param>
        private static async ETTask PreloadPackagesAsync(Scene clientScene)
        {
            FUIComponent fuiComponent = clientScene.GetComponent<FUIComponent>();

            // await fuiComponent.AddPackageAsync("Common");
            
            CommonBinder.BindAll();
        }

        private static async ETTask HotUpdateAsync(Scene clientScene)
        {
            FUIComponent fuiComponent = clientScene.GetComponent<FUIComponent>();

            // 打开热更界面
            await fuiComponent.ShowPanelAsync(PanelId.HotUpdatePanel);

            // await TimerComponent.Instance.WaitAsync(3000);
            
            // 更新版本号
            int errorCode = await ResComponent.Instance.UpdateVersionAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("FsmUpdateStaticVersion 出错！{0}".Fmt(errorCode));
                return;
            }
            
            // 更新Manifest
            errorCode = await ResComponent.Instance.UpdateManifestAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.UpdateManifest 出错！{0}".Fmt(errorCode));
                return;
            }
            
            // 创建下载器
            errorCode = ResComponent.Instance.CreateDownloader();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.FsmCreateDownloader 出错！{0}".Fmt(errorCode));
                return;
            }
            
            // Downloader不为空，说明有需要下载的资源
            if (ResComponent.Instance.Downloader != null)
            {
                await DownloadPatch(clientScene);
            }
            else
            {
                await EnterGame(fuiComponent);
            }
        }
        
        private static async ETTask DownloadPatch(Scene clientScene)
        {
            // 下载资源
            Log.Info("Count: {0}, Bytes: {1}".Fmt(ResComponent.Instance.Downloader.TotalDownloadCount, ResComponent.Instance.Downloader.TotalDownloadBytes));
            int errorCode = await ResComponent.Instance.DonwloadWebFilesAsync(
                // 开始下载回调
                null,
                
                // 下载进度回调
                (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
                {
                    // 更新进度条
                    EventSystem.Instance.Publish(clientScene, new EventType.OnPatchDownloadProgress() { TotalDownloadCount = totalDownloadCount, CurrentDownloadCount = currentDownloadCount, TotalDownloadSizeBytes = totalDownloadBytes, CurrentDownloadSizeBytes = currentDownloadBytes });
                },
                
                // 下载失败回调
                (fileName, error) =>
                {
                    // 下载失败
                    EventSystem.Instance.Publish(clientScene, new EventType.OnPatchDownlodFailed() { FileName = fileName, Error = error });
                },
                
                // 下载完成回调
                null);

            if (errorCode != ErrorCode.ERR_Success)
            {
                // todo: 弹出错误提示，确定后重试。
                Log.Error("ResourceComponent.FsmDonwloadWebFiles 出错！{0}".Fmt(errorCode));
                return;
            }

            int modelVersion = GlobalConfig.Instance.ModelVersion;
            int hotFixVersion = GlobalConfig.Instance.HotFixVersion;
            await MonoResComponent.Instance.RestartAsync();
            bool modelChanged = modelVersion != GlobalConfig.Instance.ModelVersion;
            bool hotfixChanged = hotFixVersion != GlobalConfig.Instance.HotFixVersion;

            if (modelChanged || hotfixChanged)
            {
                // 如果dll文件有更新，则需要重启。
                GameObject.Find("Global").GetComponent<Init>().Restart();
            }
            else
            {
                // 只是资源更新就直接进入游戏。
                await EnterGame(clientScene.GetComponent<FUIComponent>());
            }
        }
        
        private static async ETTask EnterGame(FUIComponent fuiComponent)
        {
            // 关闭热更界面
            fuiComponent.ClosePanel(PanelId.HotUpdatePanel);

            // 打开登陆界面
            LoginPanel_ContextData contextData = fuiComponent.AddChild<LoginPanel_ContextData>();
            contextData.Data = "界面参数测试";
            // 显示登录界面, 并传递参数contextData
            await fuiComponent.ShowPanelAsync(PanelId.LoginPanel, contextData);
        }
    }
}