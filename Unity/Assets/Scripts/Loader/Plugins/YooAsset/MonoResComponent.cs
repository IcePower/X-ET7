using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniFramework.Event;
using UniFramework.Module;
using UnityEngine;
using YooAsset;

namespace ET
{
    public class MonoResComponent: Singleton<MonoResComponent>
    {
        public async ETTask<bool> InitAsync(bool isUseEditorMode)
        {
            // 初始化BetterStreaming
            BetterStreamingAssets.Initialize();

            // 初始化事件系统
            UniEvent.Initalize();

            // 初始化管理系统
            UniModule.Initialize();

            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);

            await InitPackage(isUseEditorMode);

            return true;
        }

        private async ETTask InitPackage(bool isUseEditorMode)
        {
            // 如果是真机，先以离线模式初始化,等热更完成后再重新以联机运行模式初始化。如果是编辑器，可以在编辑器里选择是否使用 EditorSimulateMode。 
            EPlayMode playMode = isUseEditorMode ? YooAsset.EPlayMode.EditorSimulateMode : YooAsset.EPlayMode.OfflinePlayMode;
            // 创建默认的资源包
            string packageName = "DefaultPackage";
            var package = YooAssets.TryGetAssetsPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreateAssetsPackage(packageName);
                YooAssets.SetDefaultAssetsPackage(package);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulatePatchManifestPath = EditorSimulateModeHelper.SimulateBuild(packageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }
            else{
                var createParameters = new OfflinePlayModeParameters();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            await initializationOperation;
            
            if (package.InitializeStatus != EOperationStatus.Succeed)
            {
                Debug.LogError($"{initializationOperation.Error}");
            }
        }
    }
}
