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
    /// 品质颜色配置类
    /// </summary>
    public static class QualityColorConfig
    {
        /// <summary>
        /// 获取指定品质的颜色
        /// </summary>
        /// <param name="quality">物品品质</param>
        /// <returns>对应的颜色</returns>
        public static Color GetQualityColor(int quality)
        {
            if (quality < 0) quality = 0;
            int colorIndex = Mathf.Min(quality, Constants.FancyItemsConstants.QualityColors.Length - 1);
            return Constants.FancyItemsConstants.QualityColors[colorIndex];
        }

        /// <summary>
        /// 检查指定品质是否应该显示背景
        /// </summary>
        /// <param name="quality">物品品质</param>
        /// <returns>是否显示背景</returns>
        public static bool ShouldShowBackground(int quality)
        {
            return quality > 1; // 品质0和1不显示背景
        }

        /// <summary>
        /// 获取背景透明度
        /// </summary>
        /// <param name="quality">物品品质</param>
        /// <returns>透明度值</returns>
        public static float GetBackgroundAlpha(int quality)
        {
            return GetQualityColor(quality).a;
        }
    }
}