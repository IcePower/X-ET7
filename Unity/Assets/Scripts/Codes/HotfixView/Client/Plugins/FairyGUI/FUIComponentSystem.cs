using System;
using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    [ObjectSystem]
    public class FUIComponentAwakeSystem : AwakeSystem<FUIComponent>
    {
        protected override void Awake(FUIComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class FUIComponentDestroySystem : DestroySystem<FUIComponent>
    {
        protected override void Destroy(FUIComponent self)
        {
            self.Destroy();
        }
    }

    [FriendOf(typeof(PanelCoreData))]
    [FriendOf(typeof(FUIEventComponent))]
    [FriendOf(typeof(FUIEntity))]
    [FriendOf(typeof(FUIComponent))]
    public static class FUIComponentSystem
    {
        public static void Awake(this FUIComponent self)
        {
            self.AllPanelsDic?.Clear();
            self.VisiblePanelsDic?.Clear();
            self.VisiblePanelsQueue?.Clear();
            self.HidePanelsStack?.Clear();
        }
        
        public static void Destroy(this FUIComponent self)
        {
            self.CloseAllPanel();
            self.UIPackageLocations.Clear();
        }

        /// <summary>
        /// 窗口是否是正在显示的 
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <returns></returns>
        public static bool IsPanelVisible(this FUIComponent self, PanelId id)
        {
            return self.VisiblePanelsDic.ContainsKey((int)id);
        }

        /// <summary>
        /// 隐藏最新出现的窗口
        /// </summary>
        public static void HideLastPanel(this FUIComponent self)
        {
            if (self.VisiblePanelsQueue.Count <= 0)
            {
                return;
            }

            PanelId PanelId = self.VisiblePanelsQueue[self.VisiblePanelsQueue.Count - 1];
            if (!self.IsPanelVisible(PanelId))
            {
                return;
            }

            self.HidePanel(PanelId);
        }

        /// <summary>
        /// 彻底关闭最新出现的窗口
        /// </summary>
        public static void CloseLastPanel(this FUIComponent self)
        {
            if (self.VisiblePanelsQueue.Count <= 0)
            {
                return;
            }

            PanelId PanelId = self.VisiblePanelsQueue[self.VisiblePanelsQueue.Count - 1];
            if (!self.IsPanelVisible(PanelId))
            {
                return;
            }

            self.ClosePanel(PanelId);
        }

        public static void ShowPanel<T>(this FUIComponent self, PanelId prePanelId = PanelId.Invalid) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ShowPanel(panelId, prePanelId);
        }

        /// <summary>
        /// 现实Id指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="showData"></OtherParam>
        public static void ShowPanel(this FUIComponent self, PanelId id, PanelId prePanelId = PanelId.Invalid)
        {
            FUIEntity fuiEntity = self.ReadyToShowfuiEntity(id);
            if (null != fuiEntity)
            {
                self.RealShowPanel(fuiEntity, id, prePanelId);
            }
        }

        public static async ETTask ShowPanelAsync(this FUIComponent self, PanelId id, PanelId prePanelId = PanelId.Invalid)
        {
            try
            {
                FUIEntity fuiEntity = await self.ShowFUIEntityAsync(id);
                if (null != fuiEntity)
                {
                    self.RealShowPanel(fuiEntity, id, prePanelId);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask ShowPanelAsync<T>(this FUIComponent self, PanelId prePanelId = PanelId.Invalid) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            await self.ShowPanelAsync(panelId, prePanelId);
        }

        public static void HideAndShowPanelStack(this FUIComponent self, PanelId hidePanelId, PanelId showPanelId)
        {
            self.HidePanel(hidePanelId, true);
            self.ShowPanel(showPanelId, prePanelId: hidePanelId);
        }

        public static void HideAndShowPanelStack<T, K>(this FUIComponent self) where T : Entity where K : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            PanelId showPanelId = self.GetPanelIdByGeneric<K>();
            self.HideAndShowPanelStack(hidePanelId, showPanelId);
        }

        public static async ETTask HideAndShowPanelStackAsync(this FUIComponent self, PanelId hidePanelId, PanelId showPanelId)
        {
            self.HidePanel(hidePanelId, true);
            await self.ShowPanelAsync(showPanelId, prePanelId: hidePanelId);
        }

        public static async ETTask HideAndShowPanelStackAsync<T, K>(this FUIComponent self) where T : Entity where K : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            PanelId showPanelId = self.GetPanelIdByGeneric<K>();
            await self.HideAndShowPanelStackAsync(hidePanelId, showPanelId);
        }

        /// <summary>
        /// 隐藏ID指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="onComplete"></OtherParam>
        public static void HidePanel(this FUIComponent self, PanelId id, bool isPushToStack = false)
        {
            if (!self.CheckDirectlyHide(id))
            {
                Log.Warning($"检测关闭 panelId: {id} 失败！");
                return;
            }

            if (isPushToStack)
            {
                return;
            }

            if (self.HidePanelsStack.Count <= 0)
            {
                return;
            }

            PanelId prePanelId = self.HidePanelsStack.Pop();
            self.ShowPanel(prePanelId);
        }

        public static void HidePanel<T>(this FUIComponent self, bool isPushToStack = false) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.HidePanel(hidePanelId, isPushToStack);
        }

        /// <summary>
        /// 卸载指定的UI窗口实例
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        public static void UnLoadPanel(this FUIComponent self, PanelId id, bool isDispose = true)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(id);
            if (null == fuiEntity)
            {
                Log.Error($"FUIEntity PanelId {id} is null!!!");
                return;
            }

            FUIEventComponent.Instance.GetUIEventHandler(id).BeforeUnload(fuiEntity);
            if (fuiEntity.IsPreLoad)
            {
                fuiEntity.GComponent.Dispose();
                fuiEntity.GComponent = null;
            }

            if (isDispose)
            {
                self.AllPanelsDic.Remove((int)id);
                self.VisiblePanelsDic.Remove((int)id);
                self.VisiblePanelsQueue.Remove(id);
                fuiEntity?.Dispose();
            }
        }

        public static void UnLoadPanel<T>(this FUIComponent self) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.UnLoadPanel(hidePanelId);
        }

        private static FUIEntity ReadyToShowfuiEntity(this FUIComponent self, PanelId id)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(id);
            // 如果UI不存在开始实例化新的窗口
            if (null == fuiEntity)
            {
                fuiEntity = self.AddChild<FUIEntity>();
                fuiEntity.PanelId = id;
                self.LoadFUIEntity(fuiEntity);
            }

            if (!fuiEntity.IsPreLoad)
            {
                self.LoadFUIEntity(fuiEntity);
            }

            return fuiEntity;
        }

        private static async ETTask<FUIEntity> ShowFUIEntityAsync(this FUIComponent self, PanelId id)
        {
            CoroutineLock coroutineLock = null;
            try
            {
                coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoadingPanels, (int)id);
                
                FUIEntity fuiEntity = self.GetFUIEntity(id);
                // 如果UI不存在开始实例化新的窗口
                if (null == fuiEntity)
                {
                    fuiEntity = self.AddChild<FUIEntity>();
                    fuiEntity.PanelId = id;
                    await self.LoadFUIEntitysAsync(fuiEntity);
                }

                if (!fuiEntity.IsPreLoad)
                {
                    await self.LoadFUIEntitysAsync(fuiEntity);
                }
                
                return fuiEntity;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }

        private static FUIEntity GetFUIEntity(this FUIComponent self, PanelId id)
        {
            if (self.AllPanelsDic.ContainsKey((int)id))
            {
                return self.AllPanelsDic[(int)id];
            }

            return null;
        }

        public static T GetPanelLogic<T>(this FUIComponent self, bool isNeedShowState = false) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            if (null == fuiEntity)
            {
                Log.Warning($"{panelId} is not created!");
                return null;
            }

            if (!fuiEntity.IsPreLoad)
            {
                Log.Warning($"{panelId} is not loaded!");
                return null;
            }

            if (isNeedShowState)
            {
                if (!self.IsPanelVisible(panelId))
                {
                    Log.Warning($"{panelId} is need show state!");
                    return null;
                }
            }

            return fuiEntity.GetComponent<T>();
        }

        public static PanelId GetPanelIdByGeneric<T>(this FUIComponent self) where T : Entity
        {
            if (FUIEventComponent.Instance.PanelTypeInfoDict.TryGetValue(typeof(T).Name, out PanelInfo panelInfo))
            {
                return panelInfo.PanelId;
            }

            Log.Error($"{typeof(T).FullName} is not have any PanelId!");
            return PanelId.Invalid;
        }

        public static void ClosePanel(this FUIComponent self, PanelId PanelId)
        {
            if (!self.VisiblePanelsDic.ContainsKey((int)PanelId))
            {
                return;
            }

            self.HidePanel(PanelId);
            self.UnLoadPanel(PanelId);
            Log.Info("<color=magenta>## close panel without Pop ##</color>");
        }

        public static void ClosePanel<T>(this FUIComponent self) where T : Entity
        {
            PanelId hidePanelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(hidePanelId);
        }

        public static void CloseAllPanel(this FUIComponent self)
        {
            if (self.AllPanelsDic == null)
            {
                return;
            }

            foreach (KeyValuePair<int, FUIEntity> panel in self.AllPanelsDic)
            {
                FUIEntity fuiEntity = panel.Value;
                if (fuiEntity == null || fuiEntity.IsDisposed)
                {
                    continue;
                }

                self.HidePanel(fuiEntity.PanelId);
                self.UnLoadPanel(fuiEntity.PanelId, false);
                fuiEntity?.Dispose();
            }

            self.VisiblePanelsDic.Clear();
            self.AllPanelsDic.Clear();
            self.FUIEntitylistCached.Clear();
            self.VisiblePanelsQueue.Clear();
            self.HidePanelsStack.Clear();
        }

        public static void HideAllShownPanel(this FUIComponent self, bool includeFixed = false)
        {
            self.FUIEntitylistCached.Clear();
            foreach (KeyValuePair<int, FUIEntity> panel in self.VisiblePanelsDic)
            {
                if (panel.Value.PanelCoreData.panelType == UIPanelType.Fixed && !includeFixed)
                {
                    continue;
                }
                    
                if (panel.Value.IsDisposed)
                {
                    continue;
                }

                self.FUIEntitylistCached.Add((PanelId)panel.Key);
                panel.Value.GComponent.visible = false;
                FUIEventComponent.Instance.GetUIEventHandler(panel.Value.PanelId).OnHide(panel.Value);
            }

            if (self.FUIEntitylistCached.Count > 0)
            {
                for (int i = 0; i < self.FUIEntitylistCached.Count; i++)
                {
                    self.VisiblePanelsDic.Remove((int)self.FUIEntitylistCached[i]);
                }
            }

            self.VisiblePanelsQueue.Clear();
            self.HidePanelsStack.Clear();
        }

        private static void RealShowPanel(this FUIComponent self, FUIEntity fuiEntity, PanelId id, PanelId prePanelId = PanelId.Invalid)
        {
            if (fuiEntity.PanelCoreData.panelType == UIPanelType.PopUp)
            {
                self.VisiblePanelsQueue.Add(id);
            }

            fuiEntity.GComponent.visible = true;

            FUIEventComponent.Instance.GetUIEventHandler(id).OnShow(fuiEntity);

            self.VisiblePanelsDic[(int)id] = fuiEntity;
            if (prePanelId != PanelId.Invalid)
            {
                self.HidePanelsStack.Push(prePanelId);
            }

            Log.Info("<color=magenta>### current Navigation panel </color>{0}".Fmt(fuiEntity.PanelId));
        }

        private static bool CheckDirectlyHide(this FUIComponent self, PanelId id)
        {
            if (!self.VisiblePanelsDic.ContainsKey((int)id))
            {
                return false;
            }

            FUIEntity fuiEntity = self.VisiblePanelsDic[(int)id];
            if (fuiEntity != null && !fuiEntity.IsDisposed)
            {
                fuiEntity.GComponent.visible = false;
                FUIEventComponent.Instance.GetUIEventHandler(id).OnHide(fuiEntity);
            }

            self.VisiblePanelsDic.Remove((int)id);
            self.VisiblePanelsQueue.Remove(id);
            return true;
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        private static void LoadFUIEntity(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (!FUIEventComponent.Instance.PanelIdInfoDict.TryGetValue(fuiEntity.PanelId, out PanelInfo panelInfo))
            {
                Log.Error($"{fuiEntity.PanelId} panelInfo is not Exist!");
                return;
            }

            fuiEntity.GComponent = UIPackage.CreateObject(panelInfo.PackageName, panelInfo.ComponentName).asCom;

            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitPanelCoreData(fuiEntity);

            fuiEntity.SetRoot(FUIRootHelper.GetTargetRoot(fuiEntity.PanelCoreData.panelType));

            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitComponent(fuiEntity);
            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnRegisterUIEvent(fuiEntity);

            self.AllPanelsDic[(int)fuiEntity.PanelId] = fuiEntity;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private static async ETTask LoadFUIEntitysAsync(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (!FUIEventComponent.Instance.PanelIdInfoDict.TryGetValue(fuiEntity.PanelId, out PanelInfo panelInfo))
            {
                Log.Error($"{fuiEntity.PanelId} panelInfo is not Exist!");
                return;
            }

            fuiEntity.GComponent = await UIPackageHelper.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);

            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitPanelCoreData(fuiEntity);

            fuiEntity.SetRoot(FUIRootHelper.GetTargetRoot(fuiEntity.PanelCoreData.panelType));

            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnInitComponent(fuiEntity);
            FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId).OnRegisterUIEvent(fuiEntity);

            self.AllPanelsDic[(int)fuiEntity.PanelId] = fuiEntity;
        }
    }
}