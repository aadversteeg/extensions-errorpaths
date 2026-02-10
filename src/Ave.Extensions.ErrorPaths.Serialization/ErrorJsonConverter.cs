using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ave.Extensions.ErrorPaths.Serialization
{
    /// <summary>
    /// JSON converter for <see cref="Error"/> that serializes as an object with code, message, metadata, and inner properties.
    /// </summary>
    public class ErrorJsonConverter : JsonConverter<Error>
    {
        private const string CodePropertyName = "code";
        private const string MessagePropertyName = "message";
        private const string MetadataPropertyName = "metadata";
        private const string InnerPropertyName = "inner";

        /// <summary>
        /// Reads an <see cref="Error"/> from JSON.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>The deserialized Error.</returns>
        /// <exception cref="JsonException">Thrown when the JSON structure is invalid.</exception>
        public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected start of object for Error, but got {reader.TokenType}.");
            }

            string? code = null;
            string? message = null;
            Dictionary<string, object>? metadata = null;
            Error? inner = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"Expected property name, but got {reader.TokenType}.");
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName?.ToLowerInvariant())
                {
                    case CodePropertyName:
                        code = reader.GetString();
                        break;

                    case MessagePropertyName:
                        message = reader.GetString();
                        break;

                    case MetadataPropertyName:
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            metadata = ReadMetadata(ref reader, options);
                        }
                        break;

                    case InnerPropertyName:
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            inner = Read(ref reader, typeToConvert, options);
                        }
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }

            if (string.IsNullOrEmpty(code) || message == null)
            {
                return default;
            }

            return new Error(
                new ErrorCode(code),
                message,
                metadata,
                inner);
        }

        /// <summary>
        /// Writes an <see cref="Error"/> to JSON.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The Error to serialize.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value.Code.Value) && string.IsNullOrEmpty(value.Message))
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            writer.WriteString(CodePropertyName, value.Code.Value);
            writer.WriteString(MessagePropertyName, value.Message);

            if (value.Metadata != null)
            {
                writer.WritePropertyName(MetadataPropertyName);
                WriteMetadata(writer, value.Metadata, options);
            }

            if (value.Inner.HasValue)
            {
                writer.WritePropertyName(InnerPropertyName);
                Write(writer, value.Inner.Value, options);
            }

            writer.WriteEndObject();
        }

        private static Dictionary<string, object>? ReadMetadata(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return null;
            }

            var metadata = new Dictionary<string, object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"Expected property name in metadata, but got {reader.TokenType}.");
                }

                var key = reader.GetString()!;
                reader.Read();

                var value = ReadValue(ref reader);
                metadata[key] = value;
            }

            return metadata;
        }

        private static object ReadValue(ref Utf8JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString()!;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var longValue))
                    {
                        return longValue;
                    }
                    return reader.GetDouble();

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.Null:
                    return null!;

                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ReadValue(ref reader));
                    }
                    return list;

                case JsonTokenType.StartObject:
                    var dict = new Dictionary<string, object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        var key = reader.GetString()!;
                        reader.Read();
                        dict[key] = ReadValue(ref reader);
                    }
                    return dict;

                default:
                    throw new JsonException($"Unexpected token type: {reader.TokenType}");
            }
        }

        private static void WriteMetadata(Utf8JsonWriter writer, IReadOnlyDictionary<string, object> metadata, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in metadata)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }

        private static void WriteValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            switch (value)
            {
                case string s:
                    writer.WriteStringValue(s);
                    break;

                case bool b:
                    writer.WriteBooleanValue(b);
                    break;

                case int i:
                    writer.WriteNumberValue(i);
                    break;

                case long l:
                    writer.WriteNumberValue(l);
                    break;

                case double d:
                    writer.WriteNumberValue(d);
                    break;

                case decimal dec:
                    writer.WriteNumberValue(dec);
                    break;

                case float f:
                    writer.WriteNumberValue(f);
                    break;

                case IReadOnlyDictionary<string, object> dict:
                    WriteMetadata(writer, dict, options);
                    break;

                case IDictionary<string, object> dict:
                    writer.WriteStartObject();
                    foreach (var kvp in dict)
                    {
                        writer.WritePropertyName(kvp.Key);
                        WriteValue(writer, kvp.Value, options);
                    }
                    writer.WriteEndObject();
                    break;

                case IEnumerable<object> enumerable:
                    writer.WriteStartArray();
                    foreach (var item in enumerable)
                    {
                        WriteValue(writer, item, options);
                    }
                    writer.WriteEndArray();
                    break;

                default:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }
}
