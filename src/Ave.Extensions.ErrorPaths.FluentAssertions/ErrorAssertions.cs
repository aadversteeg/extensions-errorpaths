using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Ave.Extensions.ErrorPaths.FluentAssertions
{
    public class ErrorAssertions : ReferenceTypeAssertions<Error, ErrorAssertions>
    {
        public ErrorAssertions(Error instance)
            : base(instance)
        {
        }

        protected override string Identifier => "Error";

        public AndConstraint<ErrorAssertions> HaveCode(ErrorCode expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Code.Equals(expected))
                .FailWith("Expected {context:Error} to have code {0}{reason}, but found {1}.",
                    expected.Value, Subject.Code.Value);

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> HaveMessage(string expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Message == expected)
                .FailWith("Expected {context:Error} to have message {0}{reason}, but found {1}.",
                    expected, Subject.Message);

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> MatchCode(ErrorCode ancestor, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Is(ancestor))
                .FailWith("Expected {context:Error} with code {0} to match ancestor code {1}{reason}.",
                    Subject.Code.Value, ancestor.Value);

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> NotMatchCode(ErrorCode code, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!Subject.Is(code))
                .FailWith("Expected {context:Error} with code {0} not to match code {1}{reason}.",
                    Subject.Code.Value, code.Value);

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> HaveMetadata(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Metadata != null)
                .FailWith("Expected {context:Error} to have metadata{reason}, but metadata was null.");

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> HaveNoMetadata(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Metadata == null)
                .FailWith("Expected {context:Error} to have no metadata{reason}, but metadata was present.");

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndConstraint<ErrorAssertions> HaveMetadataEntry(string key, object value, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Metadata != null)
                .FailWith("Expected {context:Error} to have metadata entry {0}={1}{reason}, but metadata was null.",
                    key, value)
                .Then
                .ForCondition(Subject.Metadata != null && Subject.Metadata.ContainsKey(key))
                .FailWith("Expected {context:Error} metadata to contain key {0}{reason}, but it was not found.",
                    key)
                .Then
                .ForCondition(Subject.Metadata != null && Subject.Metadata.ContainsKey(key) && Equals(Subject.Metadata[key], value))
                .FailWith("Expected {context:Error} metadata key {0} to have value {1}{reason}, but found {2}.",
                    key, value, Subject.Metadata != null && Subject.Metadata.ContainsKey(key) ? Subject.Metadata[key] : null);

            return new AndConstraint<ErrorAssertions>(this);
        }

        public AndWhichConstraint<ErrorAssertions, Error> HaveInnerError(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Inner.HasValue)
                .FailWith("Expected {context:Error} to have an inner error{reason}, but it did not.");

            return new AndWhichConstraint<ErrorAssertions, Error>(this, Subject.Inner!.Value);
        }

        public AndConstraint<ErrorAssertions> HaveNoInnerError(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!Subject.Inner.HasValue)
                .FailWith("Expected {context:Error} to have no inner error{reason}, but found inner error [{0}] {1}.",
                    Subject.Inner.HasValue ? Subject.Inner.Value.Code.Value : "", Subject.Inner.HasValue ? Subject.Inner.Value.Message : "");

            return new AndConstraint<ErrorAssertions>(this);
        }

        /// <summary>
        /// Asserts that the error is an aggregate created by <see cref="Errors.Aggregate"/>: it has code
        /// <see cref="ErrorCodes.Aggregate"/> and more than one part. Use <c>Which</c> to continue on the parts.
        /// </summary>
        /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
        /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because"/>.</param>
        /// <returns>A constraint whose <c>Which</c> is the list of parts.</returns>
        public AndWhichConstraint<ErrorAssertions, IReadOnlyList<Error>> BeAggregate(string because = "", params object[] becauseArgs)
        {
            var parts = Subject.Parts();
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Code.Equals(ErrorCodes.Aggregate) && parts.Count > 1)
                .FailWith("Expected {context:Error} to be an aggregate of several errors{reason}, but found [{0}] {1}.",
                    Subject.Code.Value, Subject.Message);

            return new AndWhichConstraint<ErrorAssertions, IReadOnlyList<Error>>(this, parts);
        }

        /// <summary>
        /// Asserts that one of the parts of the error has exactly the given code. For an error that is not an aggregate,
        /// the only part is the error itself. Use <c>Which</c> to continue on the first matching part.
        /// </summary>
        /// <param name="expected">The code of the part to find.</param>
        /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
        /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because"/>.</param>
        /// <returns>A constraint whose <c>Which</c> is the first part with the code.</returns>
        public AndWhichConstraint<ErrorAssertions, Error> HavePartWithCode(ErrorCode expected, string because = "", params object[] becauseArgs)
        {
            var parts = Subject.Parts();
            var match = parts.Where(part => part.Code.Equals(expected)).Select(part => (Error?)part).FirstOrDefault();
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(match.HasValue)
                .FailWith("Expected {context:Error} to have a part with code {0}{reason}, but found parts with codes {1}.",
                    expected.Value, parts.Select(part => part.Code.Value).ToList());

            return new AndWhichConstraint<ErrorAssertions, Error>(this, match.GetValueOrDefault());
        }
    }
}
