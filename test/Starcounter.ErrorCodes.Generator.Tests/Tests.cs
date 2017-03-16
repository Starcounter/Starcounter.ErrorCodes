using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Xunit;

namespace Starcounter.ErrorCodes.Generator.Tests {
    public class Tests {
        public Tests() {
            WaitForDebugger();
        }

        private void WaitForDebugger(){
            if (Environment.GetEnvironmentVariable("SCDEBUG") == "T") {
                while (!Debugger.IsAttached) {
                    Thread.Sleep(200);
                }
            }
        }

        [Fact]
        public void TestParseErrorCodesFile() {
            ErrorFile errorFile = null;
            
            errorFile = ErrorFileReader.ReadErrorCodes("errorcodes.xml");
            
            Assert.NotNull(errorFile);
            Assert.NotEmpty(errorFile.ErrorCodes);
        }
    }
}