using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPD.Compiler.Lexical
{
    public static class CharExtensions
    {
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
