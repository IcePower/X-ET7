namespace ET
{
    [FsmNode]
    public class FsmUpdateStaticVersion: AFsmNodeHandler
    {
        public override async ETTask OnEnter(FsmComponent fsmComponent)
        {
            int errorCode = await ResComponent.Instance.UpdateStaticVersionAsync();
            
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("FsmUpdateStaticVersion 出错！{0}".Fmt(errorCode));
                return;
            }
            
            fsmComponent.Transition(nameof(FsmUpdateManifest));
        }
    }
}