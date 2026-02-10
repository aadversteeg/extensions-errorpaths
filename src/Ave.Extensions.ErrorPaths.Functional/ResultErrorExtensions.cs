using System;
using System.Threading.Tasks;
using Ave.Extensions.Functional;

namespace Ave.Extensions.ErrorPaths.Functional
{
    /// <summary>
    /// Provides extension methods for transforming errors in Result types.
    /// </summary>
    public static class ResultErrorExtensions
    {
        /// <summary>
        /// If the Result is a failure, transforms its error using the provided mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="source">The source Result instance.</param>
        /// <param name="mapError">A function that transforms the error.</param>
        /// <returns>A new Result with the same success value if successful, or with the transformed error if a failure.</returns>
        public static Result<T, Error> MapError<T>(this Result<T, Error> source, Func<Error, Error> mapError)
        {
            if (mapError == null)
            {
                throw new ArgumentNullException(nameof(mapError));
            }

            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(mapError(source.Error));
            }
            return Result<T, Error>.Success(source.Value);
        }

        /// <summary>
        /// If the awaitable Result is a failure, transforms its error using the provided mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result instance.</param>
        /// <param name="mapError">A function that transforms the error.</param>
        /// <returns>A task that resolves to a new Result with the same success value if successful, or with the transformed error if a failure.</returns>
        public static async Task<Result<T, Error>> MapError<T>(this Task<Result<T, Error>> awaitableSource, Func<Error, Error> mapError)
        {
            if (mapError == null)
            {
                throw new ArgumentNullException(nameof(mapError));
            }

            var source = await awaitableSource.ConfigureAwait(false);
            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(mapError(source.Error));
            }
            return Result<T, Error>.Success(source.Value);
        }

        /// <summary>
        /// If the Result is a failure, asynchronously transforms its error using the provided mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="source">The source Result instance.</param>
        /// <param name="mapError">An asynchronous function that transforms the error.</param>
        /// <returns>A task that resolves to a new Result with the same success value if successful, or with the transformed error if a failure.</returns>
        public static async Task<Result<T, Error>> MapError<T>(this Result<T, Error> source, Func<Error, Task<Error>> mapError)
        {
            if (mapError == null)
            {
                throw new ArgumentNullException(nameof(mapError));
            }

            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(await mapError(source.Error).ConfigureAwait(false));
            }
            return Result<T, Error>.Success(source.Value);
        }

        /// <summary>
        /// If the awaitable Result is a failure, asynchronously transforms its error using the provided mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the value in the success case.</typeparam>
        /// <param name="awaitableSource">A task that resolves to a Result instance.</param>
        /// <param name="mapError">An asynchronous function that transforms the error.</param>
        /// <returns>A task that resolves to a new Result with the same success value if successful, or with the transformed error if a failure.</returns>
        public static async Task<Result<T, Error>> MapError<T>(this Task<Result<T, Error>> awaitableSource, Func<Error, Task<Error>> mapError)
        {
            if (mapError == null)
            {
                throw new ArgumentNullException(nameof(mapError));
            }

            var source = await awaitableSource.ConfigureAwait(false);
            if (source.IsFailure)
            {
                return Result<T, Error>.Failure(await mapError(source.Error).ConfigureAwait(false));
            }
            return Result<T, Error>.Success(source.Value);
        }
    }
}
