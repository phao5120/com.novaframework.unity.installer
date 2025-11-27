/// -------------------------------------------------------------------------------
/// NovaEngine Installer Framework
///
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NovaEngine.Installer
{
    /// <summary>
    /// 包展示窗口对象类，用于显示当前插件包的详细信息及安装情况
    /// </summary>
    public class PackageWindow : EditorWindow
    {
        // 插件信息类
        private class PackageInfo
        {
            public string GitUrl;
            public string Description;

            public PackageInfo(string gitUrl, string description)
            {
                GitUrl = gitUrl;
                Description = description;
            }
        }

        // 所有可配置的插件信息（包名 -> PackageInfo）
        private static readonly Dictionary<string, PackageInfo> PackageInfos = new()
    {
        {
            "com.code-philosophy.hybridclr",
            new PackageInfo(
                "https://github.com/focus-creative-games/hybridclr_unity.git",
                "HybridCLR - 热更新解决方案")
        },
        {
            "com.novaframework.unity.core.boot",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.core.boot.git",
                "NovaFramework核心启动模块")
        },
        {
            "com.novaframework.unity.core.configure",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.core.configure.git",
                "NovaFramework配置管理系统")
        },
        {
            "com.novaframework.unity.fairygui",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.fairygui.git",
                "FairyGUI UI系统集成")
        },
        {
            "com.novaframework.unity.litjson",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.litjson.git",
                "LitJSON JSON处理库")
        },
        {
            "com.novaframework.unity.luban",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.luban.git",
                "Luban配置表工具")
        },
        {
            "com.novaframework.unity.protobuf",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.protobuf.git",
                "Google Protocol Buffers")
        },
        {
            "com.novaframework.unity.unitask",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.unitask.git",
                "UniTask异步编程库")
        },
        {
            "com.yoseasoft.gooasset",
            new PackageInfo(
                "https://github.com/yoseasoft/GooAsset.git",
                "GooAsset资源管理系统")
        },
        {
            "com.yoseasoft.novaframework",
            new PackageInfo(
                "https://github.com/yoseasoft/NovaFramework.git",
                "NovaFramework核心框架")
        },
        {
            "com.novaframework.unity.sample",
            new PackageInfo(
                "https://github.com/yoseasoft/com.novaframework.unity.sample.git",
                "NovaFramework示例项目")
        }
    };

        // 当前选择的依赖项
        private readonly List<string> _selectedDependencies = new();

        // 滚动视图位置
        private Vector2 _scrollPosition;

        [MenuItem("Tools/底层框架配置")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageWindow>("底层框架配置管理");
            // 设置窗口初始大小
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(1200, 1000);
        }

        private void OnEnable()
        {
            LoadCurrentManifest();
        }

        // 读取现有manifest.json并初始化选择状态
        private void LoadCurrentManifest()
        {
            _selectedDependencies.Clear();

            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

            if (!File.Exists(manifestPath))
            {
                Debug.Log("找不到manifest.json，开始空选择状态");
                return;
            }

            try
            {
                string json = File.ReadAllText(manifestPath);
                var dependenciesSection = ExtractDependenciesSection(json);

                if (dependenciesSection == null)
                {
                    Debug.Log("manifest.json中未找到dependencies部分");
                    return;
                }

                // 解析依赖项
                var dependencies = ParseDependencies(dependenciesSection);

                // 检查哪些预定义依赖已存在
                foreach (var packageName in PackageInfos.Keys)
                {
                    if (dependencies.ContainsKey(packageName))
                    {
                        _selectedDependencies.Add(packageName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"读取manifest.json时出错: {e.Message}");
            }
        }

        private void OnGUI()
        {
            // 设置更大的边距
            GUILayout.Space(15);

            // 标题区域
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("底层框架配置管理", EditorStyles.largeLabel, GUILayout.ExpandWidth(true));
                EditorGUILayout.HelpBox("选择需要包含在项目中的底层框架组件", MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            // 滚动视图开始
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            // 使用Box样式创建分组
            EditorGUILayout.BeginVertical("Box");
            {
                // 显示所有可配置依赖项的复选框
                foreach (var package in PackageInfos)
                {
                    EditorGUILayout.BeginVertical("HelpBox");
                    {
                        // 单行显示：选择框 + 包名 + 描述
                        EditorGUILayout.BeginHorizontal();
                        {
                            // 选择框
                            bool isSelected = _selectedDependencies.Contains(package.Key);
                            bool newState = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

                            if (newState != isSelected)
                            {
                                if (newState)
                                    _selectedDependencies.Add(package.Key);
                                else
                                    _selectedDependencies.Remove(package.Key);
                            }

                            // 包名和描述在同一行
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    // 包名
                                    EditorGUILayout.LabelField(package.Key, EditorStyles.boldLabel, GUILayout.Width(300));

                                    // 描述文本
                                    EditorGUILayout.LabelField(package.Value.Description, EditorStyles.wordWrappedLabel);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);
                }
            }
            EditorGUILayout.EndVertical();

            // 滚动视图结束
            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);

            // 操作按钮区域
            EditorGUILayout.BeginVertical("Box");
            {
                // 操作按钮
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    GUILayout.FlexibleSpace();

                    // 全选/取消全选按钮
                    bool allSelected = _selectedDependencies.Count == PackageInfos.Count;
                    GUIContent toggleAllContent = allSelected ?
                        new GUIContent("取消全选") :
                        new GUIContent("全选");

                    if (GUILayout.Button(toggleAllContent, GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        if (allSelected)
                        {
                            _selectedDependencies.Clear();
                        }
                        else
                        {
                            _selectedDependencies.Clear();
                            _selectedDependencies.AddRange(PackageInfos.Keys);
                        }
                    }

                    GUILayout.Space(20);

                    if (GUILayout.Button("刷新列表", GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        LoadCurrentManifest();
                        ShowNotification(new GUIContent("已刷新依赖列表"));
                    }

                    GUILayout.Space(20);

                    if (GUILayout.Button("保存配置", GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        ApplyChanges();
                        ShowNotification(new GUIContent("配置已保存到manifest.json"));
                    }

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);
        }

        // 应用用户选择到manifest.json
        private void ApplyChanges()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

            try
            {
                // 1. 读取整个manifest.json文件
                string originalJson = File.Exists(manifestPath) ?
                    File.ReadAllText(manifestPath) :
                    "{\n  \"dependencies\": {\n  }\n}";

                // 2. 提取dependencies部分
                var dependenciesSection = ExtractDependenciesSection(originalJson);
                if (dependenciesSection == null)
                {
                    Debug.LogError("无法解析manifest.json的dependencies部分");
                    return;
                }

                // 3. 解析现有依赖项
                var dependencies = ParseDependencies(dependenciesSection);

                // 4. 更新配置的Git依赖
                foreach (var package in PackageInfos)
                {
                    if (_selectedDependencies.Contains(package.Key))
                    {
                        dependencies[package.Key] = package.Value.GitUrl;
                    }
                    else
                    {
                        if (dependencies.ContainsKey(package.Key))
                        {
                            dependencies.Remove(package.Key);
                        }
                    }
                }

                // 5. 构建新的dependencies部分
                string newDependenciesSection = BuildDependenciesSection(dependencies);

                // 6. 替换原始JSON中的dependencies部分
                string newJson = ReplaceDependenciesSection(originalJson, newDependenciesSection);

                // 7. 保存更新后的manifest
                File.WriteAllText(manifestPath, newJson);

                AssetDatabase.Refresh();

                // 在编辑器中显示成功消息
                Debug.Log($"已更新manifest.json，包含 {_selectedDependencies.Count} 个框架组件");
                Debug.Log("所有原始配置项已保留");
            }
            catch (Exception e)
            {
                Debug.LogError($"更新manifest.json时出错: {e.Message}");
            }
        }

        // 从JSON中提取dependencies部分
        private string ExtractDependenciesSection(string json)
        {
            int dependenciesStart = json.IndexOf("\"dependencies\": {");
            if (dependenciesStart == -1) return null;

            int braceCount = 0;
            int startIndex = -1;
            int endIndex = -1;

            for (int i = dependenciesStart; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    if (startIndex == -1) startIndex = i;
                    braceCount++;
                }
                else if (json[i] == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            if (startIndex != -1 && endIndex != -1)
            {
                return json.Substring(startIndex, endIndex - startIndex + 1);
            }

            return null;
        }

        // 解析dependencies部分为字典
        private Dictionary<string, string> ParseDependencies(string dependenciesJson)
        {
            var dependencies = new Dictionary<string, string>();

            // 去除外部大括号
            string content = dependenciesJson.Trim();
            if (content.StartsWith("{")) content = content.Substring(1, content.Length - 2).Trim();

            var lines = content.Split('\n');
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Length == 0 || trimmedLine == "{" || trimmedLine == "}") continue;

                if (trimmedLine.Contains("\":"))
                {
                    int colonIndex = trimmedLine.IndexOf(':');
                    if (colonIndex != -1)
                    {
                        string key = trimmedLine.Substring(0, colonIndex).Trim().Trim('"');
                        string value = trimmedLine.Substring(colonIndex + 1).Trim().Trim('"', ',', ' ');

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            dependencies[key] = value;
                        }
                    }
                }
            }

            return dependencies;
        }

        // 从依赖字典构建dependencies部分
        private string BuildDependenciesSection(Dictionary<string, string> dependencies)
        {
            // 先添加配置的Git依赖（按定义顺序）
            var sortedEntries = new List<KeyValuePair<string, string>>();

            foreach (var package in PackageInfos)
            {
                if (dependencies.ContainsKey(package.Key))
                {
                    sortedEntries.Add(new KeyValuePair<string, string>(package.Key, dependencies[package.Key]));
                }
            }

            // 然后添加其他依赖（按字母顺序排序）
            var otherEntries = dependencies
                .Where(kvp => !PackageInfos.ContainsKey(kvp.Key))
                .OrderBy(kvp => kvp.Key)
                .ToList();

            // 合并所有条目
            sortedEntries.AddRange(otherEntries);

            // 构建JSON
            string json = "{\n";
            bool first = true;
            foreach (var entry in sortedEntries)
            {
                if (!first)
                {
                    json += ",\n";
                }
                json += $"    \"{entry.Key}\": \"{entry.Value}\"";
                first = false;
            }
            json += "\n  }";

            return json;
        }

        // 替换原始JSON中的dependencies部分
        private string ReplaceDependenciesSection(string originalJson, string newDependenciesSection)
        {
            string oldDependenciesSection = ExtractDependenciesSection(originalJson);
            if (oldDependenciesSection == null) return originalJson;

            return originalJson.Replace(oldDependenciesSection, newDependenciesSection);
        }
    }
}
