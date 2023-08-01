using System;
using System.Collections.Generic;
using FairyGUI;
using FairyGUI.Dynamic;
using UnityEngine;

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

            FUIBinder.BindAll();
        }
        
        public static void Destroy(this FUIComponent self)
        {
            self.CloseAllPanel();
        }
        
        public static void Restart(this FUIComponent self)
        {
            self.CloseAllPanel();
            
            FUIBinder.BindAll();
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

        private static async ETTask ShowPanelStackAsync(this FUIComponent self, PanelId id, Entity contextData = null)
        {
            FUIEntity fuiEntity = await self.ShowFUIEntityAsync(id);
            if (fuiEntity == null)
            {
                return;
            }

            self.ShowPanelAsync(id, contextData).Coroutine();
        }
        
        /// <summary>
        /// 显示界面。没有 contextData 的重载
        /// </summary>
        public static async ETTask ShowPanelAsync(this FUIComponent self, PanelId id)
        {
            try
            {
                FUIEntity fuiEntity = await self.ShowFUIEntityAsync(id);
                if (fuiEntity != null)
                {
                    self.RealShowPanel(fuiEntity, id);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        
        /// <summary>
        /// 显示界面
        /// </summary>
        public static async ETTask ShowPanelAsync<T>(this FUIComponent self, PanelId id, T contextData) where T: Entity
        {
            try
            {
                FUIEntity fuiEntity = await self.ShowFUIEntityAsync(id);
                if (fuiEntity == null)
                {
                    return;
                }

                if ((Entity)fuiEntity.ContextData != null)
                {
                    ((Entity)fuiEntity.ContextData).Dispose();
                }

                if (contextData != null)
                {
                    fuiEntity.AddChild(contextData);
                    fuiEntity.ContextData = contextData;
                }
                
                self.RealShowPanel(fuiEntity, id, contextData);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 隐藏 hidePanelId 界面，然后显示 showPanelId 界面。并将 hidePanelId 界面压入栈中。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hidePanelId">要隐藏的界面Id</param>
        /// <param name="showPanelId">要显示的界面Id</param>
        /// <param name="contextData">界面参数</param>
        public static async ETTask HideAndShowPanelStackAsync(this FUIComponent self, PanelId hidePanelId, PanelId showPanelId, Entity contextData = null)
        {
            // 隐藏 hidePanelId
            if (!self.CheckDirectlyHide(hidePanelId))
            {
                Log.Warning($"检测关闭 panelId: {hidePanelId} 失败！");
            }
            
            // 显示 showPanelId
            await self.ShowPanelAsync(showPanelId, contextData);
            
            FUIEntity fuiEntity = self.GetFUIEntity(showPanelId);
            if (fuiEntity == null)
            {
                Log.Error($"界面 {showPanelId} 创建失败！");
                return;
            }
            
            // 将 hidePanelId 界面压入栈中
            if (hidePanelId != PanelId.Invalid)
            {
                self.HidePanelsStack.Push(hidePanelId);
                fuiEntity.IsUsingStack = true;
            }
        }

        /// <summary>
        /// 隐藏ID指定的UI窗口。如果之前使用 HideAndShowPanelStackAsync() 显示，则调用 HideAndPopPanelStack()，否则调用 CheckDirectlyHide()。
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        public static void HidePanel(this FUIComponent self, PanelId id, Entity contextData = null)
        {
            if (!self.VisiblePanelsDic.TryGetValue((int)id, out FUIEntity fuiEntity))
            {
                return;
            }

            if (fuiEntity.IsUsingStack)
            {
                self.HideAndPopPanelStack(id, contextData);
            }
            else
            {
                self.CheckDirectlyHide(id);
            }
        }
        
        private static void HideAndPopPanelStack(this FUIComponent self, PanelId id, Entity contextData = null)
        {
            if (!self.CheckDirectlyHide(id))
            {
                Log.Warning($"检测关闭 panelId: {id} 失败！");
                return;
            }

            if (self.HidePanelsStack.Count <= 0)
            {
                return;
            }
        
            PanelId prePanelId = self.HidePanelsStack.Pop();
            self.ShowPanelStackAsync(prePanelId, contextData).Coroutine();
        }

        /// <summary>
        /// 卸载指定的UI窗口实例
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        private static void UnLoadPanel(this FUIComponent self, PanelId id, bool isDispose = true)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(id);
            if (null == fuiEntity)
            {
                Log.Error($"FUIEntity PanelId {id} is null!!!");
                return;
            }

            IFUIEventHandler handler = FUIEventComponent.Instance.GetUIEventHandler(id);
            handler.BeforeUnload(fuiEntity);
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
                    fuiEntity = self.AddChild<FUIEntity>(true);
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
            if (self.AllPanelsDic.TryGetValue((int)id, out FUIEntity fuiEntity))
            {
                return fuiEntity;
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

        /// <summary>
        /// 关闭指定的UI窗口，会Unload资源。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="PanelId"></param>
        public static void ClosePanel(this FUIComponent self, PanelId PanelId, Entity contextData = null)
        {
            if (!self.VisiblePanelsDic.ContainsKey((int)PanelId))
            {
                return;
            }

            self.HidePanel(PanelId, contextData);
            self.UnLoadPanel(PanelId);
            Log.Info("<color=magenta>## close panel without Pop ##</color>");
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

                self.CheckDirectlyHide(fuiEntity.PanelId);
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

        private static void RealShowPanel(this FUIComponent self, FUIEntity fuiEntity, PanelId id, Entity contextData = null)
        {
            if (fuiEntity.PanelCoreData.panelType == UIPanelType.PopUp)
            {
                self.VisiblePanelsQueue.Add(id);
            }

            fuiEntity.GComponent.visible = true;

            self.VisiblePanelsDic[(int)id] = fuiEntity;

            FUIEventComponent.Instance.GetUIEventHandler(id).OnShow(fuiEntity, contextData);

            Log.Info("<color=magenta>### current Navigation panel </color>{0}".Fmt(fuiEntity.PanelId));
        }

        private static bool CheckDirectlyHide(this FUIComponent self, PanelId id)
        {
            if (!self.VisiblePanelsDic.TryGetValue((int)id, out FUIEntity fuiEntity))
            {
                return false;
            }
 
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
        /// 异步加载
        /// </summary>
        private static async ETTask LoadFUIEntitysAsync(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (!FUIEventComponent.Instance.PanelIdInfoDict.TryGetValue(fuiEntity.PanelId, out PanelInfo panelInfo))
            {
                Log.Error($"{fuiEntity.PanelId} panelInfo is not Exist!");
                return;
            }
            
            IFUIEventHandler fuiEventHandler = FUIEventComponent.Instance.GetUIEventHandler(fuiEntity.PanelId);
            
            // 创建组件
            fuiEntity.GComponent = await self.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);

            fuiEventHandler.OnInitPanelCoreData(fuiEntity);

            // 设置根节点
            fuiEntity.SetRoot(FUIRootHelper.GetTargetRoot(fuiEntity.PanelCoreData.panelType));

            fuiEventHandler.OnInitComponent(fuiEntity);
            fuiEventHandler.OnRegisterUIEvent(fuiEntity);
            
            // 翻译打开的界面
            SystemLanguage currentLanguage = self.ClientScene().GetComponent<LocalizeComponent>().CurrentLanguage;
            if (fuiEntity.Language != currentLanguage)
            {
                var (_, translateFUI) = self.ClientScene().GetComponent<LocalizeComponent>().GetCurrentTranslator();
                self.OnePanelTranslateText(currentLanguage, fuiEntity, translateFUI);
            }
           
            self.AllPanelsDic[(int)fuiEntity.PanelId] = fuiEntity;
        }

        private static async ETTask<GComponent> CreateObjectAsync(this FUIComponent self, string packageName, string componentName)
        {
            return (await self.ClientScene().GetComponent<FUIAssetComponent>().CreateObjectAsync(packageName, componentName)).asCom;
        }

        public static void AllPanelTranslateText(this FUIComponent self, SystemLanguage currentLanguage, Func<string, string, string> translator)
        {
            foreach (KeyValuePair<int, FUIEntity> kv in self.AllPanelsDic)
            {
                self.OnePanelTranslateText(currentLanguage, kv.Value, translator);
            }
        }
        
        private static void OnePanelTranslateText(this FUIComponent self, SystemLanguage currentLanguage, FUIEntity fuiEntity, Func<string, string, string> translator)
        {
                if (fuiEntity == null || fuiEntity.IsDisposed)
                {
                    return;
                }

                self.TranslateTextField(fuiEntity.GComponent, translator);
                self.TranslateComponent(fuiEntity.GComponent, translator);

                fuiEntity.Language = currentLanguage;
        }

        private static void TranslateTextField(this FUIComponent self, GComponent component, Func<string, string, string> translator)
        {
            int n = component.numChildren;
            for (int i = 0; i < n; i++)
            {
                GObject child = component.GetChildAt(i);
                switch (child)
                {
                    case GTextField textField:
                        string key = $"{textField.parent.resourceURL[5..]}-{textField.id}";
                        textField.text = translator(key, textField.text);
                        break;
                    case GComponent subComponent:
                        self.TranslateTextField(subComponent, translator);
                        break;
                }
            }
        }
        
        private static void TranslateComponent(this FUIComponent self, GComponent component, Func<string, string, string> translator)
        {
            int n = component.numChildren;
            for (int i = 0; i < n; i++)
            {
                GObject child = component.GetChildAt(i);
                switch (child)
                {
                    case GButton button:
                        if (button.parent?.resourceURL != null)
                        {
                            string key = $"{button.parent.resourceURL[5..]}-{button.id}";
                            button.title = translator(key, button.title);
                            button.selectedTitle = translator($"{key}-0", button.title);
                        }
                        break;
                    case GLabel label:
                        if (label.parent?.resourceURL != null)
                        {
                            string key1 = $"{label.parent.resourceURL[5..]}-{label.id}";
                            label.title = translator(key1, label.title);
                        }
                       
                        break;
                    case GComponent subComponent:
                        if (subComponent.resourceURL != null)
                        {
                            self.TranslateComponent(subComponent, translator);
                        }
                        break;
                }
            }
        }
    }
}