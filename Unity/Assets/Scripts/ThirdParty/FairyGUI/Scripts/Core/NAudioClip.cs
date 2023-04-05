using System;
using UnityEngine;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class NAudioClip
    {
        public static Action<AudioClip> CustomDestroyMethod;

        /// <summary>
        /// 
        /// </summary>
        public DestroyMethod destroyMethod;

        /// <summary>
        /// 
        /// </summary>
        public AudioClip nativeClip;

        /// <summary>
        /// 
        /// </summary>
        public int refCount;

        /// <summary>
        /// This event will trigger when ref count is zero.
        /// </summary>
        public event Action<NAudioClip> onRelease;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClip"></param>
        public NAudioClip(AudioClip audioClip)
        {
            nativeClip = audioClip;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unload()
        {
            if (nativeClip == null)
                return;

            if (destroyMethod == DestroyMethod.Unload)
                Resources.UnloadAsset(nativeClip);
            else if (destroyMethod == DestroyMethod.Destroy)
                UnityEngine.Object.DestroyImmediate(nativeClip, true);
            else if (destroyMethod == DestroyMethod.Custom)
            {
                if (CustomDestroyMethod == null)
                    Debug.LogWarning("NAudioClip.CustomDestroyMethod must be set to handle DestroyMethod.Custom");
                else
                    CustomDestroyMethod(nativeClip);
            }

            nativeClip = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClip"></param>
        public void Reload(AudioClip audioClip)
        {
            if (nativeClip != null && nativeClip != audioClip)
                Unload();

            nativeClip = audioClip;
        }

        public void AddRef()
        {
            refCount++;
        }

        public void ReleaseRef()
        {
            refCount--;

            if (refCount != 0) 
                return;

            onRelease?.Invoke(this);
        }
    }
}
