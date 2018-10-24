namespace Starcounter.ErrorCodes.Tests
{
    using System;
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
                Action createException = () => factory.CreateException(0, null, null, null, null);

                //Then
                var e = Assert.Throws<ArgumentNullException>(createException);
                Assert.NotEmpty(e.Message);
                Assert.NotEmpty(e.ParamName);
            }
        }
    }
}