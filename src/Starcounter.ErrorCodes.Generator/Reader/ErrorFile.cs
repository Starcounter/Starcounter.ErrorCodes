using System;
using System.Collections.Generic;

namespace Starcounter.ErrorCodes.Generator
{
    public sealed class ErrorFile
    {
        public readonly IList<ErrorCode> ErrorCodes;
        public readonly string SourcePath;
        public readonly DateTime Processed;

        internal ErrorFile(string sourcePath, IList<ErrorCode> codes)
        {
            this.ErrorCodes = codes;
            this.SourcePath = sourcePath;
            this.Processed = DateTime.Now;
        }

        public int Count
        {
            get
            {
                return (ErrorCodes != null) ? ErrorCodes.Count : 0;
            }
        }
    }
}