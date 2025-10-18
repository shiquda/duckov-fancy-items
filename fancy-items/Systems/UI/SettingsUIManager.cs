using Duckov.Modding;
using Duckov.UI;
using Duckov.Utilities;
using FancyItems.Core;
using HarmonyLib;
using ItemStatsSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace FancyItems.Systems.UI
{
    /// <summary>
    /// 设置界面管理器
    /// 负责创建和管理 Mod 的设置界面（使用自动化生成器）
    /// </summary>
    public class SettingsUIManager : MonoBehaviour
    {
        private GameObject settingsPanel;
        private Canvas canvas;
        private RectTransform panelRect;

        private bool isInitialized = false;

        void Awake()
        {
            // 订阅配置变更事件
            ConfigManager.OnConfigChanged += OnConfigChanged;
        }

        /// <summary>
        /// 显示设置界面
        /// </summary>
        public void ShowSettings()
        {
            if (!isInitialized)
            {
                InitializeSettingsPanel();
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                UpdateUIValues();
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面已显示");
            }
        }

        /// <summary>
        /// 隐藏设置界面
        /// </summary>
        public void HideSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面已隐藏");
            }
        }

        /// <summary>
        /// 检查设置界面是否可见
        /// </summary>
        public bool IsVisible()
        {
            return settingsPanel != null && settingsPanel.activeSelf;
        }

        /// <summary>
        /// 初始化设置界面
        /// </summary>
        private void InitializeSettingsPanel()
        {
            try
            {
                CreateSettingsPanel();
                CreateUIComponents();
                BindUIEvents();

                // 验证设置面板已创建
                if (settingsPanel != null)
                {
                    isInitialized = true;
                    Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} 设置界面初始化完成");
                }
                else
                {
                    Debug.LogError($"{Constants.FancyItemsConstants.LogPrefix} 设置面板创建失败");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{Constants.FancyItemsConstants.LogPrefix} 设置界面初始化失败: {e.Message}");
                Debug.LogError($"{Constants.FancyItemsConstants.LogPrefix} 堆栈跟踪: {e.StackTrace}");
            }
        }

        /// <summary>
        /// 创建设置面板
        /// </summary>
        private void CreateSettingsPanel()
        {
            // 查找或创建 Canvas
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("FancyItems Settings Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建设置面板
            settingsPanel = new GameObject("FancyItems Settings Panel");
            settingsPanel.transform.SetParent(canvas.transform, false);

            // 添加背景面板
            Image backgroundImage = settingsPanel.AddComponent<Image>();
            backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            panelRect = settingsPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.2f);
            panelRect.anchorMax = new Vector2(0.7f, 0.8f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // 默认隐藏
            settingsPanel.SetActive(false);
        }

        /// <summary>
        /// 创建 UI 组件（使用自动化生成器）
        /// </summary>
        private void CreateUIComponents()
        {
            // 定义设置项
            var settings = new List<SettingItem>
            {
                new SettingItem("enableSoundEffects", "启用物品品质音效", "开启后检查物品时会播放相应品质的音效", SettingType.Toggle, true, false, "Audio"),
                new SettingItem("enableSearchOptimization", "启用低级物品搜索加速", "开启后低级物品的搜索时间会减少", SettingType.Toggle, true, true, "Optimization"), // 需要重启
                new SettingItem("enableQualityVisuals", "启用物品品质颜色特效", "开启后物品边框会显示对应品质的颜色", SettingType.Toggle, true, true, "Visual") // 需要重启
            };

            // 使用自动化生成器创建界面
            AutoSettingsGenerator.GenerateSettingsUI(settingsPanel, settings);
        }

        /// <summary>
        /// 绑定 UI 事件（自动化生成器内部处理）
        /// </summary>
        private void BindUIEvents()
        {
            // 自动化生成器会自动绑定事件
            // 这里可以添加额外的事件处理
        }

        /// <summary>
        /// 更新 UI 值（自动化生成器内部处理）
        /// </summary>
        private void UpdateUIValues()
        {
            // 自动化生成器会自动更新UI值
            // 这里可以添加额外的UI更新逻辑
        }

        /// <summary>
        /// 配置变更事件处理
        /// </summary>
        private void OnConfigChanged(FancyItemsConfig config)
        {
            UpdateUIValues();
        }

        /// <summary>
        /// 清理 UI
        /// </summary>
        private void CleanupUI()
        {
            if (settingsPanel != null)
            {
                DestroyImmediate(settingsPanel);
                settingsPanel = null;
            }
        }

        void OnDestroy()
        {
            // 取消订阅配置变更事件
            ConfigManager.OnConfigChanged -= OnConfigChanged;

            // 清理 UI
            CleanupUI();
        }
    }
}