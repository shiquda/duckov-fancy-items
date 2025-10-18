using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FancyItems.Systems.UI;

namespace FancyItems.Core
{
    /// <summary>
    /// 自动化设置界面生成器
    /// 根据配置项数据自动生成UI界面
    /// </summary>
    public static class AutoSettingsGenerator
    {
        private const float PADDING = 0.1f;
        private const float SPACING = 0.1f;
        private const float BUTTON_HEIGHT = 0.04f;
        private const float HINT_HEIGHT = 0.04f;
        private const float TOGGLE_SIZE = 12f;

        /// <summary>
        /// 生成设置界面
        /// </summary>
        public static void GenerateSettingsUI(GameObject parent, List<SettingItem> settings)
        {
            float currentY = 0.9f; // 从顶部开始

            // 创建标题
            CreateTitle(parent, "Fancy Items 设置", currentY);
            currentY -= SPACING;

            // 按类别分组设置项
            var categories = new Dictionary<string, List<SettingItem>>();
            foreach (var setting in settings)
            {
                if (!categories.ContainsKey(setting.category))
                {
                    categories[setting.category] = new List<SettingItem>();
                }
                categories[setting.category].Add(setting);
            }

            // 生成每个类别的设置
            foreach (var kvp in categories)
            {
                // 创建类别标题
                CreateCategoryTitle(parent, kvp.Key, currentY);
                currentY -= SPACING * 0.6f;

                // 创建该类别下的设置项
                foreach (var setting in kvp.Value)
                {
                    CreateSettingItem(parent, setting, currentY);
                    currentY -= SPACING * 0.9f; // 增加间距
                }

                currentY -= SPACING * 0.3f; // 类别间间距
            }

            // 创建重启提示（如果有的设置需要重启）
            bool hasRestartRequired = false;
            foreach (var setting in settings)
            {
                if (setting.requiresRestart)
                {
                    hasRestartRequired = true;
                    break;
                }
            }

            if (hasRestartRequired)
            {
                currentY -= SPACING * 0.3f;
                CreateRestartHint(parent, currentY);
                currentY -= HINT_HEIGHT;
            }

            // 创建关闭按钮
            CreateCloseButton(parent, currentY);
        }

        /// <summary>
        /// 创建标题
        /// </summary>
        private static void CreateTitle(GameObject parent, string title, float y)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent.transform, false);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.fontSize = 24;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;

