
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Starcounter.ErrorCodes.Generator {
internal class Generator {
    private static readonly Regex MultipleWhitespace = new Regex(@"\s+");

    internal static void GenerateCodeFiles(string srcFilePath, string csOutputFilePath) {
        ErrorFile errorFile = ErrorFileReader.ReadErrorCodes(srcFilePath);
        GenerateCSharpFile(errorFile, csOutputFilePath);
    }

    private static void GenerateCSharpFile(ErrorFile errorFile, string csOutputFilePath) {
        const string Indent = "    ";
        const string Indent2 = Indent + Indent;
        const string Indent3 = Indent2 + Indent;

        using (var writer = new StreamWriter(new FileStream(csOutputFilePath, FileMode.Create))) {
            // write head
            writer.WriteLine("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *");
            writer.WriteLine(" *");
            writer.WriteLine(" * THIS FILE IS AUTOMATICALLY GENERATED. DO NOT EDIT.");
            writer.WriteLine(" *");
            writer.WriteLine(" */");
            writer.WriteLine();
            writer.WriteLine("namespace Starcounter.ErrorCodes");
            writer.WriteLine("{");
            writer.WriteLine(Indent + "/// <summary>");
            writer.WriteLine(Indent + "/// Class Error");
            writer.WriteLine(Indent + "/// </summary>");
            writer.WriteLine(Indent + "public static class Error");
            writer.WriteLine(Indent + "{");

            // Write categories/facilities

            IList<Facility> facilitesWritten;
            
            facilitesWritten = new List<Facility>();
            writer.WriteLine(Indent2 + "public enum Category");
            writer.WriteLine(Indent2 + "{");

            foreach (ErrorCode ec in errorFile.ErrorCodes) {
                if (facilitesWritten.Contains(ec.Facility))
                    continue;

                facilitesWritten.Add(ec.Facility);
                writer.WriteLine("{0}{1} = {2},", Indent3, ec.Facility.Name, ec.Facility.Code * 1000);
            }

            writer.WriteLine(Indent2 + "}");
            writer.WriteLine();

            // write error codes
            foreach (ErrorCode ec in errorFile.ErrorCodes) {
                writer.WriteLine(Indent2 + "/// <summary> ");
                writer.Write(Indent2 + "/// ");
                writer.Write(WebUtility.HtmlEncode(ec.Description));
                writer.WriteLine();
                writer.WriteLine(Indent2 + "/// </summary>");

                if (ec.RemarkParagraphs.Count == 1) {
                    writer.WriteLine(Indent2 + "/// <remarks>");
                    writer.Write(Indent2 + "/// ");
                    writer.Write(WebUtility.HtmlEncode(ec.RemarkParagraphs[0]));
                    writer.WriteLine();
                    writer.WriteLine(Indent2 + "/// </remarks>");
                } else if (ec.RemarkParagraphs.Count > 1) {
                    writer.WriteLine(Indent2 + "/// <remarks>");
                    foreach (string remark in ec.RemarkParagraphs) {
                        writer.Write(Indent2 + "/// <para>");
                        writer.Write(WebUtility.HtmlEncode(remark));
                        writer.WriteLine(" </para>");
                    }
                    writer.WriteLine(Indent2 + "/// </remarks>");
                }
                writer.WriteLine(Indent2 + "public const uint {0} = {1};", ec.ConstantName, ec.CodeWithFacility);
            }
        
            writer.WriteLine(Indent + "}");
            writer.WriteLine("}");
            writer.Flush();
        }
    }
}
}
