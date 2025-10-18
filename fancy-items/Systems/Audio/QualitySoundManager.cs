using Duckov.Modding;
using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace FancyItems.Systems.Audio
{
    /// <summary>
    /// 品质音效管理器
    /// </summary>
    public static class QualitySoundManager
    {
        /// <summary>
        /// 播放物品品质音效
        /// </summary>
        /// <param name="quality">物品品质</param>
        public static void PlayQualitySound(int quality)
        {
            if (!Core.ModConfiguration.EnableSoundEffects) return;

            string soundName;
            float volume;

            switch (quality)
            {
                case 1:
                    soundName = "event:/UI/click";
                    volume = 1.0f;
                    break;
                case 2:
                    soundName = "event:/UI/click";
                    volume = 3.0f;
                    break;
                case 3:
                    soundName = "event:/UI/confirm";
                    volume = 3.0f;
                    break;
                case 4:
                    soundName = "event:/UI/ui_skill_up";
                    volume = 1.0f;
                    break;
                case 5:
                    soundName = "event:/UI/level_up";
                    volume = 2.0f;
                    break;
                default: // quality 6+
                    soundName = "event:/UI/level_up";
                    volume = 8.0f;
                    break;
            }

            PlaySound(soundName, volume, quality);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        private static void PlaySound(string soundName, float volume, int quality)
        {
            try
            {
                // 应用全局音量倍数
                float adjustedVolume = volume * Core.ModConfiguration.GlobalVolumeMultiplier;

                FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(soundName);
                eventInstance.setVolume(adjustedVolume);
                eventInstance.start();
                eventInstance.release();

                if (Core.ModConfiguration.DebugMode)
                {
                    Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 🔊 播放品质 {quality}: {soundName} (音量: {adjustedVolume * 100}%)");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{Constants.FancyItemsConstants.LogPrefix} ❌ 播放失败: {soundName} - {e.Message}");
            }
        }

        /// <summary>
        /// 测试所有品质音效
        /// </summary>
        public static IEnumerator TestAllQualitySounds()
        {
            if (!Core.ModConfiguration.EnableSoundEffects)
            {
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 音效功能已禁用");
                yield break;
            }

            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 🎵 开始测试音效...");

            for (int quality = 1; quality <= 6; quality++)
            {
                PlayQualitySound(quality);
                yield return new WaitForSeconds(1f);
            }

            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 🎵 音效测试完成！");
        }
    }
}