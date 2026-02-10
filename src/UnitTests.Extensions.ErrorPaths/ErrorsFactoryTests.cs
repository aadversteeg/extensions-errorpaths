using Ave.Extensions.ErrorPaths;
using FluentAssertions;
using System;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths
{
    public class ErrorsFactoryTests
    {
        [Fact(DisplayName = "EF-001: Validation should create error with Validation code")]
        public void EF001()
        {
            // act
            var error = Errors.Validation("Invalid input.");

            // assert
            error.Code.Should().Be(ErrorCodes.Validation._);
            error.Message.Should().Be("Invalid input.");
        }

        [Fact(DisplayName = "EF-002: Required should create error with Required code and field metadata")]
        public void EF002()
        {
            // act
            var error = Errors.Required("email");

            // assert
            error.Code.Should().Be(ErrorCodes.Validation.Required);
            error.Message.Should().Contain("email");
            error.Message.Should().Contain("required");
            error.Metadata.Should().NotBeNull();
            error.Metadata!["field"].Should().Be("email");
        }

        [Fact(DisplayName = "EF-003: Format should create error with Format code and metadata")]
        public void EF003()
        {
            // act
            var error = Errors.Format("email", "valid email address");

            // assert
            error.Code.Should().Be(ErrorCodes.Validation.Format);
            error.Message.Should().Contain("email");
            error.Message.Should().Contain("valid email address");
            error.Metadata.Should().NotBeNull();
            error.Metadata!["field"].Should().Be("email");
            error.Metadata!["expected"].Should().Be("valid email address");
        }

        [Fact(DisplayName = "EF-004: NotFound with message should create error with NotFound code")]
        public void EF004()
        {
            // act
            var error = Errors.NotFound("Resource not found.");

            // assert
            error.Code.Should().Be(ErrorCodes.NotFound._);
            error.Message.Should().Be("Resource not found.");
        }

        [Fact(DisplayName = "EF-005: NotFound with entity and id should create error with Entity code")]
        public void EF005()
        {
            // act
            var error = Errors.NotFound("User", 42);

            // assert
            error.Code.Should().Be(ErrorCodes.NotFound.Entity);
            error.Message.Should().Contain("User");
            error.Message.Should().Contain("42");
            error.Metadata.Should().NotBeNull();
            error.Metadata!["entity"].Should().Be("User");
            error.Metadata!["id"].Should().Be(42);
        }

        [Fact(DisplayName = "EF-006: NotFound with string id should work correctly")]
        public void EF006()
        {
            // act
            var error = Errors.NotFound("Document", "abc-123");

            // assert
            error.Metadata!["id"].Should().Be("abc-123");
        }

        [Fact(DisplayName = "EF-007: Unauthorized without reason should use default message")]
        public void EF007()
        {
            // act
            var error = Errors.Unauthorized();

            // assert
            error.Code.Should().Be(ErrorCodes.Auth.Unauthorized);
            error.Message.Should().Be("Authentication is required.");
        }

        [Fact(DisplayName = "EF-008: Unauthorized with reason should use provided message")]
        public void EF008()
        {
            // act
            var error = Errors.Unauthorized("Token expired.");

            // assert
            error.Code.Should().Be(ErrorCodes.Auth.Unauthorized);
            error.Message.Should().Be("Token expired.");
        }

        [Fact(DisplayName = "EF-009: Forbidden without reason should use default message")]
        public void EF009()
        {
            // act
            var error = Errors.Forbidden();

            // assert
            error.Code.Should().Be(ErrorCodes.Auth.Forbidden);
            error.Message.Should().Be("Access to this resource is forbidden.");
        }

        [Fact(DisplayName = "EF-010: Forbidden with reason should use provided message")]
        public void EF010()
        {
            // act
            var error = Errors.Forbidden("Insufficient permissions.");

            // assert
            error.Code.Should().Be(ErrorCodes.Auth.Forbidden);
            error.Message.Should().Be("Insufficient permissions.");
        }

        [Fact(DisplayName = "EF-011: Timeout should create error with Timeout code and operation metadata")]
        public void EF011()
        {
            // act
            var error = Errors.Timeout("database query");

            // assert
            error.Code.Should().Be(ErrorCodes.IO.Timeout);
            error.Message.Should().Contain("database query");
            error.Message.Should().Contain("timed out");
            error.Metadata.Should().NotBeNull();
            error.Metadata!["operation"].Should().Be("database query");
        }

        [Fact(DisplayName = "EF-012: Network should create error with Network code")]
        public void EF012()
        {
            // act
            var error = Errors.Network("Connection refused.");

            // assert
            error.Code.Should().Be(ErrorCodes.IO.Network);
            error.Message.Should().Be("Connection refused.");
        }

        [Fact(DisplayName = "EF-013: Unexpected with message should create error with Unexpected code")]
        public void EF013()
        {
            // act
            var error = Errors.Unexpected("Something went wrong.");

            // assert
            error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            error.Message.Should().Be("Something went wrong.");
        }

        [Fact(DisplayName = "EF-014: Unexpected with exception should create error with exception info")]
        public void EF014()
        {
            // arrange
            var exception = new InvalidOperationException("Invalid state.");

            // act
            var error = Errors.Unexpected(exception);

            // assert
            error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            error.Message.Should().Be("Invalid state.");
            error.Metadata.Should().NotBeNull();
            error.Metadata!["exceptionType"].Should().Be("System.InvalidOperationException");
        }

        [Fact(DisplayName = "EF-015: Unexpected with null exception should throw ArgumentNullException")]
        public void EF015()
        {
            // act
            var act = () => Errors.Unexpected((Exception)null!);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("ex");
        }

        [Fact(DisplayName = "EF-016: From should create error with custom code and message")]
        public void EF016()
        {
            // arrange
            var customCode = new ErrorCode("Custom.Error");

            // act
            var error = Errors.From(customCode, "Custom error message.");

            // assert
            error.Code.Should().Be(customCode);
            error.Message.Should().Be("Custom error message.");
        }

        [Fact(DisplayName = "EF-017: All factory errors should be matchable by their category")]
        public void EF017()
        {
            // act & assert
            Errors.Validation("msg").Is(ErrorCodes.Validation._).Should().BeTrue();
            Errors.Required("field").Is(ErrorCodes.Validation._).Should().BeTrue();
            Errors.Format("field", "format").Is(ErrorCodes.Validation._).Should().BeTrue();
            Errors.NotFound("msg").Is(ErrorCodes.NotFound._).Should().BeTrue();
            Errors.NotFound("entity", 1).Is(ErrorCodes.NotFound._).Should().BeTrue();
            Errors.Unauthorized().Is(ErrorCodes.Auth._).Should().BeTrue();
            Errors.Forbidden().Is(ErrorCodes.Auth._).Should().BeTrue();
            Errors.Timeout("op").Is(ErrorCodes.IO._).Should().BeTrue();
            Errors.Network("msg").Is(ErrorCodes.IO._).Should().BeTrue();
            Errors.Unexpected("msg").Is(ErrorCodes.Internal._).Should().BeTrue();
        }
    }
}
