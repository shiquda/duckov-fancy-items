using Duckov.Modding;
using Duckov.Utilities;
using Saves;
using UnityEngine;

namespace FancyItems.Core
{
    /// <summary>
    /// 配置管理器
    /// 负责配置的加载、保存和事件通知
    /// </summary>
    public static class ConfigManager
    {
        private const string CONFIG_KEY = "FancyItems_Config";
        private static FancyItemsConfig config;

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public static event System.Action<FancyItemsConfig> OnConfigChanged;

        /// <summary>
        /// 获取当前配置
        /// </summary>
        public static FancyItemsConfig Config => config;

        /// <summary>
        /// 加载配置
        /// </summary>
        public static void LoadConfig()
        {
            try
            {
                string json = SavesSystem.LoadGlobal(CONFIG_KEY, "");

                if (string.IsNullOrEmpty(json))
                {
                    config = new FancyItemsConfig();
                    Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 创建默认配置");
                }
                else
                {
                    config = JsonUtility.FromJson<FancyItemsConfig>(json);
                    if (config == null)
                    {
                        config = new FancyItemsConfig();
                        Debug.LogWarning($"{Constants.FancyItemsConstants.LogPrefix} 配置加载失败，使用默认配置");
                    }
                    else
                    {
                        Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 配置加载成功");
                    }
                }
            }
            catch (System.Exception e)
            {
                config = new FancyItemsConfig();
                Debug.LogError($"{Constants.FancyItemsConstants.LogPrefix} 配置加载异常: {e.Message}");
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void SaveConfig()
        {
            try
            {
                if (config == null)
                {
                    config = new FancyItemsConfig();
                }

                string json = JsonUtility.ToJson(config, true);
                SavesSystem.SaveGlobal(CONFIG_KEY, json);

                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 配置保存成功");

                // 触发配置变更事件
                OnConfigChanged?.Invoke(config);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{Constants.FancyItemsConstants.LogPrefix} 配置保存异常: {e.Message}");
            }
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="modifyAction">配置修改动作</param>
        public static void UpdateConfig(System.Action<FancyItemsConfig> modifyAction)
        {
            if (modifyAction != null && config != null)
            {
                modifyAction(config);
                SaveConfig();
            }
        }

        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public static void ResetToDefault()
        {
            config = new FancyItemsConfig();
            SaveConfig();
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 配置已重置为默认值");
        }
    }
}