using UnityEngine;

namespace ET
{
    [FsmNode]
    public class FsmPatchDone: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            Scene zoneScene = fsmComponent.GetParent<Scene>();
            zoneScene.RemoveComponent<FsmComponent>();

            await ETTask.CompletedTask;
        }
    }
}