public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string s)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        return string.IsNullOrEmpty(s);
    }

    public static bool IsNullOrWhitespace(this string s)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        return string.IsNullOrWhiteSpace(s);
    }
}