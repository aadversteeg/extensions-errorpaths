using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ave.Extensions.ErrorPaths
{
    /// <summary>
    /// Represents an error with a hierarchical code, message, optional metadata, and optional inner error.
    /// </summary>
    [DebuggerDisplay("[{Code}] {Message}")]
    public readonly struct Error : IEquatable<Error>
    {
        private readonly ErrorCode _code;
        private readonly string _message;
        private readonly IReadOnlyDictionary<string, object>? _metadata;
        // Inner error is stored as a boxed object because a readonly struct cannot contain
        // a field of its own type (even as Nullable<T>). This incurs a small boxing/unboxing
        // cost on each access to the Inner property, which is an acceptable trade-off for
        // enabling error chaining on a value type.
        private readonly object? _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> struct.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
        public Error(ErrorCode code, string message)
            : this(code, message, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> struct with metadata.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message. Must not be null.</param>
        /// <param name="metadata">Optional metadata dictionary.</param>
        /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
        public Error(ErrorCode code, string message, IReadOnlyDictionary<string, object>? metadata)
            : this(code, message, metadata, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> struct with metadata and inner error.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message. Must not be null.</param>
        /// <param name="metadata">Optional metadata dictionary.</param>
        /// <param name="inner">Optional inner error for chaining.</param>
        /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
        public Error(ErrorCode code, string message, IReadOnlyDictionary<string, object>? metadata, Error? inner)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _code = code;
            _message = message;
            _metadata = metadata;
            _inner = inner.HasValue ? (object)inner.Value : null;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public ErrorCode Code => _code;

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message => _message ?? string.Empty;

        /// <summary>
        /// Gets the optional metadata dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, object>? Metadata => _metadata;

        /// <summary>
        /// Gets the optional inner error.
        /// </summary>
        public Error? Inner => _inner != null ? (Error)_inner : (Error?)null;

        /// <summary>
        /// Determines whether this error's code is a child of (or equal to) the specified ancestor code.
        /// </summary>
        /// <param name="ancestor">The potential ancestor error code.</param>
        /// <returns>true if this error's code matches the ancestor hierarchy; otherwise, false.</returns>
        public bool Is(ErrorCode ancestor)
        {
            return _code.IsChildOf(ancestor);
        }

        /// <summary>
        /// Creates a new error that wraps this error with additional context.
        /// </summary>
        /// <param name="code">The wrapper error code.</param>
        /// <param name="message">The wrapper error message.</param>
        /// <returns>A new Error with this error as its inner error.</returns>
        public Error Wrap(ErrorCode code, string message)
        {
            return new Error(code, message, null, this);
        }

        /// <summary>
        /// Creates a new error with an additional metadata entry.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>A new Error with the additional metadata.</returns>
        /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
        public Error With(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var newMetadata = new Dictionary<string, object>();
            if (_metadata != null)
            {
                foreach (var kvp in _metadata)
                {
                    newMetadata[kvp.Key] = kvp.Value;
                }
            }
            newMetadata[key] = value;

            return new Error(_code, _message, newMetadata, Inner);
        }

        /// <summary>
        /// Returns a string representation of this error.
        /// </summary>
        /// <returns>A formatted string containing the error code and message.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            sb.Append(_code.Value);
            sb.Append("] ");
            sb.Append(_message ?? string.Empty);

            if (_inner != null)
            {
                sb.Append(" ---> ");
                sb.Append(((Error)_inner).ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code value that represents this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + _code.GetHashCode();
                hash = hash * 23 + (_message?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Determines whether this instance equals another Error instance.
        /// Equality is based on <see cref="Code"/> and <see cref="Message"/> only.
        /// <see cref="Metadata"/> and <see cref="Inner"/> are intentionally excluded because
        /// errors are primarily identified by their code and message, and metadata is supplemental context.
        /// </summary>
        /// <param name="other">The Error to compare with.</param>
        /// <returns>true if the instances have the same code and message; otherwise, false.</returns>
        public bool Equals(Error other)
        {
            return _code.Equals(other._code)
                && string.Equals(_message, other._message, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether this instance equals another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if the object is an Error and equals this instance; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is Error other && Equals(other);
        }

        /// <summary>
        /// Determines whether two Error instances are equal.
        /// </summary>
        /// <param name="left">The first Error to compare.</param>
        /// <param name="right">The second Error to compare.</param>
        /// <returns>true if the Error instances are equal; otherwise, false.</returns>
        public static bool operator ==(Error left, Error right) => left.Equals(right);

        /// <summary>
        /// Determines whether two Error instances are not equal.
        /// </summary>
        /// <param name="left">The first Error to compare.</param>
        /// <param name="right">The second Error to compare.</param>
        /// <returns>true if the Error instances are not equal; otherwise, false.</returns>
        public static bool operator !=(Error left, Error right) => !left.Equals(right);
    }
}
