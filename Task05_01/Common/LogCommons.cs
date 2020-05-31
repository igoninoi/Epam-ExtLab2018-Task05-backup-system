using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    public abstract class LogCommons
    {
        public static readonly string CommentLineStart = "#";

        public string LogPath { get; protected set; }

        public bool IsDublicateToConsole { get; set; }
    }
}
