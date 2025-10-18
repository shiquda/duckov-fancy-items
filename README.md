# Fancy Items - 炫彩物品品质显示

[Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3588329796)

为《逃离鸭科夫》中的所有物品图标添加品质背景色，让你一眼识别物品稀有度！还有搜索音效~

## ✨ 功能特性

- 🎨 视觉增强：所有物品图标根据品质显示彩色背景（背包、装备栏、地面等）
- 🎵 音效反馈：搜索物品时根据品质播放不同音效
- ⚡ 低级物品加速：降低低品质物品的搜索时间，提升游戏流畅度
- 🛠️ 配置系统：内置完整的设置面板，支持可视化配置，可独立控制各种功能开关和参数

## 📥 安装方法

### 方式一：通过 Steam 创意工坊下载安装（推荐）

前往 [Steam 创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3588329796)，或搜索Fancy Items，点击订阅，即可下载最新版本。

### 方式二：通过 Release 下载安装

该方式适用于在非Steam平台下游玩的情况。

1. 前往 [Releases 页面](https://github.com/shiquda/fancy-items/releases) 下载最新版本
2. 下载 `FancyItems-vX.X.X.zip` 文件
3. 解压到游戏目录：`[游戏目录]/Duckov_Data/Mods/FancyItems/`
4. 启动游戏，并在Mod菜单中启用 "Fancy Items"

### 方式三：自行编译安装（开发者）

1. 配置dotnet开发环境
2. 运行 `build.bat` 构建mod
3. 将 `Release/FancyItems` 目录解压到游戏目录：`[游戏目录]/Duckov_Data/Mods/FancyItems/`
4. 启动游戏，在Mod菜单中启用 "Fancy Items"

> 💡 **提示**：游戏目录通常位于 `XXX\Steam\steamapps\common\Escape from Duckov\`

## 🎮 效果说明

| Quality | 视觉效果 |
|---------|---------|
| 1       | 无背景（普通物品） |
| 2       | 🟢 绿色背景 |
| 3       | 🔵 蓝色背景 |
| 4       | 🟣 紫色背景 |
| 5       | 🟠 金色背景 |
| 6+      | 🔴 红色背景 |

## ⚙️ 设置选项

可按下 `F7` 通过设置面板可以调整以下选项：

- 🎨 视觉增强：所有物品图标根据品质显示彩色背景（背包、装备栏、地面等）
- 🎵 音效反馈：搜索物品时根据品质播放不同音效
- ⚡ 低级物品加速：降低低品质物品的搜索时间，提升游戏流畅度

## ❓ 常见问题

**Q: 看不到彩色背景？**
A:

- 确保Mod已启用
- 普通物品（Quality 1）不显示背景，保持原有透明状态
- 按下F7，检查设置中是否开启了品质背景功能

**Q: 听不到音效？**
A:

- 音效只在物品品质搜索完成时播放
- 检查设置中是否开启了音效系统
- 确保游戏音效音量不是静音

**Q: 与其他Mod兼容吗？**
A:

本Mod仅修改UI显示，不改变游戏逻辑，理论上和其他MOD兼容性好

**Q: 设置不生效？**
A:

- 设置会自动保存，重启游戏后保持
- 部分设置需要返回游戏菜单，重新进入游戏后才会生效。也可尝试重启游戏

> 作者非游戏专业开发者，若发现bug欢迎pr修复，谢谢~

## 更新日志

[CHANGELOG.md](CHANGELOG.md)

## 许可证

MIT License
