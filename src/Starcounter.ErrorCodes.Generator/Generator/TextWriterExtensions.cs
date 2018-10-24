using System.IO;

namespace Starcounter.ErrorCodes.Generator
{
    internal static class TextWriterExtensions
    {
        internal static void WriteIndented(this TextWriter writer, string str, string indent)
        {
            writer.Write(indent);
            writer.WriteLine(str);
        }
    }
}