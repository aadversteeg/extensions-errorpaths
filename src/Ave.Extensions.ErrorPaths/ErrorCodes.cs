namespace Ave.Extensions.ErrorPaths
{
    /// <summary>
    /// Provides well-known error code hierarchies for common error scenarios.
    /// Use these codes directly or extend them with the / operator.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// Validation error codes for input validation failures.
        /// </summary>
        public static class Validation
        {
            /// <summary>
            /// The root validation error code.
            /// </summary>
            public static readonly ErrorCode _ = new ErrorCode("Validation");

            /// <summary>
            /// A required field or value is missing.
            /// </summary>
            public static readonly ErrorCode Required = _ / "Required";

            /// <summary>
            /// A value has an invalid format.
            /// </summary>
            public static readonly ErrorCode Format = _ / "Format";

            /// <summary>
            /// A value is out of the allowed range.
            /// </summary>
            public static readonly ErrorCode Range = _ / "Range";

            /// <summary>
            /// A value exceeds the maximum allowed length.
            /// </summary>
            public static readonly ErrorCode Length = _ / "Length";

            /// <summary>
            /// A value does not match the expected pattern.
            /// </summary>
            public static readonly ErrorCode Pattern = _ / "Pattern";

            /// <summary>
            /// A value is a duplicate when uniqueness is required.
            /// </summary>
            public static readonly ErrorCode Duplicate = _ / "Duplicate";

            /// <summary>
            /// A value is invalid for unspecified reasons.
            /// </summary>
            public static readonly ErrorCode Invalid = _ / "Invalid";
        }

        /// <summary>
        /// Not found error codes for missing resources.
        /// </summary>
        public static class NotFound
        {
            /// <summary>
            /// The root not found error code.
            /// </summary>
            public static readonly ErrorCode _ = new ErrorCode("NotFound");

            /// <summary>
            /// An entity or record was not found.
            /// </summary>
            public static readonly ErrorCode Entity = _ / "Entity";

            /// <summary>
            /// A file was not found.
            /// </summary>
            public static readonly ErrorCode File = _ / "File";

            /// <summary>
            /// A resource was not found.
            /// </summary>
            public static readonly ErrorCode Resource = _ / "Resource";
        }

        /// <summary>
        /// Authentication and authorization error codes.
        /// </summary>
        public static class Auth
        {
            /// <summary>
            /// The root authentication/authorization error code.
            /// </summary>
            public static readonly ErrorCode _ = new ErrorCode("Auth");

            /// <summary>
            /// The user is not authenticated.
            /// </summary>
            public static readonly ErrorCode Unauthorized = _ / "Unauthorized";

            /// <summary>
            /// The user is authenticated but not authorized for this action.
            /// </summary>
            public static readonly ErrorCode Forbidden = _ / "Forbidden";

            /// <summary>
            /// The authentication token has expired.
            /// </summary>
            public static readonly ErrorCode TokenExpired = _ / "TokenExpired";

            /// <summary>
            /// The authentication token is invalid.
            /// </summary>
            public static readonly ErrorCode TokenInvalid = _ / "TokenInvalid";
        }

        /// <summary>
        /// I/O and external service error codes.
        /// </summary>
        public static class IO
        {
            /// <summary>
            /// The root I/O error code.
            /// </summary>
            public static readonly ErrorCode _ = new ErrorCode("IO");

            /// <summary>
            /// A network operation failed.
            /// </summary>
            public static readonly ErrorCode Network = _ / "Network";

            /// <summary>
            /// An operation timed out.
            /// </summary>
            public static readonly ErrorCode Timeout = _ / "Timeout";

            /// <summary>
            /// A file system operation failed.
            /// </summary>
            public static readonly ErrorCode FileSystem = _ / "FileSystem";

            /// <summary>
            /// A database operation failed.
            /// </summary>
            public static readonly ErrorCode Database = _ / "Database";

            /// <summary>
            /// An external service call failed.
            /// </summary>
            public static readonly ErrorCode ExternalService = _ / "ExternalService";
        }

        /// <summary>
        /// Internal error codes for unexpected failures.
        /// </summary>
        public static class Internal
        {
            /// <summary>
            /// The root internal error code.
            /// </summary>
            public static readonly ErrorCode _ = new ErrorCode("Internal");

            /// <summary>
            /// An unexpected error occurred.
            /// </summary>
            public static readonly ErrorCode Unexpected = _ / "Unexpected";

            /// <summary>
            /// A configuration error occurred.
            /// </summary>
            public static readonly ErrorCode Configuration = _ / "Configuration";

            /// <summary>
            /// An assertion or invariant was violated.
            /// </summary>
            public static readonly ErrorCode Assertion = _ / "Assertion";
        }
    }
}
