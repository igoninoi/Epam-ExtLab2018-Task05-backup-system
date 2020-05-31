using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.IO.Path;

namespace SimpleBackupSystem
{
    public class LogReader : LogCommons
    {
        public LogReader(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found.", path);
            }

            this.LogPath = path;
        }

        ~LogReader()
        {
            this.Close();
        }

        public StreamReader StreamReader { get; private set; }

        public bool EndOfStream => this.StreamReader == null || this.StreamReader.EndOfStream;

        public string ReadLine(bool excludeComments = true)
        {
            if (this.StreamReader == null)
            {
                throw new IOException("StreamReader do not set up.");
            }

            string result;
            do
            {
                if (this.StreamReader.EndOfStream)
                {
                    this.Close();
                    result = string.Empty;
                    break;
                }
                else
                {
                    result = this.StreamReader.ReadLine();
                }

                excludeComments &= result.StartsWith(LogCommons.CommentLineStart);
            }
            while (excludeComments);

            if (this.IsDublicateToConsole)
            {
                Console.WriteLine(result);
            }

            return result;
        }

        public void Close()
        {
            try
            {
                this.StreamReader?.Close();
            }
            finally
            {
                this.StreamReader = null;
            }
        }

        public void Reset()
        {
            this.StreamReader?.Close();

            FileStream file;
            try
            {
                file = File.Open(this.LogPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                throw new IOException($"Can not open log file '{this.LogPath}'.", ex);
            }

            this.StreamReader = new StreamReader(file);
        }
    }
}