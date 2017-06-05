using System;
using System.Text.RegularExpressions;

namespace Starcounter.ErrorCodes {
    /// <summary>
    /// Exposes the properties of a parsed error message string in the
    /// form or a <see cref="ErrorMessage"/>.
    /// </summary>
    public sealed class ParsedErrorMessage : ErrorMessage {
        private readonly string givenMessage;
        private readonly uint code;
        private readonly string header;
        private readonly string body;
        private readonly string message;
        private readonly string helplink;
        private readonly string version;

        /// <summary>
        /// Creates an error message from an error message string.
        /// </summary>
        /// <param name="errorMessage">The message string to parse.</param>
        /// <returns>An error message exposing the properties of the parsed
        /// error message string.</returns>
        internal new static ParsedErrorMessage Parse(string errorMessage) {
            try {
                return InternalParseMessage(errorMessage);
            } catch (Exception e) {
                if (ErrorCode.IsFromErrorCode(e))
                    throw;

                throw ToParsingException(errorMessage, e);
            }
        }

        private static ParsedErrorMessage InternalParseMessage(string errorMessage) {
            int index;
            int indexToDecoration;
            string header;
            uint code;
            string helplink;
            string message;
            string body;
            string versionMessage;
            string version;

            if (string.IsNullOrEmpty(errorMessage))
                throw new ArgumentNullException("errorMessage");

            // First extract the header, and via that, get the
            // error code. They are needed to parse the string.

            code = 0;
            index = ErrorMessage.IndexOfHeaderBodyDelimiter(errorMessage);
            header = errorMessage.Substring(0, index);
            if (!header.Contains("(") || !header.Contains(")")) {
                header = header.ToLowerInvariant();
                if (header.Contains(".") && header.EndsWith("exception")) {
                    errorMessage = errorMessage.Substring(index + 1);
                    return InternalParseMessage(errorMessage);
                }

                throw ToParsingException(errorMessage);
            }

            // Get the error code from the header
            
            // We expect 1 match with 3 groups, where the last group will be the digits of the code.
            // If not, we have an invalid message that does not contain a Starcounter specific code.
            MatchCollection matches = Regex.Matches(header, @"(?i)SC(ERR|WARN)(\d+)");
            if (matches.Count != 1 || matches[0].Groups.Count != 3) throw ToParsingException(errorMessage);
            
            string number = matches[0].Groups[2].Value; // Both indexes are verified in the statement above.
            code = uint.Parse(number);

            // Get the decoration. The parsing of the message assumes
            // the message string is from the current version; if it is
            // not, parsing will fail.

            helplink = ErrorCode.ToHelpLink(code);

            version = ErrorCode.ExceptionFactory.StarcounterVersion; 
            versionMessage = ErrorCode.ToVersionMessage();
            indexToDecoration = errorMessage.LastIndexOf(versionMessage);
            if (indexToDecoration == -1) throw ToParsingException(errorMessage);

            // With the index of the header-body delimiter still in the
            // register, get the message and the body.

            message = errorMessage.Remove(indexToDecoration);
            message = message.Trim();
            body = message.Substring(index + 1);
            body = body.Trim();

            return new ParsedErrorMessage(errorMessage, code, header, body, message, version, helplink);
        }

        /// <inheritdoc />
        public override uint Code {
            get { return code; }
        }

        /// <inheritdoc />
        public override string Header {
            get { return header; }
        }

        /// <inheritdoc />
        public override string Body {
            get { return body; }
        }

        /// <inheritdoc />
        public override string ShortMessage {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Message {
            get { return message; }
        }

        /// <inheritdoc />
        public override string Helplink {
            get { return helplink; }
        }

        /// <inheritdoc />
        public override string Version {
            get { return version; }
        }

        /// <inheritdoc />
        public override string ToString() {
            return givenMessage;
        }

        private ParsedErrorMessage(
            string input,
            uint code,
            string header,
            string body,
            string message,
            string version,
            string helplink) {
            this.givenMessage = input;
            this.code = code;
            this.header = header;
            this.body = body;
            this.message = message;
            this.version = version;
            this.helplink = helplink;
        }

        internal static Exception ToParsingException(string parsedMessage) {
            return ToParsingException(parsedMessage, null);
        }

        internal static Exception ToParsingException(string parsedMessage, Exception innerException) {
            return ErrorCode.ToException(
                Error.SCERRWRONGERRORMESSAGEFORMAT,
                innerException,
                string.Format("Message: {0}", parsedMessage)
                );
        }
    }
}
