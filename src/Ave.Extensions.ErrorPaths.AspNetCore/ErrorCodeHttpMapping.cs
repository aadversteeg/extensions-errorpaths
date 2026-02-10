using System.Collections.Generic;
using System.Net;

namespace Ave.Extensions.ErrorPaths.AspNetCore
{
    /// <summary>
    /// Provides mapping between error codes and HTTP status codes.
    /// </summary>
    public static class ErrorCodeHttpMapping
    {
        private static readonly Dictionary<string, HttpStatusCode> _customMappings = new Dictionary<string, HttpStatusCode>();

        private static readonly Dictionary<string, HttpStatusCode> _defaultMappings = new Dictionary<string, HttpStatusCode>
        {
            { "Validation", HttpStatusCode.BadRequest },
            { "NotFound", HttpStatusCode.NotFound },
            { "Auth.Unauthorized", HttpStatusCode.Unauthorized },
            { "Auth.Forbidden", HttpStatusCode.Forbidden },
            { "Auth.TokenExpired", HttpStatusCode.Unauthorized },
            { "Auth.TokenInvalid", HttpStatusCode.Unauthorized },
            { "Auth", HttpStatusCode.Unauthorized },
            { "IO.Timeout", HttpStatusCode.GatewayTimeout },
            { "IO", HttpStatusCode.BadGateway },
            { "Internal", HttpStatusCode.InternalServerError }
        };

        /// <summary>
        /// Registers a custom mapping from an error code to an HTTP status code.
        /// Custom mappings take precedence over default mappings.
        /// </summary>
        /// <param name="code">The error code to map.</param>
        /// <param name="statusCode">The HTTP status code to return for this error code.</param>
        public static void Register(ErrorCode code, HttpStatusCode statusCode)
        {
            _customMappings[code.Value] = statusCode;
        }

        /// <summary>
        /// Registers a custom mapping from an error code to an HTTP status code using an integer.
        /// Custom mappings take precedence over default mappings.
        /// </summary>
        /// <param name="code">The error code to map.</param>
        /// <param name="statusCode">The HTTP status code to return for this error code.</param>
        public static void Register(ErrorCode code, int statusCode)
        {
            _customMappings[code.Value] = (HttpStatusCode)statusCode;
        }

        /// <summary>
        /// Clears all custom mappings.
        /// </summary>
        public static void ClearCustomMappings()
        {
            _customMappings.Clear();
        }

        /// <summary>
        /// Gets the HTTP status code for the specified error code.
        /// Walks up the error code hierarchy until a mapping is found.
        /// </summary>
        /// <param name="code">The error code to map.</param>
        /// <returns>The HTTP status code, or 500 Internal Server Error if no mapping is found.</returns>
        public static HttpStatusCode ToHttpStatusCode(this ErrorCode code)
        {
            if (string.IsNullOrEmpty(code.Value))
            {
                return HttpStatusCode.InternalServerError;
            }

            // Try exact match in custom mappings first
            if (_customMappings.TryGetValue(code.Value, out var customStatus))
            {
                return customStatus;
            }

            // Try exact match in default mappings
            if (_defaultMappings.TryGetValue(code.Value, out var defaultStatus))
            {
                return defaultStatus;
            }

            // Walk up the hierarchy
            var current = code.Parent;
            while (current.HasValue)
            {
                if (_customMappings.TryGetValue(current.Value.Value, out customStatus))
                {
                    return customStatus;
                }

                if (_defaultMappings.TryGetValue(current.Value.Value, out defaultStatus))
                {
                    return defaultStatus;
                }

                current = current.Value.Parent;
            }

            // Default to 500 Internal Server Error
            return HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Gets the HTTP status code as an integer for the specified error code.
        /// </summary>
        /// <param name="code">The error code to map.</param>
        /// <returns>The HTTP status code as an integer.</returns>
        public static int ToHttpStatusCodeInt(this ErrorCode code)
        {
            return (int)code.ToHttpStatusCode();
        }
    }
}
