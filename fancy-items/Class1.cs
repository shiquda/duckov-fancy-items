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

namespace FancyItems
{
    // Helper组件:实时监控ItemDisplay变化并更新背景
    public class ItemDisplayQualityHelper : MonoBehaviour
    {
        private ItemDisplay itemDisplay;
        private ProceduralImage background;
        private UniformModifier roundedModifier;
        private Item lastItem;
        private int lastQuality = -1;
        private bool lastInspected = false;
        private bool initialized = false;
        private bool soundPlayed = false; // Track if sound was played for current item

        // 性能优化:降频更新间隔(秒)
        private const float UpdateInterval = 0.1f;
        private float nextUpdateTime = 0f;
        private bool isDirty = true; // 脏标记:需要更新

        private static readonly Color[] QualityColors = new Color[]
        {
            new Color(0f, 0f, 0f, 0f),              // Quality 0: 透明
            new Color(0f, 0f, 0f, 0f),              // Quality 1: 透明（普通物品不显示）
            new Color(0.6f, 0.9f, 0.6f, 0.24f),     // Quality 2: 柔和浅绿
            new Color(0.6f, 0.8f, 1.0f, 0.30f),     // Quality 3: 天蓝浅色
            new Color(1.0f, 0.50f, 1.0f, 0.40f),   // Quality 4: 亮浅紫（提亮，略粉）
            new Color(1.0f, 0.75f, 0.2f, 0.60f),   // Quality 5: 柔亮橙（更偏橙、更暖）
            new Color(1.0f, 0.3f, 0.3f, 0.4f),     // Quality 6+: 明亮红（亮度提升、透明度降低）
        };

        // 圆角半径(像素) - 匹配游戏原生UI
        private const float CornerRadius = 15f;

        private void OnEnable()
        {
            // 延迟初始化,确保ItemDisplay组件已准备好
            if (!initialized)
            {
                StartCoroutine(DelayedInitialize());
            }
            else
            {
                // 重新激活时标记为脏,需要更新
                isDirty = true;
            }
        }

        private IEnumerator DelayedInitialize()
        {
            // 等待一帧,确保ItemDisplay完全初始化
            yield return null;

            if (!initialized)
            {
                itemDisplay = GetComponent<ItemDisplay>();
                if (itemDisplay != null)
                {
                    CreateBackground();
                    initialized = true;
                    isDirty = true; // 初始化后需要更新
                }
            }
        }

        private void CreateBackground()
        {
            if (background != null) return;

            // 创建背景GameObject
            GameObject bgObject = new GameObject("FancyItems_Background");
            background = bgObject.AddComponent<ProceduralImage>();

            // 添加UniformModifier用于圆角
            roundedModifier = bgObject.AddComponent<UniformModifier>();
            roundedModifier.Radius = CornerRadius;

            // 添加LayoutElement并设置ignoreLayout,防止LayoutGroup干扰
            LayoutElement layoutElement = bgObject.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // 设置为当前ItemDisplay的子对象(使用false保留本地坐标)
            bgObject.transform.SetParent(transform, false);

            // 获取RectTransform并完全重置
            RectTransform rect = bgObject.GetComponent<RectTransform>();

            // 重置所有transform属性
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            // 设置锚点和pivot到左下角
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);

            // 清空所有offset
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            // 将背景移到最底层(最先渲染)
            bgObject.transform.SetAsFirstSibling();

            background.color = Color.clear;

            // 禁用raycast,避免阻挡点击事件
            background.raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (!initialized || itemDisplay == null || background == null)
            {
                return;
            }

            // 性能优化:降频检查 - 只在时间间隔到达或有脏标记时才检查
            float currentTime = Time.time;
            if (!isDirty && currentTime < nextUpdateTime)
            {
                return;
            }

            nextUpdateTime = currentTime + UpdateInterval;
            isDirty = false;

            // 获取当前物品
            Item currentItem = itemDisplay.Target;

            // 物品变化检测
            if (currentItem != lastItem)
            {
                lastItem = currentItem;
                lastQuality = -1;
                // 初始化为当前物品的实际状态，避免在打开背包时触发音效
                lastInspected = (currentItem != null) ? currentItem.Inspected : false;
                soundPlayed = false;
            }

            if (currentItem == null)
            {
                if (background.gameObject.activeSelf)
                {
                    background.gameObject.SetActive(false);
                }
                return;
            }

            // Monitor inspection state and play sound when item is inspected
            if (currentItem.Inspected && !lastInspected && !soundPlayed)
            {
                soundPlayed = true;
                PlayInspectionSound(currentItem.Quality);
                Debug.Log($"[FancyItems] Item inspected: {currentItem.DisplayName}, Quality: {currentItem.Quality}");
            }
            lastInspected = currentItem.Inspected;

            // 检查物品是否已被检查(搜索完成) - 商店物品除外
            // 商店中的物品(IsStockshopSample=true)应该直接显示品质，无需检查
            bool isShopItem = itemDisplay.IsStockshopSample;
            if (!currentItem.Inspected && !isShopItem)
            {
                // 未检查的物品不显示品质背景
                if (background.gameObject.activeSelf)
                {
                    background.gameObject.SetActive(false);
                }
                return;
            }

