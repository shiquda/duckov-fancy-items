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

namespace FancyItems.Systems.Optimization
{
    /// <summary>
    /// 搜索时间优化器
    /// 负责优化低级物品的搜索时间
    /// </summary>
    public static class SearchTimeOptimizer
    {
        /// <summary>
        /// 优化后的搜索时间配置（秒）- 只优化低级物品
        /// </summary>
        private static readonly float[] OptimizedInspectingTimes = new float[]
        {
            0.8f,  // Quality 0: 垃圾物品 - 优化为0.8秒
            0.8f,  // Quality 1: 普通物品 - 优化为0.8秒
            0.9f,  // Quality 2: 优良物品 - 优化为1.2秒
            -1f,   // Quality 3: 精良物品 - 保持原时间
            -1f,   // Quality 4: 史诗物品 - 保持原时间
            -1f,   // Quality 5: 传说物品 - 保持原时间
            -1f,   // Quality 6+: 神话物品 - 保持原时间
        };

        /// <summary>
        /// 获取优化后的搜索时间
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>优化后的时间（秒），-1表示保持原时间</returns>
        public static float GetOptimizedInspectingTime(Item item)
        {
            if (item == null) return 1f;

            int quality = item.Quality;
            if (quality < 0) quality = 0;
            if (quality >= OptimizedInspectingTimes.Length)
                quality = OptimizedInspectingTimes.Length - 1;

            float optimizedTime = OptimizedInspectingTimes[quality];

            // 如果值为-1，表示保持原始时间，返回原始时间（在Postfix中已经获取）
            if (optimizedTime < 0)
            {
                // 这里返回-1，Postfix会处理
                return -1f;
            }

            return optimizedTime;
        }

        /// <summary>
        /// 处理搜索时间优化
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="originalTime">原始时间</param>
        /// <returns>优化后的时间</returns>
        public static float ProcessSearchTimeOptimization(Item item, float originalTime)
        {
            if (!Core.ModConfiguration.EnableSearchOptimization) return originalTime;
            if (item == null) return originalTime;

            float optimizedTime = GetOptimizedInspectingTime(item);

            // 如果返回-1，表示保持原始时间，不需要修改
            if (optimizedTime < 0)
            {
                return originalTime;
            }

            // 记录对比信息并应用优化 - 只记录被优化的品质(0、1、2)
            if (item.Quality <= 2)
            {
                LogOptimizationInfo(item, originalTime, optimizedTime);
            }

            return optimizedTime;
        }

        /// <summary>
        /// 记录优化信息
        /// </summary>
        private static void LogOptimizationInfo(Item item, float originalTime, float optimizedTime)
        {
            string itemName = item.DisplayName ?? "Unknown";
            float reductionPercent = (originalTime > 0) ?
                ((originalTime - optimizedTime) / originalTime * 100f) : 0f;

            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 时间优化: {itemName} (品质{item.Quality}) " +
                     $"{originalTime:F1}s → {optimizedTime:F1}s " +
                     $"(减少{reductionPercent:F0}%)");
        }
    }
}