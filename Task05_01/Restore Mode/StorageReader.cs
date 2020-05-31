using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.IO.Path;

namespace SimpleBackupSystem
{
    public class StorageReader
    {
        public StorageReader(string storageDir)
        {
            if (storageDir == null)
            {
                throw new ArgumentNullException();
            }

            if (!Directory.Exists(storageDir))
            {
                throw new DirectoryNotFoundException();
            }

            this.StoragePath = storageDir;
        }

        public string StoragePath { get; private set; }

        public bool Restore(string storageName, string filePath)
        {
            string storageFilePath = Combine(this.StoragePath, storageName);

            if (!File.Exists(storageFilePath))
            {
                throw new FileNotFoundException("File not found in storage.", storageFilePath);
            }

            try
            {
                File.Copy(storageFilePath, filePath, true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}