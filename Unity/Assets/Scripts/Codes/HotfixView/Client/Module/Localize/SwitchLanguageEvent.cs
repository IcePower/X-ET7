using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class SwitchLanguageEvent : AEvent<Scene, SwitchLanguage>
    {
        protected override async ETTask Run(Scene scene, SwitchLanguage arg)
        {
            var (translateExcel, translateFUI) = scene.GetComponent<LocalizeComponent>().GetCurrentTranslator();
            
            ConfigComponent.Instance.TranslateText(translateExcel);
            
            scene.GetComponent<FUIComponent>().AllPanelTranslateText(arg.Language, translateFUI);
            
            await ETTask.CompletedTask;
        }
    }
}