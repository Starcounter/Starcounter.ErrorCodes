using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Xunit;
using Starcounter.ErrorCodes;

namespace Starcounter.ErrorCodes.Tests
{
    public class Tests
    {
        private const uint ERR_CODE1 = Error.SCERRBADSERVERCONFIG;
        private const string ERR_MSG1 = "ScErrBadServerConfig (SCERR2115): Server configuration is invalid. Version: 0.0.0.";

        private const uint ERR_CODE2 = Error.SCERRFIELDREDECLARATION;
        private const string ERR_MSG2 = "SCDCV06 - ScErrFieldRedeclaration (SCERR4050): A database class declared a "
                                        + "persistent field that has already been declared in one of the parent classes. "
                                        + "Version: 0.0.0. Help page: https://github.com/Starcounter/Starcounter/wiki/SCERR4050.";

        public Tests()
        {
            WaitForDebugger();
        }

        private void WaitForDebugger()
        {
            if (Environment.GetEnvironmentVariable("SCDEBUG") == "T")
            {
                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(200);
                }
            }
        }

        [Theory]
        [InlineData(ERR_CODE1, ERR_MSG1)]
        [InlineData(ERR_CODE2, ERR_MSG2)]
        public void TestParseErrorMessage(uint errorCode, string errorStr)
        {
            var errorMessage = ErrorMessage.Parse(errorStr);

            Assert.NotNull(errorMessage);
            Assert.Equal(errorCode, errorMessage.Code);
        }
    }
}