using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// For an aggregate error (see <see cref="Errors.Aggregate"/>) the status follows <see cref="GetHttpStatusCode"/>
        /// and an <c>errors</c> extension lists every part with its code, message and metadata.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <param name="instance">Optional instance URI identifying the specific occurrence.</param>
        /// <returns>A ProblemDetails instance representing the error.</returns>
        public static ProblemDetails ToProblemDetails(this Error error, string? instance = null)
        {
            var statusCode = error.GetHttpStatusCode();

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

            // Add the parts of an aggregate error
            var parts = error.Parts();
            if (parts.Count > 1)
            {
                problemDetails.Extensions["errors"] = parts.Select(ToPartEntry).ToList();
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
        /// For an aggregate error (see <see cref="Errors.Aggregate"/>) a custom mapping registered for
        /// <see cref="ErrorCodes.Aggregate"/> wins. Without one, the status follows from the parts: their common status when
        /// they all map to the same one, 400 Bad Request when they are all client errors (4xx), and 500 Internal Server Error otherwise.
        /// </summary>
        /// <param name="error">The error to get the status code for.</param>
        /// <returns>The HTTP status code as an integer.</returns>
        public static int GetHttpStatusCode(this Error error)
        {
            var parts = error.Parts();
            if (parts.Count <= 1)
            {
                return error.Code.ToHttpStatusCodeInt();
            }

            if (ErrorCodeHttpMapping.TryGetCustomMapping(error.Code, out var custom))
            {
                return (int)custom;
            }

            var statuses = parts.Select(part => part.Code.ToHttpStatusCodeInt()).Distinct().ToList();
            if (statuses.Count == 1)
            {
                return statuses[0];
            }

            return statuses.All(status => status >= 400 && status < 500)
                ? (int)HttpStatusCode.BadRequest
                : (int)HttpStatusCode.InternalServerError;
        }

        private static Dictionary<string, object?> ToPartEntry(Error part)
        {
            var entry = new Dictionary<string, object?>
            {
                ["code"] = part.Code.Value,
                ["message"] = part.Message
            };

            if (part.Metadata != null)
            {
                entry["metadata"] = part.Metadata;
            }

            return entry;
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
