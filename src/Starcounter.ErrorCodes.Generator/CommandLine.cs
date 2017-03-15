using System;

namespace Starcounter.ErrorCodes.Generator {
    // TODO:
    // Can probably be removed, and instead use standard commandline parsing methods. 
    // OR altogether changed to be triggered as a task instead...
    internal static class CommandLine {
        public static void ParseArgs(string[] args, out string srcFilePath, out string csFilePath, out bool verbose) {
            srcFilePath = null;
            verbose = false;
            csFilePath = null;
            
            if (args.Length == 0) {
                PrintUsage();
                return;
            }
            
            srcFilePath = args[0];
            for (int i = 1; i < args.Length; i++) {
                switch (args[i]) {
                    case "-v": 
                        verbose = true;
                        break; // handled elsewhere
                    case "-cs":
                        if (args.Length <= (i+1))
                            throw new ArgumentException("No argument supplied for -cs switch");
                        csFilePath = args[i+1];
                        i++;
                        break;
                    default:
                        throw new ArgumentException("Unknown switch: " + args[i]);
                }
            }
        }

        private static void PrintUsage() {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("Starcounter.ErrorCodes.Generator.exe infile.xml [options]");
            Console.Error.WriteLine("Where [options] are:");
            Console.Error.WriteLine("-v             Verbose mode");
            Console.Error.WriteLine("-cs [csfile]   Filepath for generated code c# code.");
        }
    }
}
