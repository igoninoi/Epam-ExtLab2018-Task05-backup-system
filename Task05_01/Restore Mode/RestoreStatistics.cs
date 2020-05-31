using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    public struct RestoreStatistics
    {
        public int ValidRecords;

        public int InvalidRecords;

        public int FailedOperations;

        public long MinActionTime;

        public long MaxActionTime;

        public RestoreStatistics(bool forActionTimeCount) : this()
        {
            if (forActionTimeCount)
            {
                this.MinActionTime = long.MaxValue;
                this.MaxActionTime = long.MinValue;
            }
        }

        public void Register(BackupAction action)
        {
            if (action.ParseIsValid)
            {
                this.ValidRecords++;
                this.MinActionTime = Math.Min(this.MinActionTime, action.ActionTime);
                this.MaxActionTime = Math.Max(this.MaxActionTime, action.ActionTime);
            }
            else
            {
                this.InvalidRecords++;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine("--- Statistics ---");
            result.AppendLine($"Valid Records: {this.ValidRecords}");
            result.AppendLine($"Error records (!): {this.InvalidRecords}");

            DateTime time = new DateTime(this.MinActionTime);
            result.AppendLine($"Min action time: {time.ToShortDateString()} {time.TimeOfDay}");

            time = new DateTime(this.MaxActionTime);
            result.AppendLine($"Max action time: {time.ToShortDateString()} {time.TimeOfDay}");
            result.AppendLine("------------------");

            return result.ToString();
        }
    }
}