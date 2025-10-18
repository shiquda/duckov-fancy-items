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

namespace FancyItems.Patches
{
    /// <summary>
    /// ItemDisplay 相关的 Harmony 补丁
    /// </summary>
    public static class ItemDisplayPatches
    {
        [HarmonyPatch(typeof(ItemDisplay), "OnEnable")]
        public static class ItemDisplayOnEnablePatch
        {
            static void Postfix(ItemDisplay __instance)
            {
                // 添加视觉效果组件
                if (Core.ModConfiguration.EnableQualityVisuals)
                {
                    if (__instance != null && __instance.gameObject != null &&
                        __instance.GetComponent<Systems.UI.ItemQualityVisualizer>() == null)
                    {
                        __instance.gameObject.AddComponent<Systems.UI.ItemQualityVisualizer>();
                    }
                }

                // 添加独立的音效触发器组件
                if (Core.ModConfiguration.EnableSoundEffects)
                {
                    if (__instance != null && __instance.gameObject != null &&
                        __instance.GetComponent<Systems.Audio.ItemSoundTrigger>() == null)
                    {
                        __instance.gameObject.AddComponent<Systems.Audio.ItemSoundTrigger>();
                    }
                }
            }
        }

        public static int ProcessExistingItemDisplays()
        {
            ItemDisplay[] displays = Object.FindObjectsOfType<ItemDisplay>();
            int processed = 0;

            foreach (ItemDisplay display in displays)
            {
                if (display == null) continue;

                // 添加视觉效果组件
                if (Core.ModConfiguration.EnableQualityVisuals)
                {
                    if (display.GetComponent<Systems.UI.ItemQualityVisualizer>() == null)
                    {
                        display.gameObject.AddComponent<Systems.UI.ItemQualityVisualizer>();
                        processed++;
                    }
                }

                // 添加独立的音效触发器组件
                if (Core.ModConfiguration.EnableSoundEffects)
                {
                    if (display.GetComponent<Systems.Audio.ItemSoundTrigger>() == null)
                    {
                        display.gameObject.AddComponent<Systems.Audio.ItemSoundTrigger>();
                        processed++;
                    }
                }
            }
            return processed;
        }

        public static int CleanupAllQualityVisualizers()
        {
            int cleanedCount = 0;

            // 清理视觉效果组件
            Systems.UI.ItemQualityVisualizer[] visualizers = Object.FindObjectsOfType<Systems.UI.ItemQualityVisualizer>();
            foreach (var visualizer in visualizers)
            {
                if (visualizer != null) Object.Destroy(visualizer);
                cleanedCount++;
            }

            // 清理音效触发器组件
            Systems.Audio.ItemSoundTrigger[] soundTriggers = Object.FindObjectsOfType<Systems.Audio.ItemSoundTrigger>();
            foreach (var soundTrigger in soundTriggers)
            {
                if (soundTrigger != null) Object.Destroy(soundTrigger);
                cleanedCount++;
            }

            return cleanedCount;
        }
    }

    /// <summary>
    /// 搜索时间优化相关的 Harmony 补丁
    /// </summary>
    public static class LootTimePatches
    {
        [HarmonyPatch(typeof(GameplayDataSettings.LootingData), "MGetInspectingTime")]
        public static class LootingDataMGetInspectingTimePatch
        {
            static void Postfix(Item item, ref float __result)
            {
                if (!Core.ModConfiguration.EnableSearchOptimization) return;

                // 在原始方法执行后，获取原始结果
                float originalTime = __result;

                // 使用我们的优化时间计算
                float optimizedTime = Systems.Optimization.SearchTimeOptimizer.GetOptimizedInspectingTime(item);

                // 如果返回-1，表示保持原始时间，不需要做任何修改
                if (optimizedTime < 0)
                {
                    // __result已经是原始时间，保持不变
                    return;
                }

                // 记录对比信息并应用优化 - 只记录被优化的品质(0、1、2)
                if (item != null && item.Quality <= 2)
                {
                    string itemName = item.DisplayName ?? "Unknown";
                    float reductionPercent = (originalTime > 0) ?
                        ((originalTime - optimizedTime) / originalTime * 100f) : 0f;

                    Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 时间优化: {itemName} (品质{item.Quality}) " +
                             $"{originalTime:F1}s → {optimizedTime:F1}s " +
                             $"(减少{reductionPercent:F0}%)");
                }

                // 只有被优化的物品才应用新时间
                __result = optimizedTime;
            }
        }
    }
}