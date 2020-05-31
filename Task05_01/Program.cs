using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine("Simple Backup System");
            Console.WriteLine($"Usage: {Path.GetFileName(Environment.GetCommandLineArgs()[0])} <directory>");

            string targetDir = args[0];
            string filter = "*.txt";
            string storageDir = Path.Combine(args[0], @".backup");

            Console.WriteLine($"Terget dir: '{targetDir}'");
            Console.WriteLine($"Storage dir: '{storageDir}'");
            Console.WriteLine($"Filter: {filter}");
            Console.WriteLine();

            RunModeChoosing(targetDir, filter, storageDir);

            Console.WriteLine();
        }

        private static void RunModeChoosing(string targetDir, string filter, string storageDir)
        {
            string choice = string.Empty;
            do
            {
                Console.WriteLine("\nChoose mode:");
                Console.WriteLine("   b or backup  - backup mode");
                Console.WriteLine("   r or restore - restore mode");
                Console.WriteLine("   exit         - exit");
                Console.Write("mode ?>");
                choice = Console.ReadLine().Trim();

                ParseModeCommand(choice, targetDir, filter, storageDir);
            }
            while (choice != "exit");
        }

        private static void ParseModeCommand(string choice, string targetDir, string filter, string storageDir)
        {
            switch (choice.ToLower())
            {
                case "b":
                case "backup":
                    RunBackupMode(targetDir, filter, storageDir);
                    break;

                case "r":
                case "restore":
                    RunRestoreMode(targetDir, filter, storageDir);
                    break;

                case "exit":
                    break;

                default:
                    Console.WriteLine($"Unknown command '{choice}'");
                    break;
            }
        }

        private static void RunBackupMode(string targetDir, string filter, string storageDir)
        {
            Console.WriteLine($"\nBackup mode choosen");

            Backup backup;
            try
            {
                backup = new Backup(targetDir, filter, storageDir);
            }
            catch (Exception)
            {
                Console.WriteLine($"Can't init backup mode. Try to close all programs that use '{storageDir}' and run backup mode again.");
                return;
            }

            do
            {
                Console.WriteLine("\nEnter 'exit' to terminate backup mode");
                Console.Write("backup ?>");
            }
            while (Console.ReadLine() != "exit");

            backup.Close();
        }

        private static void RunRestoreMode(string targetDir, string filter, string storageDir)
        {
            Console.WriteLine($"\nRestore mode choosen");

            Restore restore;
            try
            {
                restore = new Restore(targetDir, filter, storageDir);
            }
            catch (Exception)
            {
                Console.WriteLine($"Can't init restore mode. Try to close all programs that use '{storageDir}' and run backup mode again.");
                return;
            }

            string choice = string.Empty;
            do
            {
                Console.WriteLine("\nChoose restore command:");
                Console.WriteLine("   l or list     - list log file");
                Console.WriteLine("   r or rollback - rollback (restore state from storage)");
                Console.WriteLine("   exit          - exit");
                Console.Write("restore ?>");

                choice = Console.ReadLine().Trim();
                ParseRestoreCommand(choice, restore);
            }
            while (choice != "exit");

            restore.Close();
        }

        private static void ParseRestoreCommand(string choice, Restore restore)
        {
            switch (choice.ToLower())
            {
                case "l":
                case "list":
                    ExecuteRestoreListCommand(restore);
                    break;

                case "r":
                case "rollback":
                    ExecuteRestoreRollbackCommand(choice, restore);
                    break;

                case "exit":
                    break;

                default:
                    Console.WriteLine($"Unknown command '{choice}'");
                    break;
            }
        }

        private static void ExecuteRestoreListCommand(Restore restore)
        {
            Console.WriteLine($"\nList command choosen");

            RestoreStatistics statistics;
            restore.WriteLogToConsole(out statistics);
            Console.WriteLine(statistics);
        }

        private static void ExecuteRestoreRollbackCommand(string choice, Restore restore)
        {
            Console.WriteLine($"\nRestore command choosen");

            DateTime time;
            string input;
            do
            {
                var now = DateTime.Now;
                Console.WriteLine("\nEnter rollback time (can use exact format) or 'exit': ");
                Console.WriteLine($"(Exact time format example of now moment: {now.ToShortDateString()} { now.TimeOfDay})");
                Console.Write("time ?>");
                input = Console.ReadLine().Trim();
            }
            while (!DateTime.TryParse(input, out time) && input.ToLower() != "exit");

            if (input.ToLower() == "exit")
            {
                return;
            }

            RestoreStatistics statistics;
            restore.Rollback(time, out statistics);

            Console.WriteLine(statistics);
        }
    }
}
