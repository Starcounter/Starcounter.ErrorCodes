
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
                Console.Error.WriteLine("VERBOSE: " + fmt, args);
            }
        }

        static void Main(string[] args) {
            try {
                string srcFilePath = null;
                string csFilePath = null;
                ErrorFile errorFile;
                CSharpCodeGenerator csGenerator;

                CommandLine.ParseArgs(args, out srcFilePath, out csFilePath, out Program.verbose);

                if (srcFilePath == null)
                    return;

                srcFilePath = Path.GetFullPath(srcFilePath);

                Verbose("Generating codefiles...");
                errorFile = ErrorFileReader.ReadErrorCodes(srcFilePath);
                csGenerator = new CSharpCodeGenerator();
                csGenerator.Generate(errorFile, csFilePath);
                Verbose("All codefiles generated succesfully.");
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }
    }
}