            // 品质更新
            int quality = currentItem.Quality;
            if (quality != lastQuality)
            {
                lastQuality = quality;
                UpdateBackgroundColor(quality);
            }
        }

        private void UpdateBackgroundColor(int quality)
        {
            if (background == null) return;

            if (quality <= 1)
            {
                background.gameObject.SetActive(false);
                return;
            }

            background.gameObject.SetActive(true);
            int colorIndex = Mathf.Min(quality, QualityColors.Length - 1);
            background.color = QualityColors[colorIndex];
        }

        private void PlayInspectionSound(int quality)
        {
            // Map quality levels to game built-in FMOD sounds
            string soundName;
            float volume; // 音量：0.0 (静音) ~ 1.0 (原音量) ~ 更高值（放大）
            if (quality == 1)
            {
                soundName = "event:/UI/click";
                volume = 1.0f;
            }
            else if (quality == 2)
            {
                soundName = "event:/UI/click";
                volume = 3.0f;
            }
            else if (quality == 3)
            {
                soundName = "event:/UI/confirm";
                volume = 3.0f;
            }
            else if (quality == 4)
            {
                soundName = "event:/UI/ui_skill_up";
                volume = 1.0f;
            }
            else if (quality == 5)
            {
                soundName = "event:/UI/level_up";
                volume = 2.0f;
            }
            else
            {
                soundName = "event:/UI/level_up";
                volume = 8.0f;
            }

            try
            {
                // 创建音效实例以便控制音量
                FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(soundName);

                // 设置音量
                eventInstance.setVolume(volume);

                // 播放并释放
                eventInstance.start();
                eventInstance.release();

                Debug.Log($"[FancyItems] Playing sound: {soundName} for quality {quality} at volume {volume}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FancyItems] Failed to play sound {soundName}: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            if (background != null)
            {
                Destroy(background.gameObject);
            }
        }

        // 性能优化:提供外部方法标记为脏,强制更新
        public void MarkDirty()
        {
            isDirty = true;
        }
    }

    // Harmony Patch: Hook ItemDisplay.OnEnable 自动添加Helper
    [HarmonyPatch(typeof(ItemDisplay), "OnEnable")]
    public static class ItemDisplayOnEnablePatch
    {
        static void Postfix(ItemDisplay __instance)
        {
            // 在ItemDisplay启用时自动添加Helper组件(如果没有的话)
            if (__instance != null &&
                __instance.gameObject != null &&
                __instance.GetComponent<ItemDisplayQualityHelper>() == null)
            {
                __instance.gameObject.AddComponent<ItemDisplayQualityHelper>();
                // Debug.Log($"[FancyItems] Auto-added Helper to {__instance.gameObject.name} via Harmony Hook");
            }
        }
    }

    // 搜索时间优化器
    public class FancySearchTimeOptimizer
    {
        // 优化后的搜索时间配置（秒）- 只优化低级物品
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

        // 获取优化后的搜索时间
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
    }

    // Harmony Patch: 优化搜索时间计算 - 先调用原始方法获取时间
    [HarmonyPatch(typeof(GameplayDataSettings.LootingData), "MGetInspectingTime")]
    public static class LootingDataGetInspectingTimePatch
    {
        static void Postfix(Item item, ref float __result)
        {
            // 在原始方法执行后，获取原始结果
            float originalTime = __result;

            // 使用我们的优化时间计算
            float optimizedTime = FancySearchTimeOptimizer.GetOptimizedInspectingTime(item);

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

                Debug.Log($"[FancyItems] 时间优化: {itemName} (品质{item.Quality}) " +
                         $"{originalTime:F1}s → {optimizedTime:F1}s " +
                         $"(减少{reductionPercent:F0}%)");
            }

            // 只有被优化的物品才应用新时间
            __result = optimizedTime;
        }
    }

    // Harmony Patch: 优化静态方法调用
    [HarmonyPatch(typeof(GameplayDataSettings.LootingData), "GetInspectingTime")]
    public static class LootingDataStaticGetInspectingTimePatch
    {
        static void Postfix(Item item, ref float __result)
        {
            // 使用我们的优化时间计算
            float optimizedTime = FancySearchTimeOptimizer.GetOptimizedInspectingTime(item);

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

                Debug.Log($"[FancyItems] 时间优化(静态): {itemName} (品质{item.Quality}) " +
                         $"{originalTime:F1}s → {optimizedTime:F1}s " +
                         $"(减少{reductionPercent:F0}%)");
            }

