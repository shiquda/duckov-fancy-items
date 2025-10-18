using Duckov.Modding;
using Duckov.UI;
using Duckov.Utilities;
using FancyItems.Systems.UI;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace FancyItems.Core
{
    /// <summary>
    /// FancyItems Mod 主入口类
    /// </summary>
    public class FancyItemsMod : Duckov.Modding.ModBehaviour
    {
        private bool initialized = false;
        private Harmony harmony;
        private SettingsUIManager settingsUIManager;

        private void OnEnable()
        {
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Mod已启用 - 带设置界面版本");
            if (!initialized)
            {
                StartCoroutine(InitializeMod());
            }
        }

        private IEnumerator InitializeMod()
        {
            if (initialized) yield break;

            // 加载配置
            ConfigManager.LoadConfig();
            yield return null;

            // 应用配置
            ModConfiguration.ApplyConfiguration();
            yield return null;

            // 创建设置 UI 管理器
            settingsUIManager = gameObject.AddComponent<SettingsUIManager>();
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面管理器已创建");
            yield return null;

            // 订阅配置变更事件
            ConfigManager.OnConfigChanged += OnConfigChanged;
            yield return null;

            // 初始化Harmony
            harmony = new Harmony(Constants.FancyItemsConstants.HarmonyId);
            harmony.PatchAll();
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Harmony补丁应用成功");
            yield return null;

            // 处理现有对象
            int processed = Patches.ItemDisplayPatches.ProcessExistingItemDisplays();
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 处理了 {processed} 个现有ItemDisplay对象");
            yield return null;

            initialized = true;
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 初始化完成！");
        }

        private void Update()
        {
            // 检查快捷键 F7 切换设置界面显示/隐藏
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (settingsUIManager != null)
                {
                    // 检查当前是否显示
                    bool isCurrentlyVisible = settingsUIManager.IsVisible();

                    if (isCurrentlyVisible)
                    {
                        settingsUIManager.HideSettings();
                        Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面已关闭");
                    }
                    else
                    {
                        settingsUIManager.ShowSettings();
                        Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面已打开");
                    }
                }
                else
                {
                    Debug.LogWarning($"{Constants.FancyItemsConstants.LogPrefix} 设置界面管理器未初始化");
                }
            }
        }

        private void OnDisable()
        {
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Mod已禁用");
            CleanupMod();
        }

        private void OnDestroy()
        {
            CleanupMod();
        }

        public void CleanupMod()
        {
            StopAllCoroutines();

            // 取消订阅配置变更事件
            ConfigManager.OnConfigChanged -= OnConfigChanged;

            // 保存配置
            ConfigManager.SaveConfig();

            if (harmony != null)
            {
                harmony.UnpatchAll(Constants.FancyItemsConstants.HarmonyId);
            }

            int cleaned = Patches.ItemDisplayPatches.CleanupAllQualityVisualizers();
            if (cleaned > 0)
            {
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 清理了 {cleaned} 个品质可视化组件");
            }
        }

        /// <summary>
        /// 配置变更事件处理
        /// </summary>
        private void OnConfigChanged(FancyItemsConfig config)
        {
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 配置已更新");
            ModConfiguration.ApplyConfiguration();
        }
    }
}