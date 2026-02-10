using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.AspNetCore;
using FluentAssertions;
using System.Net;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths.AspNetCore
{
    public class ErrorCodeHttpMappingTests
    {
        public ErrorCodeHttpMappingTests()
        {
            // Clear custom mappings before each test
            ErrorCodeHttpMapping.ClearCustomMappings();
        }

        [Fact(DisplayName = "HM-001: Validation code should map to 400 BadRequest")]
        public void HM001()
        {
            // act
            var status = ErrorCodes.Validation._.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "HM-002: Validation.Required should map to 400 BadRequest")]
        public void HM002()
        {
            // act
            var status = ErrorCodes.Validation.Required.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "HM-003: NotFound code should map to 404 NotFound")]
        public void HM003()
        {
            // act
            var status = ErrorCodes.NotFound._.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "HM-004: NotFound.Entity should map to 404 NotFound")]
        public void HM004()
        {
            // act
            var status = ErrorCodes.NotFound.Entity.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "HM-005: Auth.Unauthorized should map to 401 Unauthorized")]
        public void HM005()
        {
            // act
            var status = ErrorCodes.Auth.Unauthorized.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "HM-006: Auth.Forbidden should map to 403 Forbidden")]
        public void HM006()
        {
            // act
            var status = ErrorCodes.Auth.Forbidden.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "HM-007: Auth.TokenExpired should map to 401 Unauthorized")]
        public void HM007()
        {
            // act
            var status = ErrorCodes.Auth.TokenExpired.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "HM-008: IO.Timeout should map to 504 GatewayTimeout")]
        public void HM008()
        {
            // act
            var status = ErrorCodes.IO.Timeout.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.GatewayTimeout);
        }

        [Fact(DisplayName = "HM-009: IO.Network should map to 502 BadGateway")]
        public void HM009()
        {
            // act
            var status = ErrorCodes.IO.Network.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.BadGateway);
        }

        [Fact(DisplayName = "HM-010: Internal.Unexpected should map to 500 InternalServerError")]
        public void HM010()
        {
            // act
            var status = ErrorCodes.Internal.Unexpected.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact(DisplayName = "HM-011: Unknown code should map to 500 InternalServerError")]
        public void HM011()
        {
            // arrange
            var unknownCode = new ErrorCode("Unknown.Error");

            // act
            var status = unknownCode.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact(DisplayName = "HM-012: Custom nested code should walk up hierarchy")]
        public void HM012()
        {
            // arrange
            var customCode = ErrorCodes.Validation.Required / "Email" / "Format";

            // act
            var status = customCode.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "HM-013: Register custom mapping should override default")]
        public void HM013()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Validation._, HttpStatusCode.UnprocessableEntity);

            // act
            var status = ErrorCodes.Validation.Required.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact(DisplayName = "HM-014: Register custom mapping with int should work")]
        public void HM014()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Validation._, 422);

            // act
            var status = ErrorCodes.Validation._.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact(DisplayName = "HM-015: Custom mapping for specific code takes precedence")]
        public void HM015()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Validation.Required, HttpStatusCode.Conflict);

            // act
            var requiredStatus = ErrorCodes.Validation.Required.ToHttpStatusCode();
            var formatStatus = ErrorCodes.Validation.Format.ToHttpStatusCode();

            // assert
            requiredStatus.Should().Be(HttpStatusCode.Conflict);
            formatStatus.Should().Be(HttpStatusCode.BadRequest); // Falls back to Validation default
        }

        [Fact(DisplayName = "HM-016: ClearCustomMappings should remove custom mappings")]
        public void HM016()
        {
            // arrange
            ErrorCodeHttpMapping.Register(ErrorCodes.Validation._, HttpStatusCode.UnprocessableEntity);
            ErrorCodeHttpMapping.ClearCustomMappings();

            // act
            var status = ErrorCodes.Validation._.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "HM-017: ToHttpStatusCodeInt should return integer")]
        public void HM017()
        {
            // act
            var status = ErrorCodes.NotFound._.ToHttpStatusCodeInt();

            // assert
            status.Should().Be(404);
        }

        [Fact(DisplayName = "HM-018: Default ErrorCode should map to 500")]
        public void HM018()
        {
            // arrange
            var code = default(ErrorCode);

            // act
            var status = code.ToHttpStatusCode();

            // assert
            status.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