            // 应用优化时间
            __result = optimizedTime;
        }
    }

    // 主Mod类:使用Harmony自动Hook ItemDisplay创建
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? harmony;
        private const string HarmonyId = "com.fancyitems.mod";

        private void OnEnable()
        {
            Debug.Log("[FancyItems] Mod已启用 - Harmony Hook版本 (零轮询 + 搜索时间优化)");

            // 初始化Harmony并应用所有Patch
            try
            {
                harmony = new Harmony(HarmonyId);
                harmony.PatchAll(); // 自动应用所有标记了[HarmonyPatch]的类
                Debug.Log("[FancyItems] Harmony patches applied successfully");

                // 显示搜索时间优化信息
                LogSearchTimeOptimizations();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FancyItems] Failed to apply Harmony patches: {e}");
            }

            // 启动音效测试（调试用，正式版可注释掉）
            // StartCoroutine(TestSoundEffects());

            // 处理现有的ItemDisplay(Harmony只能Hook新创建的)
            StartCoroutine(ProcessExistingDisplays());
        }

        private void OnDisable()
        {
            Debug.Log("[FancyItems] Mod已禁用");

            // 移除所有Harmony Patch
            if (harmony != null)
            {
                harmony.UnpatchAll(HarmonyId);
                Debug.Log("[FancyItems] Harmony patches removed");
            }

            StopAllCoroutines();
            CleanupAllHelpers();
        }

        private void OnDestroy()
        {
            // 移除所有Harmony Patch
            if (harmony != null)
            {
                harmony.UnpatchAll(HarmonyId);
            }

            CleanupAllHelpers();
        }

        // 测试音效：从品质1到6每隔1秒播放一次
        private IEnumerator TestSoundEffects()
        {
            Debug.Log("[FancyItems] 🎵 开始音效测试...");
            yield return new WaitForSeconds(1f);

            for (int quality = 1; quality <= 6; quality++)
            {
                Debug.Log($"[FancyItems] 🎵 测试品质 {quality} 音效");
                TestPlaySound(quality);
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("[FancyItems] 🎵 音效测试完成！");
        }

        // 测试播放音效的静态方法
        private void TestPlaySound(int quality)
        {
            string soundName;
            float volume;

            if (quality == 1)
            {
                soundName = "event:/UI/click";
                volume = 1.0f;
            }
            else if (quality == 2)
            {
                soundName = "event:/UI/click";
                volume = 3.0f;
            }
            else if (quality == 3)
            {
                soundName = "event:/UI/confirm";
                volume = 3.0f;
            }
            else if (quality == 4)
            {
                soundName = "event:/UI/ui_skill_up";
                volume = 1.0f;
            }
            else if (quality == 5)
            {
                soundName = "event:/UI/level_up";
                volume = 2.0f;
            }
            else
            {
                soundName = "event:/UI/level_up";
                volume = 8.0f;
            }

            try
            {
                FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(soundName);
                eventInstance.setVolume(volume);
                eventInstance.start();
                eventInstance.release();
                Debug.Log($"[FancyItems] 🔊 播放品质 {quality}: {soundName} (音量: {volume * 100}%)");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[FancyItems] ❌ 播放失败: {soundName} - {e.Message}");
            }
        }

        // 处理现有的ItemDisplay(只在启动时执行一次)
        private IEnumerator ProcessExistingDisplays()
        {
            yield return new WaitForSeconds(0.1f);

            ItemDisplay[] displays = FindObjectsOfType<ItemDisplay>();
            Debug.Log($"[FancyItems] 找到 {displays.Length} 个现有ItemDisplay");

            int processed = 0;
            foreach (ItemDisplay display in displays)
            {
                if (display != null && display.GetComponent<ItemDisplayQualityHelper>() == null)
                {
                    display.gameObject.AddComponent<ItemDisplayQualityHelper>();
                    processed++;

                    // 每处理10个等一帧
                    if (processed % 10 == 0)
                    {
                        yield return null;
                    }
                }
            }

            Debug.Log($"[FancyItems] 已为 {processed} 个现有ItemDisplay添加Helper");
            Debug.Log($"[FancyItems] 后续新增的ItemDisplay将通过Harmony Hook自动处理");
        }

        // 清理所有Helper
        private void CleanupAllHelpers()
        {
            ItemDisplayQualityHelper[] helpers = FindObjectsOfType<ItemDisplayQualityHelper>();

            foreach (var helper in helpers)
            {
                if (helper != null)
                {
                    Destroy(helper);
                }
            }

            if (helpers.Length > 0)
            {
                Debug.Log($"[FancyItems] 清理了 {helpers.Length} 个Helper");
            }
        }

        // 记录搜索时间优化信息
        private void LogSearchTimeOptimizations()
        {
            Debug.Log("[FancyItems] 🚀 搜索时间优化已启用！");
            Debug.Log("[FancyItems] 📋 优化详情:");
            Debug.Log("[FancyItems]   品质0-1 (垃圾/普通): 0.8秒 ⚡");
            Debug.Log("[FancyItems]   品质2 (优良): 1.2秒 ⚡");
            Debug.Log("[FancyItems]   品质3+ (精良+): 保持原时间");
            Debug.Log("[FancyItems] 🎯 预期效果: 早期物品检查略微加速");
        }
    }
}
