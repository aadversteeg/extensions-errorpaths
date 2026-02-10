using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.FluentAssertions;
using FluentAssertions;
using System;
using Xunit;
using Xunit.Sdk;

namespace UnitTests.Extensions.ErrorPaths.FluentAssertions
{
    public class ErrorAssertionTests
    {
        [Fact(DisplayName = "EA-001: HaveCode should pass for matching code")]
        public void EA001()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().HaveCode(ErrorCodes.Validation.Required);
        }

        [Fact(DisplayName = "EA-002: HaveCode should fail for non-matching code")]
        public void EA002()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().HaveCode(ErrorCodes.NotFound.Entity);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*code \"NotFound.Entity\"*found \"Validation.Required\"*");
        }

        [Fact(DisplayName = "EA-003: HaveMessage should pass for matching message")]
        public void EA003()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().HaveMessage("Field is required.");
        }

        [Fact(DisplayName = "EA-004: HaveMessage should fail for non-matching message")]
        public void EA004()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().HaveMessage("Wrong message.");

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*message \"Wrong message.\"*found \"Field is required.\"*");
        }

        [Fact(DisplayName = "EA-005: MatchCode should pass for ancestor match")]
        public void EA005()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().MatchCode(ErrorCodes.Validation._);
        }

        [Fact(DisplayName = "EA-006: MatchCode should fail for non-ancestor")]
        public void EA006()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().MatchCode(ErrorCodes.NotFound._);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*Validation.Required*match ancestor code*NotFound*");
        }

        [Fact(DisplayName = "EA-007: NotMatchCode should pass for non-ancestor")]
        public void EA007()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().NotMatchCode(ErrorCodes.NotFound._);
        }

        [Fact(DisplayName = "EA-008: NotMatchCode should fail for ancestor match")]
        public void EA008()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().NotMatchCode(ErrorCodes.Validation._);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*Validation.Required*not*match*Validation*");
        }

        [Fact(DisplayName = "EA-009: HaveMetadata should pass when metadata exists")]
        public void EA009()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.")
                .With("field", "email");

            // act & assert
            error.Should().HaveMetadata();
        }

        [Fact(DisplayName = "EA-010: HaveMetadata should fail when metadata is null")]
        public void EA010()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().HaveMetadata();

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*metadata*null*");
        }

        [Fact(DisplayName = "EA-011: HaveNoMetadata should pass when metadata is null")]
        public void EA011()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().HaveNoMetadata();
        }

        [Fact(DisplayName = "EA-012: HaveNoMetadata should fail when metadata exists")]
        public void EA012()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.")
                .With("field", "email");

            // act
            Action act = () => error.Should().HaveNoMetadata();

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*no metadata*present*");
        }

        [Fact(DisplayName = "EA-013: HaveMetadataEntry should pass for matching key-value")]
        public void EA013()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.")
                .With("field", "email");

            // act & assert
            error.Should().HaveMetadataEntry("field", "email");
        }

        [Fact(DisplayName = "EA-014: HaveMetadataEntry should fail for missing key")]
        public void EA014()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.")
                .With("field", "email");

            // act
            Action act = () => error.Should().HaveMetadataEntry("missing", "value");

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*key \"missing\"*not found*");
        }

        [Fact(DisplayName = "EA-015: HaveInnerError should pass when inner exists")]
        public void EA015()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var outer = inner.Wrap(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // act & assert
            outer.Should().HaveInnerError();
        }

        [Fact(DisplayName = "EA-016: HaveInnerError should fail when inner is null")]
        public void EA016()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            Action act = () => error.Should().HaveInnerError();

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*inner error*did not*");
        }

        [Fact(DisplayName = "EA-017: HaveNoInnerError should pass when inner is null")]
        public void EA017()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act & assert
            error.Should().HaveNoInnerError();
        }

        [Fact(DisplayName = "EA-018: HaveNoInnerError should fail when inner exists")]
        public void EA018()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var outer = inner.Wrap(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // act
            Action act = () => outer.Should().HaveNoInnerError();

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*no inner error*IO.Network*");
        }

        [Fact(DisplayName = "EA-019: HaveInnerError should allow chaining to inner error assertions")]
        public void EA019()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var outer = inner.Wrap(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // act & assert
            outer.Should().HaveInnerError()
                .Which.Should().HaveCode(ErrorCodes.IO.Network)
                .And.HaveMessage("Connection failed.");
        }
    }
}
