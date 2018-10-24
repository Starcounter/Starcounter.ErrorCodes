using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.IO;

namespace Starcounter.ErrorCodes.Generator
{
    internal class CommandLineInterface
    {
        private CommandLineApplication commandLineApplication;
        private CommandArgument sourceArgument;
        private CommandOption csharpOption;
        private CommandOption cOption;
        private CommandOption headerOption;
        private CommandOption verboseOption;
        private CommandOption helpOption;
        private CommandOption debugOption;
        private CommandOption countOption;

        public CommandLineInterface()
        {
            commandLineApplication = new CommandLineApplication();
            commandLineApplication.OnExecute(() => Run());

            sourceArgument = commandLineApplication.Argument("sourcefile", "Path to the xml-sourcefile to read errorcodes from.");

            csharpOption = commandLineApplication.Option(
                "-cs | --csharp <csharpfile>", "Path to write generated C# code to.",
                CommandOptionType.SingleValue
            );
            cOption = commandLineApplication.Option(
                "-c | --c <cfile>", "Path to write generated C code to.",
                CommandOptionType.SingleValue
            );
            headerOption = commandLineApplication.Option(
                "-header | --header <headerfile>", "Path to write generated header to.",
                CommandOptionType.SingleValue
            );
            verboseOption = commandLineApplication.Option(
                "-v | --verbose", "Verbose mode.",
                CommandOptionType.NoValue
            );
            countOption = commandLineApplication.Option(
                "-cnt | --count <file>", "Path to write the number of errorcodes to.",
                CommandOptionType.SingleValue
            );
            debugOption = commandLineApplication.Option(
                "-d | --debug", "Wait until debugger is attached.",
                CommandOptionType.NoValue
            );

            debugOption.ShowInHelpText = false;
            helpOption = commandLineApplication.HelpOption("-? | -h | --help");
        }

        public int Execute(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    commandLineApplication.ShowHelp();
                    return 0;
                }
                int i = commandLineApplication.Execute(args);
                return i;
            }
            catch (Exception)
            {
                //                commandLineApplication.ShowHelp();
                throw;
            }
        }

        private int Run()
        {
            string srcFilePath;
            ErrorFile errorFile;
            CodeGenerator generator;

            if (debugOption.HasValue())
            {
                Program.WaitForDebugger();
            }

            if (sourceArgument.Value == null)
            {
                throw new ArgumentException("Sourcefile argument is mandatory and must be specified.");
            }

            if (!csharpOption.HasValue() && !cOption.HasValue() && !headerOption.HasValue() && !countOption.HasValue())
            {
                Verbose("No output specified.");
                return -1;
            }

            srcFilePath = Path.GetFullPath(sourceArgument.Value);

            Verbose("Reading file with errorcodes ({0})", srcFilePath);
            errorFile = ErrorFileReader.ReadErrorCodes(srcFilePath);

            if (csharpOption.HasValue())
            {
                Verbose("Generating cs code to {0}", csharpOption.Value());
                generator = new CSharpCodeGenerator();
                generator.Generate(errorFile, csharpOption.Value());
            }

            if (cOption.HasValue())
            {
                Verbose("Generating c code to {0}", cOption.Value());
                generator = new CCodeGenerator();
                generator.Generate(errorFile, cOption.Value());
            }

            if (headerOption.HasValue())
            {
                Verbose("Generating c header to {0}", headerOption.Value());
                generator = new CHeaderGenerator();
                generator.Generate(errorFile, headerOption.Value());
            }

            if (countOption.HasValue())
            {
                Verbose("Writing errorcode count to {0]", headerOption.Value());
                File.WriteAllText(countOption.Value(), errorFile.Count.ToString());
            }

            Verbose("All codefiles generated succesfully.");
            return 0;
        }

        private void Verbose(string str, params object[] args)
        {
            if (verboseOption.HasValue())
            {
                Trace.TraceInformation(str, args);
                commandLineApplication.Out.WriteLine(str, args);
            }
        }
    }
}
