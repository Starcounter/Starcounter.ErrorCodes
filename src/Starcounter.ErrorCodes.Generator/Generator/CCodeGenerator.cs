using System.IO;

namespace Starcounter.ErrorCodes.Generator
{
    internal class CCodeGenerator : CodeGenerator
    {
        private const string INDENT = "  ";
        private const string INDENT2 = INDENT + INDENT;
        private const string CASE = "case {0}: return \"{1}\";";

        protected override void WriteHeader(TextWriter writer, ErrorFile errorFile)
        {
            base.WriteHeader(writer, errorFile);

            string dateStr = errorFile.Processed.ToString("yyyyMMdd");

            writer.WriteLine("#if defined(_MSC_VER)");
            writer.WriteLine("#if !defined(_CRT_SECURE_NO_WARNINGS)");
            writer.WriteLine("#define _CRT_SECURE_NO_WARNINGS");
            writer.WriteLine("#endif");
            writer.WriteLine("#endif");
            writer.WriteLine();
            writer.WriteLine("#include <stdio.h>");
            writer.WriteLine("#include <string.h>");
            writer.WriteLine("#include \"{0}\"", "sccoreerr_gen.h");
            writer.WriteLine();
            writer.WriteLine("#define __SCCOREERR_INTERNAL_C {0}", dateStr);
            writer.WriteLine("#if __SCCOREERR_INTERNAL_H != __SCCOREERR_INTERNAL_C");
            writer.WriteLine("#error (\"__SCCOREERR_INTERNAL_H (\" __SCCOREERR_INTERNAL_H \") != __SCCOREERR_INTERNAL_C (\" __SCCOREERR_INTERNAL_C \")\")");
            writer.WriteLine("#endif");
            writer.WriteLine();
            writer.WriteLine("#ifdef __cplusplus");
            writer.WriteLine("extern \"C\"");
            writer.WriteLine("#else");
            writer.WriteLine("extern");
            writer.WriteLine("#endif /* __cplusplus */");
            writer.WriteLine();
        }

        protected override void WriteContent(TextWriter writer, ErrorFile errorFile)
        {
            base.WriteContent(writer, errorFile);

            writer.WriteLine("SCCOREERR_EXPORT const char* sccoreerr_message(long ec) {");
            writer.WriteIndented("static char s_unknown_text[32];", INDENT);
            writer.WriteIndented("switch (ec) {", INDENT);

            foreach (ErrorCode ec in errorFile.ErrorCodes)
            {
                writer.WriteIndented(
                    string.Format(CASE, ec.ConstantNameToUpper, ec.FormattedCodeWithDescription),
                    INDENT2
                );
            }

            writer.WriteIndented("}", INDENT);
            writer.WriteIndented("memset(s_unknown_text, 0, sizeof(s_unknown_text));", INDENT);
            writer.WriteIndented("sprintf(s_unknown_text, \"(SCERR%ld)\", ec);", INDENT);
            writer.WriteIndented("return s_unknown_text;", INDENT);
            writer.WriteLine("}");
        }

        protected override void WriteFooter(TextWriter writer, ErrorFile errorFile)
        {
            base.WriteFooter(writer, errorFile);
        }
    }
}