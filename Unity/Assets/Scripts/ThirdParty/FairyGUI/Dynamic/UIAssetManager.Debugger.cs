using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
#if UNITY_EDITOR

        /// <summary>
        /// 获取UIPackage引用计数 仅供调试使用
        /// </summary>
        private void GetUIPackageRefCounts(Dictionary<string, int> buffer)
        {
            buffer.Clear();

            foreach (var pair in m_UIPackageRefs)
                buffer.Add(pair.Key, pair.Value.RefCount);
        }

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

            public Dictionary<string, int> GetUIPackageRefCounts()
            {
                m_Manager.GetUIPackageRefCounts(m_UIPackageRefCounts);
                return m_UIPackageRefCounts;
            }
            
            public void UnloadAllUIPackages()
            {
                m_Manager.UnloadAllUIPackages();
            }
            
            public void UnloadUnusedUIPackages()
            {
                m_Manager.UnloadUnusedUIPackages();
            }

            private UIAssetManager m_Manager;
            private readonly Dictionary<string, int> m_UIPackageRefCounts = new Dictionary<string, int>();
        }
#endif
    }
}