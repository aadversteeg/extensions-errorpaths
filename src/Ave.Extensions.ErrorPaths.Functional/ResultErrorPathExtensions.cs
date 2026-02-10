using System;
using System.Threading.Tasks;
using Ave.Extensions.Functional;

namespace Ave.Extensions.ErrorPaths.Functional
{
    /// <summary>
    /// Provides extension methods for error path matching on Result types.
    /// </summary>
    public static class ResultErrorPathExtensions
    {
        /// <summary>
        /// Checks if the Result is a failure with an error code that matches the specified ancestor code.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="source">The source Result instance.</param>
        /// <param name="code">The error code to match against.</param>
        /// <returns>true if the Result is a failure and its error code is a child of (or equal to) the specified code; otherwise, false.</returns>
        public static bool HasError<T>(this Result<T, Error> source, ErrorCode code)
        {
            return source.IsFailure && source.Error.Is(code);
        }

        /// <summary>
        /// Checks if the awaitable Result is a failure with an error code that matches the specified ancestor code.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result instance.</param>
        /// <param name="code">The error code to match against.</param>
        /// <returns>A task that resolves to true if the Result is a failure and its error code is a child of (or equal to) the specified code; otherwise, false.</returns>
        public static async Task<bool> HasError<T>(this Task<Result<T, Error>> awaitableSource, ErrorCode code)
        {
            var source = await awaitableSource.ConfigureAwait(false);
            return source.IsFailure && source.Error.Is(code);
        }

        /// <summary>
        /// If the Result is a failure, wraps its error with a new error code and message.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="source">The source Result instance.</param>
        /// <param name="code">The wrapper error code.</param>
        /// <param name="message">The wrapper error message.</param>
        /// <returns>A new Result with the same success value if successful, or with the wrapped error if a failure.</returns>
        public static Result<T, Error> WrapError<T>(this Result<T, Error> source, ErrorCode code, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(source.Error.Wrap(code, message));
            }
            return Result<T, Error>.Success(source.Value);
        }

        /// <summary>
        /// If the awaitable Result is a failure, wraps its error with a new error code and message.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result instance.</param>
        /// <param name="code">The wrapper error code.</param>
        /// <param name="message">The wrapper error message.</param>
        /// <returns>A task that resolves to a new Result with the same success value if successful, or with the wrapped error if a failure.</returns>
        public static async Task<Result<T, Error>> WrapError<T>(this Task<Result<T, Error>> awaitableSource, ErrorCode code, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var source = await awaitableSource.ConfigureAwait(false);
            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(source.Error.Wrap(code, message));
            }
            return Result<T, Error>.Success(source.Value);
        }
    }
}
