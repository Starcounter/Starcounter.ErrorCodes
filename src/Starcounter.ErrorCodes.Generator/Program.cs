
using System;
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

                CommandLine.ParseArgs(args, out srcFilePath, out csFilePath, out Program.verbose);

                if (srcFilePath == null)
                    return;

                Verbose("Generating codefiles...");
                Generator.GenerateCodeFiles(srcFilePath, csFilePath);
                Verbose("All codefiles generated succesfully.");
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }
    }
}
