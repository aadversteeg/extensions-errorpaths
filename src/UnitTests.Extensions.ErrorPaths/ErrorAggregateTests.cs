using Ave.Extensions.ErrorPaths;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths
{
    public class ErrorAggregateTests
    {
        private static readonly Error First = Errors.Required("name");
        private static readonly Error Second = Errors.NotFound("User", 42);
        private static readonly Error Third = Errors.Timeout("lookup");

        [Fact(DisplayName = "AGG-001: Aggregate code should be the root code Aggregate")]
        public void AGG001()
        {
            // assert
            ErrorCodes.Aggregate.Value.Should().Be("Aggregate");
            ErrorCodes.Aggregate.Depth.Should().Be(1);
        }

        [Fact(DisplayName = "AGG-002: Aggregate of an empty list should throw ArgumentException")]
        public void AGG002()
        {
            // act
            Action act = () => Errors.Aggregate(Array.Empty<Error>());

            // assert
            act.Should().Throw<ArgumentException>().WithParameterName("errors");
        }

        [Fact(DisplayName = "AGG-003: Aggregate of null should throw ArgumentNullException")]
        public void AGG003()
        {
            // act
            Action act = () => Errors.Aggregate(null!);

            // assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("errors");
        }

        [Fact(DisplayName = "AGG-004: Aggregate of a single error should return that error unchanged")]
        public void AGG004()
        {
            // act
            var error = Errors.Aggregate(new[] { First });

            // assert
            error.Should().Be(First);
            error.Code.Should().Be(ErrorCodes.Validation.Required);
            error.Metadata.Should().BeSameAs(First.Metadata);
            error.Parts().Should().Equal(First);
        }

        [Fact(DisplayName = "AGG-005: Aggregate of several errors should have the Aggregate code and keep the parts in order")]
        public void AGG005()
        {
            // act
            var error = Errors.Aggregate(new[] { First, Second, Third });

            // assert
            error.Code.Should().Be(ErrorCodes.Aggregate);
            error.Parts().Should().Equal(First, Second, Third);
        }

        [Fact(DisplayName = "AGG-006: Aggregate message should count and list the part messages in order")]
        public void AGG006()
        {
            // arrange
            var a = new Error(ErrorCodes.Validation._, "a is wrong");
            var b = new Error(ErrorCodes.Validation._, "b is wrong");

            // act
            var error = Errors.Aggregate(new[] { a, b });

            // assert
            error.Message.Should().Be("2 errors: a is wrong; b is wrong");
        }

        [Fact(DisplayName = "AGG-007: Aggregate should flatten nested aggregates in order")]
        public void AGG007()
        {
            // arrange
            var inner = Errors.Aggregate(new[] { Second, Third });
            var a = new Error(ErrorCodes.Validation._, "a");

            // act
            var error = Errors.Aggregate(new[] { First, inner, a });

            // assert
            error.Parts().Should().Equal(First, Second, Third, a);
            error.Parts().Should().NotContain(part => part.Code == ErrorCodes.Aggregate);
            error.Message.Should().StartWith("4 errors: ");
        }

        [Fact(DisplayName = "AGG-008: Aggregate of two aggregates should flatten both")]
        public void AGG008()
        {
            // arrange
            var left = Errors.Aggregate(new[] { First, Second });
            var right = Errors.Aggregate(new[] { Third, First });

            // act
            var error = Errors.Aggregate(new[] { left, right });

            // assert
            error.Parts().Should().Equal(First, Second, Third, First);
        }

        [Fact(DisplayName = "AGG-009: Aggregate of a single aggregate should return it unchanged")]
        public void AGG009()
        {
            // arrange
            var inner = Errors.Aggregate(new[] { First, Second });

            // act
            var error = Errors.Aggregate(new[] { inner });

            // assert
            error.Should().Be(inner);
            error.Parts().Should().Equal(First, Second);
        }

        [Fact(DisplayName = "AGG-010: Parts of a plain error should yield the error itself")]
        public void AGG010()
        {
            // act
            var parts = First.Parts();

            // assert
            parts.Should().Equal(First);
        }

        [Fact(DisplayName = "AGG-011: Parts of a hand-made error with the Aggregate code should yield the error itself")]
        public void AGG011()
        {
            // arrange
            var error = new Error(ErrorCodes.Aggregate, "Not built by Errors.Aggregate.");

            // act
            var parts = error.Parts();

            // assert
            parts.Should().Equal(error);
        }

        [Fact(DisplayName = "AGG-012: With on an aggregate should keep the parts")]
        public void AGG012()
        {
            // arrange
            var error = Errors.Aggregate(new[] { First, Second });

            // act
            var withMetadata = error.With("document", "invoice-1");

            // assert
            withMetadata.Parts().Should().Equal(First, Second);
            withMetadata.Metadata!["document"].Should().Be("invoice-1");
        }

        [Fact(DisplayName = "AGG-013: Aggregate should not change when the input list changes afterwards")]
        public void AGG013()
        {
            // arrange
            var input = new List<Error> { First, Second };
            var error = Errors.Aggregate(input);

            // act
            input.Add(Third);

            // assert
            error.Parts().Should().Equal(First, Second);
        }

        [Fact(DisplayName = "AGG-014: Parts of an aggregate cannot be changed through a cast to an array")]
        public void AGG014()
        {
            // act
            var parts = Errors.Aggregate(new[] { First, Second }).Parts();

            // assert
            parts.Should().NotBeAssignableTo<Error[]>();
            parts.Should().NotBeAssignableTo<List<Error>>();
        }

        [Fact(DisplayName = "AGG-015: Wrapping an aggregate should keep the aggregate as inner error with its parts")]
        public void AGG015()
        {
            // arrange
            var error = Errors.Aggregate(new[] { First, Second });

            // act
            var wrapped = error.Wrap(ErrorCodes.Internal.Unexpected, "Import failed.");

            // assert
            wrapped.Parts().Should().Equal(wrapped);
            wrapped.Inner!.Value.Parts().Should().Equal(First, Second);
        }

        [Fact(DisplayName = "AGG-016: Two aggregates of the same parts should be equal")]
        public void AGG016()
        {
            // act
            var a = Errors.Aggregate(new[] { First, Second });
            var b = Errors.Aggregate(new[] { First, Second });

            // assert
            a.Should().Be(b);
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact(DisplayName = "AGG-017: ToString of an aggregate should show the Aggregate code and the listed messages")]
        public void AGG017()
        {
            // arrange
            var error = Errors.Aggregate(new[] { new Error(ErrorCodes.Validation._, "a"), new Error(ErrorCodes.Validation._, "b") });

            // act
            var text = error.ToString();

            // assert
            text.Should().Be("[Aggregate] 2 errors: a; b");
        }
    }
}
