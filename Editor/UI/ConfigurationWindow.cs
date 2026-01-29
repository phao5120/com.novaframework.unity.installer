/// -------------------------------------------------------------------------------
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using UnityEngine.SceneManagement;

namespace NovaFramework.Editor.Installer
{
    internal class ConfigurationWindow : EditorWindow
    {
        private static ConfigurationWindow _window;
        
        private int _selectedTab = 0;
        private string[] _tabNames = { "插件配置", "环境目录配置", "程序集配置" };
        
        private PackageConfigurationView _packageView;
        private DirectoryConfigurationView _directoryView;
        private AssemblyConfigurationView _assemblyView;
        
        // 添加自动配置相关字段
        private bool _isAutoConfiguring = false;
        private float _autoConfigTimer = 0f;
        private int _currentStep = 0;
        
        // 添加菜单项以启动自动配置
        [MenuItem("Tools/启动自动配置向导 &_F8", false, 1)]
        public static void StartAutoConfigurationWizard()
        {
            StartAutoConfiguration();
        }
        
        public static void ShowWindow()
        {
            _window = (ConfigurationWindow)EditorWindow.GetWindow(typeof(ConfigurationWindow));
            _window.titleContent = new GUIContent("框架配置中心");
            _window.minSize = new Vector2(800, 700);
            _window.Show();
        }
        
        // 添加自动配置方法
        public static void StartAutoConfiguration()
        {
            ShowWindow();
            _window._isAutoConfiguring = true;
            _window._currentStep = 0;
            _window._autoConfigTimer = 0f;
        }
        
        void OnEnable()
        {
            //重新加载数据
            PackageManager.LoadData();
            
            _packageView = new PackageConfigurationView();
            _directoryView = new DirectoryConfigurationView();
            _assemblyView = new AssemblyConfigurationView();
        }
        
        void OnGUI()
        {
            // 处理自动配置流程
            if (_isAutoConfiguring)
            {
                HandleAutoConfiguration();
            }
            
            // 添加大号标题
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 24;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("框架配置中心", titleStyle);
            EditorGUILayout.Space(10);
            
            // 标签页选择，增加高度和字体大小
            GUIStyle tabStyle = new GUIStyle(GUI.skin.button);
            tabStyle.fontSize = 16;
            tabStyle.fixedHeight = 35;
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames, tabStyle);
            EditorGUILayout.Space(10);
            
            // 根据选中的标签页显示不同内容
            switch (_selectedTab)
            {
                case 0:
                    _packageView.DrawView();
                    break;
                case 1:
                    _directoryView.DrawView();
                    break;
                case 2:
                    if (_assemblyView != null)
                    {
                        _assemblyView.DrawView();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("程序集配置视图未初始化", MessageType.Error);
                    }
                    break;
            }

        }
        
        private void HandleAutoConfiguration()
        {
            _autoConfigTimer += Time.deltaTime;
            
            // 每隔一段时间执行一步
            if (_autoConfigTimer >= 1.0f) // 每秒执行一步
            {
                _autoConfigTimer = 0f;
                
                switch (_currentStep)
                {
                    case 0: // 第一步：自动完成插件配置
                        // 在插件配置视图中自动保存（通过调用相应方法）
                        // 这里模拟点击保存按钮的操作
                        GitManager.HandleSelectPackages(DataManager.LoadPersistedSelectedPackages(), PackageManager.GetSelectedPackageNames());
                        DataManager.SavePersistedSelectedPackages(PackageManager.GetSelectedPackageNames());
                        UnityEditor.PackageManager.Client.Resolve();
                        Debug.Log("自动配置：完成插件配置并保存");
                        break;
                    case 1: // 第二步：自动完成环境目录配置
                        // 刷新数据确保环境变量配置是最新的
                        _directoryView.RefreshData();
                        // 保存目录配置
                        _directoryView.SaveDirectoryConfiguration();
                        Debug.Log("自动配置：完成环境目录配置并保存");
                        break;
                    case 2: // 第三步：自动完成程序集配置
                        // 刷新数据确保程序集配置是最新的
                        _assemblyView.RefreshData();
                        // 保存程序集配置
                        _assemblyView.SaveAssemblyConfiguration();
                        Debug.Log("自动配置：完成程序集配置并保存");
                        break;
                    case 3: // 第四步：自动导出配置
                        // 调用导出配置方法
                        ExportConfigurationMenu.ExportConfiguration();
                        Debug.Log("自动配置：完成配置导出");
                        break;
                    case 4: // 第五步：提示运行游戏
                        EditorUtility.DisplayDialog("配置完成", "框架配置已全部完成！\n现在您可以开始运行游戏了。", "确定");
                        _isAutoConfiguring = false; // 结束自动配置流程
                        return;
                }
                
                _currentStep++;
            }
        }
             
    }
}