using System;
using System.Text;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    public class LocalizeComponentAwakeSystem : AwakeSystem<LocalizeComponent>
    {
        protected override void Awake(LocalizeComponent self)
        {
            self.CurrentLanguage = LocalizeComponentSystem.DefaultLanguage;
        }
    }
    [FriendOf(typeof(LocalizeComponent))]
    public static class LocalizeComponentSystem
    {
        public const SystemLanguage DefaultLanguage = SystemLanguage.ChineseSimplified;
        
        public static bool SwitchLanguage(this LocalizeComponent self, SystemLanguage language)
        {
            if (self.CurrentLanguage == language)
            {
                return false;
            }
            
            self.CurrentLanguage = language;
            
            EventSystem.Instance.Publish(self.ClientScene(), new EventType.SwitchLanguage() {Language = language});

            return true;
        }

        public static (Func<string, string, string>, Func<string, string, string>) GetCurrentTranslator(this LocalizeComponent self)
        {
            switch (self.CurrentLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    return (self.Translate_Excel_CHS, self.Translate_FUI_CHS);
                
                case SystemLanguage.ChineseTraditional:
                    return (self.Translate_Excel_CHT, self.Translate_FUI_CHT);
                
                case SystemLanguage.English:
                default:
                    return (self.Translate_Excel_EN, self.Translate_FUI_EN);
            }
        } 

        private static string Translate_Excel_CHS(this LocalizeComponent self, string key, string originText)
        {
            return LocalizeConfig_Excel_CHSCategory.Instance.GetOrDefault(key)?.TextCHS ?? originText;
        }
        
        private static string Translate_Excel_CHT(this LocalizeComponent self, string key, string originText)
        {
            return LocalizeConfig_Excel_CHTCategory.Instance.GetOrDefault(key)?.TextCHT ?? originText;
        }
        
        private static string Translate_Excel_EN(this LocalizeComponent self, string key, string originText)
        {
            return LocalizeConfig_Excel_ENCategory.Instance.GetOrDefault(key)?.TextEN ?? originText;
        }
        
        private static string Translate_FUI_CHS(this LocalizeComponent self, string key, string originText)
        {
            string str = LocalizeConfig_UI_CHSCategory.Instance.GetOrDefault(key)?.TextCHS ?? originText;
            str = DecodeString(str);
            return str;
        }
        
        private static string Translate_FUI_CHT(this LocalizeComponent self, string key, string originText)
        {
            string str = LocalizeConfig_UI_CHTCategory.Instance.GetOrDefault(key)?.TextCHT ?? originText;
            str = DecodeString(str);
            return str;
        }
        
        private static string Translate_FUI_EN(this LocalizeComponent self, string key, string originText)
        {
            string str = LocalizeConfig_UI_ENCategory.Instance.GetOrDefault(key)?.TextEN ?? originText;
            str = DecodeString(str);
            return str;
        }

        private static string DecodeString(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            sb.Replace("\\n", "\n");
            sb.Replace("\\t", "\t");
            sb.Replace("\\r", "\r");
            return sb.ToString();
        }
    }
}