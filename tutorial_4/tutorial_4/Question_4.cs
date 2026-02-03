using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace tutorial_4
{
    internal class Question_4
    {
        private static readonly string WatchPath = Path.Combine(Path.GetTempPath(), "FileWatcherDemo");
        private static readonly string ProcessedPath = Path.Combine(WatchPath, "Processed");
        private static FileSystemWatcher _watcher;
        private static readonly ConcurrentQueue<string> _fileQueue = new ConcurrentQueue<string>();
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        public static void DemonstrateFileMonitoringAndProcessing()
        {
            Console.WriteLine("=== File Monitoring & Concurrent Processing Demo ===\n");

            // Prepare directories
            Directory.CreateDirectory(WatchPath);
            Directory.CreateDirectory(ProcessedPath);

            Console.WriteLine($"Watching directory: {WatchPath}");
            Console.WriteLine($"Compressed files will go to: {ProcessedPath}\n");

            SetupFileSystemWatcher();

            // Start background processor (one or more consumers)
            Task processor = Task.Run(() => ProcessQueueAsync(_cts.Token));

            Console.WriteLine("→ Watcher is running. Drop .txt files into the watched folder to test.");
            Console.WriteLine("→ Press ENTER to stop...\n");

            Console.ReadLine();

            // Cleanup
            _cts.Cancel();
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            processor.Wait(TimeSpan.FromSeconds(5));
            Console.WriteLine("\nDemo stopped. Check the 'Processed' subfolder for .gz files.");
        }

        private static void SetupFileSystemWatcher()
        {
            _watcher = new FileSystemWatcher
            {
                Path = WatchPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.txt",               // only watch .txt for simplicity
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            _watcher.Created += OnFileCreated;
            // Optional: _watcher.Changed += OnFileChanged;  (but often causes duplicates)
        }

        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Quick exit from event handler → enqueue only
            Console.WriteLine($"[Event] New file detected: {e.FullPath}");
            _fileQueue.Enqueue(e.FullPath);
        }

        private static async Task ProcessQueueAsync(CancellationToken token)
        {
            // You can increase parallelism by using Parallel.ForEach or multiple consumers
            const int MaxDegreeOfParallelism = 4;

            while (!token.IsCancellationRequested)
            {
                if (_fileQueue.TryDequeue(out string filePath))
                {
                    // Process one file (can be parallelized)
                    await ProcessFileAsync(filePath, token);
                }
                else
                {
                    await Task.Delay(200, token); // avoid tight loop
                }
            }
        }

        private static async Task ProcessFileAsync(string filePath, CancellationToken token)
        {
            try
            {
                // Wait briefly if file is still being written
                if (!IsFileReady(filePath))
                {
                    Console.WriteLine($"  → File not ready yet: {Path.GetFileName(filePath)}");
                    await Task.Delay(500, token);
                    if (!IsFileReady(filePath)) return; // skip if still locked
                }

                string fileName = Path.GetFileName(filePath);
                string compressedPath = Path.Combine(ProcessedPath, fileName + ".gz");

                Console.WriteLine($"  → Processing: {fileName}");

                // Read → Compress → Save
                using (FileStream input = File.OpenRead(filePath))
                using (FileStream output = File.Create(compressedPath))
                using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress))
                {
                    await input.CopyToAsync(gzip, 81920, token);
                }

                // Optional: move or delete original
                string moved = Path.Combine(ProcessedPath, fileName);
                File.Move(filePath, moved); // or File.Delete(filePath);

                Console.WriteLine($"  → Compressed & moved: {fileName} → {Path.GetFileName(compressedPath)}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"  → IO error (file locked?): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  → Error processing {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private static bool IsFileReady(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }

}