public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

    public static bool IsNullOrWhitespace(this string s) => string.IsNullOrWhiteSpace(s);
}