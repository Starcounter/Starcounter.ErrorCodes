using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Xunit;

namespace scerrcc.Tests {
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
            
            using (FileStream stream = File.Open("errorcodes.xml", FileMode.Open)) {
                errorFile = ErrorFileReader.ReadErrorCodes(stream);
            }
            Assert.NotNull(errorFile);
            Assert.NotEmpty(errorFile.ErrorCodes);
        }
    }
}