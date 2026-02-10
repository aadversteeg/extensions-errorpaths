using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Serialization;
using FluentAssertions;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.Serialization
{
    public class ErrorJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public ErrorJsonConverterTests()
        {
            _options = ErrorPathsJsonOptions.Configure();
        }

        [Fact(DisplayName = "EJ-001: Serialize simple Error should produce object with code and message")]
        public void EJ001()
        {
            // arrange
            var error = new Error(new ErrorCode("Validation.Required"), "Field is required.");

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Contain("\"code\":\"Validation.Required\"");
            json.Should().Contain("\"message\":\"Field is required.\"");
        }

        [Fact(DisplayName = "EJ-002: Deserialize object should produce Error")]
        public void EJ002()
        {
            // arrange
            var json = "{\"code\":\"Validation.Required\",\"message\":\"Field is required.\"}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Value.Should().Be("Validation.Required");
            error.Message.Should().Be("Field is required.");
        }

        [Fact(DisplayName = "EJ-003: Round-trip serialization should preserve Error")]
        public void EJ003()
        {
            // arrange
            var original = new Error(ErrorCodes.Validation.Required, "Email is required.");

            // act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Code.Should().Be(original.Code);
            deserialized.Message.Should().Be(original.Message);
        }

        [Fact(DisplayName = "EJ-004: Serialize Error with metadata should include metadata object")]
        public void EJ004()
        {
            // arrange
            var error = Errors.Required("email");

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Contain("\"metadata\":");
            json.Should().Contain("\"field\":\"email\"");
        }

        [Fact(DisplayName = "EJ-005: Deserialize Error with metadata should restore metadata")]
        public void EJ005()
        {
            // arrange
            var json = "{\"code\":\"Validation.Required\",\"message\":\"Field is required.\",\"metadata\":{\"field\":\"email\",\"count\":5}}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Metadata.Should().NotBeNull();
            error.Metadata!["field"].Should().Be("email");
            ((long)error.Metadata["count"]).Should().Be(5);
        }

        [Fact(DisplayName = "EJ-006: Round-trip with metadata should preserve values")]
        public void EJ006()
        {
            // arrange
            var original = new Error(ErrorCodes.Validation.Required, "Field is required.")
                .With("field", "email")
                .With("maxLength", 100)
                .With("isValid", false);

            // act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Metadata.Should().NotBeNull();
            deserialized.Metadata!["field"].Should().Be("email");
            ((long)deserialized.Metadata["maxLength"]).Should().Be(100);
            ((bool)deserialized.Metadata["isValid"]).Should().BeFalse();
        }

        [Fact(DisplayName = "EJ-007: Serialize Error with inner error should include inner object")]
        public void EJ007()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var outer = inner.Wrap(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // act
            var json = JsonSerializer.Serialize(outer, _options);

            // assert
            json.Should().Contain("\"inner\":");
            json.Should().Contain("\"code\":\"IO.Network\"");
            json.Should().Contain("\"message\":\"Connection failed.\"");
        }

        [Fact(DisplayName = "EJ-008: Deserialize Error with inner error should restore chain")]
        public void EJ008()
        {
            // arrange
            var json = "{\"code\":\"Internal.Unexpected\",\"message\":\"Operation failed.\",\"inner\":{\"code\":\"IO.Network\",\"message\":\"Connection failed.\"}}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Value.Should().Be("Internal.Unexpected");
            error.Inner.Should().NotBeNull();
            error.Inner!.Value.Code.Value.Should().Be("IO.Network");
            error.Inner!.Value.Message.Should().Be("Connection failed.");
        }

        [Fact(DisplayName = "EJ-009: Round-trip with inner error should preserve chain")]
        public void EJ009()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Database, "Query failed.");
            var middle = inner.Wrap(ErrorCodes.NotFound.Entity, "User not found.");
            var outer = middle.Wrap(ErrorCodes.Internal.Unexpected, "Request failed.");

            // act
            var json = JsonSerializer.Serialize(outer, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Code.Value.Should().Be("Internal.Unexpected");
            deserialized.Inner.Should().NotBeNull();
            deserialized.Inner!.Value.Code.Value.Should().Be("NotFound.Entity");
            deserialized.Inner!.Value.Inner.Should().NotBeNull();
            deserialized.Inner!.Value.Inner!.Value.Code.Value.Should().Be("IO.Database");
        }

        [Fact(DisplayName = "EJ-010: Deserialize null should produce default Error")]
        public void EJ010()
        {
            // arrange
            var json = "null";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Value.Should().Be(string.Empty);
            error.Message.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "EJ-011: Serialize default Error should produce null")]
        public void EJ011()
        {
            // arrange
            var error = default(Error);

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Be("null");
        }

        [Fact(DisplayName = "EJ-012: Deserialize with extra properties should ignore them")]
        public void EJ012()
        {
            // arrange
            var json = "{\"code\":\"Validation\",\"message\":\"Error\",\"unknownField\":\"ignored\",\"anotherField\":123}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Value.Should().Be("Validation");
            error.Message.Should().Be("Error");
        }

        [Fact(DisplayName = "EJ-013: Metadata with nested object should serialize correctly")]
        public void EJ013()
        {
            // arrange
            var nestedData = new Dictionary<string, object>
            {
                { "nested", "value" }
            };
            var error = new Error(ErrorCodes.Validation._, "Error")
                .With("data", nestedData);

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Contain("\"data\":{\"nested\":\"value\"}");
        }

        [Fact(DisplayName = "EJ-014: Metadata with array should serialize correctly")]
        public void EJ014()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation._, "Multiple errors")
                .With("fields", new List<object> { "email", "name", "age" });

            // act
            var json = JsonSerializer.Serialize(error, _options);

            // assert
            json.Should().Contain("\"fields\":[\"email\",\"name\",\"age\"]");
        }

        [Fact(DisplayName = "EJ-015: CreateDefault should use camelCase")]
        public void EJ015()
        {
            // arrange
            var options = ErrorPathsJsonOptions.CreateDefault();
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            var json = JsonSerializer.Serialize(error, options);

            // assert
            json.Should().Contain("\"code\":");
            json.Should().Contain("\"message\":");
        }

        [Fact(DisplayName = "EJ-016: Case insensitive property matching on deserialize")]
        public void EJ016()
        {
            // arrange
            var json = "{\"CODE\":\"Validation\",\"MESSAGE\":\"Error\"}";

            // act
            var error = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            error.Code.Value.Should().Be("Validation");
            error.Message.Should().Be("Error");
        }

        [Fact(DisplayName = "EJ-017: Error from factory methods should serialize correctly")]
        public void EJ017()
        {
            // arrange
            var error = Errors.NotFound("User", 42);

            // act
            var json = JsonSerializer.Serialize(error, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Code.Should().Be(ErrorCodes.NotFound.Entity);
            deserialized.Metadata.Should().NotBeNull();
            deserialized.Metadata!["entity"].Should().Be("User");
            ((long)deserialized.Metadata["id"]).Should().Be(42);
        }

        [Fact(DisplayName = "EJ-018: Complex Error with metadata and inner should round-trip")]
        public void EJ018()
        {
            // arrange
            var inner = Errors.Network("Connection refused.")
                .With("host", "localhost")
                .With("port", 5432);
            var outer = inner.Wrap(ErrorCodes.IO.Database, "Database connection failed.");

            // act
            var json = JsonSerializer.Serialize(outer, _options);
            var deserialized = JsonSerializer.Deserialize<Error>(json, _options);

            // assert
            deserialized.Code.Value.Should().Be("IO.Database");
            deserialized.Inner.Should().NotBeNull();
            deserialized.Inner!.Value.Metadata!["host"].Should().Be("localhost");
            ((long)deserialized.Inner!.Value.Metadata["port"]).Should().Be(5432);
        }
    }
}
