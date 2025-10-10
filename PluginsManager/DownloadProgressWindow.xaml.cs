using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PluginManagerWPF
{
    public partial class DownloadProgressWindow : Window
    {
        private PluginInfo plugin;
        private CancellationTokenSource cancellationTokenSource;
        private HttpClient? httpClient;
        private string filePath = string.Empty;
        private long totalBytes;
        private bool isDownloading;
        private bool isDownloadCompleted = false;

        private DateTime startTime;
        private long lastBytesRead;
        private DateTime lastUpdateTime;
        private double smoothedSpeed;

        public event Action<object, bool>? DownloadCompleted;

        public DownloadProgressWindow(PluginInfo plugin)
        {
            InitializeComponent();
            this.plugin = plugin;
            cancellationTokenSource = new CancellationTokenSource();
            Title = $"下载 {plugin.DisplayName}";

            Loaded += async (s, e) => await StartDownloadAsync();
            Closing += OnWindowClosing;
        }

        private async Task StartDownloadAsync()
        {
            isDownloading = true;
            isDownloadCompleted = false;
            startTime = DateTime.Now;
            lastBytesRead = 0;
            lastUpdateTime = startTime;
            smoothedSpeed = 0;

            try
            {
                string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
                if (!Directory.Exists(pluginsDir))
                {
                    Directory.CreateDirectory(pluginsDir);
                }

                filePath = Path.Combine(pluginsDir, plugin.FileName);

                using (httpClient = CreateHttpClient())
                {
                    totalBytes = await GetFileSize(plugin.DownloadUrl);
                    await DownloadFileAsync(plugin.DownloadUrl, filePath);
                }

                isDownloadCompleted = true;
                isDownloading = false;

                DownloadCompleted?.Invoke(this, true);
                DialogResult = true;
                Close();
            }
            catch (OperationCanceledException)
            {
                isDownloading = false;
                DeleteIncompleteFile();
                DownloadCompleted?.Invoke(this, false);
                DialogResult = false;
            }
            catch (Exception ex)
            {
                isDownloading = false;
                DeleteIncompleteFile();
                MessageBox.Show($"下载失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DownloadCompleted?.Invoke(this, false);
                DialogResult = false;
            }
            finally
            {
                isDownloading = false;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        private async Task<long> GetFileSize(string url)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, url))
                using (var response = await httpClient!.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.Headers.ContentLength ?? 0;
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        private async Task DownloadFileAsync(string url, string savePath)
        {
            using (var response = await httpClient!.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var contentLength = response.Content.Headers.ContentLength ?? 0;
                if (totalBytes == 0) totalBytes = contentLength;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    var buffer = new byte[8192];
                    var totalRead = 0L;
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationTokenSource.Token);
                        totalRead += bytesRead;

                        if (totalBytes > 0)
                        {
                            var progress = (int)((double)totalRead / totalBytes * 100);
                            UpdateProgress(progress, totalRead);
                        }

                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }
            }
        }

        private void UpdateProgress(int percentage, long bytesRead)
        {
            var currentTime = DateTime.Now;
            var timeDiff = (currentTime - lastUpdateTime).TotalSeconds;

            if (timeDiff >= 0.5)
            {
                double currentSpeed = (bytesRead - lastBytesRead) / timeDiff;

                if (smoothedSpeed == 0)
                {
                    smoothedSpeed = currentSpeed;
                }
                else
                {
                    smoothedSpeed = 0.7 * smoothedSpeed + 0.3 * currentSpeed;
                }

                lastBytesRead = bytesRead;
                lastUpdateTime = currentTime;
            }

            Dispatcher.Invoke(() =>
            {
                DownloadProgressBar.Value = percentage;
                StatusLabel.Text = $"下载中: {percentage}% ({FormatFileSize(bytesRead)} / {FormatFileSize(totalBytes)})";

                var speedInfo = FormatSpeed(smoothedSpeed);
                SpeedLabel.Text = $"{speedInfo.speed:F1} {speedInfo.unit}";

                if (smoothedSpeed > 0 && totalBytes > 0)
                {
                    long remainingBytes = totalBytes - bytesRead;
                    double etaSeconds = remainingBytes / smoothedSpeed;
                    EtaLabel.Text = FormatTime(etaSeconds);
                }
                else
                {
                    EtaLabel.Text = "计算中...";
                }
            });
        }

        private (double speed, string unit) FormatSpeed(double bytesPerSecond)
        {
            if (bytesPerSecond >= 1024 * 1024) // MB/s
            {
                return (bytesPerSecond / (1024 * 1024), "MB/s");
            }
            else if (bytesPerSecond >= 1024) // KB/s
            {
                return (bytesPerSecond / 1024, "KB/s");
            }
            else // B/s
            {
                return (bytesPerSecond, "B/s");
            }
        }

        private string FormatTime(double seconds)
        {
            if (seconds < 60)
            {
                return $"{seconds:F0}秒";
            }
            else if (seconds < 3600)
            {
                return $"{seconds / 60:F0}分{seconds % 60:F0}秒";
            }
            else
            {
                return $"{seconds / 3600:F1}小时";
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.00} {sizes[order]}";
        }

        private void DeleteIncompleteFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            if (isDownloading && !isDownloadCompleted)
            {
                var result = MessageBox.Show("下载正在进行中，确定要取消吗？", "确认关闭",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    cancellationTokenSource?.Cancel();
                    DeleteIncompleteFile();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            cancellationTokenSource?.Dispose();
            httpClient?.Dispose();
            base.OnClosed(e);
        }
    }
}