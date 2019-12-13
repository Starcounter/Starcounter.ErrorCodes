﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Starcounter.ErrorCodes.Generator
{
    internal class MdDocsGenerator : CodeGenerator
    {
        protected override void WriteHeader(TextWriter writer, ErrorFile errorFile)
        {
            writer.WriteLine("<!--");
            writer.WriteLine(" * THIS FILE IS AUTOMATICALLY GENERATED. DO NOT EDIT.");
            writer.WriteLine(" * Source: " + errorFile.SourcePath);
            writer.WriteLine(" * Generated: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"));
            writer.WriteLine("-->");
            writer.WriteLine();

            writer.WriteLine("# Starcounter Error Codes");
            writer.WriteLine();
            writer.WriteLine("This article lists the error codes that Starcounter uses, along with their severity and descriptions of what they mean.");
            writer.WriteLine();
        }

        protected override void WriteContent(TextWriter writer, ErrorFile errorFile)
        {
            base.WriteContent(writer, errorFile);
            WriteFacilites(writer, errorFile);
            WriteErrorCodes(writer, errorFile);
        }

        protected override void WriteFooter(TextWriter writer, ErrorFile errorFile)
        {
        }

        private void WriteFacilites(TextWriter writer, ErrorFile errorFile)
        {
            List<string> processedFacilityNames = new List<string>();

            writer.WriteLine("## Categories");
            writer.WriteLine();

            foreach (ErrorCode ec in errorFile.ErrorCodes)
            {
                if (processedFacilityNames.Contains(ec.Facility.Name))
                    continue;

                processedFacilityNames.Add(ec.Facility.Name);
                writer.WriteLine(string.Format("- `{0}` -  {1}.", ec.Facility.Code * 1000, ec.Facility.Name));
            }

            writer.WriteLine();
        }

        private void WriteErrorCodes(TextWriter writer, ErrorFile errorFile)
        {
            writer.WriteLine("## Error Codes");
            writer.WriteLine();
            writer.WriteLine("Category | Code | Name | Description");
            writer.WriteLine("-------- | ---- | ---- | -----------");


            foreach (ErrorCode ec in errorFile.ErrorCodes)
            {
                writer.Write(ec.Facility.Code * 1000);
                writer.Write(" | ");
                writer.Write(ec.CodeWithFacility);
                writer.Write(" | `");
                writer.Write(ec.Name);
                writer.Write("` | ");
                writer.Write(ec.Description);
                writer.Write(" ");

                writer.WriteLine();
            }

            writer.WriteLine();

            foreach (ErrorCode ec in errorFile.ErrorCodes)
            {
                if (!ec.RemarkParagraphs.Any())
                {
                    continue;
                }

                writer.Write("### `");
                writer.Write(ec.Name);
                writer.Write("`, `");
                writer.Write(ec.CodeWithFacility);
                writer.WriteLine("`");
                writer.WriteLine();

                foreach (string remark in ec.RemarkParagraphs)
                {
                    writer.WriteLine(WebUtility.HtmlEncode(remark));
                }

                writer.WriteLine();
            }
        }
    }
}