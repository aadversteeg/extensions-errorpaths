using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ave.Extensions.ErrorPaths.AspNetCore
{
    /// <summary>
    /// Provides extension methods for converting errors to HTTP responses.
    /// </summary>
    public static class ErrorHttpExtensions
    {
        /// <summary>
        /// Converts an <see cref="Error"/> to a <see cref="ProblemDetails"/> instance.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <param name="instance">Optional instance URI identifying the specific occurrence.</param>
        /// <returns>A ProblemDetails instance representing the error.</returns>
        public static ProblemDetails ToProblemDetails(this Error error, string? instance = null)
        {
            var statusCode = error.Code.ToHttpStatusCodeInt();

            var problemDetails = new ProblemDetails
            {
                Type = $"urn:error:{error.Code.Value}",
                Title = GetTitleForStatusCode(statusCode),
                Status = statusCode,
                Detail = error.Message,
                Instance = instance
            };

            // Add metadata as extension properties
            if (error.Metadata != null)
            {
                foreach (var kvp in error.Metadata)
                {
                    problemDetails.Extensions[kvp.Key] = kvp.Value;
                }
            }

            // Add inner error info if present
            if (error.Inner.HasValue)
            {
                problemDetails.Extensions["innerError"] = new
                {
                    code = error.Inner.Value.Code.Value,
                    message = error.Inner.Value.Message
                };
            }

            return problemDetails;
        }

        /// <summary>
        /// Converts an <see cref="Error"/> to an <see cref="IResult"/> for minimal APIs.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <param name="instance">Optional instance URI identifying the specific occurrence.</param>
        /// <returns>An IResult that produces a Problem response.</returns>
        public static IResult ToHttpResult(this Error error, string? instance = null)
        {
            var problemDetails = error.ToProblemDetails(instance);
            return Results.Problem(problemDetails);
        }

        /// <summary>
        /// Gets the HTTP status code for an error.
        /// </summary>
        /// <param name="error">The error to get the status code for.</param>
        /// <returns>The HTTP status code as an integer.</returns>
        public static int GetHttpStatusCode(this Error error)
        {
            return error.Code.ToHttpStatusCodeInt();
        }

        private static string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                408 => "Request Timeout",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                429 => "Too Many Requests",
                500 => "Internal Server Error",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                _ => "Error"
            };
        }
    }
}
