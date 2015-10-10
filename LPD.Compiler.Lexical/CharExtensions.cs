using System.Globalization;
using System.Linq;
using System.Text;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Helper class for the <see cref="System.Char"/> class.
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// Finds for a diacritic in the specified <see cref="System.Char"/>.
        /// </summary>
        /// <param name="c">The character to look diacritic for.</param>
        /// <returns>true fi the specified character has a diacritic; false otherwise.</returns>
        public static bool HasDiacritic(this char c)
        {
            var normalized = c.ToString().Normalize(NormalizationForm.FormD);

            if (normalized.Length == 1)
            {
                return false;
            }

            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            var possibleDiacritics = normalized.Substring(1);

            return char.IsLetter(normalized[0]) &&
                   possibleDiacritics.All(ch => CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark);
        }
    }
}
