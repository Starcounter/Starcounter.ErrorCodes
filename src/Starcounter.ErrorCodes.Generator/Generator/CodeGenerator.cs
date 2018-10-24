using System;
using System.IO;

namespace Starcounter.ErrorCodes.Generator
{
    internal class CodeGenerator
    {
        internal void Generate(ErrorFile errorFile, string outputFilePath)
        {
            using (var writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create)))
            {
                WriteHeader(writer, errorFile);
                WriteContent(writer, errorFile);
                WriteFooter(writer, errorFile);
                writer.Flush();
            }
        }

        protected virtual void WriteHeader(TextWriter writer, ErrorFile errorFile)
        {
            writer.WriteLine("/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *");
            writer.WriteLine(" * THIS FILE IS AUTOMATICALLY GENERATED. DO NOT EDIT.");
            writer.WriteLine(" * Source: " + errorFile.SourcePath);
            writer.WriteLine(" * Generated: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"));
            writer.WriteLine(" */");
            writer.WriteLine();
        }

        protected virtual void WriteContent(TextWriter writer, ErrorFile errorFile)
        {
        }

        protected virtual void WriteFooter(TextWriter writer, ErrorFile errorFile)
        {
        }
    }
}
