using System.Collections.Generic;

namespace Starcounter.ErrorCodes.Generator {
    public sealed class ErrorFile {
        public readonly IList<ErrorCode> ErrorCodes;

        internal ErrorFile(IList<ErrorCode> codes) {
            this.ErrorCodes = codes;
        }
    }
}