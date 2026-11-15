using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ave.Extensions.Functional;

namespace Ave.Extensions.ErrorPaths.Functional
{
    /// <summary>
    /// Provides extension methods that apply an <see cref="Error"/>-returning function to every item of a sequence and
    /// collect the outcome into a single Result, combining the errors with <see cref="Errors.Aggregate"/>.
    /// </summary>
    public static class ResultErrorTraverseExtensions
    {
        /// <summary>
        /// Applies a Result-returning function to every item of the sequence. Every item is visited, also after a
        /// failure, so the caller hears about all problems at once instead of only the first.
        /// </summary>
        /// <typeparam name="Tin">The type of the items in the source sequence.</typeparam>
        /// <typeparam name="Tout">The type of the value in each successful Result.</typeparam>
        /// <param name="source">The items to map.</param>
        /// <param name="map">A function that maps one item to a Result. It is called exactly once for every item, in sequence order.</param>
        /// <returns>A successful Result with all values in sequence order when every item succeeded (an empty list for an empty sequence);
        /// otherwise a failure Result with the errors collected by <see cref="Errors.Aggregate"/>: the error itself when one item failed,
        /// an aggregate holding every error in sequence order when several failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="map"/> is null.</exception>
        public static Result<IReadOnlyList<Tout>, Error> TraverseAll<Tin, Tout>(this IEnumerable<Tin> source, Func<Tin, Result<Tout, Error>> map)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            var values = new List<Tout>();
            var errors = new List<Error>();
            foreach (var item in source)
            {
                Collect(map(item), values, errors);
            }
            return Complete(values, errors);
        }

        /// <summary>
        /// Applies an asynchronous Result-returning function to every item of the sequence. Every item is visited, also after a
        /// failure, so the caller hears about all problems at once instead of only the first. The items are mapped one after
        /// the other: the function for an item is not started before the task for the previous item has completed.
        /// </summary>
        /// <typeparam name="Tin">The type of the items in the source sequence.</typeparam>
        /// <typeparam name="Tout">The type of the value in each successful Result.</typeparam>
        /// <param name="source">The items to map.</param>
        /// <param name="awaitableMap">An asynchronous function that maps one item to a Result. It is called exactly once for every item, in sequence order.</param>
        /// <returns>A task that resolves to a successful Result with all values in sequence order when every item succeeded
        /// (an empty list for an empty sequence); otherwise to a failure Result with the errors collected by <see cref="Errors.Aggregate"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="awaitableMap"/> is null.</exception>
        public static Task<Result<IReadOnlyList<Tout>, Error>> TraverseAll<Tin, Tout>(this IEnumerable<Tin> source, Func<Tin, Task<Result<Tout, Error>>> awaitableMap)
        {
            // Validate eagerly, so that a null argument throws at the call and not when the task is awaited.
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (awaitableMap == null)
            {
                throw new ArgumentNullException(nameof(awaitableMap));
            }

            return TraverseAllCore(source, awaitableMap);
        }

        private static async Task<Result<IReadOnlyList<Tout>, Error>> TraverseAllCore<Tin, Tout>(IEnumerable<Tin> source, Func<Tin, Task<Result<Tout, Error>>> awaitableMap)
        {
            var values = new List<Tout>();
            var errors = new List<Error>();
            foreach (var item in source)
            {
                Collect(await awaitableMap(item).ConfigureAwait(false), values, errors);
            }
            return Complete(values, errors);
        }

        private static void Collect<Tout>(Result<Tout, Error> result, List<Tout> values, List<Error> errors)
        {
            if (result.IsSuccess)
            {
                values.Add(result.Value);
            }
            else
            {
                errors.Add(result.Error);
            }
        }

        private static Result<IReadOnlyList<Tout>, Error> Complete<Tout>(List<Tout> values, List<Error> errors)
        {
            if (errors.Count > 0)
            {
                return Result<IReadOnlyList<Tout>, Error>.Failure(Errors.Aggregate(errors.AsReadOnly()));
            }
            return Result<IReadOnlyList<Tout>, Error>.Success(values.AsReadOnly());
        }
    }
}
