using Ave.Extensions.ErrorPaths;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths
{
    public class ErrorTests
    {
        [Fact(DisplayName = "ERR-001: Constructor with code and message should create Error")]
        public void ERR001()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var message = "The field is required.";

            // act
            var error = new Error(code, message);

            // assert
            error.Code.Should().Be(code);
            error.Message.Should().Be(message);
            error.Metadata.Should().BeNull();
            error.Inner.Should().BeNull();
        }

        [Fact(DisplayName = "ERR-002: Constructor with null message should throw ArgumentNullException")]
        public void ERR002()
        {
            // arrange
            var code = new ErrorCode("Validation");

            // act
            var act = () => new Error(code, null!);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("message");
        }

        [Fact(DisplayName = "ERR-003: Constructor with metadata should create Error with metadata")]
        public void ERR003()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var message = "The field is required.";
            var metadata = new Dictionary<string, object> { { "field", "email" } };

            // act
            var error = new Error(code, message, metadata);

            // assert
            error.Metadata.Should().NotBeNull();
            error.Metadata!["field"].Should().Be("email");
        }

        [Fact(DisplayName = "ERR-004: Constructor with inner error should create Error with inner")]
        public void ERR004()
        {
            // arrange
            var innerCode = new ErrorCode("IO.Network");
            var innerError = new Error(innerCode, "Connection failed.");
            var outerCode = new ErrorCode("Internal.Unexpected");

            // act
            var error = new Error(outerCode, "An unexpected error occurred.", null, innerError);

            // assert
            error.Inner.Should().NotBeNull();
            error.Inner!.Value.Code.Should().Be(innerCode);
            error.Inner!.Value.Message.Should().Be("Connection failed.");
        }

        [Fact(DisplayName = "ERR-005: Is should return true when code matches ancestor")]
        public void ERR005()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");
            var error = new Error(code, "Email is required.");

            // act
            var result = error.Is(new ErrorCode("Validation"));

            // assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "ERR-006: Is should return true for exact code match")]
        public void ERR006()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var error = new Error(code, "Field is required.");

            // act
            var result = error.Is(new ErrorCode("Validation.Required"));

            // assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "ERR-007: Is should return false when code does not match")]
        public void ERR007()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var error = new Error(code, "Field is required.");

            // act
            var result = error.Is(new ErrorCode("NotFound"));

            // assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "ERR-008: Wrap should create new Error with this as inner")]
        public void ERR008()
        {
            // arrange
            var innerError = new Error(new ErrorCode("IO.Network"), "Connection failed.");
            var wrapperCode = new ErrorCode("Internal.Unexpected");

            // act
            var wrapped = innerError.Wrap(wrapperCode, "An error occurred during processing.");

            // assert
            wrapped.Code.Should().Be(wrapperCode);
            wrapped.Message.Should().Be("An error occurred during processing.");
            wrapped.Inner.Should().NotBeNull();
            wrapped.Inner!.Value.Should().Be(innerError);
        }

        [Fact(DisplayName = "ERR-009: With should add metadata entry")]
        public void ERR009()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation.Required"), "Field is required.");

            // act
            var withMetadata = error.With("field", "email");

            // assert
            withMetadata.Metadata.Should().NotBeNull();
            withMetadata.Metadata!["field"].Should().Be("email");
            withMetadata.Code.Should().Be(error.Code);
            withMetadata.Message.Should().Be(error.Message);
        }

        [Fact(DisplayName = "ERR-010: With should preserve existing metadata")]
        public void ERR010()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation"), "Invalid input.")
                .With("field1", "value1");

            // act
            var extended = error.With("field2", "value2");

            // assert
            extended.Metadata.Should().NotBeNull();
            extended.Metadata!["field1"].Should().Be("value1");
            extended.Metadata!["field2"].Should().Be("value2");
        }

        [Fact(DisplayName = "ERR-011: With should overwrite existing metadata key")]
        public void ERR011()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation"), "Invalid input.")
                .With("field", "oldValue");

            // act
            var updated = error.With("field", "newValue");

            // assert
            updated.Metadata.Should().NotBeNull();
            updated.Metadata!["field"].Should().Be("newValue");
        }

        [Fact(DisplayName = "ERR-012: With with null key should throw ArgumentNullException")]
        public void ERR012()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation"), "Invalid input.");

            // act
            var act = () => error.With(null!, "value");

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("key");
        }

        [Fact(DisplayName = "ERR-013: ToString should format code and message")]
        public void ERR013()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation.Required"), "Field is required.");

            // act
            var result = error.ToString();

            // assert
            result.Should().Be("[Validation.Required] Field is required.");
        }

        [Fact(DisplayName = "ERR-014: ToString should include inner error chain")]
        public void ERR014()
        {
            // arrange
            var inner = new Error(new ErrorCode("IO.Network"), "Connection failed.");
            var outer = inner.Wrap(new ErrorCode("Internal"), "Operation failed.");

            // act
            var result = outer.ToString();

            // assert
            result.Should().Be("[Internal] Operation failed. ---> [IO.Network] Connection failed.");
        }

        [Fact(DisplayName = "ERR-015: Equal Errors should be equal")]
        public void ERR015()
        {
            // arrange
            var error1 = new Error(new ErrorCode("Validation"), "Invalid input.");
            var error2 = new Error(new ErrorCode("Validation"), "Invalid input.");

            // act & assert
            error1.Should().Be(error2);
            (error1 == error2).Should().BeTrue();
            (error1 != error2).Should().BeFalse();
            error1.Equals(error2).Should().BeTrue();
            error1.Equals((object)error2).Should().BeTrue();
        }

        [Fact(DisplayName = "ERR-016: Errors with different codes should not be equal")]
        public void ERR016()
        {
            // arrange
            var error1 = new Error(new ErrorCode("Validation"), "Invalid input.");
            var error2 = new Error(new ErrorCode("NotFound"), "Invalid input.");

            // act & assert
            error1.Should().NotBe(error2);
            (error1 == error2).Should().BeFalse();
            (error1 != error2).Should().BeTrue();
        }

        [Fact(DisplayName = "ERR-017: Errors with different messages should not be equal")]
        public void ERR017()
        {
            // arrange
            var error1 = new Error(new ErrorCode("Validation"), "Message 1");
            var error2 = new Error(new ErrorCode("Validation"), "Message 2");

            // act & assert
            error1.Should().NotBe(error2);
        }

        [Fact(DisplayName = "ERR-018: Equal Errors should have same hash code")]
        public void ERR018()
        {
            // arrange
            var error1 = new Error(new ErrorCode("Validation"), "Invalid input.");
            var error2 = new Error(new ErrorCode("Validation"), "Invalid input.");

            // act & assert
            error1.GetHashCode().Should().Be(error2.GetHashCode());
        }

        [Fact(DisplayName = "ERR-019: Default Error should have empty code and message")]
        public void ERR019()
        {
            // arrange
            var error = default(Error);

            // act & assert
            error.Code.Value.Should().Be(string.Empty);
            error.Message.Should().Be(string.Empty);
            error.Metadata.Should().BeNull();
            error.Inner.Should().BeNull();
        }

        [Fact(DisplayName = "ERR-020: With should preserve inner error")]
        public void ERR020()
        {
            // arrange
            var inner = new Error(new ErrorCode("IO"), "IO error.");
            var error = new Error(new ErrorCode("App"), "App error.", null, inner);

            // act
            var withMeta = error.With("key", "value");

            // assert
            withMeta.Inner.Should().NotBeNull();
            withMeta.Inner!.Value.Code.Value.Should().Be("IO");
        }
    }
}
