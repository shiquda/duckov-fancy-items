# 更新日志

本文档记录 Fancy Items Mod 的所有重要变更。

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [0.4.0] - 2025-10-19

### 新增 ✨

- **完整设置界面系统**：内置游戏内设置面板，支持可视化配置
  - 按 F7 键即可打开设置界面
  - 实时预览设置效果，即时生效
  - 支持所有功能的独立开关控制

- **配置持久化系统**：设置自动保存和加载
  - 使用 JSON 格式存储配置文件
  - 重启游戏后自动恢复上次设置
  - 支持一键重置为默认设置

- **自动化设置生成器**：基于代码自动生成配置选项
  - 通过反射自动发现可配置属性
  - 支持 UI 元素、音效系统、性能优化等模块
  - 减少手动维护配置代码的工作量

- **搜索时间优化系统**：降低低级物品的搜索时间
  - 根据物品品质智能调整搜索速度
  - 低品质物品搜索更快，提升游戏流畅度
  - 可在设置中独立开启/关闭

### 重构 🔧

- **模块化架构重构**：全面优化代码结构
  - 分离 UI、音效、配置等独立模块
  - 提升代码可维护性和扩展性
  - 为未来功能开发奠定良好基础

- **性能优化模块**：独立的性能管理系统
  - 集中管理所有性能相关设置
  - 支持多种性能模式选择
  - 智能检测游戏场景，自适应调整

### 技术细节 🛠️

- **配置管理器**：`ConfigManager.cs` - 负责配置的保存/加载
- **设置UI管理器**：`SettingsUIManager.cs` - 游戏内设置界面
- **自动设置生成器**：`AutoSettingsGenerator.cs` - 自动生成配置选项
- **搜索时间优化器**：`SearchTimeOptimizer.cs` - 搜索速度优化
- **设置项类**：`SettingItem.cs` - 配置项的数据结构

### 用户体验提升 💡

- **直观的操作界面**：所有设置都有清晰的说明和即时反馈
- **灵活的配置选项**：用户可以根据喜好自定义每种功能
- **稳定可靠的系统**：配置保存机制确保设置不会丢失

## [0.3.1] - 2025-10-18

修复商店界面品质显示问题，新增47个游戏音效的完整测试系统，为音效优化提供丰富选择。

## [0.3.0] - 2025-10-17

### 新增 ✨

- **物品检索音效系统**：搜索物品时根据品质播放不同的音效
  - 集成 FMOD Unity 音频系统，使用游戏内置音效
  - 品质越高，音效越响亮，提供更强的反馈感
  - 智能防重复播放机制，避免音效重叠

### 音效映射 🎵

| 品质等级 | 音效 | 音量 | 说明 |
|---------|------|------|------|
| Quality 1 | `UI/click` | 100% | 普通物品 - 简单点击 |
| Quality 2 | `UI/click` | 500% | 绿色物品 - 增强点击 |
| Quality 3 | `UI/confirm` | 300% | 蓝色物品 - 确认音效 |
| Quality 4 | `UI/game_start` | 300% | 紫色物品 - 游戏开始 |
| Quality 5 | `UI/level_up` | 200% | 橙色物品 - 升级音效 |
| Quality 6+ | `UI/level_up` | 800% | 红色物品 - 超级升级 |

### 修复 🐛

- 修复打开背包时已检索物品错误播放音效的问题
  - 问题原因：ItemDisplay组件初始化时 `lastInspected` 为 false，而背包物品已是 Inspected 状态
  - 解决方案：物品切换时同步 `lastInspected` 为物品的实际状态

### 技术细节 🔧

- 新增依赖：`FMODUnity.dll` (游戏音频系统)
- 新增 using：`Duckov.Utilities`
- 实现方法：
  - 使用 `FMODUnity.RuntimeManager.CreateInstance()` 创建音效实例
  - 通过 `EventInstance.setVolume()` 精确控制音量
  - 使用 `EventInstance.start()` 和 `release()` 管理播放生命周期
- 音效触发时机：
  - 监听 `Item.Inspected` 状态从 false → true 的变化
  - 使用 `soundPlayed` 标记防止重复播放
  - 物品切换时自动重置播放状态
- 调试功能：
  - 添加 `TestSoundEffects()` 协程用于音效测试
  - 从品质1到6每隔1秒依次播放，便于调试调优
  - 正式版默认注释，需要时可取消注释

### 性能影响

- **极低性能开销**：音效播放使用 FMOD 引擎的异步系统
- **无额外轮询**：基于现有的品质检测机制，无新增性能损耗
- **内存友好**：使用游戏内置音效，不增加额外资源加载

## [0.2.0] - 2025-10-17

### 性能优化 🚀

- **重大性能提升**：引入 Harmony 库实现零轮询架构
  - ❌ 移除 `FindObjectsOfType` 高频轮询（原0.2秒/次 → 1秒/次 → **完全移除**）
  - ✅ 使用 Harmony Hook 拦截 `ItemDisplay.OnEnable`，实时自动添加Helper
  - ✅ 添加脏标记机制，LateUpdate从每帧检查 → 每0.1秒检查
  - ✅ 消除重复检查，避免99%的无效更新
  - **预期性能提升**：CPU占用降低约90%，彻底解决大量物品时的卡顿问题

### 技术细节

- 新增依赖：`Lib.Harmony 2.3.3`
- Harmony Patch点：`ItemDisplay.OnEnable.Postfix`
- 工作原理：
  - Mod启动时：一次性处理现有ItemDisplay（批处理优化）
  - 运行时：通过Harmony Hook自动捕获新创建的ItemDisplay
  - 关闭时：自动移除所有Harmony Patch，确保干净卸载

## [0.1.2] - 2025-10-17

### 新增

- ✨ **圆角背景支持**：品质背景现在使用圆角矩形，与游戏原生UI风格一致
  - 使用 `ProceduralImage` 替代普通 `Image` 组件
  - 添加 `UniformModifier` 实现15像素圆角效果
  - 完美匹配游戏内物品框的视觉风格

### 优化

- 🎨 调整品质颜色透明度，提升视觉效果：
  - Quality 2 (绿色): 透明度从 0.10 提升至 0.24
  - Quality 3 (蓝色): 透明度从 0.14 提升至 0.30
  - Quality 4 (紫色): 色调调整，透明度提升至 0.40

### 技术细节

- 添加 `Plugins.dll` 引用以访问 `ProceduralImage` 组件
- 圆角半径设置为15像素，经测试与游戏UI完美匹配
- 背景渲染使用专用shader：`UI/Procedural UI Image`

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
