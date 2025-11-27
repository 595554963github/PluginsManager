using System;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace PluginManagerWPF
{
    public partial class MainWindow : Window
    {
        private PluginManager pluginManager;
        private ListSortDirection currentSortDirection = ListSortDirection.Ascending;
        private ICollectionView? pluginsView;

        public MainWindow()
        {
            InitializeComponent();
            pluginManager = new PluginManager();
            InitializePluginsView();
        }

        private void InitializePluginsView()
        {
            pluginsView = CollectionViewSource.GetDefaultView(pluginManager.Plugins);
            PluginsDataGrid.ItemsSource = pluginsView;
            pluginsView.Filter = PluginFilter;
            SortByDisplayName(currentSortDirection);
        }
        private bool PluginFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox?.Text))
                return true;

            var plugin = item as PluginInfo;
            if (plugin == null)
                return false;

            string searchText = SearchTextBox.Text.Trim().ToLower();
            return plugin.DisplayName.ToLower().Contains(searchText) ||
                   plugin.FileName.ToLower().Contains(searchText) ||
                   plugin.Name.ToLower().Contains(searchText);
        }
        private void RefreshPluginList()
        {
            pluginsView?.Refresh();
        }

        private void SortByDisplayName(ListSortDirection direction)
        {
            if (pluginsView == null) return;

            pluginsView.SortDescriptions.Clear();
            pluginsView.SortDescriptions.Add(new SortDescription("DisplayName", direction));
            currentSortDirection = direction;

            SortButton.Content = direction == ListSortDirection.Ascending ? "排序↑" : "排序↓";
        }

        private PluginInfo? GetSelectedPlugin()
        {
            return PluginsDataGrid.SelectedItem as PluginInfo;
        }
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            pluginsView?.Refresh();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var plugin = GetSelectedPlugin();
            if (plugin != null)
            {
                var downloadWindow = new DownloadProgressWindow(plugin);
                downloadWindow.Owner = this;
                downloadWindow.DownloadCompleted += (s, success) =>
                {
                    if (success)
                    {
                        pluginManager.InitializePlugins();
                        RefreshPluginList();
                        MessageBox.Show($"{plugin.DisplayName}下载完成！", "成功",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                };
                downloadWindow.ShowDialog();
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            var plugin = GetSelectedPlugin();
            if (plugin != null)
            {
                pluginManager.LaunchPlugin(plugin);
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            var plugin = GetSelectedPlugin();
            if (plugin != null && plugin.IsDownloaded)
            {
                var result = MessageBox.Show($"确定要卸载{plugin.DisplayName}吗？", "确认卸载",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        pluginManager.UninstallPlugin(plugin);
                        RefreshPluginList();

                        MessageBox.Show($"{plugin.DisplayName}卸载完成！", "成功",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("重启程序"))
                        {
                            var restartResult = MessageBox.Show(
                                $"{ex.Message}\n\n是否立即重启插件管理器？",
                                "需要重启",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

                            if (restartResult == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                                    var mainModule = currentProcess?.MainModule;

                                    if (mainModule != null && !string.IsNullOrEmpty(mainModule.FileName))
                                    {
                                        System.Diagnostics.Process.Start(mainModule.FileName);
                                        Application.Current.Shutdown();
                                    }
                                    else
                                    {
                                        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                                        if (!string.IsNullOrEmpty(assemblyLocation))
                                        {
                                            System.Diagnostics.Process.Start(assemblyLocation);
                                            Application.Current.Shutdown();
                                        }
                                        else
                                        {
                                            MessageBox.Show("无法确定程序路径，请手动重启程序。", "提示",
                                                MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                    }
                                }
                                catch (Exception restartEx)
                                {
                                    MessageBox.Show($"重启失败: {restartEx.Message}\n请手动重启程序。", "错误",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"卸载失败: {ex.Message}", "错误",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        RefreshPluginList();
                    }
                }
            }
            else if (plugin != null && !plugin.IsDownloaded)
            {
                MessageBox.Show("该插件尚未下载，无法卸载", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("请先选择一个插件", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var newDirection = currentSortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            SortByDisplayName(newDirection);
        }      
    }
}
