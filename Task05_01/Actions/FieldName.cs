using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    /// <summary>
    /// Ordered enum of field names of BackupAction. Order of values in this enum sets the order of fields in string representation of BackupAction.
    /// Values must start from 0 to proper work of methods. 
    /// </summary>
    public enum FieldName : uint
    {
        Comment = 0,
        ActionTime,
        ActionType,
        FullPath,
        LastWriteTime,
        StorageName,
    }
}
