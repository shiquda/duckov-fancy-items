@echo off
:: Fancy Items Mod 构建脚本 (Windows批处理版本)
:: 一键构建并准备发布文件

echo 🚀 开始构建 Fancy Items Mod...

:: 清理旧的构建文件
echo 🧹 清理旧的构建文件...
if exist "Release" (
    rmdir /s /q "Release"
)

:: 构建项目
echo 🔨 构建项目...
cd fancy-items
dotnet build --configuration Release

if %ERRORLEVEL% neq 0 (
    echo ❌ 构建失败！
    pause
    exit /b 1
)

echo ✅ 构建成功！

:: 创建发布文件夹结构
echo 📁 创建发布文件夹...
if not exist "..\Release\FancyItems" mkdir "..\Release\FancyItems"

:: 复制必要文件
echo 📋 复制文件到发布目录...

:: 复制 DLL
copy "bin\Release\netstandard2.1\fancy-items.dll" "..\Release\FancyItems\FancyItems.dll" >nul
echo   ✓ FancyItems.dll

:: 复制 Harmony 依赖 DLL
set HARMONY_DLL=bin\Release\netstandard2.1\0Harmony.dll
set NUGET_HARMONY=%USERPROFILE%\.nuget\packages\lib.harmony\2.3.3\lib\net48\0Harmony.dll

if exist "%HARMONY_DLL%" (
    copy "%HARMONY_DLL%" "..\Release\FancyItems\" >nul
    echo   ✓ 0Harmony.dll
) else if exist "%NUGET_HARMONY%" (
    copy "%NUGET_HARMONY%" "..\Release\FancyItems\" >nul
    echo   ✓ 0Harmony.dll (from NuGet cache - net48)
) else (
    echo   ⚠️  0Harmony.dll 未找到！Mod可能无法正常工作
    echo   请检查: %NUGET_HARMONY%
)

:: 复制配置文件
copy "info.ini" "..\Release\FancyItems\" >nul
echo   ✓ info.ini

:: 检查预览图
if not exist "preview.png" (
    echo   ⚠️  preview.png 不存在，请手动添加 256x256 的预览图
) else (
    copy "preview.png" "..\Release\FancyItems\" >nul
    echo   ✓ preview.png
)

:: 显示发布文件夹内容
echo 📦 发布文件夹内容：
dir "..\Release\FancyItems" /b

echo.
echo 🎯 构建完成！安装方法：
echo 1. 将 Release\FancyItems 文件夹复制到：
echo    D:\Program Files (x86)\Steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods\
echo 2. 启动游戏，在Mod菜单中启用 'Fancy Items'
echo 3. 查看游戏内日志终端确认Mod加载成功

cd ..