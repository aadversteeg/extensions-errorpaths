using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.AspNetCore;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.AspNetCore
{
    [Collection("HttpMapping")]
    public class ErrorHttpExtensionsTests
    {
        public ErrorHttpExtensionsTests()
        {
            // Clear custom mappings before each test to avoid static state pollution
            ErrorCodeHttpMapping.ClearCustomMappings();
        }

        [Fact(DisplayName = "HE-001: ToProblemDetails should set correct type URN")]
        public void HE001()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Type.Should().Be("urn:error:Validation.Required");
        }

        [Fact(DisplayName = "HE-002: ToProblemDetails should set status from error code")]
        public void HE002()
        {
            // arrange
            var error = new Error(ErrorCodes.NotFound.Entity, "User not found.");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Status.Should().Be(404);
        }

        [Fact(DisplayName = "HE-003: ToProblemDetails should set detail from message")]
        public void HE003()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "The email field is required.");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Detail.Should().Be("The email field is required.");
        }

        [Fact(DisplayName = "HE-004: ToProblemDetails should set title from status code")]
        public void HE004()
        {
            // arrange
            var error = new Error(ErrorCodes.Auth.Unauthorized, "Token expired.");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Title.Should().Be("Unauthorized");
        }

        [Fact(DisplayName = "HE-005: ToProblemDetails should include instance when provided")]
        public void HE005()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            var problemDetails = error.ToProblemDetails("/api/users/123");

            // assert
            problemDetails.Instance.Should().Be("/api/users/123");
        }

        [Fact(DisplayName = "HE-006: ToProblemDetails should include metadata as extensions")]
        public void HE006()
        {
            // arrange
            var error = Errors.Required("email");

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Extensions.Should().ContainKey("field");
            problemDetails.Extensions["field"].Should().Be("email");
        }

        [Fact(DisplayName = "HE-007: ToProblemDetails should include inner error info")]
        public void HE007()
        {
            // arrange
            var inner = new Error(ErrorCodes.IO.Network, "Connection failed.");
            var outer = inner.Wrap(ErrorCodes.Internal.Unexpected, "Operation failed.");

            // act
            var problemDetails = outer.ToProblemDetails();

            // assert
            problemDetails.Extensions.Should().ContainKey("innerError");
        }

        [Fact(DisplayName = "HE-008: ToProblemDetails with multiple metadata should include all")]
        public void HE008()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Range, "Value out of range.")
                .With("min", 0)
                .With("max", 100)
                .With("actual", 150);

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Extensions.Should().ContainKey("min");
            problemDetails.Extensions.Should().ContainKey("max");
            problemDetails.Extensions.Should().ContainKey("actual");
        }

        [Fact(DisplayName = "HE-009: GetHttpStatusCode should return correct status")]
        public void HE009()
        {
            // arrange
            var error = new Error(ErrorCodes.Auth.Forbidden, "Access denied.");

            // act
            var status = error.GetHttpStatusCode();

            // assert
            status.Should().Be(403);
        }

        [Fact(DisplayName = "HE-010: ToHttpResult should return ProblemHttpResult with correct status")]
        public void HE010()
        {
            // arrange
            var error = new Error(ErrorCodes.Validation.Required, "Field is required.");

            // act
            var result = error.ToHttpResult();

            // assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.IStatusCodeHttpResult>();
            var statusCodeResult = (Microsoft.AspNetCore.Http.IStatusCodeHttpResult)result;
            statusCodeResult.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "HE-011: Different error codes should produce different titles")]
        public void HE011()
        {
            // arrange & act & assert
            new Error(ErrorCodes.Validation._, "msg").ToProblemDetails().Title.Should().Be("Bad Request");
            new Error(ErrorCodes.NotFound._, "msg").ToProblemDetails().Title.Should().Be("Not Found");
            new Error(ErrorCodes.Auth.Unauthorized, "msg").ToProblemDetails().Title.Should().Be("Unauthorized");
            new Error(ErrorCodes.Auth.Forbidden, "msg").ToProblemDetails().Title.Should().Be("Forbidden");
            new Error(ErrorCodes.IO.Timeout, "msg").ToProblemDetails().Title.Should().Be("Gateway Timeout");
            new Error(ErrorCodes.Internal._, "msg").ToProblemDetails().Title.Should().Be("Internal Server Error");
        }

        [Fact(DisplayName = "HE-012: Factory errors should produce correct ProblemDetails")]
        public void HE012()
        {
            // arrange
            var error = Errors.NotFound("User", 42);

            // act
            var problemDetails = error.ToProblemDetails();

            // assert
            problemDetails.Status.Should().Be(404);
            problemDetails.Type.Should().Be("urn:error:NotFound.Entity");
            problemDetails.Extensions["entity"].Should().Be("User");
            problemDetails.Extensions["id"].Should().Be(42);
        }
    }
}
