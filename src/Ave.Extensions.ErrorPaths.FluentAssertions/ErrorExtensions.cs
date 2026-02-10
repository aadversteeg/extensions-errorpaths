namespace Ave.Extensions.ErrorPaths.FluentAssertions
{
    public static class ErrorExtensions
    {
        public static ErrorAssertions Should(this Error instance) => new ErrorAssertions(instance);
    }
}
