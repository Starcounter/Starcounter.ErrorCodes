using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Starcounter.ErrorCodes {
    /// <summary>
    /// Utility class exposing the API entrypoint methods used to handle errors
    /// and exceptions in Starcounter code, to make it confirm to the standards.
    /// </summary>
    public static class ErrorCode {
        public const string EC_TRANSPORT_KEY = "554CD586-C9DB-4d4a-B952-EA9890D1FB96";
        public const string CodeDecorationPrefix = "SCERR";

        public static ExceptionFactory ExceptionFactory { get; private set; }

        private static string DecoratedCodePattern = string.Concat("^", CodeDecorationPrefix, @"\d{3,5}$");

        static ErrorCode() {
            ErrorCode.ExceptionFactory = new ExceptionFactory();
        }

        public static FactoryErrorMessage ToMessage(uint errorCode) {
            return InternalToMessage(errorCode, string.Empty);
        }

        public static FactoryErrorMessage ToMessage(uint errorCode, string postfix) {
            return InternalToMessage(errorCode, postfix);
        }

        public static string ToMessage(string prefix, uint errorCode, string postfix) {
            string message = InternalToMessage(errorCode, postfix);

            if (!string.IsNullOrEmpty(prefix))
                message = string.Concat(prefix, " ", message);

            return message;
        }

        public static FactoryErrorMessage ToMessageWithArguments(uint errorCode, string messagePostfix, params object[] messageArguments) {
            return InternalToMessage(errorCode, messagePostfix, messageArguments);
        }

        public static string ToHelpLink(uint errorCode) {
            return string.Format("https://docs.starcounter.io/search/?query=SCERR{0}", errorCode);
        }

        /// <summary>
        /// Creates the help link message from a given error code.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>The help link message.</returns>
        /// <seealso cref="ToHelpLinkMessage(string)"/>
        private static string ToHelpLinkMessage(uint errorCode) {
            return ToHelpLinkMessage(ToHelpLink(errorCode));
        }

        /// <summary>
        /// Creates the help link message from a given help link.
        /// </summary>
        /// <param name="helplink">The help link in raw URL form.</param>
        /// <returns>The help link message.</returns>
        /// <example>
        /// string msg = ToHelpLinkMessage("http://www.starcounter.com/wiki/SCERR12345");
        /// Console.Write(msg);
        /// /* Output: Help page: http://www.starcounter.com/wiki/SCERR12345. */
        /// </example>
        public static string ToHelpLinkMessage(string helplink) {
            return string.Concat("Help page: ", helplink, ".");
        }

        /// <summary>
        /// Creates the version message from current version.
        /// </summary>
        /// <returns>A string including the version.</returns>
        /// <example>
        /// string msg = ToVersionMessage();
        /// Console.Write(msg);
        /// /* Output: Version: 2.0.123. */
        /// </example>
        public static string ToVersionMessage() {
            return string.Concat("Version: ", ExceptionFactory.StarcounterVersion, ".");
        }

        /// <summary>
        /// Converts an error code to a decorated code string, i.e. the
        /// code 1234 would return a string such as "SCERR1234".
        /// </summary>
        /// <param name="errorCode">The error code to convert.</param>
        /// <returns>The decorated code string.</returns>
        public static string ToDecoratedCode(uint errorCode) {
            return string.Concat(ErrorCode.CodeDecorationPrefix, errorCode);
        }

        public static bool TryGetOrigMessage(Exception exception, out String origMessage) {
            if (exception == null)
                throw ErrorCode.ToException(Error.SCERRBADARGUMENTS);

            if (!exception.Data.Contains(ErrorCode.EC_TRANSPORT_KEY)) {
                origMessage = null;
                return false;
            }

            // TODO: Figure out how to extract postfix message.
            origMessage = exception.Message;
            return true;
        }

        /// <summary>
        /// Gets a value indicating if the given exception stems from an
        /// error code, i.e. is most likely created using the 
        /// <see cref="ExceptionFactory"/> or a specialization thereof.
        /// </summary>
        /// <param name="exception">The exception to evaluate.</param>
        /// <returns>
        /// True if the exception stems from a error code, false if not.
        /// </returns>
        public static bool IsFromErrorCode(Exception exception) {
            uint ignored;
            return TryGetCode(exception, out ignored);
        }

        /// <summary>
        /// Gets a value indicating if the given exception stems from an
        /// error code, i.e. is most likely created using the 
        /// <see cref="ExceptionFactory"/> or a specialization thereof,
        /// and the error code is equal to <c>code</c>.
        /// </summary>
        /// <param name="exception">The exception to evaluate.</param>
        /// <param name="code">The error code to compare against.</param>
        /// <returns>
        /// True if the exception stems from the error code <c>code</c>;
        /// false if not.
        /// </returns>
        public static bool IsFromErrorCode(Exception exception, uint code) {
            uint candidate;
            if (TryGetCode(exception, out candidate)) {
                return candidate == code;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating if the given string matches the format
        /// of a decorated Starcounter error code (e.g. "SCERR1234").
        /// </summary>
        /// <param name="errorCodeString">The string to validate.</param>
        /// <returns>True if match. False otherwise.</returns>
        public static bool IsDecoratedErrorCode(string errorCodeString) {
            if (string.IsNullOrEmpty(errorCodeString))
                throw ErrorCode.ToException(Error.SCERRBADARGUMENTS);

            return Regex.IsMatch(errorCodeString, ErrorCode.DecoratedCodePattern);
        }

        /// <summary>
        /// Gets the error code from a given decorated error code string,
        /// i.e one with the "SCERR1234" format.
        /// </summary>
        /// <remarks>
        /// Its up to the caller to check that the string confirms, either
        /// by knowing so or by testing with <see cref="IsDecoratedErrorCode"/>.
        /// Failure to confirm will result in unexpected behaviour. To do
        /// both the check and the parsing, use <see cref="TryParseDecorated"/>.
        /// </remarks>
        /// <param name="errorCodeString">The decorated error code string,
        /// in the form "SCERR1234".</param>
        /// <returns>The error code parsed out from the string and converted
        /// to a number.</returns>
        public static uint ParseDecorated(string errorCodeString) {
            return uint.Parse(errorCodeString.Substring(5));
        }

        /// <summary>
        /// Tries to get the error code from a given decorated error code string,
        /// i.e one with the "SCERR1234" format.
        /// </summary>
        /// <param name="errorCodeString">The decorated error code string,
        /// in the form "SCERR1234".</param>
        /// <param name="errorCode">
        /// The parsed out error code if the method succeeds.
        /// </param>
        /// <returns>True if the given string confirmed to standard. False if
        /// not.</returns>
        public static bool TryParseDecorated(string errorCodeString, out uint errorCode) {
            if (IsDecoratedErrorCode(errorCodeString)) {
                errorCode = ParseDecorated(errorCodeString);
                return true;
            }

            errorCode = 0;
            return false;
        }

        /// <summary>
        /// Tries the get code.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="errorCode">The error code.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool TryGetCode(Exception exception, out uint errorCode) {
            if (exception == null)
                throw ErrorCode.ToException(Error.SCERRBADARGUMENTS);

            if (!exception.Data.Contains(ErrorCode.EC_TRANSPORT_KEY)) {
                errorCode = 0;
                return false;
            }

            errorCode = (uint)exception.Data[ErrorCode.EC_TRANSPORT_KEY];
            return true;
        }

        /// <summary>
        /// Attempts to get the <see cref="ErrorMessage"/> from an <see cref="Exception"/>
        /// that is assumed to be created using a factory method exposed by this class.
        /// </summary>
        /// <param name="exception">
        /// The <see cref="Exception"/> to get the message from.</param>
        /// <param name="message">The <see cref="ErrorMessage"/> retrieved.</param>
        /// <returns>True if a message was retreived; else false.</returns>
        public static bool TryGetCodedMessage(Exception exception, out ErrorMessage message) {
            // Very rudimentary implementation, using the message parser
            // when attempting to fetch a message, and even relying on
            // exception handling in a Try... method pattern.
            //
            // We should try to redesign the factory method to instead
            // cache the ErrorMessage object in the Data dictionary when
            // exceptions are created and just return that.

            if (exception == null)
                throw ErrorCode.ToException(Error.SCERRBADARGUMENTS);

            uint code;
            if (TryGetCode(exception, out code)) {
                try {
                    message = ErrorMessage.Parse(exception.Message);
                    if (message.Code == code) {
                        // We do this check as an extra safety measure to assure
                        // we detect any future problem parsing a certain message
                        // that might look like an error message but really isnt.
                        // If the error code comes out correct, we at least know
                        // there is no risk of a serious damage (i.e. the will at
                        // least represent the same error).
                        return true;
                    }
                }
                catch { }
            }

            message = null;
            return false;
        }

        public static uint ToFacilityCode(uint errorCode) {
            return errorCode / 1000;
        }

        public static Exception ToException(uint errorCode) {
            return InternalCreateException(errorCode, null, null, null, null);
        }

        public static Exception ToException(uint errorCode, Func<string, Exception, Exception> customFactory) {
            return InternalCreateException(errorCode, null, null, customFactory, null);
        }

        public static Exception ToException(uint errorCode, Exception innerException) {
            return InternalCreateException(errorCode, innerException, null, null, null);
        }

        public static Exception ToException(uint errorCode, Exception innerException, Func<string, Exception, Exception> customFactory) {
            return InternalCreateException(errorCode, innerException, null, customFactory, null);
        }

        public static Exception ToException(uint errorCode, string messagePostfix) {
            return InternalCreateException(errorCode, null, messagePostfix, null, null);
        }

        public static Exception ToException(uint errorCode, string messagePostfix, Func<string, Exception, Exception> customFactory) {
            return InternalCreateException(errorCode, null, messagePostfix, customFactory, null);
        }

        public static Exception ToException(uint errorCode, Exception innerException, string messagePostfix) {
            return InternalCreateException(errorCode, innerException, messagePostfix, null, null);
        }

        public static Exception ToException(
            uint errorCode,
            Exception innerException,
            string messagePostfix,
            Func<string, Exception, Exception> customFactory) {
            return InternalCreateException(errorCode, innerException, messagePostfix, customFactory, null);
        }

        public static Exception ToException(
            uint errorCode,
            Func<string, Exception, Exception> customFactory,
            params object[] messageArguments
            ) {
            return InternalCreateException(errorCode, null, null, customFactory, messageArguments);
        }

        public static Exception ToException(
            uint errorCode,
            Exception innerException,
            Func<string, Exception, Exception> customFactory,
            params object[] messageArguments
            ) {
            return InternalCreateException(errorCode, innerException, null, customFactory, messageArguments);
        }

        public static Exception ToException(
            uint errorCode,
            Exception innerException,
            string messagePostfix,
            params object[] messageArguments
            ) {
            return InternalCreateException(errorCode, innerException, messagePostfix, null, messageArguments);
        }

        public static Exception ToException(
            uint errorCode,
            Exception innerException,
            string messagePostfix,
            Func<string, Exception, Exception> customFactory,
            params object[] messageArguments
            ) {
            return InternalCreateException(errorCode, innerException, messagePostfix, customFactory, messageArguments);
        }

        public static void SetExceptionFactory(ExceptionFactory factory) {
            if (factory == null) throw new ArgumentNullException("factory");
            ErrorCode.ExceptionFactory = factory;
        }

        /// <summary>
        /// Recreates an exception from a given error code with an
        /// already available error message string. Used by the
        /// infrastructure to recreate exceptions after they have
        /// travelled from one node to another.
        /// </summary>
        /// <remarks>
        /// There is currently a minor flaw in this design: errors
        /// using so called "custom exception factories" to create
        /// their exceptions will not be able to be exactly
        /// recreated. The best thing to do is to add a fallback for
        /// all custom-created exceptions, since they usually can
        /// be recreated using the same type, but maybe only with a
        /// little less accurate exception properties.
        /// </remarks>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="innerException">A possible inner exception.</param>
        /// <returns>The exception recreated.</returns>
        internal static Exception RecreateException(
            uint errorCode,
            string message,
            Exception innerException)
        {
            Func<uint, string, object[], string> messageFactory;

            messageFactory = delegate(uint code, string postfix, object[] arguments)
            {
                return message;
            };

            return ErrorCode.ExceptionFactory.CreateException(
                errorCode,
                innerException,
                string.Empty,
                messageFactory
                );
        }

        private static FactoryErrorMessage InternalToMessage(
            uint errorCode,
            string messagePostfix,
            params object[] messageArguments
            ) {
            
            string msg = Error.ToMessage(errorCode);
            return new FactoryErrorMessage(errorCode, msg, messagePostfix, messageArguments);
        }

        private static Exception InternalCreateException(
            uint errorCode,
            Exception innerException,
            string messagePostfix,
            Func<string, Exception, Exception> customFactory,
            params object[] messageArguments) {
            Func<uint, string, object[], string> messageFactory;

            if (errorCode == 0) {
                throw ErrorCode.ToException(
                    Error.SCERRBADARGUMENTS,
                    "ErrorCode.ToException(0) is invalid. Code 0 (zero) is reserved for successful results, not errors.");
            }

            // Use default message creation factory

            messageFactory = delegate(uint code, string postfix, object[] arguments) {
                return ToMessageWithArguments(code, postfix, arguments).ToString();
            };

            // Decide if we have a custom exception factory to consider or
            // not, and invoke the proper exception creation routine.

            if (customFactory == null) {
                return ErrorCode.ExceptionFactory.CreateException(
                    errorCode,
                    innerException,
                    messagePostfix,
                    messageFactory,
                    messageArguments
                    );
            }

            return CreateExceptionUsingCustomFactory(
                errorCode,
                innerException,
                messagePostfix,
                customFactory,
                messageFactory,
                messageArguments
                );
        }

        private static Exception CreateExceptionUsingCustomFactory(
            uint errorCode,
            Exception innerException,
            string messagePostfix,
            Func<string, Exception, Exception> customFactory,
            Func<uint, string, object[], string> messageFactory,
            params object[] messageArguments
            ) {
            string msg;
            Exception ex;

            // Format the message, according to the given input,
            // using the supplied error message factory.

            msg = messageFactory(errorCode, messagePostfix, messageArguments);

            // Create the appropriate exception type by invoking the
            // custom exception factory.

            ex = customFactory(msg, innerException);

            // Validate the result of the custom factory (compare message, check not null, etc)
            // Since this targets our own developers, we just use asserts of the Trace API to
            // check all is correct.

            Debug.Assert(ex != null);
            Debug.Assert(
                !ex.GetType().FullName.Equals("Starcounter.DbException"),
                "Do not use ErrorCode.ToException API with custom exception factory to create DbException.");
            Debug.Assert(
                !string.IsNullOrEmpty(ex.Message),
                "Message must be specified and must contain the message passed to the custom exception factory.");
            Debug.Assert(
                ex.Message.Contains(msg),
                "Message must contain the message passed to the custom exception factory.");

            // Finally decorate the exception before we return it.

            return DecorateException(ex, errorCode);
        }

        internal static Exception DecorateException(Exception exception, uint errorCode) {
            exception.Data[ErrorCode.EC_TRANSPORT_KEY] = errorCode;
            exception.HelpLink = ErrorCode.ToHelpLink(errorCode);
            exception.Source = string.Format(
                "Starcounter({0})",
                ExceptionFactory.StarcounterVersion
                );
            return exception;
        }
    }
}
