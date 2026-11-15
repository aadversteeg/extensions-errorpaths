using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Serialization;
using FluentAssertions;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Serialization
{
    public class ErrorJsonConverterAggregateTests
    {
        private readonly JsonSerializerOptions _options;

        public ErrorJsonConverterAggregateTests()
        {
            _options = ErrorPathsJsonOptions.Configure();
        }

        [Fact(DisplayName = "EJA-001: Serialize aggregate should write its parts in order under errors")]
        public void EJA001()
        {
            // arrange
            var error = Errors.Aggregate(new[]
            {
                new Error(ErrorCodes.Validation.Required, "a"),
                new Error(ErrorCodes.NotFound.Entity, "b")
            });

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Be(
                "{\"code\":\"Aggregate\",\"message\":\"2 errors: a; b\",\"errors\":[" +
                "{\"code\":\"Validation.Required\",\"message\":\"a\"}," +
                "{\"code\":\"NotFound.Entity\",\"message\":\"b\"}]}");
        }

        [Fact(DisplayName = "EJA-002: Serialize plain error should not write errors")]
        public void EJA002()
        {
            // arrange
            var error = Errors.Required("email");

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().NotContain("\"errors\"");
        }

        [Fact(DisplayName = "EJA-003: Round trip should keep an aggregate with its parts, their order and their metadata")]
        public void EJA003()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.NotFound("User", 42), Errors.Timeout("lookup") })
                .With("document", "invoice-1");

            // act
            var json = JsonSerializer.Serialize(error, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Should().Be(error);
            deserialized.Code.Should().Be(ErrorCodes.Aggregate);
            deserialized.Parts().Should().Equal(error.Parts());
            deserialized.Parts()[0].Metadata!["field"].Should().Be("name");
            ((long)deserialized.Parts()[1].Metadata!["id"]).Should().Be(42);
            deserialized.Metadata!["document"].Should().Be("invoice-1");
        }

        [Fact(DisplayName = "EJA-004: Round trip of a flattened nested aggregate should give the flat parts")]
        public void EJA004()
        {
            // arrange
            var nested = Errors.Aggregate(new[] { Errors.Validation("b"), Errors.Validation("c") });
            var error = Errors.Aggregate(new[] { Errors.Validation("a"), nested });

            // act
            var json = JsonSerializer.Serialize(error, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Parts().Select(part => part.Message).Should().Equal("a", "b", "c");
        }

        [Fact(DisplayName = "EJA-005: Deserialize should read errors also when it comes before code")]
        public void EJA005()
        {
            // arrange
            var json = "{\"errors\":[{\"code\":\"Validation\",\"message\":\"a\"},{\"code\":\"Validation\",\"message\":\"b\"}]," +
                       "\"code\":\"Aggregate\",\"message\":\"2 errors: a; b\"}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Should().Be(ErrorCodes.Aggregate);
            error.Parts().Select(part => part.Message).Should().Equal("a", "b");
        }

        [Fact(DisplayName = "EJA-006: Deserialize should ignore errors on a code other than Aggregate")]
        public void EJA006()
        {
            // arrange
            var json = "{\"code\":\"Validation\",\"message\":\"m\",\"errors\":[{\"code\":\"Validation\",\"message\":\"a\"}," +
                       "{\"code\":\"Validation\",\"message\":\"b\"}]}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Should().Be(ErrorCodes.Validation._);
            error.Message.Should().Be("m");
            error.Parts().Should().Equal(error);
        }

        [Fact(DisplayName = "EJA-007: Deserialize Aggregate code without errors should give a plain error")]
        public void EJA007()
        {
            // arrange
            var json = "{\"code\":\"Aggregate\",\"message\":\"m\"}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Should().Be(ErrorCodes.Aggregate);
            error.Message.Should().Be("m");
            error.Parts().Should().Equal(error);
        }

        [Fact(DisplayName = "EJA-008: Deserialize errors that is not an array should throw JsonException")]
        public void EJA008()
        {
            // arrange
            var json = "{\"code\":\"Aggregate\",\"message\":\"m\",\"errors\":\"oops\"}";

            // act
            var act = () => JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            act.Should().Throw<JsonException>();
        }

        [Fact(DisplayName = "EJA-009: Serialize an object holding an aggregate should nest the parts")]
        public void EJA009()
        {
            // arrange
            var holder = new { Failure = Errors.Aggregate(new[] { Errors.Validation("a"), Errors.Validation("b") }) };

            // act
            var json = JsonSerializer.Serialize(holder, _options);
            using var document = JsonDocument.Parse(json);

            // assert
            document.RootElement.GetProperty("Failure").GetProperty("errors").GetArrayLength().Should().Be(2);
        }
    }
}
