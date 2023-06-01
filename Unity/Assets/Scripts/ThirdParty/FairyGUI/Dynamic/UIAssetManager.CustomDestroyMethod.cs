using System;
using UnityEngine;

namespace FairyGUI.Dynamic
{
    public partial class UIAssetManager
    {
        private void DestroyTexture(Texture texture)
        {
            if (texture == null)
                return;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_LoadedTextures.Remove(texture);
            m_AssetLoader.UnloadTexture(texture);
        }

        private void DestroyAudioClip(AudioClip audioClip)
        {
            if (audioClip == null)
                return;

            if (m_AssetLoader == null)
                throw new Exception("请设置AssetLoader");

            m_LoadedAudioClips.Remove(audioClip);
            m_AssetLoader.UnloadAudioClip(audioClip);
        }
    }
}