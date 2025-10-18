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

namespace FancyItems.Core
{
    /// <summary>
    /// FancyItems Mod 主入口类
    /// </summary>
    public class FancyItemsMod : Duckov.Modding.ModBehaviour
    {
        private bool initialized = false;
        private Harmony harmony;

        private void OnEnable()
        {
            Debug.Log($"{Constants.FancyItemsConstants.LogPrefix} Mod已启用 - 模块化架构版本");
            if (!initialized)
            {
                StartCoroutine(InitializeMod());
            }
        }

        private IEnumerator InitializeMod()
        {
            if (initialized) yield break;

            // 初始化配置
            ModConfiguration.ApplyConfiguration();
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
    }
}