# Ave.Extensions.ErrorPaths

A NuGet library providing hierarchical, path-based error codes as value types for .NET.

## Why This Library?

### The Problem

When building applications with multiple layers or libraries, error handling becomes fragmented:

- **String-based errors** are flexible but impossible to match programmatically. Is `"User not found"` the same error as `"user_not_found"` or `"UserNotFound"`?
- **Enum-based errors** are type-safe but don't compose across library boundaries. Your `UserService.Error.NotFound` can't be matched against `OrderService.Error.NotFound` without manual mapping.
- **Exception hierarchies** require try-catch blocks and don't work well with Result types for explicit error handling.

### The Solution

Error codes as **hierarchical paths** that work like file paths:

```
Validation.Required.Email
NotFound.Entity.User
IO.Database.Connection
```

This gives you:

- **Hierarchical matching**: Catch all validation errors with `Validation._`, or be specific with `Validation.Required.Email`
- **Cross-library compatibility**: Any library can define `Validation.Required.CustomField` and it automatically matches `Validation._`
- **No enum mapping**: Libraries don't need to know about each other's error types
- **Composability**: Extend existing codes with the `/` operator: `ErrorCodes.Validation.Required / "Email"`

### Key Advantages

| Approach | Type-Safe | Composable | Cross-Library | Hierarchical |
|----------|-----------|------------|---------------|--------------|
| Strings | No | Yes | Yes | No |
| Enums | Yes | No | No | No |
| Exceptions | Yes | Yes | Partial | Yes |
| **ErrorPaths** | **Yes** | **Yes** | **Yes** | **Yes** |

### Real-World Example

```csharp
// In your domain layer
Result<User, Error> GetUser(int id) =>
    Result<User, Error>.Failure(Errors.NotFound("User", id));

// In your API layer - match ANY not-found error, regardless of source
if (result.HasError(ErrorCodes.NotFound._))
    return Results.NotFound();

// Or be specific
if (result.HasError(ErrorCodes.NotFound.Entity))
    return Results.NotFound();
```

Your API layer doesn't need to know whether the error came from `UserService`, `OrderService`, or a third-party library. If it's a `NotFound` error, it matches.

## Packages

| Package | Description | Target |
|---------|-------------|--------|
| `Ave.Extensions.ErrorPaths` | Core library with ErrorCode, Error, and factory methods | netstandard2.0 |
| `Ave.Extensions.ErrorPaths.Functional` | Integration with Ave.Extensions.Functional Result types | netstandard2.0 |
| `Ave.Extensions.ErrorPaths.Serialization` | JSON serialization with System.Text.Json | netstandard2.0 |
| `Ave.Extensions.ErrorPaths.AspNetCore` | ASP.NET Core integration (ProblemDetails, HTTP status) | net10.0 |

## Installation

```bash
dotnet add package Ave.Extensions.ErrorPaths
```

## Quick Start

### Creating Errors

```csharp
using Ave.Extensions.ErrorPaths;

// Use well-known error codes
var error = new Error(ErrorCodes.Validation.Required, "Email is required.");

// Use factory methods
var notFound = Errors.NotFound("User", 42);
var validation = Errors.Required("email");
var timeout = Errors.Timeout("database query");

// Create custom error codes with the / operator
var customCode = ErrorCodes.Validation.Required / "Email" / "Format";
var customError = new Error(customCode, "Invalid email format.");
```

### Error Code Hierarchy

Error codes form a hierarchy using dot notation. You can match errors against ancestor codes:

```csharp
var error = new Error(ErrorCodes.Validation.Required, "Field required.");

// Check if error matches a category
error.Is(ErrorCodes.Validation._);        // true
error.Is(ErrorCodes.Validation.Required); // true
error.Is(ErrorCodes.NotFound._);          // false
```

### Adding Metadata

```csharp
var error = Errors.Required("email")
    .With("maxLength", 100)
    .With("allowedDomains", new[] { "example.com" });

// Access metadata
var field = error.Metadata["field"]; // "email"
```

### Error Chaining

```csharp
var innerError = Errors.Network("Connection refused.");
var outerError = innerError.Wrap(ErrorCodes.IO.Database, "Failed to connect to database.");

// Access the chain
outerError.Inner.Value.Message; // "Connection refused."
```

## Integration with Ave.Extensions.Functional

```csharp
using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Functional;
using Ave.Extensions.Functional;

Result<User, Error> GetUser(int id)
{
    // Return errors using the Error type
    return Result<User, Error>.Failure(Errors.NotFound("User", id));
}

// Check for specific error types
var result = GetUser(42);
if (result.HasError(ErrorCodes.NotFound._))
{
    // Handle not found
}

// Transform errors
var wrapped = result.WrapError(ErrorCodes.Internal._, "User lookup failed.");

// Map errors
var mapped = result.MapError(e => e.With("timestamp", DateTime.UtcNow));
```

