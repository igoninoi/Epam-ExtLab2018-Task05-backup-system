using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    public class PathsCollector
    {
        public PathsCollector()
        {
            this.ForUpdate = new Dictionary<string, BackupAction>();
            this.ForDelete = new HashSet<string>();
        }

        public Dictionary<string, BackupAction> ForUpdate { get; private set; }

        public HashSet<string> ForDelete { get; private set; }

        public void InsertForUpdate(string fullPath, BackupAction action)
        {
            if (this.ForUpdate.ContainsKey(fullPath))
            {
                this.ForUpdate.Remove(fullPath);
            }

            this.ForUpdate.Add(fullPath, action);
        }

        public void InsertForDelete(string fullPath)
        {
            if (!this.ForUpdate.ContainsKey(fullPath))
            {
                this.ForDelete.Add(fullPath);
            }
        }
    }
}