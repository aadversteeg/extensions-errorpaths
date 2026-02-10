using Ave.Extensions.ErrorPaths;
using FluentAssertions;
using System;
using Xunit;

namespace UnitTests.Extensions.ErrorPaths
{
    public class ErrorCodeTests
    {
        [Fact(DisplayName = "EC-001: Constructor with valid value should create ErrorCode")]
        public void EC001()
        {
            // arrange
            var value = "Validation";

            // act
            var errorCode = new ErrorCode(value);

            // assert
            errorCode.Value.Should().Be("Validation");
        }

        [Fact(DisplayName = "EC-002: Constructor with null value should throw ArgumentNullException")]
        public void EC002()
        {
            // arrange

            // act
            var act = () => new ErrorCode(null!);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("value");
        }

        [Fact(DisplayName = "EC-003: Constructor with empty value should throw ArgumentException")]
        public void EC003()
        {
            // arrange

            // act
            var act = () => new ErrorCode("");

            // assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("value");
        }

        [Fact(DisplayName = "EC-004: Constructor with whitespace value should throw ArgumentException")]
        public void EC004()
        {
            // arrange

            // act
            var act = () => new ErrorCode("   ");

            // assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("value");
        }

        [Fact(DisplayName = "EC-005: Division operator should combine codes with separator")]
        public void EC005()
        {
            // arrange
            var parent = new ErrorCode("Validation");

            // act
            var child = parent / "Required";

            // assert
            child.Value.Should().Be("Validation.Required");
        }

        [Fact(DisplayName = "EC-006: Division operator with multiple levels should build full path")]
        public void EC006()
        {
            // arrange
            var root = new ErrorCode("App");

            // act
            var code = root / "Validation" / "Required" / "Email";

            // assert
            code.Value.Should().Be("App.Validation.Required.Email");
        }

        [Fact(DisplayName = "EC-007: Division operator with null child should throw ArgumentNullException")]
        public void EC007()
        {
            // arrange
            var parent = new ErrorCode("Validation");

            // act
            var act = () => parent / null!;

            // assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("child");
        }

        [Fact(DisplayName = "EC-008: Division operator with empty child should throw ArgumentException")]
        public void EC008()
        {
            // arrange
            var parent = new ErrorCode("Validation");

            // act
            var act = () => parent / "";

            // assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("child");
        }

        [Fact(DisplayName = "EC-009: IsChildOf should return true for exact match")]
        public void EC009()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var ancestor = new ErrorCode("Validation.Required");

            // act
            var result = code.IsChildOf(ancestor);

            // assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "EC-010: IsChildOf should return true for direct child")]
        public void EC010()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var ancestor = new ErrorCode("Validation");

            // act
            var result = code.IsChildOf(ancestor);

            // assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "EC-011: IsChildOf should return true for nested descendant")]
        public void EC011()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");
            var ancestor = new ErrorCode("Validation");

            // act
            var result = code.IsChildOf(ancestor);

            // assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "EC-012: IsChildOf should return false for non-ancestor")]
        public void EC012()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");
            var notAncestor = new ErrorCode("NotFound");

            // act
            var result = code.IsChildOf(notAncestor);

            // assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "EC-013: IsChildOf should return false for partial prefix match")]
        public void EC013()
        {
            // arrange
            var code = new ErrorCode("ValidationError");
            var notAncestor = new ErrorCode("Validation");

            // act
            var result = code.IsChildOf(notAncestor);

            // assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "EC-014: Parent should return parent code for nested code")]
        public void EC014()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            var parent = code.Parent;

            // assert
            parent.Should().NotBeNull();
            parent!.Value.Value.Should().Be("Validation");
        }

        [Fact(DisplayName = "EC-015: Parent should return null for root code")]
        public void EC015()
        {
            // arrange
            var code = new ErrorCode("Validation");

            // act
            var parent = code.Parent;

            // assert
            parent.Should().BeNull();
        }

        [Fact(DisplayName = "EC-016: Depth should return 1 for root code")]
        public void EC016()
        {
            // arrange
            var code = new ErrorCode("Validation");

            // act
            var depth = code.Depth;

            // assert
            depth.Should().Be(1);
        }

        [Fact(DisplayName = "EC-017: Depth should return correct count for nested code")]
        public void EC017()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");

            // act
            var depth = code.Depth;

            // assert
            depth.Should().Be(3);
        }

        [Fact(DisplayName = "EC-018: Leaf should return last segment")]
        public void EC018()
        {
            // arrange
            var code = new ErrorCode("Validation.Required.Email");

            // act
            var leaf = code.Leaf;

            // assert
            leaf.Should().Be("Email");
        }

