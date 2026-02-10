using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Serialization;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Serialization
{
    public class ErrorCodeJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public ErrorCodeJsonConverterTests()
        {
            _options = ErrorPathsJsonOptions.Configure();
        }

        [Fact(DisplayName = "ECJ-001: Serialize simple ErrorCode should produce string")]
        public void ECJ001()
        {
            // arrange
            var code = new ErrorCode("Validation");

            // act
            var json = JsonSerializer.Serialize(code, _options);

            // assert
            json.Should().Be("\"Validation\"");
        }

        [Fact(DisplayName = "ECJ-002: Serialize nested ErrorCode should produce dotted string")]
        public void ECJ002()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");

            // act
            var json = JsonSerializer.Serialize(code, _options);

            // assert
            json.Should().Be("\"Validation.Required.Email\"");
        }

        [Fact(DisplayName = "ECJ-003: Deserialize string should produce ErrorCode")]
        public void ECJ003()
        {
            // arrange
            var json = "\"Validation.Required\"";

            // act
            var code = JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            code.Value.Should().Be("Validation.Required");
        }

        [Fact(DisplayName = "ECJ-004: Round-trip serialization should preserve value")]
        public void ECJ004()
        {
            // arrange
            var original = ErrorCodes.Validation.Required / "Email";

            // act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            deserialized.Should().Be(original);
            deserialized.Value.Should().Be("Validation.Required.Email");
        }

        [Fact(DisplayName = "ECJ-005: Deserialize null should produce default ErrorCode")]
        public void ECJ005()
        {
            // arrange
            var json = "null";

            // act
            var code = JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            code.Value.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "ECJ-006: Serialize default ErrorCode should produce null")]
        public void ECJ006()
        {
            // arrange
            var code = default(ErrorCode);

            // act
            var json = JsonSerializer.Serialize(code, _options);

            // assert
            json.Should().Be("null");
        }

        [Fact(DisplayName = "ECJ-007: Deserialize empty string should produce default ErrorCode")]
        public void ECJ007()
        {
            // arrange
            var json = "\"\"";

            // act
            var code = JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            code.Value.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "ECJ-008: Deserialize whitespace string should produce default ErrorCode")]
        public void ECJ008()
        {
            // arrange
            var json = "\"   \"";

            // act
            var code = JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            code.Value.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "ECJ-009: Deserialize number should throw JsonException")]
        public void ECJ009()
        {
            // arrange
            var json = "123";

            // act
            var act = () => JsonSerializer.Deserialize<ErrorCode>(json, _options);

            // assert
            act.Should().Throw<JsonException>();
        }

        [Fact(DisplayName = "ECJ-010: ErrorCode in object should serialize correctly")]
        public void ECJ010()
        {
            // arrange
            var wrapper = new { Code = new ErrorCode("Validation.Required") };

            // act
            var json = JsonSerializer.Serialize(wrapper, _options);

            // assert
            json.Should().Contain("\"Code\":\"Validation.Required\"");
        }
    }
}
