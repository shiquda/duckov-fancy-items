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

namespace FancyItems.Systems.UI
{
    /// <summary>
    /// 物品品质可视化组件
    /// </summary>
    public class ItemQualityVisualizer : MonoBehaviour
    {
        private ItemDisplay itemDisplay;
        private ProceduralImage background;
        private UniformModifier roundedModifier;
        private Item lastItem;
        private int lastQuality = -1;
        private bool lastInspected = false;
        private bool initialized = false;
        private bool soundPlayed = false;
        private bool isDirty = true;
        private float nextUpdateTime = 0f;

        private void OnEnable()
        {
            if (!initialized)
            {
                StartCoroutine(DelayedInitialize());
            }
            else
            {
                isDirty = true;
            }
        }

        private IEnumerator DelayedInitialize()
        {
            yield return null;
            if (!initialized)
            {
                itemDisplay = GetComponent<ItemDisplay>();
                if (itemDisplay != null)
                {
                    CreateBackground();
                    initialized = true;
                    isDirty = true;
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
            roundedModifier.Radius = Constants.FancyItemsConstants.BackgroundCornerRadius;

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

            // 将背景移到最下层（最先渲染）
            bgObject.transform.SetAsFirstSibling();

            background.color = Color.clear;

            // 禁用raycast，避免阻挡点击事件
            background.raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (!initialized || itemDisplay == null || background == null) return;

            if (!isDirty && Time.time < nextUpdateTime) return;

            nextUpdateTime = Time.time + Constants.FancyItemsConstants.PerformanceUpdateInterval;
            isDirty = false;

            UpdateItemDisplay();
        }

        private void UpdateItemDisplay()
        {
            Item currentItem = itemDisplay.Target;
            if (currentItem != lastItem)
            {
                lastItem = currentItem;
                lastQuality = -1;
                lastInspected = (currentItem != null) ? currentItem.Inspected : false;
                soundPlayed = false;
            }

            if (currentItem == null)
            {
                if (background.gameObject.activeSelf) background.gameObject.SetActive(false);
                return;
            }

            if (currentItem.Inspected && !lastInspected && !soundPlayed)
            {
                soundPlayed = true;
                // 播放品质音效
                Systems.Audio.QualitySoundManager.PlayQualitySound(currentItem.Quality);
            }
            lastInspected = currentItem.Inspected;

            bool isShopItem = itemDisplay.IsStockshopSample;
            if (!currentItem.Inspected && !isShopItem)
            {
                if (background.gameObject.activeSelf) background.gameObject.SetActive(false);
                return;
            }

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
            if (!QualityColorConfig.ShouldShowBackground(quality))
            {
                background.gameObject.SetActive(false);
                return;
            }
            background.gameObject.SetActive(true);
            background.color = QualityColorConfig.GetQualityColor(quality);
        }

        private void OnDestroy()
        {
            if (background != null) Destroy(background.gameObject);
        }

        public void MarkDirty() { isDirty = true; }
    }
}