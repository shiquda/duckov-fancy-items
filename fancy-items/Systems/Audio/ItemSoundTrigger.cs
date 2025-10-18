using Duckov.Modding;
using Duckov.UI;
using Duckov.Utilities;
using FancyItems.Core;
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
    /// 独立的物品音效触发器
    /// 不依赖于视觉效果，独立处理音效播放
    /// </summary>
    public class ItemSoundTrigger : MonoBehaviour
    {
        private ItemDisplay itemDisplay;
        private Item currentItem;
        private Item lastItem;
        private bool lastInspected = false;
        private bool soundPlayed = false;

        void Awake()
        {
            itemDisplay = GetComponent<ItemDisplay>();
        }

        void Start()
        {
            // 延迟初始化，确保所有组件都已加载
            StartCoroutine(DelayedInitialize());
        }

        void Update()
        {
            if (!ModConfiguration.EnableSoundEffects) return;

            if (itemDisplay != null)
            {
                currentItem = itemDisplay.Target;

                if (currentItem != lastItem)
                {
                    lastItem = currentItem;
                    // 保持与旧版本一致的逻辑：初始化时记录物品当前的检查状态
                    lastInspected = (currentItem != null) ? currentItem.Inspected : false;
                    soundPlayed = false;
                }

                if (currentItem != null && currentItem.Inspected && !lastInspected && !soundPlayed)
                {
                    soundPlayed = true;
                    // 播放品质音效
                    QualitySoundManager.PlayQualitySound(currentItem.Quality);
                }

                lastInspected = currentItem?.Inspected ?? false;
            }
        }

        private IEnumerator DelayedInitialize()
        {
            yield return null;
            // 确保组件已正确初始化
        }

        void OnDestroy()
        {
            // 清理资源
        }
    }
}