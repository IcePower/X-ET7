using System;
using System.Collections.Generic;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        private UIPackage AddUIPackage(string name, byte[] bytes, string assetNamePrefix)
        {
            var packageRef = FindUIPackageRef(name);
            if (packageRef != null)
                return packageRef.UIPackage;
            
            var uiPackage = UIPackage.AddPackage(bytes, assetNamePrefix, LoadResourceAsync);
            if (uiPackage == null)
            {
                Debug.LogError($"UIPackage.AddPackage({name}, {assetNamePrefix}) failed.");
                return null;
            }
            
            packageRef = new UIPackageRef(uiPackage, OnUIPackageRefZero);
            m_UIPackageRefs.Add(name, packageRef);
            return packageRef.UIPackage;
        }

        private void RemoveUIPackage(string name)
        {
            var packageRef = FindUIPackageRef(name);
            if (packageRef == null)
                return;
            
            m_UIPackageRefs.Remove(name);
            UIPackage.RemovePackage(packageRef.Name);
        }
        
        private UIPackageRef FindUIPackageRef(string packageName)
        {
            m_UIPackageRefs.TryGetValue(packageName, out var uiPackageRef);
            return uiPackageRef;
        }

        private void OnUIPackageAcquire(UIPackage uiPackage, string _)
        {
            var packageRef = FindUIPackageRef(uiPackage.name);
            if (packageRef == null || packageRef.UIPackage != uiPackage)
                return;

            packageRef.AddRef();
        }

        private void OnUIPackageRelease(UIPackage uiPackage, string _)
        {
            var packageRef = FindUIPackageRef(uiPackage.name);
            if (packageRef == null || packageRef.UIPackage != uiPackage)
                return;

            packageRef.RemoveRef();
        }

        private void OnUIPackageRefZero(UIPackageRef packageRef)
        {
            if (!m_UnloadUnusedUIPackageImmediately)
                return;

            if (FindUIPackageRef(packageRef.Name) != packageRef)
                return;

            RemoveUIPackage(packageRef.Name);
        }

        private sealed class UIPackageRef
        {
            public string Name { get; }
            public UIPackage UIPackage { get; }
            public int RefCount { get; private set; }

            public UIPackageRef(UIPackage uiPackage, Action<UIPackageRef> onRefZero)
            {
                Name = uiPackage.name;
                UIPackage = uiPackage;
                m_OnRefZero = onRefZero;
            }

            public void AddRef()
            {
                ++RefCount;
            }

            public void RemoveRef()
            {
                if (RefCount == 0)
                {
                    Debug.LogError($"UIPackageRef[{Name}] RefCount is 0!");
                    return;
                }

                --RefCount;

                if (RefCount == 0)
                    m_OnRefZero?.Invoke(this);
            }

            private readonly Action<UIPackageRef> m_OnRefZero;
        }

        private readonly Dictionary<string, UIPackageRef> m_UIPackageRefs = new Dictionary<string, UIPackageRef>();
    }
}