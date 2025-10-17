# 更新日志

本文档记录 Fancy Items Mod 的所有重要变更。

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [0.1.1] - 2025-10-17

### 修复

- 修复交易页面背包中背景位置向上偏移约半个单元格的问题
- 修复LayoutGroup环境下背景定位不准确的bug

### 优化

- 为背景添加 `LayoutElement` 组件并设置 `ignoreLayout = true`，防止布局系统干扰
- 移除 `LateUpdate` 中每帧调用的 `SetAsFirstSibling()`，避免布局重计算
- 优化 `RectTransform` 设置顺序，提升定位精确度
- 添加 `raycastTarget = false`，防止背景阻挡物品点击事件

### 技术细节

- 问题根源：交易页面使用 `GridLayoutGroup`/`VerticalLayoutGroup`，背景GameObject被误认为布局元素
- 解决方案：使用 `ignoreLayout` 标记背景为装饰性覆盖层，不参与布局计算

## [0.1.0] - 2025-10-17

### 新增

- 首次发布
- 实现全局物品品质背景显示功能
- 支持 Quality 1-6+ 完整颜色方案：
  - Quality 2: 绿色背景
  - Quality 3: 蓝色背景
  - Quality 4: 紫色背景
  - Quality 5: 橙色背景
  - Quality 6+: 红色背景
- 实时监控 `ItemDisplay` 组件，动态添加品质背景
- 高性能优化：0.2秒轮询检测新增物品显示
- 支持所有UI场景：背包、仓库、搜索页面、交易页面等

### 技术实现

- 使用 `ItemDisplayQualityHelper` 组件管理背景生命周期
- 通过 `LateUpdate` 实时检测物品品质变化
- 仅对已检查（Inspected）的物品显示背景
- 自动清理机制，Mod禁用时移除所有Helper组件
