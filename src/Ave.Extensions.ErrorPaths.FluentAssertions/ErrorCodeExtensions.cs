namespace Ave.Extensions.ErrorPaths.FluentAssertions
{
    public static class ErrorCodeExtensions
    {
        public static ErrorCodeAssertions Should(this ErrorCode instance) => new ErrorCodeAssertions(instance);
    }
}
