using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Utility class for enum things.
    /// </summary>
    public static class EnumUtils
    {

        private static Dictionary<Type, int> _enumLengths;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _enumLengths = null;
        }

        /// <summary>
        /// Checks the total number of items for an enum type.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>The number of enum values</returns>
        public static int GetCount<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            return GetCount(typeof(T));
        }

        /// <summary>
        /// Checks the total number of items for an enum type.
        /// </summary>
        /// <typeparam name="enumType">The enum type</typeparam>
        /// <returns>The number of enum values</returns>
        public static int GetCount(Type enumType)
        {
            DebugUtils.AssertIsEnumType(enumType);

            if (_enumLengths == null)
            {
                _enumLengths = new Dictionary<Type, int>();
            }

            if (!_enumLengths.TryGetValue(enumType, out int count))
            {
                count = Enum.GetValues(enumType).Length;
                _enumLengths.Add(enumType, count);
            }

            return count;
        }

        public static bool TryParse(Type enumType, string enumName, out object enumObject)
        {
            try
            {
                enumObject = Enum.Parse(enumType, enumName);
            }
            catch
            {
                enumObject = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the next value in an enum after a specific value. Or the first value if the current value is the last in the enum. ASSUMES THE ENUM VALUES START AT 0 AND ARE ALL ONE INTEGER APART.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="currentValue">The current enum value</param>
        /// <returns>The next (wrapping) value in the enum</returns>
        public static T GetNextValueWrapped<T>(T currentValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();
            return (T)(object)MathUtils.Wrap((int)(object)currentValue + 1, 0, Enum.GetValues(typeof(T)).Length - 1);
        }

        /// <summary>
        /// Returns previous next value in an enum after a specific value. Or the last value if the current value is the first in the enum. ASSUMES THE ENUM VALUES START AT 0 AND ARE ALL ONE INTEGER APART.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="currentValue">The current enum value</param>
        /// <returns>The previous (wrapping) value in the enum</returns>
        public static T GetPreviousValueWrapped<T>(T currentValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();
            return (T)(object)MathUtils.Wrap((int)(object)currentValue - 1, 0, Enum.GetValues(typeof(T)).Length - 1);
        }

        /// <summary>
        /// Checks the number items with unqiue underlying values for an enum type.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>The number of unique values</returns>
        public static int CountUnique<T>() where T : struct, IComparable, IConvertible, IFormattable
        {

            DebugUtils.AssertIsEnumType<T>();
            Array values = Enum.GetValues(typeof(T));
            ArrayList set = new ArrayList(values.Length);


            for (int i = 0; i < values.Length; i++)
            {
                if (!set.Contains(values.GetValue(i)))
                {
                    set.Add(values.GetValue(i));
                }
            }

            return set.Count;
        }

        /// <summary>
        /// Checks the number items with positive integers as underlying values for an enum type. No.te that 0 is also treated as positive
        /// </summary>
        /// <typeparam name="T">The enum type, must have be an underlying int type</typeparam>
        /// <returns>The number of positive values</returns>
        public static int CountPositive<T>() where T : struct, IComparable, IConvertible, IFormattable
        {

            DebugUtils.AssertIsEnumType<T>();
            Assert.IsTrue(Enum.GetUnderlyingType(typeof(T)) == typeof(int));
            Array values = Enum.GetValues(typeof(T));
            int count = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if ((int)values.GetValue(i) >= 0)
                {
                    count++;
                }
            }

            return count;
        }

        public static T[] GetAllValues<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();
            Assert.IsTrue(Enum.GetUnderlyingType(typeof(T)) == typeof(int));

            return (T[])Enum.GetValues(typeof(T));
        }

    }
}