## JSON Serialization

```csharp
using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.Serialization;
using System.Text.Json;

var options = ErrorPathsJsonOptions.Configure();
// or with defaults (camelCase, indented)
var options = ErrorPathsJsonOptions.CreateDefault();

var error = Errors.Required("email");
var json = JsonSerializer.Serialize(error, options);
// {"code":"Validation.Required","message":"The field 'email' is required.","metadata":{"field":"email"}}

var deserialized = JsonSerializer.Deserialize<Error>(json, options);
```

## ASP.NET Core Integration

```csharp
using Ave.Extensions.ErrorPaths;
using Ave.Extensions.ErrorPaths.AspNetCore;

app.MapGet("/users/{id}", (int id) =>
{
    var result = GetUser(id);
    if (result.IsFailure)
    {
        return result.Error.ToHttpResult();
    }
    return Results.Ok(result.Value);
});
```

### HTTP Status Code Mapping

Default mappings:

| Error Code | HTTP Status |
|------------|-------------|
| `Validation.*` | 400 Bad Request |
| `NotFound.*` | 404 Not Found |
| `Auth.Unauthorized` | 401 Unauthorized |
| `Auth.Forbidden` | 403 Forbidden |
| `Auth.TokenExpired` | 401 Unauthorized |
| `IO.Timeout` | 504 Gateway Timeout |
| `IO.*` | 502 Bad Gateway |
| `Internal.*` | 500 Internal Server Error |

Custom mappings:

```csharp
ErrorCodeHttpMapping.Register(ErrorCodes.Validation._, HttpStatusCode.UnprocessableEntity);
```

### ProblemDetails

```csharp
var error = Errors.NotFound("User", 42);
var problemDetails = error.ToProblemDetails();

// Result:
// {
//   "type": "urn:error:NotFound.Entity",
//   "title": "Not Found",
//   "status": 404,
//   "detail": "User with id '42' was not found.",
//   "entity": "User",
//   "id": 42
// }
```

## Well-Known Error Codes

```csharp
ErrorCodes.Validation._           // Validation
ErrorCodes.Validation.Required    // Validation.Required
ErrorCodes.Validation.Format      // Validation.Format
ErrorCodes.Validation.Range       // Validation.Range
ErrorCodes.Validation.Length      // Validation.Length
ErrorCodes.Validation.Pattern     // Validation.Pattern
ErrorCodes.Validation.Duplicate   // Validation.Duplicate
ErrorCodes.Validation.Invalid     // Validation.Invalid

ErrorCodes.NotFound._             // NotFound
ErrorCodes.NotFound.Entity        // NotFound.Entity
ErrorCodes.NotFound.File          // NotFound.File
ErrorCodes.NotFound.Resource      // NotFound.Resource

ErrorCodes.Auth._                 // Auth
ErrorCodes.Auth.Unauthorized      // Auth.Unauthorized
ErrorCodes.Auth.Forbidden         // Auth.Forbidden
ErrorCodes.Auth.TokenExpired      // Auth.TokenExpired
ErrorCodes.Auth.TokenInvalid      // Auth.TokenInvalid

ErrorCodes.IO._                   // IO
ErrorCodes.IO.Network             // IO.Network
ErrorCodes.IO.Timeout             // IO.Timeout
ErrorCodes.IO.FileSystem          // IO.FileSystem
ErrorCodes.IO.Database            // IO.Database
ErrorCodes.IO.ExternalService     // IO.ExternalService

ErrorCodes.Internal._             // Internal
ErrorCodes.Internal.Unexpected    // Internal.Unexpected
ErrorCodes.Internal.Configuration // Internal.Configuration
ErrorCodes.Internal.Assertion     // Internal.Assertion
```

## Factory Methods

```csharp
Errors.Validation(message)           // Validation error
Errors.Required(field)               // Required field error with metadata
Errors.Format(field, expected)       // Format error with metadata
Errors.NotFound(message)             // Generic not found
Errors.NotFound(entity, id)          // Entity not found with metadata
Errors.Unauthorized(reason?)         // Authentication error
Errors.Forbidden(reason?)            // Authorization error
Errors.Timeout(operation)            // Timeout error with metadata
Errors.Network(message)              // Network error
Errors.Unexpected(message)           // Unexpected error
Errors.Unexpected(exception)         // Unexpected error from exception
Errors.From(code, message)           // Custom error code
```

## License

MIT License - see LICENSE file for details.
