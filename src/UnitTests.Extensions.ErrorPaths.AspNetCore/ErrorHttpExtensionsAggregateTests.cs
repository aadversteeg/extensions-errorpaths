using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.AspNetCore;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.AspNetCore
{
    [Collection("HttpMapping")]
    public class ErrorHttpExtensionsAggregateTests
    {
        public ErrorHttpExtensionsAggregateTests()
        {
            // Clear custom mappings before each test to avoid static state pollution
            ErrorCodeHttpMapping.ClearCustomMappings();
        }

        [Fact(DisplayName = "HEA-001: Aggregate of parts with the same status should get that status")]
        public void HEA001()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.NotFound("User", 1), Errors.NotFound("Order", 2) });

            // act & assert
            error.GetHttpStatusCode().Should().Be(404);
            error.ToProblemDetails().Status.Should().Be(404);
            error.ToProblemDetails().Title.Should().Be("Not Found");
        }

        [Fact(DisplayName = "HEA-002: Aggregate of different client errors should get 400 Bad Request")]
        public void HEA002()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.NotFound("User", 1), Errors.Forbidden() });

            // act & assert
            error.GetHttpStatusCode().Should().Be(400);
        }

        [Fact(DisplayName = "HEA-003: Aggregate with a server error among client errors should get 500")]
        public void HEA003()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.Timeout("lookup") });

            // act & assert
            error.GetHttpStatusCode().Should().Be(500);
        }

        [Fact(DisplayName = "HEA-004: Aggregate of different server errors should get 500")]
        public void HEA004()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Timeout("lookup"), Errors.Network("refused") });

            // act & assert
            error.GetHttpStatusCode().Should().Be(500);
        }

        [Fact(DisplayName = "HEA-005: A custom mapping for the Aggregate code should win over the parts")]
        public void HEA005()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Aggregate, HttpStatusCode.UnprocessableEntity);
            var error = Errors.Aggregate(new[] { Errors.NotFound("User", 1), Errors.NotFound("Order", 2) });

            // act & assert
            error.GetHttpStatusCode().Should().Be(422);
        }

        [Fact(DisplayName = "HEA-006: Custom mappings of the parts should be used for the aggregate status")]
        public void HEA006()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Validation._, HttpStatusCode.UnprocessableEntity);
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.Format("date", "yyyy-MM-dd") });

            // act & assert
            error.GetHttpStatusCode().Should().Be(422);
        }

        [Fact(DisplayName = "HEA-007: A plain error with the Aggregate code should map like any unmapped code")]
        public void HEA007()
        {
            // arrange
            var error = new Error(ErrorCodes.Aggregate, "Hand-made.");

            // act & assert
            error.GetHttpStatusCode().Should().Be(500);
            error.ToProblemDetails().Extensions.Should().NotContainKey("errors");
        }

        [Fact(DisplayName = "HEA-008: ProblemDetails of an aggregate should list every part in order under errors")]
        public void HEA008()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.Validation("Too short.") });

            // act
            var problemDetails = error.ToProblemDetails("/documents/1");

            // assert
            problemDetails.Type.Should().Be("urn:error:Aggregate");
            problemDetails.Detail.Should().Be(error.Message);
            problemDetails.Instance.Should().Be("/documents/1");
            problemDetails.Extensions.Should().ContainKey("errors");

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            using var document = JsonDocument.Parse(json);
            var errors = document.RootElement.GetProperty("errors");
            errors.GetArrayLength().Should().Be(2);
            errors[0].GetProperty("code").GetString().Should().Be("Validation.Required");
            errors[0].GetProperty("message").GetString().Should().Be("The field 'name' is required.");
            errors[0].GetProperty("metadata").GetProperty("field").GetString().Should().Be("name");
            errors[1].GetProperty("code").GetString().Should().Be("Validation");
            errors[1].TryGetProperty("metadata", out _).Should().BeFalse();
        }

        [Fact(DisplayName = "HEA-009: ProblemDetails of a plain error should not get an errors extension")]
        public void HEA009()
        {
            // arrange
            var error = Errors.Required("name");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Extensions.Should().NotContainKey("errors");
        }

        [Fact(DisplayName = "HEA-010: ProblemDetails of an aggregate with metadata should keep that metadata as extensions")]
        public void HEA010()
        {
            // arrange
            var error = Errors.Aggregate(new[] { Errors.Required("name"), Errors.Required("date") }).With("document", "invoice-1");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Extensions["document"].Should().Be("invoice-1");
            problemDetails.Status.Should().Be(400);
        }
    }
}
