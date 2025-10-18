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

namespace FancyItems.Constants
{
    /// <summary>
    /// FancyItems Mod 全局常量定义
    /// </summary>
    public static class FancyItemsConstants
    {
        /// <summary>
        /// Harmony ID，用于补丁管理
        /// </summary>
        public const string HarmonyId = "com.fancyitems.mod";

        /// <summary>
        /// Mod日志前缀
        /// </summary>
        public const string LogPrefix = "[FancyItems]";

        /// <summary>
        /// 品质背景圆角半径（像素）
        /// </summary>
        public const float BackgroundCornerRadius = 15f;

        /// <summary>
        /// 性能优化更新间隔（秒）
        /// </summary>
        public const float PerformanceUpdateInterval = 0.1f;

        /// <summary>
        /// 搜索时间优化配置（秒）
        /// </summary>
        public static readonly float[] OptimizedSearchTimes = new float[]
        {
            0.8f,  // Quality 0: 垃圾物品
            0.8f,  // Quality 1: 普通物品
            0.9f,  // Quality 2: 优良物品
            -1f,   // Quality 3: 精良物品（保持原时间）
            -1f,   // Quality 4: 史诗物品（保持原时间）
            -1f,   // Quality 5: 传说物品（保持原时间）
            -1f    // Quality 6+: 神话物品（保持原时间）
        };

        /// <summary>
        /// 品质颜色配置（RGBA）
        /// </summary>
        public static readonly Color[] QualityColors = new Color[]
        {
            new Color(0f, 0f, 0f, 0f),              // Quality 0: 透明
            new Color(0f, 0f, 0f, 0f),              // Quality 1: 透明（普通物品不显示）
            new Color(0.6f, 0.9f, 0.6f, 0.24f),     // Quality 2: 柔和浅绿
            new Color(0.6f, 0.8f, 1.0f, 0.30f),     // Quality 3: 天蓝浅色
            new Color(1.0f, 0.50f, 1.0f, 0.40f),   // Quality 4: 亮浅紫（提亮，略粉）
            new Color(1.0f, 0.75f, 0.2f, 0.60f),   // Quality 5: 柔亮橙（更偏橙、更暖）
            new Color(1.0f, 0.3f, 0.3f, 0.4f),     // Quality 6+: 明亮红（亮度提升、透明度降低）
        };
    }
}