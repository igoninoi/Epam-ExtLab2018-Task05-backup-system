using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    public enum ActionType
    {
        /// <summary>
        /// File image is old
        /// </summary>
        FileOld = 0,

        /// <summary>
        /// File image is new
        /// </summary>
        FileNew,

        /// <summary>
        /// File image is new but have to be ignored by virtue of program logic
        /// </summary>
        FileNewIgnored,

        /// <summary>
        /// File image is new but saving error occurred
        /// </summary>
        FileNewErrorSaving,

        /// <summary>
        /// Directory info is old
        /// </summary>
        DirOld,

        /// <summary>
        /// Directory info is new
        /// </summary>
        DirNew,
    }
}
