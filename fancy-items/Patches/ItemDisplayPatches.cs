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
                if (!Core.ModConfiguration.EnableQualityVisuals) return;

                if (__instance != null && __instance.gameObject != null &&
                    __instance.GetComponent<Systems.UI.ItemQualityVisualizer>() == null)
                {
                    __instance.gameObject.AddComponent<Systems.UI.ItemQualityVisualizer>();
                }
            }
        }

        public static int ProcessExistingItemDisplays()
        {
            if (!Core.ModConfiguration.EnableQualityVisuals) return 0;

            ItemDisplay[] displays = Object.FindObjectsOfType<ItemDisplay>();
            int processed = 0;
            foreach (ItemDisplay display in displays)
            {
                if (display != null && display.GetComponent<Systems.UI.ItemQualityVisualizer>() == null)
                {
                    display.gameObject.AddComponent<Systems.UI.ItemQualityVisualizer>();
                    processed++;
                }
            }
            return processed;
        }

        public static int CleanupAllQualityVisualizers()
        {
            Systems.UI.ItemQualityVisualizer[] visualizers = Object.FindObjectsOfType<Systems.UI.ItemQualityVisualizer>();
            foreach (var visualizer in visualizers)
            {
                if (visualizer != null) Object.Destroy(visualizer);
            }
            return visualizers.Length;
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

        [HarmonyPatch(typeof(GameplayDataSettings.LootingData), "GetInspectingTime")]
        public static class LootingDataStaticGetInspectingTimePatch
        {
            static void Postfix(Item item, ref float __result)
            {
                if (!Core.ModConfiguration.EnableSearchOptimization) return;

                // 使用我们的优化时间计算
                float optimizedTime = Systems.Optimization.SearchTimeOptimizer.GetOptimizedInspectingTime(item);

                // 如果返回-1，表示保持原始时间，不需要修改
                if (optimizedTime < 0)
                {
                    return; // 保持原始时间
                }

                // 记录对比信息并应用优化 - 只记录被优化的品质(0、1、2)
                if (item != null && item.Quality <= 2)
                {
                    // 获取原始时间（已经在__result中）
                    float originalTime = __result;
                    string itemName = item.DisplayName ?? "Unknown";
                    float reductionPercent = (originalTime > 0) ?
                        ((originalTime - optimizedTime) / originalTime * 100f) : 0f;

                    Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 时间优化(静态): {itemName} (品质{item.Quality}) " +
                             $"{originalTime:F1}s → {optimizedTime:F1}s " +
                             $"(减少{reductionPercent:F0}%)");
                }

                // 应用优化时间
                __result = optimizedTime;
            }
        }
    }
}