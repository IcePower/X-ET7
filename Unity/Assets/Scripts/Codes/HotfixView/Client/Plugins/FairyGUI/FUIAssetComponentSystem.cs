using System.Collections.Generic;
using FairyGUI;
using FairyGUI.Dynamic;
using UnityEngine;

namespace ET.Client
{
    [ObjectSystem]
    public class FUIAssetComponentAwakeSystem: AwakeSystem<FUIAssetComponent>
    {
        protected override void Awake(FUIAssetComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class FUIAssetComponentDestroySystem: DestroySystem<FUIAssetComponent>
    {
        protected override void Destroy(FUIAssetComponent self)
        {
            self.Destroy();
        }
    }

    [FriendOf(typeof (FUIAssetComponent))]
    public static class FUIAssetComponentSystem
    {
        public static void Awake(this FUIAssetComponent self)
        {
            void LoadUIPackageAsyncHandler(string packageName, LoadUIPackageCallback callback)
            {
                self.LoadUIPackageAsyncInner(packageName, callback).Coroutine();
            }

            void LoadTextureAsyncHandler(string packageName, string assetName, string extension, LoadTextureCallback callback)
            {
                self.LoadTextureAsyncInner(assetName, callback).Coroutine();
            }

            void ReleaseTextureHandler(Texture texture)
            {
                self.ReleaseAssetInner(texture);
            }

            void LoadAudioClipAsyncHandler(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
            {
                self.LoadAudioClipAsyncInner(assetName, callback).Coroutine();
            }

            void ReleaseAudioClipHandler(AudioClip audioClip)
            {
                self.ReleaseAssetInner(audioClip);
            }

            self.Locations = new Dictionary<int, string>();
            self.UIAssetLoader = new DelegateUIAssetLoader(LoadUIPackageAsyncHandler, LoadTextureAsyncHandler, ReleaseTextureHandler, LoadAudioClipAsyncHandler, ReleaseAudioClipHandler);
            self.UIAssetManager = new UIAssetManager(self.UIAssetLoader, null);
        }

        public static void Destroy(this FUIAssetComponent self)
        {
            self.UIAssetManager.Dispose();
            self.UIAssetManager = null;
            self.UIAssetLoader = null;

            if (ResComponent.Instance != null && !ResComponent.Instance.IsDisposed)
            {
                foreach (string location in self.Locations.Values)
                {
                    ResComponent.Instance.UnloadAsset(location);
                }
            }

            self.Locations.Clear();
        }

        public static ETTask<UIPackage> LoadUIPackageAsync(this FUIAssetComponent self, string packageName)
        {
            ETTask<UIPackage> task = ETTask<UIPackage>.Create(true);
            self.UIAssetManager.LoadUIPackageAsync(packageName, package => { task.SetResult(package); });

            return task;
        }

        private static async ETTask LoadUIPackageAsyncInner(this FUIAssetComponent self, string packageName, LoadUIPackageCallback callback)
        {
            string location = "{0}{1}".Fmt(packageName, "_fui");
            byte[] descData = await ResComponent.Instance.LoadRawFileDataAsync(location);
            callback(descData, packageName);
        }

        private static async ETTask LoadTextureAsyncInner(this FUIAssetComponent self, string assetName, LoadTextureCallback callback)
        {
            Texture res = await ResComponent.Instance.LoadAssetAsync<Texture>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static async ETTask LoadAudioClipAsyncInner(this FUIAssetComponent self, string assetName, LoadAudioClipCallback callback)
        {
            AudioClip res = await ResComponent.Instance.LoadAssetAsync<AudioClip>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static void ReleaseAssetInner(this FUIAssetComponent self, UnityEngine.Object texture)
        {
            if (texture == null)
                return;

            int instanceId = texture.GetInstanceID();
            if (!self.Locations.TryGetValue(instanceId, out string location))
                return;

            self.Locations.Remove(instanceId);

            if (ResComponent.Instance != null && !ResComponent.Instance.IsDisposed)
            {
                ResComponent.Instance.UnloadAsset(location);
            }
        }
    }
}