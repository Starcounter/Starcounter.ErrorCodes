using System.Collections.Generic;

namespace Starcounter.ErrorCodes.Generator {
    public sealed class ErrorFile {
        public readonly IList<ErrorCode> ErrorCodes;
        public readonly string SourcePath;

        internal ErrorFile(string sourcePath, IList<ErrorCode> codes) {
            this.ErrorCodes = codes;
            this.SourcePath = sourcePath;
        }
    }
}