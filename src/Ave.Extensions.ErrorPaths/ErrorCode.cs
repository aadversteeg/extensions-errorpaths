using System;

namespace Ave.Extensions.ErrorPaths
{
    /// <summary>
    /// Represents a hierarchical, path-based error code.
    /// Error codes compose with the / operator like file paths, enabling cross-library error matching.
    /// </summary>
    public readonly struct ErrorCode : IEquatable<ErrorCode>, IComparable<ErrorCode>
    {
        /// <summary>
        /// The separator used between segments in the error code path.
        /// </summary>
        public const char Separator = '.';

        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode"/> struct.
        /// </summary>
        /// <param name="value">The error code value. Must not be null or whitespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        /// <exception cref="ArgumentException">Thrown when value is empty or whitespace.</exception>
        public ErrorCode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Error code value cannot be empty or whitespace.", nameof(value));
            }

            _value = value;
        }

        /// <summary>
        /// Gets the string value of the error code.
        /// </summary>
        public string Value => _value ?? string.Empty;

        /// <summary>
        /// Gets the number of segments in the error code path.
        /// </summary>
        public int Depth
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    return 0;
                }

                var count = 1;
                for (var i = 0; i < _value.Length; i++)
                {
                    if (_value[i] == Separator)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Gets the last segment of the error code path.
        /// </summary>
        public string Leaf
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    return string.Empty;
                }

                var lastSeparatorIndex = _value.LastIndexOf(Separator);
                return lastSeparatorIndex < 0 ? _value : _value.Substring(lastSeparatorIndex + 1);
            }
        }

        /// <summary>
        /// Gets the parent error code, or null if this is a root-level code.
        /// </summary>
        public ErrorCode? Parent
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    return null;
                }

                var lastSeparatorIndex = _value.LastIndexOf(Separator);
                if (lastSeparatorIndex < 0)
                {
                    return null;
                }

                return new ErrorCode(_value.Substring(0, lastSeparatorIndex));
            }
        }

        /// <summary>
        /// Determines whether this error code is a child of (or equal to) the specified ancestor code.
        /// </summary>
        /// <param name="ancestor">The potential ancestor error code.</param>
        /// <returns>true if this code equals the ancestor or starts with the ancestor followed by the separator; otherwise, false.</returns>
        public bool IsChildOf(ErrorCode ancestor)
        {
            if (string.IsNullOrEmpty(_value) || string.IsNullOrEmpty(ancestor._value))
            {
                return false;
            }

            if (_value.Equals(ancestor._value, StringComparison.Ordinal))
            {
                return true;
            }

            return _value.StartsWith(ancestor._value + Separator, StringComparison.Ordinal);
        }

        /// <summary>
        /// Combines this error code with a child segment using the / operator.
        /// </summary>
        /// <param name="parent">The parent error code.</param>
        /// <param name="child">The child segment to append.</param>
        /// <returns>A new error code representing the combined path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when child is null.</exception>
        /// <exception cref="ArgumentException">Thrown when child is empty or whitespace.</exception>
        public static ErrorCode operator /(ErrorCode parent, string child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (string.IsNullOrWhiteSpace(child))
            {
                throw new ArgumentException("Child segment cannot be empty or whitespace.", nameof(child));
            }

            if (string.IsNullOrEmpty(parent._value))
            {
                return new ErrorCode(child);
            }

            return new ErrorCode(parent._value + Separator + child);
        }

        /// <summary>
        /// Implicitly converts an error code to its string representation.
        /// </summary>
        /// <param name="errorCode">The error code to convert.</param>
        public static implicit operator string(ErrorCode errorCode) => errorCode.Value;

        /// <summary>
        /// Returns the string representation of this error code.
        /// </summary>
        /// <returns>The error code value.</returns>
        public override string ToString() => Value;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code value that represents this instance.</returns>
        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Determines whether this instance equals another ErrorCode instance.
        /// </summary>
        /// <param name="other">The ErrorCode to compare with.</param>
        /// <returns>true if the instances are equal; otherwise, false.</returns>
        public bool Equals(ErrorCode other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether this instance equals another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if the object is an ErrorCode and equals this instance; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is ErrorCode other && Equals(other);
        }

        /// <summary>
        /// Compares this instance to another ErrorCode.
        /// </summary>
        /// <param name="other">The ErrorCode to compare with.</param>
        /// <returns>A value indicating the relative order of the instances.</returns>
        public int CompareTo(ErrorCode other)
        {
            return string.Compare(_value ?? string.Empty, other._value ?? string.Empty, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether two ErrorCode instances are equal.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if the ErrorCode instances are equal; otherwise, false.</returns>
        public static bool operator ==(ErrorCode left, ErrorCode right) => left.Equals(right);

        /// <summary>
        /// Determines whether two ErrorCode instances are not equal.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if the ErrorCode instances are not equal; otherwise, false.</returns>
        public static bool operator !=(ErrorCode left, ErrorCode right) => !left.Equals(right);

        /// <summary>
        /// Determines whether the left ErrorCode is less than the right ErrorCode.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(ErrorCode left, ErrorCode right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether the left ErrorCode is less than or equal to the right ErrorCode.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(ErrorCode left, ErrorCode right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether the left ErrorCode is greater than the right ErrorCode.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        public static bool operator >(ErrorCode left, ErrorCode right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether the left ErrorCode is greater than or equal to the right ErrorCode.
        /// </summary>
        /// <param name="left">The first ErrorCode to compare.</param>
        /// <param name="right">The second ErrorCode to compare.</param>
        /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(ErrorCode left, ErrorCode right) => left.CompareTo(right) >= 0;
    }
}
