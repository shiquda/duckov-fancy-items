using System;

namespace FancyItems.Core
{
    /// <summary>
    /// 设置项数据结构
    /// </summary>
    [Serializable]
    public class SettingItem
    {
        public string key;
        public string displayName;
        public string description;
        public SettingType type;
        public object defaultValue;
        public bool requiresRestart;
        public string category;

        public SettingItem(string key, string displayName, string description, SettingType type, object defaultValue, bool requiresRestart = false, string category = "General")
        {
            this.key = key;
            this.displayName = displayName;
            this.description = description;
            this.type = type;
            this.defaultValue = defaultValue;
            this.requiresRestart = requiresRestart;
            this.category = category;
        }
    }

    /// <summary>
    /// 设置项类型
    /// </summary>
    public enum SettingType
    {
        Toggle,
        Slider,
        Dropdown,
        Input
    }
}