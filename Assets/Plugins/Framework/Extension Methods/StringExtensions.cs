using System.Text;
using UnityEngine.Assertions;

namespace Framework
{
    public static class StringExtensions
    {

        public static string Combine(this string[] strings)
        {
            if (strings.Length == 0) return "";

            string result = strings[0];

            for (int i = 1; i < strings.Length; i++)
            {
                result += strings[i];
            }

            return result;
        }
        public static string Combine(this string[] strings, char seperator)
        {
            if (strings.Length == 0) return "";

            string result = strings[0];

            for (int i = 1; i < strings.Length; i++)
            {
                result += seperator + strings[i];
            }

            return result;
        }

        public static bool Contains(this string s, char c)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        public static string Trim(this string s, char trimCharacter)
        {
            int startTrim = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == trimCharacter)
                {
                    startTrim++;
                }
                else
                {
                    break;
                }
            }

            int endTrim = 0;
            for (int i = s.Length - 1; i >= s.Length; i--)
            {
                if (s[i] == trimCharacter)
                {
                    endTrim++;
                }
                else
                {
                    break;
                }
            }

            if (startTrim + endTrim == 0) return s;

            return s.Substring(startTrim, s.Length - startTrim - endTrim);
        }

        public static string RemoveFromStart(this string s, string substring)
        {
            if (s.StartsWith(substring))
            {
                return s.Substring(substring.Length);
            }

            return s;
        }

        public static string RemoveFromEnd(this string s, string substring)
        {
            if (s.EndsWith(substring))
            {
                return s.Substring(0, s.Length - substring.Length);
            }

            return s;
        }

        public static string TrimFromLastIndexOF(this string s, char c)
        {
            int index = s.LastIndexOf(c);
            if (index >= 0) return s.Substring(0, index);

            return s;
        }

        public static string TrimFromLastIndexOF(this string s, string substring)
        {
            int index = s.LastIndexOf(substring);
            if (index >= 0) return s.Substring(0, index);

            return s;
        }

        public static int CountOccurencesOf(this string s, string substring)
        {
            int index = 0;
            int count = 0;

            while ((index < s.Length) && (index = s.IndexOf(substring, index)) != -1)
            {
                count++;
                index += substring.Length;
            }

            return count;
        }

        public static int CountOccurencesOf(this string s, char c)
        {
            int count = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c) count++;
            }

            return count;
        }

        public static int NthIndexOf(this string s, int n, string match)
        {
            Assert.IsTrue(n > 0);

            int i = 1;
            int index = -1;

            while (i <= n && (index = s.IndexOf(match, index + 1)) != -1)
            {
                if (i == n)
                    return index;

                i++;
            }

            return -1;
        }

        public static int NthIndexOf(this string s, int n, char c)
        {
            Assert.IsTrue(n > 0);

            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns whether or not a string is either null or empty.
        /// </summary>
        /// <returns>Whether or not the string is null or empty</returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Clears the current string of a string builder.
        /// </summary>
        public static void Clear(this StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
        }

        public static void Append(this StringBuilder stringBuilder, object obj1, object obj2)
        {
            stringBuilder.Append(obj1 == null ? "null" : obj1);
            stringBuilder.Append(obj2 == null ? "null" : obj2);
        }

        public static void Append(this StringBuilder stringBuilder, object obj1, object obj2, object obj3)
        {
            stringBuilder.Append(obj1 == null ? "null" : obj1);
            stringBuilder.Append(obj2 == null ? "null" : obj2);
            stringBuilder.Append(obj3 == null ? "null" : obj3);
        }

        public static void AppendLine(this StringBuilder stringBuilder, object obj1, object obj2)
        {
            stringBuilder.Append(obj1 == null ? "null" : obj1);
            stringBuilder.Append(obj2 == null ? "null" : obj2);
            stringBuilder.AppendLine();
        }

        public static void AppendLine(this StringBuilder stringBuilder, object obj1, object obj2, object obj3)
        {
            stringBuilder.Append(obj1 == null ? "null" : obj1);
            stringBuilder.Append(obj2 == null ? "null" : obj2);
            stringBuilder.Append(obj3 == null ? "null" : obj3);
            stringBuilder.AppendLine();
        }
    }
}
