using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework
{
    public enum PlayerPrefsValueType
    {
        String,
        Int,
        Float
    }

    public class PlayerPrefsEntry
    {

        public string Key => _key;
        public object Value => _value;
        public PlayerPrefsValueType Type => _type;
        public int IntValue => (int)_value;
        public float FloatValue => (float)_value;
        public string StringValue => (string)_value;

        private string _key;
        private object _value;
        private PlayerPrefsValueType _type;

        public PlayerPrefsEntry(string key, int value)
        {
            _key = key;
            _type = PlayerPrefsValueType.Int;
            _value = value;
        }

        public PlayerPrefsEntry(string key, float value)
        {
            _key = key;
            _type = PlayerPrefsValueType.Float;
            _value = value;
        }

        public PlayerPrefsEntry(string key, string value)
        {
            _key = key;
            _type = PlayerPrefsValueType.String;
            _value = value;
        }
    }
}

