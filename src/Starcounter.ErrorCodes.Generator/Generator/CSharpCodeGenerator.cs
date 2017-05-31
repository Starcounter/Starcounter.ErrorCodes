using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Starcounter.ErrorCodes.Generator {
    internal class CSharpCodeGenerator : CodeGenerator {
        const string Indent = "    ";
        const string Indent2 = Indent + Indent;
        const string Indent3 = Indent2 + Indent;

        protected override void WriteHeader(TextWriter writer, ErrorFile errorFile) {
            base.WriteHeader(writer, errorFile);

            writer.WriteLine("namespace Starcounter.ErrorCodes {");
            writer.WriteIndented("public static class Error {", Indent);
        } 

        protected override void WriteContent(TextWriter writer, ErrorFile errorFile) {
            base.WriteContent(writer, errorFile);
            WriteFacilites(writer, errorFile);
            WriteErrorCodes(writer, errorFile);
            WriteErrorCodeToMessageMethod(writer, errorFile);
        }

        protected override void WriteFooter(TextWriter writer, ErrorFile errorFile) {
            writer.WriteIndented("}", Indent); // class
            writer.WriteLine("}"); // namespace
        }

        private void WriteFacilites(TextWriter writer, ErrorFile errorFile) {
            List<string> processedFacilityNames = new List<string>();
            
            writer.WriteIndented("public enum Category {", Indent2);
            foreach (ErrorCode ec in errorFile.ErrorCodes) {
                if (processedFacilityNames.Contains(ec.Facility.Name))
                    continue;

                processedFacilityNames.Add(ec.Facility.Name);
                writer.WriteIndented(string.Format("{0} = {1},", ec.Facility.Name, ec.Facility.Code * 1000), Indent3);
            }

            writer.WriteIndented("}", Indent2);
            writer.WriteLine();
        }

        private void WriteErrorCodes(TextWriter writer, ErrorFile errorFile) {
            foreach (ErrorCode ec in errorFile.ErrorCodes) {
                writer.WriteIndented("/// <summary>", Indent2);
                writer.WriteIndented("/// " + WebUtility.HtmlEncode(ec.Description), Indent2);
                writer.WriteIndented("/// </summary>", Indent2);

                if (ec.RemarkParagraphs.Count == 1) {
                    writer.WriteIndented("/// <remarks>", Indent2);
                    writer.WriteIndented("/// " + WebUtility.HtmlEncode(ec.RemarkParagraphs[0]), Indent2);
                    writer.WriteIndented("/// </remarks>", Indent2);
                } else if (ec.RemarkParagraphs.Count > 1) {
                    writer.WriteIndented("/// <remarks>", Indent2);
                    foreach (string remark in ec.RemarkParagraphs) {
                        writer.WriteIndented("/// <para>" + WebUtility.HtmlEncode(remark) + "</para>", Indent2);
                    }
                    writer.WriteIndented("/// </remarks>", Indent2);
                }
                writer.WriteIndented(string.Format("public const uint {0} = {1};", ec.ConstantNameToUpper, ec.CodeWithFacility), Indent2);
            }
            writer.WriteLine();
        }

        private void WriteErrorCodeToMessageMethod(TextWriter writer, ErrorFile errorFile) {
            string indent4 = Indent3 + Indent;
            string indent5 = indent4 + Indent;
            string caseAndRet = "case {0}: return \"{1}\";";

            writer.WriteIndented("public static string ToMessage(uint errorCode) {", Indent2);
            writer.WriteIndented("switch (errorCode) {", Indent3);

            foreach (ErrorCode ec in errorFile.ErrorCodes) {
                writer.WriteIndented(string.Format(caseAndRet, ec.ConstantNameToUpper, ec.FormattedCodeWithDescription), indent4);
            }
            writer.WriteIndented("default: return \"Unknown errorcode:\" + errorCode;", indent4);
            
            writer.WriteIndented("}", Indent3); // switch
            writer.WriteIndented("}", Indent2); // ToMessage method
        }
    }
}