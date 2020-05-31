using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackupSystem
{
    public class BackupAction
    {
        public const char DefaultSeparator = '|';

        private static char separator = char.MinValue;

        static BackupAction()
        {
            Parsers = new Dictionary<FieldName, BoxingParser>((int)FieldNamesCount());

            Parsers.Add(FieldName.ActionTime, BoxingParseLong);
            Parsers.Add(FieldName.ActionType, BoxingParseActionType);
            Parsers.Add(FieldName.FullPath, BoxingParseStringProxy);
            Parsers.Add(FieldName.LastWriteTime, BoxingParseLong);
            Parsers.Add(FieldName.StorageName, BoxingParseStringProxy);
            Parsers.Add(FieldName.Comment, BoxingParseStringProxy);
        }

        public BackupAction(long actionTime, ActionType actionType, string fullPath, long lastWriteTime, string storageName, string comment)
        {
            this.ActionTime = actionTime;
            this.ActionType = actionType;
            this.FullPath = fullPath;
            this.LastWriteTime = lastWriteTime;
            this.StorageName = storageName;
            this.Comment = comment;

            this.ParseIsValid = true;
        }

        public BackupAction(long actionTime, ActionType actionType)
        {
            this.ActionTime = actionTime;
            this.ActionType = actionType;

            this.ParseIsValid = true;
        }

        public BackupAction(string source)
        {
            this.FromString(source);
        }

        private delegate object BoxingParser(string source, out bool isOK);

        public static char Separator
        {
            get
            {
                if (separator == char.MinValue)
                {
                    separator = DefaultSeparator;
                }

                return separator;
            }

            set
            {
                if (separator == char.MinValue)
                {
                    separator = value;
                }
                else
                {
                    throw new Exception($"Can not change {typeof(BackupAction)}.{nameof(Separator)} after it had been used. Set it's value befor first use");
                }
            }
        }

        public long ActionTime { get; private set; }

        public ActionType ActionType { get; set; }

        public string FullPath { get; set; }

        public long LastWriteTime { get; set; }

        public string StorageName { get; set; }

        public string Comment { get; set; }

        public bool ParseIsValid { get; private set; }

        private static Dictionary<FieldName, BoxingParser> Parsers { get; set; }

        private string this[FieldName index]
        {
            get
            {
                switch (index)
                {
                    case FieldName.ActionTime:
                        return this.ActionTime.ToString();

                    case FieldName.ActionType:
                        return this.ActionType.ToString();

                    case FieldName.FullPath:
                        return this.FullPath;

                    case FieldName.LastWriteTime:
                        return this.LastWriteTime.ToString();

                    case FieldName.StorageName:
                        return this.StorageName;

                    case FieldName.Comment:
                        return this.Comment;

                    default:
                        throw new IndexOutOfRangeException();
                }
            }

            set
            {
                if (!Parsers.ContainsKey(index))
                {
                    throw new NotImplementedException($"Can't find out parser for field '{index.ToString()}'.");
                }

                var parser = Parsers[index];
                bool isOK = false;

                var result = parser?.Invoke(value, out isOK);
                if (!isOK)
                {
                    this.ParseIsValid = false;
                    return;
                }

                switch (index)
                {
                    case FieldName.ActionTime:
                        this.ActionTime = (long)result;
                        break;

                    case FieldName.ActionType:
                        this.ActionType = (ActionType)result;
                        break;

                    case FieldName.FullPath:
                        this.FullPath = (string)result;
                        break;

                    case FieldName.LastWriteTime:
                        this.LastWriteTime = (long)result;
                        break;

                    case FieldName.StorageName:
                        this.StorageName = (string)result;
                        break;

                    case FieldName.Comment:
                        this.Comment = (string)result;
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }

                this.ParseIsValid = true;
            }
        }

        private string this[uint index]
        {
            get
            {
                var field = FieldNameByIndex(index);
                return this[field];
            }

            set
            {
                var field = FieldNameByIndex(index);
                this[field] = value;
            }
        }

        public static uint FieldNamesCount()
        {
            return (uint)Enum.GetValues(typeof(FieldName)).Length;
        }

        public static FieldName FieldNameByIndex(uint index)
        {
            if (index >= FieldNamesCount())
            {
                throw new IndexOutOfRangeException();
            }

            return (FieldName)Enum.ToObject(typeof(FieldName), index);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            uint n = FieldNamesCount();

            uint i = 0;
            if (i < n)
            {
                result.Append(this[i].ToString());
            }

            for (i = 1; i < n; i++)
            {
                result.Append(Separator);
                result.Append(this[i].ToString());
            }

            return result.ToString();
        }

        private static object BoxingParseLong(string source, out bool isOK)
        {
            long result;
            isOK = long.TryParse(source, out result);
            return result;
        }

        private static object BoxingParseActionType(string source, out bool isOK)
        {
            ActionType result;
            isOK = Enum.TryParse(source, true, out result);
            return result;
        }

        private static object BoxingParseStringProxy(string source, out bool isOK)
        {
            isOK = true;
            return source;
        }

        private void FromString(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            var parts = source.Split(Separator);
            if (parts.Length != FieldNamesCount())
            {
                this.ParseIsValid = false;
                return;
            }

            bool isValid = true;
            for (uint i = 0; i < parts.Length; i++)
            {
                this[i] = parts[i];
                isValid &= this.ParseIsValid;
            }

            this.ParseIsValid = isValid;
        }
    }
}
