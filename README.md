# 统一游戏开发框架配置中心

这是一个Unity编辑器扩展工具，用于管理和配置公司统一的游戏开发框架。

## 功能特性

1. **配置中心**：
   - 插件配置：管理项目中启用的插件
   - 环境目录配置：设置各种路径变量
   - 程序集配置：管理程序集的加载顺序和标签

2. **包管理功能**：
   - 自动处理包依赖关系（勾选插件时自动选择依赖，取消勾选时检查是否被依赖）
   - 支持基础包和普通包的区分处理
   - 基础包解压到Assets\Sources目录，普通包安装到Packages目录

3. **程序集配置**：
   - 支持配置程序集的order、name、tags
   - tags包括：Core、Module、Game、Tutorial、Test、Shared、Hotfix
   - 配置保存到Assets\Resources目录

4. **自动安装功能**：
   - 一键自动安装所有必需包及其依赖
   - 自动创建环境目录结构
   - 自动配置程序集设置
   - 安装完成后自动打开主场景

5. **环境验证功能**：
   - 验证路径配置是否正确
   - 检查必要目录是否存在
   - 提供详细的验证结果反馈

6. **帮助系统**：
   - 内置使用指南
   - 快速访问菜单说明
   - 操作步骤指导

## 快捷菜单

- `Tools/配置中心` (Ctrl+Shift+C) - 打开框架配置中心
- `Tools/自动安装` (F8) - 启动自动安装流程
- `Tools/验证环境` - 验证当前环境配置
- `Tools/帮助` - 显示使用指南
- `Tools/检查更新` - 检查框架安装器更新
- `Tools/重置安装` (Ctrl+Alt+R) - 重置所有安装配置

## 配置文件

- `Configs/repo_manifest.xml`：定义可安装的插件包及其依赖关系
- `Configs/DefaultFrameworkConfig.json`：默认配置文件
- `Assets/Resources/FrameworkSetting.asset`：框架设置
- `Assets/Resources/SystemVariables.json`：系统变量配置
- `Assets/Resources/AssemblyConfig.json`：程序集配置

## 使用方法

1. 在Unity中打开菜单 `Tools/配置中心` 打开框架配置中心
2. 或使用快捷键 `Ctrl+Shift+C` 打开配置中心
3. 使用 `F8` 键启动自动安装流程
4. 安装完成后使用 `Tools/验证环境` 检查配置是否正确

## 依赖关系处理

- 勾选插件时自动选中其依赖包
- 取消勾选时检查是否有其他包依赖此包，如有则不允许取消
- 必需包自动选中且无法取消

## 扩展性

此工具可打包成.unitypackage包分发给其他项目使用，新项目导入后即可通过菜单打开安装向导。

## 注意事项

1. 确保系统已安装Git并添加到PATH环境变量
2. 需要网络连接以下载Git仓库和框架包
3. 首次使用建议先查看帮助界面的说明
4. 重启Unity后配置才会完全生效
5. 自动安装完成后会自动打开Assets/Scenes/main.unity场景

## 文件结构

```
Assets/
└── Editor/
    └── FrameworkInstaller/
        ├── Scripts/
        │   ├── AutoInstall/                        # 自动安装功能
        │   │   └── AutoInstallManager.cs          # 自动安装管理器
        │   ├── Definitions/                        # 类型定义文件
        │   │   ├── AssemblyConfigData.cs          # 程序集配置数据
        │   │   ├── Constants.cs                   # 常量定义
        │   │   └── FrameworkSetting.cs            # 框架设置
        │   ├── Data/                              # 数据管理
        │   │   └── DataManager.cs                 # 数据管理器
        │   ├── UI/                                # 界面文件
        │   │   ├── ConfigurationWindow.cs         # 配置中心主窗口
        │   │   ├── HelpWindow.cs                  # 帮助窗口
        │   │   ├── PluginConfigurationView.cs     # 插件配置界面
        │   │   ├── DirectoryConfigurationView.cs  # 目录配置界面
        │   │   └── AssemblyConfigurationView.cs   # 程序集配置界面
        │   ├── Tools/                             # 工具类文件
        │   │   ├── BuildFrameworkInstaller.cs     # 导出安装包工具
        │   │   ├── DownloadHelper.cs              # 下载辅助类
        │   │   ├── EnvironmentValidator.cs        # 环境验证器
        │   │   ├── GitHelper.cs                   # Git辅助类
        │   │   ├── PackageManagerHelper.cs        # 包管理辅助类
        │   │   ├── PackageManifestParser.cs       # 包清单解析器
        │   │   ├── ResetManager.cs                # 重置管理器
        │   │   └── UpdateManager.cs               # 更新管理器
        │   └── Menus/                             # 菜单入口
        │       └── MainMenu.cs                    # 主菜单
        └── Configs/
            ├── repo_manifest.xml
            └── DefaultFrameworkConfig.json
```