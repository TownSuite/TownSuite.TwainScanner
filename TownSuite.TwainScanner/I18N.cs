using System;
using System.Globalization;
using System.Resources;

namespace TownSuite.TwainScanner
{
    internal static class I18N
    {
        // Cached ResourceManager — creating one per call was needlessly expensive
        private static readonly ResourceManager _rm =
            new ResourceManager("TownSuite.TwainScanner.Resources.Resources",
                                typeof(I18N).Assembly);

        // Resolved once at startup so every lookup is O(1)
        private static readonly CultureInfo _culture = ResolveCulture();

        private static CultureInfo ResolveCulture()
        {
            var current = CultureInfo.CurrentUICulture;
            // Walk up the hierarchy (e.g. fr-CA → fr → invariant)
            // until we find a culture that has at least one translated string.
            while (!current.Equals(CultureInfo.InvariantCulture))
            {
                try
                {
                    var probe = _rm.GetString("Scan", current);
                    if (!string.IsNullOrEmpty(probe))
                        return current;
                }
                catch { /* satellite assembly not present for this culture */ }

                current = current.Parent;
            }
            return CultureInfo.InvariantCulture; // neutral → English
        }

        /// <summary>Look up a localised string by key.</summary>
        public static string GetString(string key)
        {
            try
            {
                return _rm.GetString(key, _culture) ?? key;
            }
            catch
            {
                return key; // never crash — return the key itself as a last resort
            }
        }

        /// <summary>Look up a localised format string and apply <paramref name="args"/>.</summary>
        public static string GetString(string key, params object[] args)
        {
            string fmt = GetString(key);
            try { return string.Format(fmt, args); }
            catch { return fmt; }
        }
    }
}
