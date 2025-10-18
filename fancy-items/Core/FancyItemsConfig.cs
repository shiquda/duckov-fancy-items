using System;

namespace FancyItems.Core
{
    /// <summary>
    /// FancyItems Mod 配置数据类
    /// </summary>
    [Serializable]
    public class FancyItemsConfig
    {
        /// <summary>
        /// 是否启用音效系统
        /// </summary>
        public bool enableSoundEffects = true;

        /// <summary>
        /// 是否启用搜索时间优化
        /// </summary>
        public bool enableSearchOptimization = true;

        /// <summary>
        /// 是否启用品质视觉效果
        /// </summary>
        public bool enableQualityVisuals = true;
    }
}