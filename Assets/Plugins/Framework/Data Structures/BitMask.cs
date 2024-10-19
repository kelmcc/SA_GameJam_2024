using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//WRITE COMMENTS ROBBIE

namespace Framework
{
    [Serializable]
    public class BitMask
    {
        public int NumFlags => _numFlags;

        [SerializeField]
        private int _numFlags;

        [SerializeField]
        private byte[] _bytes = new byte[0];

        public BitMask()
        {

        }

        public BitMask(int numFlags, bool defaultValue = false)
        {
            _numFlags = numFlags;
            _bytes = new byte[(numFlags / 8) + (numFlags % 8 > 0 ? 1 : 0)];

            if (defaultValue)
            {
                SetAllFlags(true);
            }
        }

        public BitMask(params bool[] flagValues)
        {
            _numFlags = flagValues.Length;
            _bytes = new byte[(_numFlags / 8) + (_numFlags % 8 > 0 ? 1 : 0)];

            for (int i = 0; i < _numFlags; i++)
            {
                SetFlag(i, flagValues[i]);
            }
        }

        public BitMask(BitMask mask)
        {
            Assert.IsNotNull(mask);

            _numFlags = mask._numFlags;
            _bytes = new byte[mask._bytes.Length];

            for (int i = 0; i < _numFlags; i++)
            {
                _bytes[i] = mask._bytes[i];
            }
        }

        public static BitMask EnumMask<T>(params T[] setEnumValues) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            BitMask mask = new BitMask(EnumUtils.GetCount(typeof(T)));

            for (int i = 0; i < setEnumValues.Length; i++)
            {
                mask.SetFlag((int)(object)setEnumValues[i], true);
            }

            return mask;
        }

        public static BitMask EnumMask<T>(bool defaultValue = false) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            BitMask mask = new BitMask(EnumUtils.GetCount(typeof(T)));

            if (defaultValue)
            {
                mask.SetAllFlags(true);
            }

            return mask;
        }

        public void SetFlag(int index, bool value, bool expandIfNecessary = false)
        {
            if (index >= _numFlags && expandIfNecessary)
            {
                Resize(index + 1);
            }

            if (index >= _numFlags || index < 0)
            {
                throw new IndexOutOfRangeException("Invalid index (" + index + ") for BitMask of length: " + _numFlags);
            }

            int byteIndex = index / 8;
            int indexInByte = index % 8;

            if (value)
            {
                _bytes[byteIndex] = (byte)(_bytes[byteIndex] | (1 << indexInByte));
            }
            else
            {
                _bytes[byteIndex] = (byte)(_bytes[byteIndex] & ~(1 << indexInByte));
            }
        }

        public void SetEnumFlag<T>(T enumFlag, bool value, bool expandIfNecessary = false) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            SetFlag((int)(object)enumFlag, value, expandIfNecessary);
        }

        public void SetEnumFlags<T>(bool value, params T[] enumFlags) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            for (int i = 0; i < enumFlags.Length; i++)
            {
                SetFlag((int)(object)enumFlags[i], value);
            }
        }


        public void SetAllFlags(bool value)
        {
            int flagsInPaddedByte = _numFlags % 8;

            if (value)
            {
                for (int i = 0; i < _bytes.Length; i++)
                {
                    if (flagsInPaddedByte > 0 && i == _bytes.Length - 1)
                    {
                        byte lastByte = 0;
                        for (int j = 0; j < flagsInPaddedByte; j++)
                        {
                            lastByte = (byte)(lastByte | (1 << j));
                        }

                        _bytes[i] = lastByte;
                    }
                    else
                    {
                        _bytes[i] = 255;
                    }
                }

            }
            else
            {
                _bytes = new byte[(_numFlags / 8) + (flagsInPaddedByte > 0 ? 1 : 0)];
            }
        }

        public void InvertFlag(int index)
        {
            if (index >= _numFlags || index < 0)
            {
                throw new IndexOutOfRangeException("Invalid index (" + index + ") for BitMask of length: " + _numFlags);
            }

            int byteIndex = index / 8;
            int indexInByte = index % 8;

            if ((_bytes[byteIndex] & (1 << indexInByte)) != 0)
            {
                _bytes[byteIndex] = (byte)(_bytes[byteIndex] & ~(1 << indexInByte));
            }
            else
            {
                _bytes[byteIndex] = (byte)(_bytes[byteIndex] | (1 << indexInByte));
            }
        }

        public bool HasFlag(int index)
        {
            return index >= 0 && index < _numFlags;
        }

        public void InvertEnumFlag<T>(T enumFlag) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            InvertFlag((int)(object)enumFlag);
        }

        public bool GetFlag(int index)
        {
            if (index >= _numFlags || index < 0)
            {
                throw new IndexOutOfRangeException("Invalid index (" + index + ") for BitMask of length: " + _numFlags);
            }

            int byteIndex = index / 8;
            int indexInByte = index % 8;

            return (_bytes[byteIndex] & (1 << indexInByte)) != 0;
        }

        public bool GetFlagIfItExists(int index)
        {
            if (index < 0 || index >= _numFlags)
            {
                return false;
            }

            int byteIndex = index / 8;
            int indexInByte = index % 8;

            if (byteIndex < 0 || byteIndex >= _bytes.Length || indexInByte < 0 || indexInByte > 7)
            {
                return false;
            }

            return (_bytes[byteIndex] & (1 << indexInByte)) != 0;
        }

        public bool GetEnumFlag<T>(T enumFlag) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            return GetFlag((int)(object)enumFlag);
        }

        public bool GetEnumFlagIfItExists<T>(T enumFlag) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            return GetFlagIfItExists((int)(object)enumFlag);
        }

        public bool HasEnumFlag<T>(T enumFlag) where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            return HasFlag((int)(object)enumFlag);
        }

        public T[] GetTrueEnumFlags<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            DebugUtils.AssertIsEnumType<T>();

            List<T> trueFlags = new List<T>();
            for (int i = 0; i < _numFlags; i++)
            {
                if (GetFlag(i))
                {
                    trueFlags.Add((T)(object)i);
                }
            }

            return trueFlags.ToArray();
        }

        public bool[] ToArray()
        {
            bool[] flags = new bool[_numFlags];
            for (int i = 0; i < _numFlags; i++)
            {
                flags[i] = GetFlag(i);
            }

            return flags;
        }

        public int CountTrueFlags()
        {
            int count = 0;
            for (int i = 0; i < _numFlags; i++)
            {
                if (GetFlag(i))
                {
                    count++;
                }
            }

            return count;
        }

        public void Resize(int numFlags)
        {
            Assert.IsTrue(numFlags >= 0);

            byte[] oldBytes = (byte[])_bytes.Clone();
            _bytes = new byte[(numFlags / 8) + (numFlags % 8 > 0 ? 1 : 0)];

            for (int i = 0; i < _bytes.Length; i++)
            {
                _bytes[i] = i < oldBytes.Length ? oldBytes[i] : (byte)0;
            }

            _numFlags = numFlags;
        }

    }
}
