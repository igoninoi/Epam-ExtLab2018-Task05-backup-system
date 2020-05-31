using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static System.IO.Path;

namespace SimpleBackupSystem
{
    public class Restore
    {
        public Restore(string targetDir, string filter, string storageDir)
        {
            if (!Directory.Exists(targetDir))
            {
                throw new DirectoryNotFoundException(targetDir);
            }

            if (!Directory.Exists(storageDir))
            {
                throw new DirectoryNotFoundException(storageDir);
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            this.Init(targetDir, filter, storageDir);
        }

        public StorageReader Storage { get; private set; }

        public LogReader Log { get; private set; }

        public void Close()
        {
            this.Log.Close();
        }

        public void WriteLogToConsole(out RestoreStatistics statistics)
        {
            statistics = new RestoreStatistics(forActionTimeCount: true);

            this.Log.Reset();
            while (!this.Log.EndOfStream)
            {
                var line = this.Log.ReadLine(excludeComments: true);
                if (line == string.Empty)
                {
                    continue;
                }

                var action = new BackupAction(line);
                statistics.Register(action);
                if (action.ParseIsValid)
                {
                    var time = new DateTime(action.ActionTime);
                    Console.WriteLine($"*** {statistics.ValidRecords}: {time.ToShortDateString()} {time.TimeOfDay} {action.Comment}");
                }
            }
        }

        public void Rollback(DateTime targetTime, out RestoreStatistics statistics)
        {
            Console.WriteLine($"\nStart rollback at moment {targetTime.ToShortDateString()} {targetTime.TimeOfDay}");

            var time = targetTime.Ticks;

            var paths = this.CollectPaths(time, out statistics);

            foreach (var path in paths.ForUpdate)
            {
                this.DoUpdate(path, ref statistics);
            }

            foreach (var path in paths.ForDelete)
            {
                Console.WriteLine($"Deleting: '{path}' if empty");
                if (!TryDeletePath(path))
                {
                    Console.WriteLine("...failed");
                }
            }
        }

        private static bool TryDeletePath(string path)
        {
            if (Directory.Exists(path))
            {
                return TryDeleteDir(path);
            }

            return TryDeleteFile(path);
        }

        private static bool TryDeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryDeleteDir(string filePath)
        {
            try
            {
                Directory.Delete(filePath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryCreateDir(string filePath)
        {
            try
            {
                Directory.CreateDirectory(filePath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void Init(string targetDir, string filter, string storageDir)
        {
            if (targetDir == null || filter == null || storageDir == null)
            {
                throw new ArgumentNullException();
            }

            this.Storage = new StorageReader(storageDir);

            var path = Combine(storageDir, ".log");
            this.Log = new LogReader(path);
        }

        private PathsCollector CollectPaths(long targetTime, out RestoreStatistics logStatistics)
        {
            var pathsCollector = new PathsCollector();

            logStatistics = new RestoreStatistics(forActionTimeCount: true);

            this.Log.Reset();
            while (!this.Log.EndOfStream)
            {
                var line = this.Log.ReadLine(excludeComments: true);
                if (line == string.Empty)
                {
                    continue;
                }

                var action = new BackupAction(line);
                logStatistics.Register(action);
                if (!action.ParseIsValid)
                {
                    continue;
                }

                if (action.ActionTime <= targetTime)
                {
                    switch (action.ActionType)
                    {
                        case ActionType.FileNewIgnored:
                        case ActionType.FileNewErrorSaving:
                            break;
                        default:
                            pathsCollector.InsertForUpdate(action.FullPath, action);
                            break;
                    }
                }
                else
                {
                    pathsCollector.InsertForDelete(action.FullPath);
                }
            }

            return pathsCollector;
        }

        private void DoUpdate(KeyValuePair<string, BackupAction> path, ref RestoreStatistics statistics)
        {
            switch (path.Value.ActionType)
            {
                case ActionType.FileOld:
                    Console.WriteLine($"Deleting: '{path.Key}'");
                    if (!TryDeleteFile(path.Key))
                    {
                        Console.WriteLine("...failed");
                    }

                    break;

                case ActionType.FileNew:
                    Console.WriteLine($"Restore: '{path.Key}' StorageName: {path.Value.StorageName}");
                    if (!this.Storage.Restore(path.Value.StorageName, path.Key))
                    {
                        Console.WriteLine("...failed");
                    }

                    break;

                case ActionType.DirOld:
                    Console.WriteLine($"Deleting: '{path.Key}' if empty");
                    if (!TryDeleteDir(path.Key))
                    {
                        Console.WriteLine("...failed");
                    }

                    break;

                case ActionType.DirNew:
                    Console.WriteLine($"Creating: '{path.Key}' if not exists");
                    if (!TryCreateDir(path.Key))
                    {
                        Console.WriteLine("...failed");
                    }

                    break;

                default:
                    break;
            }
        }
    }
}