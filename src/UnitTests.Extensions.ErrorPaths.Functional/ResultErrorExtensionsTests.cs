using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Functional;
using Ave.Extensions.Functional;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Functional
{
    public class ResultErrorExtensionsTests
    {
        [Fact(DisplayName = "REE-001: MapError on failure should transform error")]
        public void REE001()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Result<int, Error>.Failure(error);

            // act
            var mapped = result.MapError(e => e.With("transformed", true));

            // assert
            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Metadata.Should().NotBeNull();
            mapped.Error.Metadata!["transformed"].Should().Be(true);
        }

        [Fact(DisplayName = "REE-002: MapError on success should preserve value")]
        public void REE002()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var mapped = result.MapError(e => e.With("transformed", true));

            // assert
            mapped.IsSuccess.Should().BeTrue();
            mapped.Value.Should().Be(42);
        }

        [Fact(DisplayName = "REE-003: MapError with null function should throw ArgumentNullException")]
        public void REE003()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var act = () => result.MapError((Func<Error, Error>)null!);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapError");
        }

        [Fact(DisplayName = "REE-004: MapError should change error code")]
        public void REE004()
        {
            // arrange
            var error = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var result = Result<string, Error>.Failure(error);

            // act
            var mapped = result.MapError(e => new Error(ErrorCodes.Internal.Unexpected, "Wrapped: " + e.Message));

            // assert
            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Code.Should().Be(ErrorCodes.Internal.Unexpected);
            mapped.Error.Message.Should().Contain("Connection failed.");
        }

        [Fact(DisplayName = "REE-005: Async MapError on Task failure should transform error")]
        public async Task REE005()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Task.FromResult(Result<int, Error>.Failure(error));

            // act
            var mapped = await result.MapError(e => e.With("async", true));

            // assert
            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Metadata!["async"].Should().Be(true);
        }

        [Fact(DisplayName = "REE-006: Async MapError on Task success should preserve value")]
        public async Task REE006()
        {
            // arrange
            var result = Task.FromResult(Result<int, Error>.Success(42));

            // act
            var mapped = await result.MapError(e => e.With("async", true));

            // assert
            mapped.IsSuccess.Should().BeTrue();
            mapped.Value.Should().Be(42);
        }

        [Fact(DisplayName = "REE-007: MapError with async function on failure should transform error")]
        public async Task REE007()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Result<int, Error>.Failure(error);

            // act
            var mapped = await result.MapError(async e =>
            {
                await Task.Delay(1);
                return e.With("asyncFunc", true);
            });

            // assert
            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Metadata!["asyncFunc"].Should().Be(true);
        }

        [Fact(DisplayName = "REE-008: MapError with async function on success should preserve value")]
        public async Task REE008()
        {
            // arrange
            var result = Result<int, Error>.Success(42);

            // act
            var mapped = await result.MapError(async e =>
            {
                await Task.Delay(1);
                return e.With("asyncFunc", true);
            });

            // assert
            mapped.IsSuccess.Should().BeTrue();
            mapped.Value.Should().Be(42);
        }

        [Fact(DisplayName = "REE-009: Async MapError on Task with async function should transform error")]
        public async Task REE009()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");
            var result = Task.FromResult(Result<int, Error>.Failure(error));

            // act
            var mapped = await result.MapError(async e =>
            {
                await Task.Delay(1);
                return e.With("fullyAsync", true);
            });

            // assert
            mapped.IsFailure.Should().BeTrue();
            mapped.Error.Metadata!["fullyAsync"].Should().Be(true);
        }

        [Fact(DisplayName = "REE-010: Async MapError on Task with async function on success should preserve value")]
        public async Task REE010()
        {
            // arrange
            var result = Task.FromResult(Result<int, Error>.Success(42));

            // act
            var mapped = await result.MapError(async e =>
            {
                await Task.Delay(1);
                return e.With("fullyAsync", true);
            });

            // assert
            mapped.IsSuccess.Should().BeTrue();
            mapped.Value.Should().Be(42);
        }
    }
}
