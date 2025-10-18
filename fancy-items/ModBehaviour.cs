using Duckov.Modding;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace FancyItems
{
    /// <summary>
    /// FancyItems Mod 入口类 - 游戏期望的 ModBehaviour 类
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Core.FancyItemsMod fancyItemsMod;

        private void OnEnable()
        {
            Debug.Log("[FancyItems] ModBehaviour 已启用");

            // 创建并初始化核心Mod组件
            fancyItemsMod = gameObject.AddComponent<Core.FancyItemsMod>();
        }

        private void OnDisable()
        {
            Debug.Log("[FancyItems] ModBehaviour 已禁用");

            if (fancyItemsMod != null)
            {
                // 调用清理方法
                fancyItemsMod.CleanupMod();
                Destroy(fancyItemsMod);
                fancyItemsMod = null;
            }
        }

        private void OnDestroy()
        {
            OnDisable();
        }
    }
}