            RectTransform rect = titleText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(PADDING, y - 0.05f);
            rect.anchorMax = new Vector2(1 - PADDING, y);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 创建类别标题
        /// </summary>
        private static void CreateCategoryTitle(GameObject parent, string category, float y)
        {
            GameObject titleObj = new GameObject($"Category_{category}");
            titleObj.transform.SetParent(parent.transform, false);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = GetCategoryEmoji(category) + " " + GetCategoryDisplayName(category);
            titleText.fontSize = 20;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Left;

            RectTransform rect = titleText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(PADDING, y - 0.03f);
            rect.anchorMax = new Vector2(1 - PADDING, y);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 创建设置项
        /// </summary>
        private static void CreateSettingItem(GameObject parent, SettingItem setting, float y)
        {
            switch (setting.type)
            {
                case SettingType.Toggle:
                    CreateToggleSetting(parent, setting, y);
                    break;
                case SettingType.Slider:
                    CreateSliderSetting(parent, setting, y);
                    break;
                case SettingType.Dropdown:
                    CreateDropdownSetting(parent, setting, y);
                    break;
                case SettingType.Input:
                    CreateInputSetting(parent, setting, y);
                    break;
            }
        }

        /// <summary>
        /// 创建开关设置
        /// </summary>
        private static void CreateToggleSetting(GameObject parent, SettingItem setting, float y)
        {
            // 创建开关
            GameObject toggleObj = new GameObject($"Toggle_{setting.key}");
            toggleObj.transform.SetParent(parent.transform, false);

            Toggle toggle = toggleObj.AddComponent<Toggle>();
            // 从ConfigManager获取当前值，如果没有则使用默认值
            bool currentValue = (bool)setting.defaultValue;
            var config = ConfigManager.Config;
            if (config != null)
            {
                switch (setting.key)
                {
                    case "enableSoundEffects":
                        currentValue = config.enableSoundEffects;
                        break;
                    case "enableSearchOptimization":
                        currentValue = config.enableSearchOptimization;
                        break;
                    case "enableQualityVisuals":
                        currentValue = config.enableQualityVisuals;
                        break;
                }
            }
            toggle.isOn = currentValue;

            // 创建背景
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(toggleObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            RectTransform bgRect = bgImage.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgRect.sizeDelta = new Vector2(TOGGLE_SIZE, TOGGLE_SIZE);

            // 创建检查标记
            GameObject checkObj = new GameObject("Checkmark");
            checkObj.transform.SetParent(toggleObj.transform, false);
            Image checkImage = checkObj.AddComponent<Image>();
            checkImage.color = Color.white;

            RectTransform checkRect = checkImage.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.2f, 0.2f);
            checkRect.anchorMax = new Vector2(0.8f, 0.8f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;

            RectTransform toggleRect = toggle.GetComponent<RectTransform>();
            // 确保锚点区域是正方形的
            float toggleSize = 0.04f; // 与高度一致
            toggleRect.anchorMin = new Vector2(PADDING, y - 0.02f);
            toggleRect.anchorMax = new Vector2(PADDING + toggleSize, y + 0.02f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;
            toggleRect.sizeDelta = Vector2.zero; // 使用锚点，不需要额外尺寸

            // 创建标签
            GameObject labelObj = new GameObject($"Label_{setting.key}");
            labelObj.transform.SetParent(parent.transform, false);

            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = setting.displayName;
            labelText.fontSize = 18;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;

            RectTransform labelRect = labelText.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(PADDING + 0.06f, y - 0.02f); // 调整左边距
            labelRect.anchorMax = new Vector2(1 - PADDING, y + 0.02f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            // 绑定事件
            toggle.onValueChanged.AddListener((value) => {
                OnSettingChanged(setting.key, value);
            });
        }

        /// <summary>
        /// 创建重启提示
        /// </summary>
        private static void CreateRestartHint(GameObject parent, float y)
        {
            GameObject hintObj = new GameObject("Restart Hint");
            hintObj.transform.SetParent(parent.transform, false);

            TextMeshProUGUI hintText = hintObj.AddComponent<TextMeshProUGUI>();
            hintText.text = "提示：部分设置需要返回主菜单重新进入游戏才能完全生效";
            hintText.fontSize = 16;
            hintText.color = new Color(1f, 0.8f, 0.4f, 1f);
            hintText.alignment = TextAlignmentOptions.Center;

            RectTransform rect = hintText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(PADDING, y - HINT_HEIGHT);
            rect.anchorMax = new Vector2(1 - PADDING, y);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 创建关闭按钮
        /// </summary>
        private static void CreateCloseButton(GameObject parent, float y)
        {
            GameObject buttonObj = new GameObject("Close Button");
            buttonObj.transform.SetParent(parent.transform, false);

            Button button = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 0.8f);

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.4f, y - BUTTON_HEIGHT);
            rect.anchorMax = new Vector2(0.6f, y);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // 创建按钮文本
            GameObject textObj = new GameObject("Close Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "关闭 (F7)";
            buttonText.fontSize = 20;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;

            RectTransform textRect = buttonText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            button.onClick.AddListener(() => {
                // 触发关闭事件
                var settingsManager = UnityEngine.Object.FindObjectOfType<SettingsUIManager>();
                if (settingsManager != null)
                {
                    settingsManager.HideSettings();
                }
            });
        }

        /// <summary>
        /// 设置变更事件处理
        /// </summary>
        private static void OnSettingChanged(string key, object value)
        {
            Debug.Log($"[AutoSettings] 设置变更: {key} = {value}");

            // 调用 ConfigManager 来更新配置
            ConfigManager.UpdateConfig(config => {
                switch (key)
                {
                    case "enableSoundEffects":
                        config.enableSoundEffects = (bool)value;
                        break;
                    case "enableSearchOptimization":
                        config.enableSearchOptimization = (bool)value;
                        break;
                    case "enableQualityVisuals":
                        config.enableQualityVisuals = (bool)value;
                        break;
                }
            });
        }

        /// <summary>
        /// 获取类别表情符号
        /// </summary>
        private static string GetCategoryEmoji(string category)
        {
            switch (category)
            {
                case "Audio": return "[音效]";
                case "Optimization": return "[优化]";
                case "Visual": return "[视觉]";
                default: return "[设置]";
            }
        }

        /// <summary>
        /// 获取类别显示名称
        /// </summary>
        private static string GetCategoryDisplayName(string category)
        {
            switch (category)
            {
                case "Audio": return "音效设置";
                case "Optimization": return "搜索优化";
                case "Visual": return "视觉效果";
                default: return "其他设置";
            }
        }

        // 其他设置类型的创建方法（占位符）
        private static void CreateSliderSetting(GameObject parent, SettingItem setting, float y) { }
        private static void CreateDropdownSetting(GameObject parent, SettingItem setting, float y) { }
        private static void CreateInputSetting(GameObject parent, SettingItem setting, float y) { }
    }
}