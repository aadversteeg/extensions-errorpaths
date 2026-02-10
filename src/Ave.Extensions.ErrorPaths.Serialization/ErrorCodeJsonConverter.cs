using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ave.Extensions.ErrorPaths.Serialization
{
    /// <summary>
    /// JSON converter for <see cref="ErrorCode"/> that serializes as a plain string.
    /// </summary>
    public class ErrorCodeJsonConverter : JsonConverter<ErrorCode>
    {
        /// <summary>
        /// Reads an <see cref="ErrorCode"/> from JSON.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>The deserialized ErrorCode.</returns>
        /// <exception cref="JsonException">Thrown when the JSON value is not a string.</exception>
        public override ErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string token for ErrorCode, but got {reader.TokenType}.");
            }

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            return new ErrorCode(value);
        }

        /// <summary>
        /// Writes an <see cref="ErrorCode"/> to JSON.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The ErrorCode to serialize.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, ErrorCode value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value.Value))
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Value);
            }
        }
    }
}
