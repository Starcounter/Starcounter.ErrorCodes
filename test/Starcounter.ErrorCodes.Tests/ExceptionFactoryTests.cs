namespace Starcounter.ErrorCodes.Tests
{
    using System;
    using System.IO;
    using Xunit;

    public class ExceptionFactoryTests
    {
        public class TheCreateExceptionClass
        {
            [Fact]
            public void ThrowsExceptionWhenMessageFactoryIsNull()
            {
                //Given
                var factory = new ExceptionFactory();

                //When
                Action createException = () => factory.CreateException(
                    0,
                    new Exception(),
                    "some string",
                    null);

                //Then
                var e = Assert.Throws<ArgumentNullException>(createException);
                Assert.NotEmpty(e.Message);
                Assert.NotEmpty(e.ParamName);
            }

            [Fact]
            public void ExceptionHasMessageCreatedByMessageFactory()
            {
                //Given
                var factory = new ExceptionFactory();
                var message = "MyMessage";
                Func<uint, string, object[], string> messageFactory = (x, y, z) => message;

                //When
                var exception = factory.CreateException(0, null, null, messageFactory);

                //Then
                Assert.Equal(message, exception.Message);
            }

            [Fact]
            public void ErrorCode3000CreatesIOException()
            {
                //Given
                uint errorCode = 3000;
                var factory = new ExceptionFactory();

                //When
                var exception = factory.CreateException(errorCode, null, null, (x, y, z) => "message");

                //Then
                Assert.IsType<IOException>(exception);
            }

            [Fact]
            public void HadInnerExceptionSetIfInnerExceptionIsPassed()
            {
                //Given
                var innerException = new Exception();
                var factory = new ExceptionFactory();

                //When
                var exception = factory.CreateException(0, innerException, null, (y, z, x) => "message");

                //Then
                Assert.Equal(innerException, exception.InnerException);
            }

            [Fact]
            public void HelpLinkIsSetForException()
            {
                //Given
                var factory = new ExceptionFactory();

                //When
                var exception = factory.CreateException(0, null, null, (x, y, z) => "message");

                //Then
                Assert.NotEmpty(exception.HelpLink);
            }

            [Fact]
            public void SourceIsSetForException()
            {
                //Given
                var factory = new ExceptionFactory();

                //When
                var exception = factory.CreateException(0, null, null, (x, y, z) => "message");

                //Then
                Assert.NotEmpty(exception.Source);
            }
        }
    }
}