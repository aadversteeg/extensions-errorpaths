using System.Text.Json;

namespace Ave.Extensions.ErrorPaths.Serialization
{
    /// <summary>
    /// Provides configuration methods for ErrorPaths JSON serialization.
    /// </summary>
    public static class ErrorPathsJsonOptions
    {
        /// <summary>
        /// Configures the specified <see cref="JsonSerializerOptions"/> to support ErrorPaths types.
        /// Registers converters for <see cref="ErrorCode"/> and <see cref="Error"/>.
        /// </summary>
        /// <param name="options">The options to configure. If null, a new instance is created and returned.</param>
        /// <returns>The configured options instance.</returns>
        public static JsonSerializerOptions Configure(JsonSerializerOptions? options = null)
        {
            options = options ?? new JsonSerializerOptions();

            options.Converters.Add(new ErrorCodeJsonConverter());
            options.Converters.Add(new ErrorJsonConverter());

            return options;
        }

        /// <summary>
        /// Creates a new <see cref="JsonSerializerOptions"/> instance configured for ErrorPaths serialization
        /// with common defaults (camelCase property naming, indented writing).
        /// </summary>
        /// <returns>A new configured options instance.</returns>
        public static JsonSerializerOptions CreateDefault()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return Configure(options);
        }
    }
}
