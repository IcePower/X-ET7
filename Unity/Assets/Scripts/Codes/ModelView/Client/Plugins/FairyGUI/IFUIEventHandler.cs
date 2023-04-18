namespace ET.Client
{
    public interface IFUIEventHandler
    {
        /// <summary>
        /// UI实体加载后,初始化窗口数据
        /// </summary>
        /// <param name="fuiEntity"></param>
        void OnInitPanelCoreData(FUIEntity fuiEntity);
        
        /// <summary>
        /// UI实体加载后，初始化业务逻辑数据
        /// </summary>
        /// <param name="fuiEntity"></param>
        void OnInitComponent(FUIEntity fuiEntity);
        
        /// <summary>
        /// 注册UI业务逻辑事件
        /// </summary>
        /// <param name="fuiEntity"></param>
        void OnRegisterUIEvent(FUIEntity fuiEntity);

        /// <summary>
        /// 打开UI窗口的业务逻辑
        /// </summary>
        /// <param name="fuiEntity"></param>
        /// <param name="contextData"></param>
        void OnShow(FUIEntity fuiEntity, Entity contextData = null);
        
        /// <summary>
        /// 隐藏UI窗口的业务逻辑
        /// </summary>
        /// <param name="fuiEntity"></param>
        void OnHide(FUIEntity fuiEntity);

        /// <summary>
        /// 完全关闭销毁UI窗口之前的业务逻辑，用于完全释放UI相关对象
        /// </summary>
        /// <param name="fuiEntity"></param>
        void BeforeUnload(FUIEntity fuiEntity);
    }
}