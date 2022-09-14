using System;
using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    public static class FUIHelper
    {
        public static void AddListnerAsync(this GObject self, Func<ETTask> action)
        {
            async ETTask ClickActionAsync()
            {
                FUIEventComponent.Instance.isClicked = true;
                await action();
                FUIEventComponent.Instance.isClicked = false;
            }

            self.onClick.Set(() =>
            {
                if (FUIEventComponent.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync().Coroutine();
            });
        }

        public static void AddListner(this GObject self, Action action)
        {
            self.onClick.Set(() =>
            {
                action?.Invoke();
            });
        }

        public static void AddListnerAsync(this GObject self, Func<EventContext, ETTask> action)
        {
            async ETTask ClickActionAsync(EventContext context)
            {
                FUIEventComponent.Instance.isClicked = true;
                await action(context);
                FUIEventComponent.Instance.isClicked = false;
            }

            self.onClick.Set((context) =>
            {
                if (FUIEventComponent.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync(context).Coroutine();
            });
        }

        public static void AddListner(this GObject self, Action<EventContext> action)
        {
            self.onClick.Set((contex) =>
            {
                action?.Invoke(contex);
            });
        }
    }
}