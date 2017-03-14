using System.Collections.Generic;

namespace scerrcc {
    public sealed class ErrorFile {
        public readonly IList<ErrorCode> ErrorCodes;

        internal ErrorFile(IList<ErrorCode> codes) {
            this.ErrorCodes = codes;
        }
    }
}