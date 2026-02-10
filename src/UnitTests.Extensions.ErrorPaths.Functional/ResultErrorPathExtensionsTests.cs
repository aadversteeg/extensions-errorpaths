using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Functional;
using Ave.Extensions.Functional;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Functional
{
    public class ResultErrorPathExtensionsTests
    {
        [Fact(DisplayName = "REP-001: HasError on failure with matching code should return true")]
        public void REP001()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Result<int, Error>.Failure(error);

            // act
            var hasError = result.HasError(ErrorCodes.Validation.Required);

            // assert
            hasError.Should().BeTrue();
        }

        [Fact(DisplayName = "REP-002: HasError on failure with ancestor code should return true")]
        public void REP002()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Result<int, Error>.Failure(error);

            // act
            var hasError = result.HasError(ErrorCodes.Validation._);

            // assert
            hasError.Should().BeTrue();
        }

        [Fact(DisplayName = "REP-003: HasError on failure with non-matching code should return false")]
        public void REP003()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Result<int, Error>.Failure(error);

            // act
            var hasError = result.HasError(ErrorCodes.NotFound._);

            // assert
            hasError.Should().BeFalse();
        }

        [Fact(DisplayName = "REP-004: HasError on success should return false")]
        public void REP004()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var hasError = result.HasError(ErrorCodes.Validation._);

            // assert
            hasError.Should().BeFalse();
        }

        [Fact(DisplayName = "REP-005: HasError with custom nested code should match ancestor")]
        public void REP005()
        {
            // arrange
            var customCode = ErrorCodes.Validation.Required / "Email";
            var error = new Error(customCode, "Email is required.");
            var result = Result<string, Error>.Failure(error);

            // act & assert
            result.HasError(customCode).Should().BeTrue();
            result.HasError(ErrorCodes.Validation.Required).Should().BeTrue();
            result.HasError(ErrorCodes.Validation._).Should().BeTrue();
            result.HasError(ErrorCodes.Auth._).Should().BeFalse();
        }

        [Fact(DisplayName = "REP-006: Async HasError on Task failure with matching code should return true")]
        public async Task REP006()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Task.FromResult(Result<int, Error>.Failure(error));

            // act
            var hasError = await result.HasError(ErrorCodes.Validation._);

            // assert
            hasError.Should().BeTrue();
        }

        [Fact(DisplayName = "REP-007: Async HasError on Task success should return false")]
        public async Task REP007()
        {
            // arrange
            var result = Task.FromResult(Result<int, Error>.Success(42));

            // act
            var hasError = await result.HasError(ErrorCodes.Validation._);

            // assert
            hasError.Should().BeFalse();
        }

        [Fact(DisplayName = "REP-008: WrapError on failure should wrap the error")]
        public void REP008()
        {
            // arrange
            var innerError = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var result = Result<int, Error>.Failure(innerError);

            // act
            var wrapped = result.WrapError(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // assert
            wrapped.IsFailure.Should().BeTrue();
            wrapped.Error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            wrapped.Error.Message.Should().Be("Operation failed.");
            wrapped.Error.Inner.Should().NotBeNull();
            wrapped.Error.Inner!.Value.Code.Should().Be(ErrorCodes.IO.Network);
        }

        [Fact(DisplayName = "REP-009: WrapError on success should preserve value")]
        public void REP009()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var wrapped = result.WrapError(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // assert
            wrapped.IsSuccess.Should().BeTrue();
            wrapped.Value.Should().Be(42);
        }

        [Fact(DisplayName = "REP-010: WrapError with null message should throw ArgumentNullException")]
        public void REP010()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var act = () => result.WrapError(ErrorCodes.Internal.Unexpected, null!);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("message");
        }

        [Fact(DisplayName = "REP-011: Async WrapError on Task failure should wrap the error")]
        public async Task REP011()
        {
            // arrange
            var innerError = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var result = Task.FromResult(Result<int, Error>.Failure(innerError));

            // act
            var wrapped = await result.WrapError(ErrorCodes.Internal.Unexpected, "Async operation failed.");

            // assert
            wrapped.IsFailure.Should().BeTrue();
            wrapped.Error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            wrapped.Error.Inner.Should().NotBeNull();
        }

        [Fact(DisplayName = "REP-012: Async WrapError on Task success should preserve value")]
        public async Task REP012()
        {
            // arrange
            var result = Task.FromResult(Result<int, Error>.Success(42));

            // act
            var wrapped = await result.WrapError(ErrorCodes.Internal.Unexpected, "Async operation failed.");

            // assert
            wrapped.IsSuccess.Should().BeTrue();
            wrapped.Value.Should().Be(42);
        }

        [Fact(DisplayName = "REP-013: Chained WrapError should create error chain")]
        public void REP013()
        {
            // arrange
            var rootError = new Error(ErrorCodes.IO.Database, "Query failed.");
            var result = Result<int, Error>.Failure(rootError);

            // act
            var wrapped = result
                .WrapError(ErrorCodes.NotFound.Entity, "User not found.")
                .WrapError(ErrorCodes.Internal.Unexpected, "Request failed.");

            // assert
            wrapped.IsFailure.Should().BeTrue();
            wrapped.Error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            wrapped.Error.Inner.Should().NotBeNull();
            wrapped.Error.Inner!.Value.Code.Should().Be(ErrorCodes.NotFound.Entity);
            wrapped.Error.Inner!.Value.Inner.Should().NotBeNull();
            wrapped.Error.Inner!.Value.Inner!.Value.Code.Should().Be(ErrorCodes.IO.Database);
        }

        [Fact(DisplayName = "REP-014: HasError should work with all error code categories")]
        public void REP014()
        {
            // arrange & act & assert
            Result<int, Error>.Failure(Errors.Validation("msg")).HasError(ErrorCodes.Validation._).Should().BeTrue();
            Result<int, Error>.Failure(Errors.NotFound("msg")).HasError(ErrorCodes.NotFound._).Should().BeTrue();
            Result<int, Error>.Failure(Errors.Unauthorized()).HasError(ErrorCodes.Auth._).Should().BeTrue();
            Result<int, Error>.Failure(Errors.Timeout("op")).HasError(ErrorCodes.IO._).Should().BeTrue();
            Result<int, Error>.Failure(Errors.Unexpected("msg")).HasError(ErrorCodes.Internal._).Should().BeTrue();
        }

        [Fact(DisplayName = "REP-015: Combined MapError and WrapError should work together")]
        public void REP015()
        {
            // arrange
            var error = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var result = Result<int, Error>.Failure(error);

            // act
            var transformed = result
                .MapError(e => e.With("retryCount", 3))
                .WrapError(ErrorCodes.Internal.Unexpected, "Service unavailable.");

            // assert
            transformed.IsFailure.Should().BeTrue();
            transformed.Error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            transformed.Error.Inner.Should().NotBeNull();
            transformed.Error.Inner!.Value.Metadata!["retryCount"].Should().Be(3);
        }
    }
}
