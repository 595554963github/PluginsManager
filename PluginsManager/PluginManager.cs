using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace PluginManagerWPF
{
    public class PluginInfo : INotifyPropertyChanged
    {
        private bool isDownloaded;

        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsDownloaded
        {
            get => isDownloaded;
            set
            {
                if (isDownloaded != value)
                {
                    isDownloaded = value;
                    OnPropertyChanged(nameof(IsDownloaded));
                }
            }
        }
        public bool IsExternalTool { get; set; }
        public bool IsBuiltInDll { get; set; }
        public bool IsZipFile { get; set; }
        public string ExeFileName { get; set; } = string.Empty;
        public string ExtractFolder { get; set; } = string.Empty;

        public string PluginType
        {
            get
            {
                if (IsBuiltInDll) return "类库文件";
                if (IsExternalTool) return "可执行程序";
                if (IsZipFile) return "压缩包文件";
                return "PE文件";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PluginManager
    {
        public ObservableCollection<PluginInfo> Plugins { get; } = new ObservableCollection<PluginInfo>();
        public string PluginsDirectory { get; }

        public PluginManager()
        {
            PluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            InitializePluginList();
            InitializePlugins();
        }

        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            try
            {
                string? assemblyName = new AssemblyName(args.Name).Name;

                if (!string.IsNullOrEmpty(assemblyName))
                {
                    string possiblePath = Path.Combine(PluginsDirectory, $"{assemblyName}.dll");

                    if (File.Exists(possiblePath))
                    {
                        return Assembly.LoadFrom(possiblePath);
                    }

                    possiblePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.dll");
                    if (File.Exists(possiblePath))
                    {
                        return Assembly.LoadFrom(possiblePath);
                    }

                    possiblePath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.dll");
                    if (File.Exists(possiblePath))
                    {
                        return Assembly.LoadFrom(possiblePath);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"程序集解析失败，名称为{args.Name}: {ex.Message}");
                return null;
            }
        }

        private void InitializePluginList()
        {
            var pluginList = new List<PluginInfo>
            {
                new PluginInfo
                {
                    Name = "UniversalFileExtractor",
                    DisplayName = "万能二进制提取器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/FileExtractor.dll",
                    FileName = "FileExtractor.dll",
                    IsExternalTool = false,
                    IsBuiltInDll = true
                },
                new PluginInfo
                {
                    Name = "UniversalByteRemover",
                    DisplayName = "万能字节移除器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/ByteRemover.dll",
                    FileName = "ByteRemover.dll",
                    IsExternalTool = false,
                    IsBuiltInDll = true
                },
                new PluginInfo
                {
                    Name = "quickbmsbatch",
                    DisplayName = "quickbms批量提取器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/quickbmsbatch.dll",
                    FileName = "quickbmsbatch.dll",
                    IsExternalTool = false,
                    IsBuiltInDll = true
                },
                new PluginInfo
                {
                    Name = "SuperToolbox",
                    DisplayName = "超级工具箱",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/Super-toolbox.dll",
                    FileName = "Super-toolbox.dll",
                    IsExternalTool = false,
                    IsBuiltInDll = true
                },
                new PluginInfo
                {
                    Name = "CriFsV2Lib.Definitions.dll",
                    DisplayName = "超级工具箱dll依赖",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/CriFsV2Lib.Definitions.dll",
                    FileName = "CriFsV2Lib.Definitions.dll",
                    IsExternalTool = false,
                    IsBuiltInDll = true
                },
                new PluginInfo
                {
                    Name = "Sofdec2Viewer",
                    DisplayName = "USM视频查看器汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/Sofdec2_Viewer.exe",
                    FileName = "Sofdec2_Viewer.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "RadVideo",
                    DisplayName = "bink视频播放器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/radvideo64.exe",
                    FileName = "radvideo64.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "QuickBMS",
                    DisplayName = "quickbms汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/quickbms.exe",
                    FileName = "quickbms.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "quickbms_4gb_files",
                    DisplayName = "quickbms_4gb_files汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/quickbms_4gb_files.exe",
                    FileName = "quickbms_4gb_files.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "RioX",
                    DisplayName = "RioX汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/RioX.exe",
                    FileName = "RioX汉化版.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "PakExplorer",
                    DisplayName = "LUCA system pak解包器汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/pak_explorer.exe",
                    FileName = "pak_explorer.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "PSound",
                    DisplayName = "PlayStation音频提取器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/PSound.exe",
                    FileName = "PSound.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "WinAsar",
                    DisplayName = "WinAsar汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/WinAsar.exe",
                    FileName = "WinAsar汉化版.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "AudioCUE",
                    DisplayName = "AudioCUE编辑器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/ACE.exe",
                    FileName = "ACE.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "WinPCK",
                    DisplayName = "完美世界pck解包工具",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/WinPCK.exe",
                    FileName = "WinPCK.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "RetsukoSoundTool",
                    DisplayName = "3DS/Wiiu音频提取器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/RetsukoSoundTool.exe",
                    FileName = "RetsukoSoundTool.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "Motrix",
                    DisplayName = "免费不限速下载器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/Motrix_x64.exe",
                    FileName = "Motrix_x64.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "UmodelHelper",
                    DisplayName = "Umodel辅助器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/UmodelHelper.exe",
                    FileName = "UmodelHelper.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "cmake-gui",
                    DisplayName = "cmakegui汉化版",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/cmake-gui.exe",
                    FileName = "cmake-gui.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "QuickWaveBank",
                    DisplayName = "xwb打包解包工具",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/QuickWaveBank.exe",
                    FileName = "QuickWaveBank.exe",
                    IsExternalTool = true,
                    IsBuiltInDll = false
                },
                new PluginInfo
                {
                    Name = "CpkFileBuilder",
                    DisplayName = "CPK官方打包解包工具",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/CpkFileBuilder.zip",
                    FileName = "CpkFileBuilder.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "CpkFileBuilder.exe",
                    ExtractFolder = "CpkFileBuilder"
                },
                new PluginInfo
                {
                    Name = "IDM",
                    DisplayName = "IDM下载器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/IDM.zip",
                    FileName = "IDM.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "IDMan.exe",
                    ExtractFolder = "IDM"
                },
                new PluginInfo
                {
                    Name = "PVRViewer",
                    DisplayName = "世嘉游戏PVR查看器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/PVRViewer.zip",
                    FileName = "PVRViewer.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "PVRViewer.exe",
                    ExtractFolder = "PVRViewer"
                },
                new PluginInfo
                {
                    Name = "GD-ROM-Explorer",
                    DisplayName = "世嘉游戏rom解包器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/GD-ROM-Explorer.zip",
                    FileName = "GD-ROM-Explorer.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "GD-ROM Explorer.exe",
                    ExtractFolder = "GD-ROM-Explorer"
                },
                new PluginInfo
                {
                    Name = "CDmage",
                    DisplayName = "光盘rom解包器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/CDmage.zip",
                    FileName = "CDmage.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "CDmage.exe",
                    ExtractFolder = "CDmage"
                },
                new PluginInfo
                {
                    Name = "VGMTrans",
                    DisplayName = "VGMTrans音频提取器",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/VGMTrans.zip",
                    FileName = "VGMTrans.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "VGMTrans.exe",
                    ExtractFolder = "VGMTrans"
                },
                new PluginInfo
                {
                    Name = "CitricComposer",
                    DisplayName = "柠檬音乐工坊",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/CitricComposer.zip",
                    FileName = "CitricComposer.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "Citric Composer.exe",
                    ExtractFolder = "CitricComposer"
                },
                new PluginInfo
                {
                    Name = "toolbox",
                    DisplayName = "任天堂toolbox",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/toolbox.zip",
                    FileName = "toolbox.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "toolbox.exe",
                    ExtractFolder = "toolbox"
                },
                new PluginInfo
                {
                    Name = "FSBank",
                    DisplayName = "FSB打包工具",
                    DownloadUrl = "https://gitee.com/valkylia-goddess/AssetStudio-Neptune/releases/download/down/fsbank.zip",
                    FileName = "fsbank.zip",
                    IsExternalTool = true,
                    IsBuiltInDll = false,
                    IsZipFile = true,
                    ExeFileName = "fsbank.exe",
                    ExtractFolder = "fsbank"
                }
            };

            foreach (var plugin in pluginList)
            {
                Plugins.Add(plugin);
            }
        }

        public void InitializePlugins()
        {
            if (!Directory.Exists(PluginsDirectory))
            {
                Directory.CreateDirectory(PluginsDirectory);
            }

            foreach (var plugin in Plugins)
            {
                string filePath;
                if (plugin.IsZipFile && !string.IsNullOrEmpty(plugin.ExtractFolder))
                {
                    filePath = Path.Combine(PluginsDirectory, plugin.ExtractFolder, plugin.ExeFileName);
                }
                else
                {
                    filePath = Path.Combine(PluginsDirectory, plugin.FileName);
                }
                plugin.IsDownloaded = File.Exists(filePath);
            }
        }

        public void LaunchPlugin(PluginInfo plugin)
        {
            if (IsDependencyFile(plugin))
            {
                MessageBox.Show($"{plugin.DisplayName} 是依赖文件，无法直接启动。", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!plugin.IsDownloaded)
            {
                MessageBox.Show($"请先下载{plugin.DisplayName}", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string filePath;
                string workingDirectory = PluginsDirectory;

                if (plugin.IsZipFile)
                {
                    filePath = Path.Combine(PluginsDirectory, plugin.ExtractFolder, plugin.ExeFileName);
                    workingDirectory = Path.Combine(PluginsDirectory, plugin.ExtractFolder);
                }
                else
                {
                    filePath = Path.Combine(PluginsDirectory, plugin.FileName);
                }

                if (!File.Exists(filePath))
                {
                    plugin.IsDownloaded = false;
                    MessageBox.Show($"{plugin.DisplayName}文件不存在，请重新下载", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (plugin.IsExternalTool || plugin.IsZipFile)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                        WorkingDirectory = workingDirectory
                    });
                }
                else if (plugin.IsBuiltInDll)
                {
                    LaunchBuiltInDll(plugin, filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动{plugin.DisplayName}失败:{ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsDependencyFile(PluginInfo plugin)
        {
            var dependencyFiles = new HashSet<string>
            {
                "CriFsV2Lib.Definitions.dll",
            };

            return dependencyFiles.Contains(plugin.FileName);
        }

        private void LaunchBuiltInDll(PluginInfo plugin, string filePath)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(filePath);

                var formTypes = assembly.GetTypes()
                    .Where(t => typeof(System.Windows.Forms.Form).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                if (formTypes.Count == 0)
                {
                    throw new Exception($"在{plugin.FileName}中找不到窗体类");
                }

                Type? mainFormType = formTypes.FirstOrDefault(t =>
                    t.Name.Contains("Main", StringComparison.OrdinalIgnoreCase)) ?? formTypes[0];

                if (mainFormType == null)
                {
                    throw new Exception($"无法确定主窗体类型");
                }

                System.Windows.Forms.Form? existingInstance = System.Windows.Forms.Application.OpenForms.Cast<System.Windows.Forms.Form>()
                    .FirstOrDefault(form => form.GetType() == mainFormType);

                if (existingInstance != null)
                {
                    if (existingInstance.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                    {
                        existingInstance.WindowState = System.Windows.Forms.FormWindowState.Normal;
                    }
                    existingInstance.BringToFront();
                    existingInstance.Focus();
                    return;
                }

                System.Windows.Forms.Form? mainForm = Activator.CreateInstance(mainFormType) as System.Windows.Forms.Form;
                if (mainForm != null)
                {
                    mainForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    mainForm.Show();
                }
                else
                {
                    throw new Exception($"无法创建窗体实例:{mainFormType.FullName}");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                string errorDetails = "类型加载失败:\n";
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    errorDetails += $"- {loaderException?.Message}\n";
                }

                MessageBox.Show($"加载DLL插件失败:{errorDetails}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载DLL插件失败:{ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UninstallPlugin(PluginInfo plugin)
        {
            try
            {
                string filePath = Path.Combine(PluginsDirectory, plugin.FileName);

                if (File.Exists(filePath))
                {
                    if (plugin.IsBuiltInDll && IsDllLoaded(plugin.FileName))
                    {
                        throw new Exception($"{plugin.DisplayName}正在使用中,请重启插件管理器后再尝试卸载。");
                    }
                    File.Delete(filePath);
                }

                if (plugin.IsZipFile && !string.IsNullOrEmpty(plugin.ExtractFolder))
                {
                    string extractFolderPath = Path.Combine(PluginsDirectory, plugin.ExtractFolder);
                    if (Directory.Exists(extractFolderPath))
                    {
                        Directory.Delete(extractFolderPath, true);
                    }
                }

                plugin.IsDownloaded = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"卸载{plugin.DisplayName}失败: {ex.Message}", ex);
            }
        }

        private bool IsDllLoaded(string fileName)
        {
            try
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                string targetPath = Path.Combine(PluginsDirectory, fileName);

                return loadedAssemblies.Any(asm =>
                    !asm.IsDynamic &&
                    !string.IsNullOrEmpty(asm.Location) &&
                    asm.Location.Equals(targetPath, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}
