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

namespace FancyItems.Core
{
    /// <summary>
    /// FancyItems Mod 配置管理类
    /// </summary>
    public static class ModConfiguration
    {
        /// <summary>
        /// 是否启用品质视觉效果
        /// </summary>
        public static bool EnableQualityVisuals { get; set; } = true;

        /// <summary>
        /// 是否启用音效系统
        /// </summary>
        public static bool EnableSoundEffects { get; set; } = true;

        /// <summary>
        /// 是否启用搜索时间优化
        /// </summary>
        public static bool EnableSearchOptimization { get; set; } = true;

        /// <summary>
        /// 全局音量倍数
        /// </summary>
        public static float GlobalVolumeMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// 调试模式开关
        /// </summary>
        public static bool DebugMode { get; set; } = false;

        /// <summary>
        /// 性能监控开关
        /// </summary>
        public static bool PerformanceMonitoring { get; set; } = false;

        /// <summary>
        /// 应用配置更改
        /// </summary>
        public static void ApplyConfiguration()
        {
            // 这里可以添加配置验证逻辑
            GlobalVolumeMultiplier = Mathf.Clamp01(GlobalVolumeMultiplier);

            if (DebugMode)
            {
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Configuration applied:");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Quality Visuals: {EnableQualityVisuals}");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Sound Effects: {EnableSoundEffects}");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Search Optimization: {EnableSearchOptimization}");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Global Volume: {GlobalVolumeMultiplier}");
            }
        }
    }
}