        [Fact(DisplayName = "EC-019: Leaf should return full value for root code")]
        public void EC019()
        {
            // arrange
            var code = new ErrorCode("Validation");

            // act
            var leaf = code.Leaf;

            // assert
            leaf.Should().Be("Validation");
        }

        [Fact(DisplayName = "EC-020: Equal ErrorCodes should be equal")]
        public void EC020()
        {
            // arrange
            var code1 = new ErrorCode("Validation.Required");
            var code2 = new ErrorCode("Validation.Required");

            // act & assert
            code1.Should().Be(code2);
            (code1 == code2).Should().BeTrue();
            (code1 != code2).Should().BeFalse();
            code1.Equals(code2).Should().BeTrue();
            code1.Equals((object)code2).Should().BeTrue();
        }

        [Fact(DisplayName = "EC-021: Different ErrorCodes should not be equal")]
        public void EC021()
        {
            // arrange
            var code1 = new ErrorCode("Validation.Required");
            var code2 = new ErrorCode("Validation.Format");

            // act & assert
            code1.Should().NotBe(code2);
            (code1 == code2).Should().BeFalse();
            (code1 != code2).Should().BeTrue();
        }

        [Fact(DisplayName = "EC-022: Equal ErrorCodes should have same hash code")]
        public void EC022()
        {
            // arrange
            var code1 = new ErrorCode("Validation.Required");
            var code2 = new ErrorCode("Validation.Required");

            // act & assert
            code1.GetHashCode().Should().Be(code2.GetHashCode());
        }

        [Fact(DisplayName = "EC-023: CompareTo should return 0 for equal codes")]
        public void EC023()
        {
            // arrange
            var code1 = new ErrorCode("Validation");
            var code2 = new ErrorCode("Validation");

            // act
            var result = code1.CompareTo(code2);

            // assert
            result.Should().Be(0);
        }

        [Fact(DisplayName = "EC-024: CompareTo should return negative for lesser code")]
        public void EC024()
        {
            // arrange
            var code1 = new ErrorCode("Auth");
            var code2 = new ErrorCode("Validation");

            // act
            var result = code1.CompareTo(code2);

            // assert
            result.Should().BeNegative();
        }

        [Fact(DisplayName = "EC-025: CompareTo should return positive for greater code")]
        public void EC025()
        {
            // arrange
            var code1 = new ErrorCode("Validation");
            var code2 = new ErrorCode("Auth");

            // act
            var result = code1.CompareTo(code2);

            // assert
            result.Should().BePositive();
        }

        [Fact(DisplayName = "EC-026: Comparison operators should work correctly")]
        public void EC026()
        {
            // arrange
            var code1 = new ErrorCode("Auth");
            var code2 = new ErrorCode("Validation");
            var code3 = new ErrorCode("Auth");

            // act & assert
            (code1 < code2).Should().BeTrue();
            (code1 <= code2).Should().BeTrue();
            (code1 <= code3).Should().BeTrue();
            (code2 > code1).Should().BeTrue();
            (code2 >= code1).Should().BeTrue();
            (code3 >= code1).Should().BeTrue();
        }

        [Fact(DisplayName = "EC-027: Implicit conversion to string should return value")]
        public void EC027()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            string value = code;

            // assert
            value.Should().Be("Validation.Required");
        }

        [Fact(DisplayName = "EC-028: ToString should return value")]
        public void EC028()
        {
            // arrange
            var code = new ErrorCode("Validation.Required");

            // act
            var result = code.ToString();

            // assert
            result.Should().Be("Validation.Required");
        }

        [Fact(DisplayName = "EC-029: Default ErrorCode should have empty value")]
        public void EC029()
        {
            // arrange
            var code = default(ErrorCode);

            // act
            var value = code.Value;

            // assert
            value.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "EC-030: Separator constant should be dot")]
        public void EC030()
        {
            // act & assert
            ErrorCode.Separator.Should().Be('.');
        }

        [Fact(DisplayName = "EC-031: Default ErrorCode Depth should return 0")]
        public void EC031()
        {
            // arrange
            var code = default(ErrorCode);

            // act
            var depth = code.Depth;

            // assert
            depth.Should().Be(0);
        }

        [Fact(DisplayName = "EC-032: Default ErrorCode Leaf should return empty string")]
        public void EC032()
        {
            // arrange
            var code = default(ErrorCode);

            // act
            var leaf = code.Leaf;

            // assert
            leaf.Should().Be(string.Empty);
        }

        [Fact(DisplayName = "EC-033: Division operator with child containing separator should throw ArgumentException")]
        public void EC033()
        {
            // arrange
            var parent = new ErrorCode("Validation");

            // act
            var act = () => parent / "Required.Email";

            // assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("child");
        }
    }
}
