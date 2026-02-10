using System;

namespace Ave.Extensions.ErrorPaths
{
    /// <summary>
    /// Factory methods for creating common errors with well-known error codes.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Creates a validation error with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A new Error with the Validation error code.</returns>
        public static Error Validation(string message)
        {
            return new Error(ErrorCodes.Validation._, message);
        }

        /// <summary>
        /// Creates a required field validation error.
        /// </summary>
        /// <param name="field">The name of the required field.</param>
        /// <returns>A new Error indicating the field is required.</returns>
        public static Error Required(string field)
        {
            return new Error(ErrorCodes.Validation.Required, $"The field '{field}' is required.")
                .With("field", field);
        }

        /// <summary>
        /// Creates a format validation error.
        /// </summary>
        /// <param name="field">The name of the field with invalid format.</param>
        /// <param name="expected">The expected format description.</param>
        /// <returns>A new Error indicating the field has an invalid format.</returns>
        public static Error Format(string field, string expected)
        {
            return new Error(ErrorCodes.Validation.Format, $"The field '{field}' has an invalid format. Expected: {expected}")
                .With("field", field)
                .With("expected", expected);
        }

        /// <summary>
        /// Creates a not found error with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A new Error with the NotFound error code.</returns>
        public static Error NotFound(string message)
        {
            return new Error(ErrorCodes.NotFound._, message);
        }

        /// <summary>
        /// Creates a not found error for a specific entity.
        /// </summary>
        /// <param name="entity">The type of entity that was not found.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>A new Error indicating the entity was not found.</returns>
        public static Error NotFound(string entity, object id)
        {
            return new Error(ErrorCodes.NotFound.Entity, $"{entity} with id '{id}' was not found.")
                .With("entity", entity)
                .With("id", id);
        }

        /// <summary>
        /// Creates an unauthorized error.
        /// </summary>
        /// <param name="reason">Optional reason for the unauthorized status.</param>
        /// <returns>A new Error with the Unauthorized error code.</returns>
        public static Error Unauthorized(string? reason = null)
        {
            var message = string.IsNullOrEmpty(reason)
                ? "Authentication is required."
                : reason;
            return new Error(ErrorCodes.Auth.Unauthorized, message);
        }

        /// <summary>
        /// Creates a forbidden error.
        /// </summary>
        /// <param name="reason">Optional reason for the forbidden status.</param>
        /// <returns>A new Error with the Forbidden error code.</returns>
        public static Error Forbidden(string? reason = null)
        {
            var message = string.IsNullOrEmpty(reason)
                ? "Access to this resource is forbidden."
                : reason;
            return new Error(ErrorCodes.Auth.Forbidden, message);
        }

        /// <summary>
        /// Creates a timeout error.
        /// </summary>
        /// <param name="operation">The operation that timed out.</param>
        /// <returns>A new Error with the Timeout error code.</returns>
        public static Error Timeout(string operation)
        {
            return new Error(ErrorCodes.IO.Timeout, $"The operation '{operation}' timed out.")
                .With("operation", operation);
        }

        /// <summary>
        /// Creates a network error.
        /// </summary>
        /// <param name="message">The error message describing the network failure.</param>
        /// <returns>A new Error with the Network error code.</returns>
        public static Error Network(string message)
        {
            return new Error(ErrorCodes.IO.Network, message);
        }

        /// <summary>
        /// Creates an unexpected internal error.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A new Error with the Unexpected error code.</returns>
        public static Error Unexpected(string message)
        {
            return new Error(ErrorCodes.Internal.Unexpected, message);
        }

        /// <summary>
        /// Creates an unexpected internal error from an exception.
        /// </summary>
        /// <param name="ex">The exception that caused the error.</param>
        /// <returns>A new Error with the Unexpected error code and exception details.</returns>
        public static Error Unexpected(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            return new Error(ErrorCodes.Internal.Unexpected, ex.Message)
                .With("exceptionType", ex.GetType().FullName ?? ex.GetType().Name);
        }

        /// <summary>
        /// Creates an error with a custom error code.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>A new Error with the specified code and message.</returns>
        public static Error From(ErrorCode code, string message)
        {
            return new Error(code, message);
        }
    }
}
