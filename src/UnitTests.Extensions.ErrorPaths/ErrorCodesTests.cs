using Ave.Extensions.ErrorPaths;
using FluentAssertions;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths
{
    public class ErrorCodesTests
    {
        [Fact(DisplayName = "ECS-001: Validation root code should have correct value")]
        public void ECS001()
        {
            // act & assert
            ErrorCodes.Validation._.Value.Should().Be("Validation");
        }

        [Fact(DisplayName = "ECS-002: Validation.Required should be child of Validation")]
        public void ECS002()
        {
            // act & assert
            ErrorCodes.Validation.Required.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
            ErrorCodes.Validation.Required.Value.Should().Be("Validation.Required");
        }

        [Fact(DisplayName = "ECS-003: Validation.Format should be child of Validation")]
        public void ECS003()
        {
            // act & assert
            ErrorCodes.Validation.Format.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
            ErrorCodes.Validation.Format.Value.Should().Be("Validation.Format");
        }

        [Fact(DisplayName = "ECS-004: Validation.Range should be child of Validation")]
        public void ECS004()
        {
            // act & assert
            ErrorCodes.Validation.Range.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-005: Validation.Length should be child of Validation")]
        public void ECS005()
        {
            // act & assert
            ErrorCodes.Validation.Length.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-006: Validation.Pattern should be child of Validation")]
        public void ECS006()
        {
            // act & assert
            ErrorCodes.Validation.Pattern.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-007: Validation.Duplicate should be child of Validation")]
        public void ECS007()
        {
            // act & assert
            ErrorCodes.Validation.Duplicate.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-008: Validation.Invalid should be child of Validation")]
        public void ECS008()
        {
            // act & assert
            ErrorCodes.Validation.Invalid.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-009: NotFound root code should have correct value")]
        public void ECS009()
        {
            // act & assert
            ErrorCodes.NotFound._.Value.Should().Be("NotFound");
        }

        [Fact(DisplayName = "ECS-010: NotFound.Entity should be child of NotFound")]
        public void ECS010()
        {
            // act & assert
            ErrorCodes.NotFound.Entity.IsChildOf(ErrorCodes.NotFound._).Should().BeTrue();
            ErrorCodes.NotFound.Entity.Value.Should().Be("NotFound.Entity");
        }

        [Fact(DisplayName = "ECS-011: NotFound.File should be child of NotFound")]
        public void ECS011()
        {
            // act & assert
            ErrorCodes.NotFound.File.IsChildOf(ErrorCodes.NotFound._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-012: NotFound.Resource should be child of NotFound")]
        public void ECS012()
        {
            // act & assert
            ErrorCodes.NotFound.Resource.IsChildOf(ErrorCodes.NotFound._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-013: Auth root code should have correct value")]
        public void ECS013()
        {
            // act & assert
            ErrorCodes.Auth._.Value.Should().Be("Auth");
        }

        [Fact(DisplayName = "ECS-014: Auth.Unauthorized should be child of Auth")]
        public void ECS014()
        {
            // act & assert
            ErrorCodes.Auth.Unauthorized.IsChildOf(ErrorCodes.Auth._).Should().BeTrue();
            ErrorCodes.Auth.Unauthorized.Value.Should().Be("Auth.Unauthorized");
        }

        [Fact(DisplayName = "ECS-015: Auth.Forbidden should be child of Auth")]
        public void ECS015()
        {
            // act & assert
            ErrorCodes.Auth.Forbidden.IsChildOf(ErrorCodes.Auth._).Should().BeTrue();
            ErrorCodes.Auth.Forbidden.Value.Should().Be("Auth.Forbidden");
        }

        [Fact(DisplayName = "ECS-016: Auth.TokenExpired should be child of Auth")]
        public void ECS016()
        {
            // act & assert
            ErrorCodes.Auth.TokenExpired.IsChildOf(ErrorCodes.Auth._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-017: Auth.TokenInvalid should be child of Auth")]
        public void ECS017()
        {
            // act & assert
            ErrorCodes.Auth.TokenInvalid.IsChildOf(ErrorCodes.Auth._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-018: IO root code should have correct value")]
        public void ECS018()
        {
            // act & assert
            ErrorCodes.IO._.Value.Should().Be("IO");
        }

        [Fact(DisplayName = "ECS-019: IO.Network should be child of IO")]
        public void ECS019()
        {
            // act & assert
            ErrorCodes.IO.Network.IsChildOf(ErrorCodes.IO._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-020: IO.Timeout should be child of IO")]
        public void ECS020()
        {
            // act & assert
            ErrorCodes.IO.Timeout.IsChildOf(ErrorCodes.IO._).Should().BeTrue();
            ErrorCodes.IO.Timeout.Value.Should().Be("IO.Timeout");
        }

        [Fact(DisplayName = "ECS-021: IO.FileSystem should be child of IO")]
        public void ECS021()
        {
            // act & assert
            ErrorCodes.IO.FileSystem.IsChildOf(ErrorCodes.IO._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-022: IO.Database should be child of IO")]
        public void ECS022()
        {
            // act & assert
            ErrorCodes.IO.Database.IsChildOf(ErrorCodes.IO._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-023: IO.ExternalService should be child of IO")]
        public void ECS023()
        {
            // act & assert
            ErrorCodes.IO.ExternalService.IsChildOf(ErrorCodes.IO._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-024: Internal root code should have correct value")]
        public void ECS024()
        {
            // act & assert
            ErrorCodes.Internal._.Value.Should().Be("Internal");
        }

        [Fact(DisplayName = "ECS-025: Internal.Unexpected should be child of Internal")]
        public void ECS025()
        {
            // act & assert
            ErrorCodes.Internal.Unexpected.IsChildOf(ErrorCodes.Internal._).Should().BeTrue();
            ErrorCodes.Internal.Unexpected.Value.Should().Be("Internal.Unexpected");
        }

        [Fact(DisplayName = "ECS-026: Internal.Configuration should be child of Internal")]
        public void ECS026()
        {
            // act & assert
            ErrorCodes.Internal.Configuration.IsChildOf(ErrorCodes.Internal._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-027: Internal.Assertion should be child of Internal")]
        public void ECS027()
        {
            // act & assert
            ErrorCodes.Internal.Assertion.IsChildOf(ErrorCodes.Internal._).Should().BeTrue();
        }

        [Fact(DisplayName = "ECS-028: Validation codes should not be children of other categories")]
        public void ECS028()
        {
            // act & assert
            ErrorCodes.Validation.Required.IsChildOf(ErrorCodes.NotFound._).Should().BeFalse();
            ErrorCodes.Validation.Required.IsChildOf(ErrorCodes.Auth._).Should().BeFalse();
            ErrorCodes.Validation.Required.IsChildOf(ErrorCodes.IO._).Should().BeFalse();
            ErrorCodes.Validation.Required.IsChildOf(ErrorCodes.Internal._).Should().BeFalse();
        }

        [Fact(DisplayName = "ECS-029: Error codes can be extended with custom subcodes")]
        public void ECS029()
        {
            // arrange
            var customCode = ErrorCodes.Validation.Required / "Email";

            // act & assert
            customCode.Value.Should().Be("Validation.Required.Email");
            customCode.IsChildOf(ErrorCodes.Validation.Required).Should().BeTrue();
            customCode.IsChildOf(ErrorCodes.Validation._).Should().BeTrue();
        }
    }
}
