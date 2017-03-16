using System.IO;

namespace Starcounter.ErrorCodes.Generator {
    internal class CHeaderGenerator : CodeGenerator {
        protected override void WriteHeader(TextWriter writer, ErrorFile errorFile) {
            base.WriteHeader(writer, errorFile);
        }

        protected override void WriteContent(TextWriter writer, ErrorFile errorFile) {
            base.WriteContent(writer, errorFile);
        }

        protected override void WriteFooter(TextWriter writer, ErrorFile errorFile) {
            base.WriteFooter(writer, errorFile);
        }
    }    
}