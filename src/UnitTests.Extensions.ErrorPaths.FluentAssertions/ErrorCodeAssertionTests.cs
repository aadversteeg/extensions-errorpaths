using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.FluentAssertions;
using FluentAssertions;
using System;
using Xunit;
using Xunit.Sdk;

namespace UnitTests.Extensions.ErrorPaths.FluentAssertions
{
    public class ErrorCodeAssertionTests
    {
        [Fact(DisplayName = "ECA-001: HaveValue should pass for matching value")]
        public void ECA001()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act & assert
            code.Should().HaveValue("Validation.Required");
        }

        [Fact(DisplayName = "ECA-002: HaveValue should fail for non-matching value")]
        public void ECA002()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            Action act = () => code.Should().HaveValue("NotFound.Entity");

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*value \"NotFound.Entity\"*found \"Validation.Required\"*");
        }

        [Fact(DisplayName = "ECA-003: BeChildOf should pass for child code")]
        public void ECA003()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var ancestor = new ErrorCode("Validation");

            // act & assert
            code.Should().BeChildOf(ancestor);
        }

        [Fact(DisplayName = "ECA-004: BeChildOf should fail for non-child code")]
        public void ECA004()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var notAncestor = new ErrorCode("NotFound");

            // act
            Action act = () => code.Should().BeChildOf(notAncestor);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*Validation.Required*child of*NotFound*");
        }

        [Fact(DisplayName = "ECA-005: NotBeChildOf should pass for non-child code")]
        public void ECA005()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var notAncestor = new ErrorCode("NotFound");

            // act & assert
            code.Should().NotBeChildOf(notAncestor);
        }

        [Fact(DisplayName = "ECA-006: NotBeChildOf should fail for child code")]
        public void ECA006()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var ancestor = new ErrorCode("Validation");

            // act
            Action act = () => code.Should().NotBeChildOf(ancestor);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*Validation.Required*not*child of*Validation*");
        }

        [Fact(DisplayName = "ECA-007: HaveDepth should pass for correct depth")]
        public void ECA007()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");

            // act & assert
            code.Should().HaveDepth(3);
        }

        [Fact(DisplayName = "ECA-008: HaveDepth should fail for incorrect depth")]
        public void ECA008()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            Action act = () => code.Should().HaveDepth(3);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*depth 3*found 2*");
        }

        [Fact(DisplayName = "ECA-009: HaveLeaf should pass for correct leaf")]
        public void ECA009()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act & assert
            code.Should().HaveLeaf("Required");
        }

        [Fact(DisplayName = "ECA-010: HaveLeaf should fail for incorrect leaf")]
        public void ECA010()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            Action act = () => code.Should().HaveLeaf("Format");

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*leaf \"Format\"*found \"Required\"*");
        }

        [Fact(DisplayName = "ECA-011: HaveParent should pass for correct parent")]
        public void ECA011()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var expectedParent = new ErrorCode("Validation");

            // act & assert
            code.Should().HaveParent(expectedParent);
        }

        [Fact(DisplayName = "ECA-012: HaveParent should fail for incorrect parent")]
        public void ECA012()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var wrongParent = new ErrorCode("NotFound");

            // act
            Action act = () => code.Should().HaveParent(wrongParent);

            // assert
            act.Should().Throw<XunitException>()
                .WithMessage("*parent \"NotFound\"*found \"Validation\"*");
        }
    }
}
