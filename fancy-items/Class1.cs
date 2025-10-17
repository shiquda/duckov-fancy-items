using Duckov.Modding;
using Duckov.UI;
using ItemStatsSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FancyItems
{
    // Helper组件：实时监控ItemDisplay变化并更新背景
    public class ItemDisplayQualityHelper : MonoBehaviour
    {
        private ItemDisplay itemDisplay;
        private Image background;
        private Item lastItem;
        private int lastQuality = -1;
        private bool initialized = false;

        private static readonly Color[] QualityColors = new Color[]
        {
            new Color(0f, 0f, 0f, 0f),              // Quality 0: 透明
            new Color(0f, 0f, 0f, 0f),              // Quality 1: 透明（普通物品不显示）
            new Color(0.6f, 0.9f, 0.6f, 0.10f),     // Quality 2: 柔和浅绿
            new Color(0.6f, 0.8f, 1.0f, 0.14f),     // Quality 3: 天蓝浅色
            new Color(1.0f, 0.65f, 1.0f, 0.35f),   // Quality 4: 亮浅紫（提亮，略粉）
            new Color(1.0f, 0.75f, 0.2f, 0.60f),   // Quality 5: 柔亮橙（更偏橙、更暖）
            new Color(1.0f, 0.3f, 0.3f, 0.4f),     // Quality 6+: 明亮红（亮度提升、透明度降低）
        };

        private void OnEnable()
        {
            // 延迟初始化，确保ItemDisplay组件已准备好
            if (!initialized)
            {
                StartCoroutine(DelayedInitialize());
            }
        }

        private IEnumerator DelayedInitialize()
        {
            // 等待一帧，确保ItemDisplay完全初始化
            yield return null;

            if (!initialized)
            {
                itemDisplay = GetComponent<ItemDisplay>();
                if (itemDisplay != null)
                {
                    CreateBackground();
                    initialized = true;
                }
            }
        }

        private void CreateBackground()
        {
            if (background != null) return;

            // 创建背景GameObject
            GameObject bgObject = new GameObject("FancyItems_Background");
            background = bgObject.AddComponent<Image>();

            // 添加LayoutElement并设置ignoreLayout，防止LayoutGroup干扰
            LayoutElement layoutElement = bgObject.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // 设置为当前ItemDisplay的子对象（使用false保留本地坐标）
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

            // 将背景移到最底层（最先渲染）
            bgObject.transform.SetAsFirstSibling();

            background.color = Color.clear;

            // 禁用raycast，避免阻挡点击事件
            background.raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (!initialized || itemDisplay == null || background == null)
            {
                return;
            }

            // 获取当前物品
            Item currentItem = itemDisplay.Target;

            // 物品变化检测
            if (currentItem != lastItem)
            {
                lastItem = currentItem;
                lastQuality = -1;
            }

            if (currentItem == null)
            {
                if (background.gameObject.activeSelf)
                {
                    background.gameObject.SetActive(false);
                }
                return;
            }

            // 检查物品是否已被检查（搜索完成）
            if (!currentItem.Inspected)
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

            // 移除了SetAsFirstSibling调用，避免在LayoutGroup环境下产生位置偏移
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

        private void OnDestroy()
        {
            if (background != null)
            {
                Destroy(background.gameObject);
            }
        }
    }

    // 主Mod类：自动为所有ItemDisplay添加Helper
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("[FancyItems] Mod已启用 - 实时监控模式");

            // 立即处理现有的ItemDisplay
            StartCoroutine(ProcessExistingDisplays());

            // 启动高频监控协程（每0.2秒检查一次新增）
            StartCoroutine(MonitorNewDisplaysCoroutine());
        }

        private void OnDisable()
        {
            Debug.Log("[FancyItems] Mod已禁用");
            StopAllCoroutines();
            CleanupAllHelpers();
        }

        private void OnDestroy()
        {
            CleanupAllHelpers();
        }

        // 处理现有的ItemDisplay
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

                    // 每处理5个等一帧
                    if (processed % 5 == 0)
                    {
                        yield return null;
                    }
                }
            }

            Debug.Log($"[FancyItems] 已为 {processed} 个ItemDisplay添加Helper");
        }

        // 高频监控新增的ItemDisplay
        private IEnumerator MonitorNewDisplaysCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f); // 每0.2秒检查一次

                ItemDisplay[] displays = FindObjectsOfType<ItemDisplay>();
                int addedCount = 0;

                foreach (ItemDisplay display in displays)
                {
                    if (display != null &&
                        display.gameObject.activeInHierarchy &&
                        display.GetComponent<ItemDisplayQualityHelper>() == null)
                    {
                        display.gameObject.AddComponent<ItemDisplayQualityHelper>();
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    Debug.Log($"[FancyItems] 新增 {addedCount} 个ItemDisplay Helper");
                }
            }
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
    }
}
