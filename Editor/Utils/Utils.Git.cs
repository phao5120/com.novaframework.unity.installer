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

using System.IO;

namespace NovaEngine.Installer
{
    /// 编辑环境下的工具箱辅助类
    internal static partial class Utils
    {
        /// <summary>
        /// 提供Git指令操作的工具类
        /// </summary>
        public static class Git
        {
            /// <summary>
            /// Git可执行文件路径（优先用环境变量，也可手动指定）
            /// </summary>
            private static string GitExecutableFilePath => System.Environment.OSVersion.Platform == System.PlatformID.Win32NT
                ? "git.exe" // 已加环境变量，直接用
                : "/usr/bin/git"; // macOS/Linux

            /// <summary>
            /// 执行Git命令
            /// </summary>
            /// <param name="command">Git子命令（如"pull"、"commit -m 'update'"）</param>
            /// <param name="workingDir">Git仓库根目录（Unity项目根目录）</param>
            /// <param name="output">命令输出结果</param>
            /// <param name="error">错误信息</param>
            /// <returns>是否执行成功（退出码0为成功）</returns>
            private static bool ExecuteGitCommand(string command, string workingDir, out string output, out string error)
            {
                output = string.Empty;
                error = string.Empty;

                // 校验工作目录
                if (!Directory.Exists(workingDir))
                {
                    error = $"工作目录不存在：{workingDir}";
                    return false;
                }

                try
                {
                    // 配置进程启动信息
                    var processStartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = GitExecutableFilePath,
                        Arguments = command,
                        WorkingDirectory = workingDir,
                        RedirectStandardOutput = true, // 捕获输出
                        RedirectStandardError = true,  // 捕获错误
                        UseShellExecute = false,       // 必须为false才能重定向输出
                        CreateNoWindow = true,         // 不弹出命令行窗口
                        StandardOutputEncoding = System.Text.Encoding.UTF8, // 解决中文乱码
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    };

                    using (var process = new System.Diagnostics.Process { StartInfo = processStartInfo })
                    {
                        process.Start();
                        // 异步读取输出（避免死锁）
                        output = process.StandardOutput.ReadToEnd();
                        error = process.StandardError.ReadToEnd();
                        process.WaitForExit(); // 等待命令执行完成

                        // 退出码0表示成功
                        return process.ExitCode == 0;
                    }
                }
                catch (System.Exception e)
                {
                    error = $"执行Git命令失败：{e.Message}";
                    return false;
                }
            }

            /// <summary>
            /// 测试Git版本
            /// </summary>
            public static void ExcuteGitVersion(string workingDir)
            {
                bool success = ExecuteGitCommand("--version", workingDir, out string output, out string error);
                if (success)
                {
                    Logger.Info($"Git版本：{output}");
                }
                else
                {
                    Logger.Error($"获取Git版本失败：{error}");
                }
            }

            /// <summary>
            /// 克隆代码
            /// </summary>
            public static void ExcuteGitClone(string httpUrl, string workingDir)
            {
                string cmd = $"clone {httpUrl}";

                bool success = ExecuteGitCommand(cmd, workingDir, out string output, out string error);
                if (success)
                {
                    Logger.Info($"git clone {httpUrl} 成功： {output}");
                }
                else
                {
                    Logger.Error($"git clone {httpUrl} 失败： {error}");
                }
            }

            /// <summary>
            /// 拉取代码
            /// </summary>
            public static void ExcuteGitPull(string workingDir)
            {
                string cmd = "pull";
                bool success = ExecuteGitCommand(cmd, workingDir, out string output, out string error);
                if (success)
                {
                    Logger.Info($"git pull {workingDir} 成功： {output}");
                }
                else
                {
                    Logger.Error($"git pull {workingDir} 失败： {error}");
                }
            }
        }
    }
}
