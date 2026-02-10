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
    }
}
