using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Ave.Extensions.ErrorPaths.FluentAssertions
{
    public class ErrorCodeAssertions : ReferenceTypeAssertions<ErrorCode, ErrorCodeAssertions>
    {
        public ErrorCodeAssertions(ErrorCode instance)
            : base(instance)
        {
        }

        protected override string Identifier => "ErrorCode";

        public AndConstraint<ErrorCodeAssertions> HaveValue(string expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Value == expected)
                .FailWith("Expected {context:ErrorCode} to have value {0}{reason}, but found {1}.",
                    expected, Subject.Value);

            return new AndConstraint<ErrorCodeAssertions>(this);
        }

        public AndConstraint<ErrorCodeAssertions> BeChildOf(ErrorCode ancestor, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.IsChildOf(ancestor))
                .FailWith("Expected {context:ErrorCode} {0} to be a child of {1}{reason}.",
                    Subject.Value, ancestor.Value);

            return new AndConstraint<ErrorCodeAssertions>(this);
        }

        public AndConstraint<ErrorCodeAssertions> NotBeChildOf(ErrorCode code, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!Subject.IsChildOf(code))
                .FailWith("Expected {context:ErrorCode} {0} not to be a child of {1}{reason}.",
                    Subject.Value, code.Value);

            return new AndConstraint<ErrorCodeAssertions>(this);
        }

        public AndConstraint<ErrorCodeAssertions> HaveDepth(int expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Depth == expected)
                .FailWith("Expected {context:ErrorCode} {0} to have depth {1}{reason}, but found {2}.",
                    Subject.Value, expected, Subject.Depth);

            return new AndConstraint<ErrorCodeAssertions>(this);
        }

        public AndConstraint<ErrorCodeAssertions> HaveLeaf(string expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Leaf == expected)
                .FailWith("Expected {context:ErrorCode} {0} to have leaf {1}{reason}, but found {2}.",
                    Subject.Value, expected, Subject.Leaf);

            return new AndConstraint<ErrorCodeAssertions>(this);
        }

        public AndConstraint<ErrorCodeAssertions> HaveParent(ErrorCode expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Parent.HasValue && Subject.Parent.Value.Equals(expected))
                .FailWith("Expected {context:ErrorCode} {0} to have parent {1}{reason}, but found {2}.",
                    Subject.Value, expected.Value, Subject.Parent.HasValue ? Subject.Parent.Value.Value : "<null>");

            return new AndConstraint<ErrorCodeAssertions>(this);
        }
    }
}
