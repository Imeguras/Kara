namespace InstanceManager.Utility
{

    /// <summary>Contains functions for working with strings.</summary>
    static class StringUtility
    {

        /// <summary>Adds quotes around the string, where needed.</summary>
        public static string WithQuotes(this string str, string quoteChar = @"""")
        {
            if (!str.StartsWith(quoteChar)) str = quoteChar + str;
            if (!str.EndsWith(quoteChar)) str += quoteChar;
            return str;
        }

        /// <summary>Replaces all instances of '\' in the string to '/'.</summary>
        public static string ToCrossPlatformPath(this string str) =>
            str.Replace('\\', '/');

        /// <summary>Replaces all instances of '/' in the string to '\'.</summary>
        public static string ToWindowsPath(this string str) =>
            str.Replace('/', '\\');

    }

}
