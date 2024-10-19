using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Utility class for handling common string manipulation tasks.
    /// </summary>
    public static class StringUtils
    {

        private static string[] _namesOfUnits;
        private static string[] _namesOfTens;

        /// <summary>
        /// Returns the input string with all white-space characters removed.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>The input string, but with all white-space characters removed</returns>
        public static string RemoveWhiteSpace(string str)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                {
                    result.Append(str[i]);
                }
            }

            return result.ToString();
        }

        public static string ReplaceSmartCharacters(string str)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '\u2013') builder.Append('-'); // en dash
                else if (c == '\u2014') builder.Append('-'); // em dash
                else if (c == '\u2015') builder.Append('-'); // horizontal bar
                else if (c == '\u2017') builder.Append('_'); // double low line
                else if (c == '\u2018') builder.Append('\''); // left single quotation mark
                else if (c == '\u2019') builder.Append('\''); // right single quotation mark
                else if (c == '\u201a') builder.Append(','); // single low-9 quotation mark
                else if (c == '\u201b') builder.Append('\''); // single high-reversed-9 quotation mark
                else if (c == '\u201c') builder.Append('\"'); // left double quotation mark
                else if (c == '\u201d') builder.Append('\"'); // right double quotation mark
                else if (c == '\u201e') builder.Append('\"'); // double low-9 quotation mark
                else if (c == '\u2026') builder.Append("..."); // horizontal ellipsis
                else if (c == '\u2032') builder.Append('\''); // prime
                else if (c == '\u2033') builder.Append('\"'); // double prime
                else builder.Append(c);
            }

            return builder.ToString();
        }

        public static string GetParameterList(bool oneLine, params object[] values)
        {

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    if (oneLine)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                }

                if (values[i] == null)
                {
                    builder.Append("null");
                }
                else
                {
                    builder.Append(values[i]);
                }
            }

            return builder.ToString();
        }

        public static string GetNamedParameterList(bool oneLine, params object[] namesAndValues)
        {
            Assert.IsTrue(namesAndValues.Length % 2 == 0);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < namesAndValues.Length; i += 2)
            {
                if (i > 0)
                {
                    if (oneLine)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                }

                builder.Append(namesAndValues[i]);
                builder.Append(": ");

                if (namesAndValues[i + 1] == null)
                {
                    builder.Append("null");
                }
                else
                {
                    builder.Append(namesAndValues[i + 1]);
                }
            }

            return builder.ToString();
        }

        public static string GetLongestCommonPrefix(string[] strings)
        {
            for (int i = 0; i < strings[0].Length; i++)
            {
                for (int j = 1; j < strings.Length; j++)
                {
                    if (i == strings[j].Length || strings[j][i] != strings[0][i]) return strings[0].Substring(0, i);
                }
            }

            return strings[0];
        }

        public static string SplitIntoLines(string text, int maxCharsInLine)
        {
            int charsInLine = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsWhiteSpace(c) && charsInLine >= maxCharsInLine)
                {
                    builder.AppendLine();
                    charsInLine = 0;
                }
                else
                {
                    builder.Append(c);
                    charsInLine++;
                }
            }
            return builder.ToString();
        }

        public static string SplitIntoLines(string text, int maxCharsInLine, out int numLines)
        {
            int charsInLine = 0;
            numLines = 1;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsWhiteSpace(c) && charsInLine >= maxCharsInLine)
                {
                    numLines++;
                    builder.AppendLine();
                    charsInLine = 0;
                }
                else
                {
                    builder.Append(c);
                    charsInLine++;
                }
            }
            return builder.ToString();
        }


        /// <summary>
        /// Returns the input string, but with the first character in upper case.
        /// </summary>
        /// <param name="word">Word to capitalise</param>
        /// <returns>Capitalised word</returns>
        public static string Capitalise(string word)
        {
            return word[0].ToString().ToUpper() + word.Substring(1);
        }

        /// <summary>
        /// Returns the string name of an integer as you would say it in English.
        /// </summary>
        /// <param name="value">The integer to name</param>
        /// <returns>The English name of the value</returns>
        public static string GetNameOfInt(int value)
        {
            if (value == 0) return "zero";
            if (value < 0) return "negative " + GetNameOfInt(-value);

            string words = "";

            if ((value / 1000000) > 0)
            {
                words += GetNameOfInt(value / 1000000) + " million";
                value %= 1000000;
            }

            if ((value / 1000) > 0)
            {
                words += GetNameOfInt(value / 1000) + " thousand";
                value %= 1000;
            }

            if ((value / 100) > 0)
            {
                words += GetNameOfInt(value / 100) + " hundred";
                value %= 100;
            }

            if (value > 0)
            {
                if (words != "")
                {
                    words += " and ";
                }


                if (_namesOfUnits == null)
                {
                    _namesOfUnits = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                }


                if (value < 20)
                {
                    words += _namesOfUnits[value];
                }
                else
                {

                    if (_namesOfTens == null)
                    {
                        _namesOfTens = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
                    }

                    words += _namesOfTens[value / 10];
                    if ((value % 10) > 0)
                    {
                        words += "-" + _namesOfUnits[value % 10];
                    }
                }
            }

            return words;
        }

        /// <summary>
        /// Takes an amount of seconds and returns a human readable version of that time period in hours, minutes and seconds.
        /// </summary>
        /// <param name="time">The amount of time, in seconds</param>
        /// <returns>The time in a nice HH:MM:SS format</returns>
        public static string GetTimeString(float time)
        {
            if (time <= 0) return "00:00";

            DateTime dateTime = new DateTime().AddSeconds(time);
            if (time >= 60)
            {
                if (time > 3600)
                {
                    return dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2") + ":" + dateTime.Second.ToString("D2");
                }
                return dateTime.Minute.ToString("D2") + ":" + dateTime.Second.ToString("D2");
            }

            return "00:" + dateTime.Second.ToString("D2");
        }

        /// <summary>
        /// Takes a camel case string and returns a human readable title case version of it. eg. "_thingNumber" becomes "Thing Number".
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The titleized string</returns>
        public static string Titelize(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            StringBuilder str = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!IsAsciiLetter(c) && !IsAsciiNumber(c))
                {
                    continue;
                }

                if (str.Length == 0)
                {
                    str.Append(char.ToUpper(c));
                }
                else if (i > 0 && char.IsWhiteSpace(input[i - 1]))
                {
                    str.Append(' ');
                    str.Append(char.ToUpper(c));
                }
                else if (char.IsUpper(c) && i > 0 && char.IsLower(input[i - 1]))
                {
                    str.Append(' ');
                    str.Append(char.ToUpper(c));
                }
                else
                {
                    str.Append(c);
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Takes title case string and returns a camel case variable-styled version of it. eg. "Thing Number"  becomes "_thingNumber".
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="lowercaseFirstLetter">Whether or not to force the first character to be lowercase</param>
        /// <param name="addUnderscorePrefix">Whether or not to prepend an unerscore to the string</param>
        /// <returns>The variableized string</returns>
        public static string Variableize(string input, bool lowercaseFirstLetter, bool addUnderscorePrefix)
        {
            input = Santise(input, false, false);

            if (lowercaseFirstLetter)
            {
                input = char.ToLower(input[0]) + input.Substring(1);
            }

            if (addUnderscorePrefix)
            {
                input = '_' + input;
            }

            return input;
        }

        public static string ReplaceDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedString[i]);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Removes any character from a string that isn't a simple ASCII character.
        /// </summary>
        /// <param name="input">The string to sanitise</param>
        /// /// <param name="allowPunctuationAndSpaces">Whether or not to allow puctuation or the space character. Note that underscore is always allowed!</param>
        /// <returns>The sanitised string</returns>
        public static string Santise(string input, bool allowPunctuationAndSpaces, bool replaceDiacritics)
        {
            if (string.IsNullOrEmpty(input)) return "";

            if (replaceDiacritics)
            {
                input = ReplaceDiacritics(input);
            }

            StringBuilder str = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (allowPunctuationAndSpaces)
                {
                    if (IsAsciiLetter(c) || IsAsciiNumber(c) || IsAsciiPunctuation(c) || c == ' ')
                    {
                        str.Append(c);
                    }
                }
                else
                {
                    if (IsAsciiLetter(c) || IsAsciiNumber(c) || c == '_')
                    {
                        str.Append(c);
                    }
                }
            }

            return str.ToString();
        }

        public static bool IsVowel(char c)
        {
            if (c == 'A' || c == 'a') return true;
            if (c == 'E' || c == 'e') return true;
            if (c == 'I' || c == 'i') return true;
            if (c == 'O' || c == 'o') return true;
            if (c == 'U' || c == 'u') return true;

            return false;
        }

        public static bool IsAsciiNumber(char c)
        {
            return c >= 48 && c < 58;
        }

        public static bool IsAsciiLetter(char c)
        {
            return (c >= 65 && c < 91) || (c >= 97 && c < 123);
        }

        public static bool IsAsciiPunctuation(char c)
        {
            return (c >= 33 && c < 48) || (c >= 58 && c < 65) || (c >= 91 && c < 97) || (c >= 123 && c < 127);
        }

        public static char[] GetAsciiNumbers()
        {
            char[] chars = new char[10];

            for (int i = 48; i < 58; i++) chars[i - 48] = (char)i;

            return chars;
        }

        public static char[] GetAsciiLetters()
        {
            char[] chars = new char[52];

            for (int i = 65; i < 91; i++) chars[i - 65] = (char)i;
            for (int i = 97; i < 123; i++) chars[i - 97 + 26] = (char)i;

            return chars;
        }

        public static char[] GetAsciiLowercaseLetters()
        {
            char[] chars = new char[26];

            for (int i = 97; i < 123; i++) chars[i - 97] = (char)i;

            return chars;
        }

        public static char[] GetUppercaseAsciiLetters()
        {
            char[] chars = new char[26];

            for (int i = 65; i < 91; i++) chars[i - 65] = (char)i;

            return chars;
        }

        public static char[] GetAsciiPunctuation()
        {
            char[] chars = new char[32];

            for (int i = 33; i < 48; i++) chars[i - 33] = (char)i;
            for (int i = 58; i < 65; i++) chars[i - 58 + 15] = (char)i;
            for (int i = 91; i < 97; i++) chars[i - 91 + 22] = (char)i;
            for (int i = 123; i < 127; i++) chars[i - 123 + 28] = (char)i;

            return chars;
        }

        /// <summary>
        /// Returns the ordinal form of a number, eg: 2nd, 23rd 19th, 1st etc.
        /// </summary>
        /// <param name="number">The number to ordinalize</param>
        /// <returns>The ordinalized string of the number</returns>
        public static string Ordinalize(int number)
        {
            return Inflector.Ordinalize(number);
        }

        /// <summary>
        /// Returns the plural form of a word.
        /// </summary>
        /// <param name="word">The singular form to pluralise</param>
        /// <returns>The plural string of the word</returns>
        public static string Pluralise(string word)
        {
            return Inflector.Pluralize(word);
        }

        /// <summary>
        /// Returns the plural form of a word if necessary. (ie. there is more than one item)
        /// </summary>
        /// <param name="word">The singular form to pluralise</param>
        /// <param name="count">The number of items</param>
        /// <returns>Either the plural or singular form of the word (depending on the count)</returns>
        public static string Pluralise(string word, int count, bool includeArticle = false)
        {
            if (count != 1)
            {
                return Inflector.Pluralize(word);
            }

            if (includeArticle)
            {
                return AddArticle(word);
            }

            return word;
        }

        public static string AddArticle(string word)
        {
            if (IsVowel(word[0]))
            {
                return "an " + word;
            }

            return "a " + word;
        }

        /// <summary>
        /// Returns the singular form of a word.
        /// </summary>
        /// <param name="pluralWord">The plural form to singularise</param>
        /// <returns>The singular string of the word</returns>
        public static string Singularise(string pluralWord)
        {
            return Inflector.Singularize(pluralWord);
        }

        //Copyright (c) 2013 Scott Kirkland, used under the MIT license (https://github.com/srkirkland/Inflector)
        static class Inflector
        {

            #region Default Rules

            static Inflector()
            {
                AddPlural("$", "s");
                AddPlural("s$", "s");
                AddPlural("(ax|test)is$", "$1es");
                AddPlural("(octop|vir|alumn|fung)us$", "$1i");
                AddPlural("(alias|status)$", "$1es");
                AddPlural("(bu)s$", "$1ses");
                AddPlural("(buffal|tomat|volcan)o$", "$1oes");
                AddPlural("([ti])um$", "$1a");
                AddPlural("sis$", "ses");
                AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
                AddPlural("(hive)$", "$1s");
                AddPlural("([^aeiouy]|qu)y$", "$1ies");
                AddPlural("(x|ch|ss|sh)$", "$1es");
                AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
                AddPlural("([m|l])ouse$", "$1ice");
                AddPlural("^(ox)$", "$1en");
                AddPlural("(quiz)$", "$1zes");

                AddSingular("s$", "");
                AddSingular("(n)ews$", "$1ews");
                AddSingular("([ti])a$", "$1um");
                AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
                AddSingular("(^analy)ses$", "$1sis");
                AddSingular("([^f])ves$", "$1fe");
                AddSingular("(hive)s$", "$1");
                AddSingular("(tive)s$", "$1");
                AddSingular("([lr])ves$", "$1f");
                AddSingular("([^aeiouy]|qu)ies$", "$1y");
                AddSingular("(s)eries$", "$1eries");
                AddSingular("(m)ovies$", "$1ovie");
                AddSingular("(x|ch|ss|sh)es$", "$1");
                AddSingular("([m|l])ice$", "$1ouse");
                AddSingular("(bus)es$", "$1");
                AddSingular("(o)es$", "$1");
                AddSingular("(shoe)s$", "$1");
                AddSingular("(cris|ax|test)es$", "$1is");
                AddSingular("(octop|vir|alumn|fung|cact)i$", "$1us");
                AddSingular("(alias|status)es$", "$1");
                AddSingular("^(ox)en", "$1");
                AddSingular("(vert|ind)ices$", "$1ex");
                AddSingular("(matr)ices$", "$1ix");
                AddSingular("(quiz)zes$", "$1");

                AddIrregular("person", "people");
                AddIrregular("man", "men");
                AddIrregular("child", "children");
                AddIrregular("sex", "sexes");
                AddIrregular("move", "moves");
                AddIrregular("goose", "geese");
                AddIrregular("alumna", "alumnae");

                AddUncountable("equipment");
                AddUncountable("information");
                AddUncountable("rice");
                AddUncountable("food");
                AddUncountable("money");
                AddUncountable("species");
                AddUncountable("series");
                AddUncountable("fish");
                AddUncountable("sheep");
                AddUncountable("deer");
                AddUncountable("aircraft");
            }

            #endregion

            private class Rule
            {
                private readonly Regex _regex;
                private readonly string _replacement;

                public Rule(string pattern, string replacement)
                {
                    _regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    _replacement = replacement;
                }

                public string Apply(string word)
                {
                    if (!_regex.IsMatch(word))
                    {
                        return null;
                    }

                    return _regex.Replace(word, _replacement);
                }
            }

            private static void AddIrregular(string singular, string plural)
            {
                AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
                AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
            }

            private static void AddUncountable(string word)
            {
                _uncountables.Add(word.ToLower());
            }

            private static void AddPlural(string rule, string replacement)
            {
                _plurals.Add(new Rule(rule, replacement));
            }

            private static void AddSingular(string rule, string replacement)
            {
                _singulars.Add(new Rule(rule, replacement));
            }

            private static readonly List<Rule> _plurals = new List<Rule>();
            private static readonly List<Rule> _singulars = new List<Rule>();
            private static readonly List<string> _uncountables = new List<string>();

            public static string Pluralize(string word)
            {
                return ApplyRules(_plurals, word);
            }

            public static string Singularize(string word)
            {
                string singular = ApplyRules(_singulars, word);
                return string.IsNullOrEmpty(singular) ? word : singular;
            }

            private static string ApplyRules(List<Rule> rules, string word)
            {
                string result = word;

                if (!_uncountables.Contains(word.ToLower()))
                {
                    for (int i = rules.Count - 1; i >= 0; i--)
                    {
                        if ((result = rules[i].Apply(word)) != null)
                        {
                            break;
                        }
                    }
                }

                return result;
            }

            public static string Titleize(string word)
            {
                return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])", delegate (System.Text.RegularExpressions.Match match) { return match.Captures[0].Value.ToUpper(); });
            }

            public static string Humanize(string lowercaseAndUnderscoredWord)
            {
                return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
            }

            public static string Pascalize(string lowercaseAndUnderscoredWord)
            {
                return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)", delegate (System.Text.RegularExpressions.Match match) { return match.Groups[1].Value.ToUpper(); });
            }

            public static string Camelize(string lowercaseAndUnderscoredWord)
            {
                return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
            }

            public static string Underscore(string pascalCasedWord)
            {
                return Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                        "$1_$2"), @"[-\s]", "_").ToLower();
            }

            public static string Capitalize(string word)
            {
                return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
            }

            public static string Uncapitalize(string word)
            {
                return word.Substring(0, 1).ToLower() + word.Substring(1);
            }

            public static string Ordinalize(string numberString)
            {
                return Ordanize(int.Parse(numberString), numberString);
            }

            public static string Ordinalize(int number)
            {
                return Ordanize(number, number.ToString());
            }

            private static string Ordanize(int number, string numberString)
            {
                int nMod100 = number % 100;

                if (nMod100 >= 11 && nMod100 <= 13)
                {
                    return numberString + "th";
                }

                switch (number % 10)
                {
                    case 1:
                        return numberString + "st";
                    case 2:
                        return numberString + "nd";
                    case 3:
                        return numberString + "rd";
                    default:
                        return numberString + "th";
                }
            }


            public static string Dasherize(string underscoredWord)
            {
                return underscoredWord.Replace('_', '-');
            }
        }
    }
}
