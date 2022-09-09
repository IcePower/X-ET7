using System;
using YooAsset;

namespace ET
{
    public class MonoResComponent: Singleton<MonoResComponent>
    {
        public async ETTask<bool> InitAsync()
        {
            YooAssets.EPlayMode playMode = Define.IsAsync ? YooAssets.EPlayMode.HostPlayMode : YooAssets.EPlayMode.EditorSimulateMode;

            ILocationServices locationServices = new AddressLocationServices();

            string cdnUrl = YooAssetHelper.GetCdnUrl();

            YooAssets.InitializeParameters parameters = playMode switch
            {
                YooAssets.EPlayMode.EditorSimulateMode => new YooAssets.EditorSimulateModeParameters()
                {
                    LocationServices = locationServices
                },
                YooAssets.EPlayMode.OfflinePlayMode => new YooAssets.OfflinePlayModeParameters()
                {
                    LocationServices = locationServices
                },
                YooAssets.EPlayMode.HostPlayMode => new YooAssets.HostPlayModeParameters()
                {
                    LocationServices = locationServices,
                    DecryptionServices = null,
                    ClearCacheWhenDirty = false,
                    DefaultHostServer = cdnUrl,
                    FallbackHostServer = cdnUrl
                },
                _ => throw new ArgumentOutOfRangeException(nameof(playMode), playMode, null)
            };

            InitializationOperation initOperation = YooAssets.InitializeAsync(parameters);
            await initOperation.GetAwaiter();

            if (initOperation.Status != EOperationStatus.Succeed)
            {
                return false;
            }

            return true;
        }
    }
}
