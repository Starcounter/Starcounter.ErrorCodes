
using System;
using System.Diagnostics;
using System.Threading;

namespace Starcounter.ErrorCodes.Generator
{
    internal class Program
    {
        internal static void WaitForDebugger()
        {
            Console.Write("Waiting for debugger");
            while (!Debugger.IsAttached)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
        }

        static int Main(string[] args)
        {
            try
            {
                var cli = new CommandLineInterface();
                cli.Execute(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine();
                return 1;
            }
            return 0;
        }
    }
}
