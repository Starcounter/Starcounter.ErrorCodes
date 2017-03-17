using System;
using System.IO;

namespace Starcounter.ErrorCodes.Generator {
    internal class CHeaderGenerator : CodeGenerator {
        protected override void WriteHeader(TextWriter writer, ErrorFile errorFile) {
            base.WriteHeader(writer, errorFile);

            writer.WriteLine("#ifndef __SCCOREERR_INTERNAL_H");
            writer.Write("#define __SCCOREERR_INTERNAL_H "); 
            writer.WriteLine(errorFile.Processed.ToString("yyyyMMdd"));
            writer.WriteLine();
            writer.WriteLine("#ifndef DLL_EXPORT");
            writer.WriteLine("#if _MSC_VER >= 1500");
            writer.WriteLine("#define DLL_EXPORT __declspec(dllexport)");
            writer.WriteLine("#endif /* _MSC_VER */");
            writer.WriteLine("#ifdef __GNUC__");
            writer.WriteLine("#define DLL_EXPORT __attribute__((visibility(\"default\")))");
            writer.WriteLine("#endif /* __GNUC__ */");
            writer.WriteLine("#ifdef __clang__");
            writer.WriteLine("#ifdef __linux__");
            writer.WriteLine("#define DLL_EXPORT __attribute__((visibility(\"default\")))");
            writer.WriteLine("#else");
            writer.WriteLine("#define DLL_EXPORT __declspec(dllexport)");
            writer.WriteLine("#endif /* __linux__ */");
            writer.WriteLine("#endif /* __clang__ */");
            writer.WriteLine("#endif /* !DLL_EXPORT */");
            writer.WriteLine();
            writer.WriteLine("#ifdef SCCOREERR_EXPORTS");
            writer.WriteLine("#define SCCOREERR_EXPORT DLL_EXPORT");
            writer.WriteLine("#else");
            writer.WriteLine("#define SCCOREERR_EXPORT");
            writer.WriteLine("#endif");
            writer.WriteLine();
            writer.WriteLine("#ifdef __cplusplus");
            writer.WriteLine("extern \"C\"");
            writer.WriteLine("#else");
            writer.WriteLine("extern");
            writer.WriteLine("#endif /* __cplusplus */");
            writer.WriteLine("SCCOREERR_EXPORT const char* sccoreerr_message(long ec);");
        }

        protected override void WriteContent(TextWriter writer, ErrorFile errorFile) {
            uint currentFacility = uint.MaxValue;

            base.WriteContent(writer, errorFile);

            foreach(ErrorCode ec in errorFile.ErrorCodes) {
                if (currentFacility != ec.Facility.Code) {
                    writer.WriteLine();
                    writer.WriteLine("/* Facility \"" + ec.Facility.Name + "\" */");
                    currentFacility = ec.Facility.Code;
                }

                writer.WriteLine("#define {0} ({1}L)", ec.ConstantNameToUpper, ec.CodeWithFacility);
            }
        }

        protected override void WriteFooter(TextWriter writer, ErrorFile errorFile) {
            base.WriteFooter(writer, errorFile);
            writer.WriteLine();
            writer.WriteLine("#endif /* __SCCOREERR_INTERNAL_H */");
        }
    }    
}