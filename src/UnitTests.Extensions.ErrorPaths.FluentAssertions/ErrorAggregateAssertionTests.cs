using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.FluentAssertions;
using FluentAssertions;
using System;
using Xunit;
using Xunit.Sdk;

namespace UnitTests.Extensions.ErrorPaths.FluentAssertions
{
    public class ErrorAggregateAssertionTests
    {
        private static readonly Error Aggregate =
            Errors.Aggregate(new[] { Errors.Required("name"), Errors.NotFound("User", 42) });

        [Fact(DisplayName = "EAA-001: BeAggregate should pass for an aggregate and expose the parts")]
        public void EAA001()
        {
            // act & assert
            Aggregate.Should().BeAggregate()
                .Which.Should().HaveCount(2);
        }

        [Fact(DisplayName = "EAA-002: BeAggregate should fail for a plain error")]
        public void EAA002()
        {
            // arrange
            var error = Errors.Required("name");

            // act
            Action act = () => error.Should().BeAggregate();

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*aggregate*Validation.Required*");
        }

        [Fact(DisplayName = "EAA-003: BeAggregate should fail for a hand-made error with the Aggregate code")]
        public void EAA003()
        {
            // arrange
            var error = new Error(ErrorCodes.Aggregate, "Hand-made.");

            // act
            Action act = () => error.Should().BeAggregate();

            // assert
            act.Should().Throw<XunitException>();
        }

        [Fact(DisplayName = "EAA-004: HavePartWithCode should pass and continue on the matching part")]
        public void EAA004()
        {
            // act & assert
            Aggregate.Should().HavePartWithCode(ErrorCodes.NotFound.Entity)
                .Which.Should().HaveMetadataEntry("entity", "User");
        }

        [Fact(DisplayName = "EAA-005: HavePartWithCode should fail and list the part codes when no part has the code")]
        public void EAA005()
        {
            // act
            Action act = () => Aggregate.Should().HavePartWithCode(ErrorCodes.IO.Timeout);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*IO.Timeout*Validation.Required*NotFound.Entity*");
        }

        [Fact(DisplayName = "EAA-006: HavePartWithCode should match exactly, not by ancestor")]
        public void EAA006()
        {
            // act
            Action act = () => Aggregate.Should().HavePartWithCode(ErrorCodes.Validation._);

            // assert
            act.Should().Throw<XunitException>();
        }

        [Fact(DisplayName = "EAA-007: HavePartWithCode on a plain error should look at the error itself")]
        public void EAA007()
        {
            // arrange
            var error = Errors.Required("name");

            // act & assert
            error.Should().HavePartWithCode(ErrorCodes.Validation.Required);
        }

        [Fact(DisplayName = "EAA-008: HaveCode and HaveMessage should work on an aggregate")]
        public void EAA008()
        {
            // act & assert
            Aggregate.Should().HaveCode(ErrorCodes.Aggregate)
                .And.HaveMessage("2 errors: The field 'name' is required.; User with id '42' was not found.");
        }
    }
}
