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
    /// 现在作为 ConfigManager 的只读包装器
    /// </summary>
    public static class ModConfiguration
    {
        /// <summary>
        /// 是否启用品质视觉效果
        /// </summary>
        public static bool EnableQualityVisuals
        {
            get
            {
                return ConfigManager.Config?.enableQualityVisuals ?? true;
            }
        }

        /// <summary>
        /// 是否启用音效系统
        /// </summary>
        public static bool EnableSoundEffects
        {
            get
            {
                return ConfigManager.Config?.enableSoundEffects ?? true;
            }
        }

        /// <summary>
        /// 是否启用搜索时间优化
        /// </summary>
        public static bool EnableSearchOptimization
        {
            get
            {
                return ConfigManager.Config?.enableSearchOptimization ?? true;
            }
        }

        /// <summary>
        /// 应用配置更改 - 现在只是触发配置系统应用
        /// </summary>
        public static void ApplyConfiguration()
        {
            // 配置现在由 ConfigManager 管理，这里只需要触发应用
            if (ConfigManager.Config != null)
            {
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Configuration applied:");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Quality Visuals: {EnableQualityVisuals}");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Sound Effects: {EnableSoundEffects}");
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} - Search Optimization: {EnableSearchOptimization}");
            }
        }
    }
}