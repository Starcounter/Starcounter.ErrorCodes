using System;
using System.Diagnostics;
using System.Threading;

namespace Starcounter.ErrorCodes.Generator {
    // TODO:
    // Can probably be removed, and instead use standard commandline parsing methods. 
    // OR altogether changed to be triggered as a task instead...
    internal static class CommandLine {
        public static void ParseArgs(
                                string[] args,
                                out string srcFilePath,
                                out string csFilePath,
                                out string cFilePath,
                                out string hFilePath,
                                out bool verbose) 
                           {
            srcFilePath = null;
            verbose = false;
            csFilePath = null;
            cFilePath = null;
            hFilePath = null;
            
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
                        csFilePath = GetNextArg(args, i++);
                        break;
                    case "-c":
                        cFilePath = GetNextArg(args, i++);
                        break;
                    case "-h":
                        hFilePath = GetNextArg(args, i++);
                        break;
                    case "-debug":
                        WaitForDebugger();
                        break;
                    default:
                        throw new ArgumentException("Unknown switch: " + args[i]);
                }
            }
        }

        private static string GetNextArg(string[] args, int index) {
            if (args.Length <= (index + 1))
                throw new ArgumentException("No argument supplied for the " + args[index] + " switch");
            return args[index + 1];
        }

        private static void PrintUsage() {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("Starcounter.ErrorCodes.Generator.exe infile.xml [options]");
            Console.Error.WriteLine("Where [options] are:");
            Console.Error.WriteLine("-v          Verbose mode");
            Console.Error.WriteLine("-cs [file]  Path for file to write generated C# code to.");
            Console.Error.WriteLine("-c [file]   Path for file to write generated C code to.");
            Console.Error.WriteLine("-h [file]   Path for file to write generated header to.");
        }

        private static void WaitForDebugger(){
            Console.Write("Waiting for debugger");
            while (!Debugger.IsAttached) {
                Console.Write(".");
                Thread.Sleep(200);
            }
        }
    }
}
