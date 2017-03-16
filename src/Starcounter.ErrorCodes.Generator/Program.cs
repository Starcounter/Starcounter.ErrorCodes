
using System;
using System.IO;
using System.Diagnostics;

namespace Starcounter.ErrorCodes.Generator {
    class Program {
        private static bool verbose = false;

        public static void Verbose(string s) {
            if (verbose) {
                Trace.TraceInformation(s);
                Console.Error.WriteLine("VERBOSE: {0}", s);
            }
        }

        public static void Verbose(string fmt, params object[] args) {
            if (verbose) {
                Trace.TraceInformation(fmt, args);
                Console.WriteLine(fmt, args);
            }
        }

        static void Main(string[] args) {
            try {
                string srcFilePath = null;
                string csFilePath = null;
                string cFilePath = null;
                string hFilePath = null;
                ErrorFile errorFile;
                CodeGenerator generator;

                CommandLine.ParseArgs(
                    args,
                    out srcFilePath,
                    out csFilePath,
                    out cFilePath,
                    out hFilePath,
                    out Program.verbose
                );

                if (srcFilePath == null) {
                    Verbose("No input specified. Quitting.");
                    return;
                }

                if (csFilePath == null && cFilePath == null && hFilePath == null) {
                    Verbose("No output specified. Quitting.");
                    return;
                }

                srcFilePath = Path.GetFullPath(srcFilePath);

                Verbose("Reading file with errorcodes ({0})", srcFilePath);
                errorFile = ErrorFileReader.ReadErrorCodes(srcFilePath);

                if (csFilePath != null) {
                    Verbose("Generating cs code to {0}", csFilePath);
                    generator = new CSharpCodeGenerator();
                    generator.Generate(errorFile, csFilePath);
                }

                if (cFilePath != null) {
                    Verbose("Generating c code to {0}", cFilePath);
                    generator = new CCodeGenerator();
                    generator.Generate(errorFile, cFilePath);
                }

                if (hFilePath != null) {
                    Verbose("Generating c header to {0}", hFilePath);
                    generator = new CHeaderGenerator();
                    generator.Generate(errorFile, hFilePath);
                }

                Verbose("All codefiles generated succesfully.");
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }
    }
}
