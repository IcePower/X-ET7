using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
#if UNITY_EDITOR
        [AddComponentMenu("")]
        public sealed class Debugger : MonoBehaviour
        {
            private static Debugger m_Debugger;
            public static Debugger CreateDebugger(UIAssetManager manager)
            {
                if (m_Debugger == null)
                {
                    m_Debugger = new GameObject("UI Asset Manager").AddComponent<Debugger>();
                    DontDestroyOnLoad(m_Debugger.gameObject);
                }

                m_Debugger.m_Manager = manager;

                return m_Debugger;
            }

            public static void DestroyDebugger()
            {
                if (m_Debugger == null)
                    return;

                GameObject.Destroy(m_Debugger.gameObject);
            }

            public Dictionary<string, UIPackageInfo> GetUIPackageInfoDict()
            {
                return m_Manager?.m_DictUIPackageInfos;
            }

            private UIAssetManager m_Manager;
        }
#endif
    }
}