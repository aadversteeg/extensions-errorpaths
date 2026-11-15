using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Functional;
using Ave.Extensions.Functional;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Functional
{
    public class ResultErrorTraverseExtensionsTests
    {
        private static Result<int, Error> ParseNumber(string text) =>
            int.TryParse(text, out var number)
                ? Result<int, Error>.Success(number)
                : Result<int, Error>.Failure(Errors.Format(text, "a number"));

        private static Task<Result<int, Error>> ParseNumberAsync(string text) =>
            Task.FromResult(ParseNumber(text));

        [Fact(DisplayName = "RET-001: TraverseAll of an empty sequence should succeed with an empty list")]
        public void RET001()
        {
            // act
            var result = Array.Empty<string>().TraverseAll(ParseNumber);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact(DisplayName = "RET-002: TraverseAll should succeed with all values in sequence order")]
        public void RET002()
        {
            // act
            var result = new[] { "3", "1", "2" }.TraverseAll(ParseNumber);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Equal(3, 1, 2);
        }

        [Fact(DisplayName = "RET-003: TraverseAll with one failure should fail with that error unchanged")]
        public void RET003()
        {
            // act
            var result = new[] { "1", "x", "3" }.TraverseAll(ParseNumber);

            // assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Format("x", "a number"));
            result.Error.Code.Should().Be(ErrorCodes.Validation.Format);
            result.Error.Metadata!["field"].Should().Be("x");
        }

        [Fact(DisplayName = "RET-004: TraverseAll with several failures should fail with an aggregate of all errors in sequence order")]
        public void RET004()
        {
            // act
            var result = new[] { "b", "1", "a", "2", "c" }.TraverseAll(ParseNumber);

            // assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(ErrorCodes.Aggregate);
            result.Error.Parts().Select(part => part.Metadata!["field"]).Should().Equal("b", "a", "c");
        }

        [Fact(DisplayName = "RET-005: TraverseAll should flatten aggregates returned by the map")]
        public void RET005()
        {
            // arrange
            Result<int, Error> Validate(string text) =>
                text == "both"
                    ? Result<int, Error>.Failure(Errors.Aggregate(new[] { Errors.Validation("too long"), Errors.Validation("bad chars") }))
                    : ParseNumber(text);

            // act
            var result = new[] { "x", "both", "1" }.TraverseAll(Validate);

            // assert
            result.Error.Parts().Should().HaveCount(3);
            result.Error.Parts().Should().NotContain(part => part.Code == ErrorCodes.Aggregate);
            result.Error.Parts()[1].Message.Should().Be("too long");
            result.Error.Parts()[2].Message.Should().Be("bad chars");
        }

        [Fact(DisplayName = "RET-006: TraverseAll should map every item exactly once, in order, also after a failure")]
        public void RET006()
        {
            // arrange
            var visited = new List<string>();

            // act
            new[] { "x", "1", "y" }.TraverseAll(item => { visited.Add(item); return ParseNumber(item); });

            // assert
            visited.Should().Equal("x", "1", "y");
        }

        [Fact(DisplayName = "RET-007: TraverseAll with a null argument should throw ArgumentNullException")]
        public void RET007()
        {
            // act
            Action nullSource = () => ((IEnumerable<string>)null!).TraverseAll(ParseNumber);
            Action nullMap = () => new[] { "1" }.TraverseAll((Func<string, Result<int, Error>>)null!);

            // assert
            nullSource.Should().Throw<ArgumentNullException>().WithParameterName("source");
            nullMap.Should().Throw<ArgumentNullException>().WithParameterName("map");
        }

        [Fact(DisplayName = "RET-008: Async TraverseAll of an empty sequence should succeed with an empty list")]
        public async Task RET008()
        {
            // act
            var result = await Array.Empty<string>().TraverseAll(ParseNumberAsync);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact(DisplayName = "RET-009: Async TraverseAll should succeed with all values in sequence order")]
        public async Task RET009()
        {
            // act
            var result = await new[] { "3", "1", "2" }.TraverseAll(ParseNumberAsync);

            // assert
            result.Value.Should().Equal(3, 1, 2);
        }

        [Fact(DisplayName = "RET-010: Async TraverseAll with one failure should fail with that error unchanged")]
        public async Task RET010()
        {
            // act
            var result = await new[] { "1", "x" }.TraverseAll(ParseNumberAsync);

            // assert
            result.Error.Should().Be(Errors.Format("x", "a number"));
        }

        [Fact(DisplayName = "RET-011: Async TraverseAll with several failures should fail with an aggregate in sequence order")]
        public async Task RET011()
        {
            // act
            var result = await new[] { "b", "1", "a" }.TraverseAll(ParseNumberAsync);

            // assert
            result.Error.Code.Should().Be(ErrorCodes.Aggregate);
            result.Error.Parts().Select(part => part.Metadata!["field"]).Should().Equal("b", "a");
        }

        [Fact(DisplayName = "RET-012: Async TraverseAll should run one item at a time, in order, and visit every item")]
        public async Task RET012()
        {
            // arrange
            var running = 0;
            var maxRunning = 0;
            var visited = new List<string>();
            async Task<Result<int, Error>> Map(string item)
            {
                running++;
                maxRunning = Math.Max(maxRunning, running);
                visited.Add(item);
                await Task.Delay(5);
                running--;
                return ParseNumber(item);
            }

            // act
            await new[] { "x", "1", "y" }.TraverseAll(Map);

            // assert
            maxRunning.Should().Be(1);
            visited.Should().Equal("x", "1", "y");
        }

        [Fact(DisplayName = "RET-013: Async TraverseAll with a null argument should throw at the call")]
        public void RET013()
        {
            // act
            Action nullSource = () => ((IEnumerable<string>)null!).TraverseAll(ParseNumberAsync);
            Action nullMap = () => new[] { "1" }.TraverseAll((Func<string, Task<Result<int, Error>>>)null!);

            // assert
            nullSource.Should().Throw<ArgumentNullException>().WithParameterName("source");
            nullMap.Should().Throw<ArgumentNullException>().WithParameterName("awaitableMap");
        }
    }
}
