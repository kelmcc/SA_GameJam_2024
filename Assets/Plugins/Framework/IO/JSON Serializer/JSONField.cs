using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    /// <summary>
    /// A JSON field (which is just a name and value pair), this base class represents a null value field.
    /// </summary>
    public class JSONField
    {
        public string Name => _name;
        public JSONValue JSONValue => _value;

        private string _name;
        private JSONValue _value;

        /// <summary>
        /// Constructs a new JSON field with a null value.
        /// </summary>
        /// <param name="name">The field's name</param>
        public JSONField(string name)
        {
            _name = name;
            _value = null;
        }

        public JSONField(string name, JSONValue value)
        {
            _name = name;
            _value = value;
        }

        public JSONField(string name, string value)
        {
            _name = name;
            _value = new JSONStringValue(value);
        }

        public JSONField(string name, int value)
        {
            _name = name;
            _value = new JSONIntValue(value);
        }

        public JSONField(string name, float value)
        {
            _name = name;
            _value = new JSONFloatValue(value);
        }

        public JSONField(string name, bool value)
        {
            _name = name;
            _value = new JSONBooleanValue(value);
        }

        public JSONField(string name, JSONObject value)
        {
            _name = name;
            _value = value;
        }

        public JSONField(string name, JSONValue[] values)
        {
            _name = name;
            _value = new JSONArrayValue(values);
        }


        public void SetName(string name)
        {
            _name = name;
        }

        public void SetValue(JSONValue value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the JSON string that represents this field.
        /// </summary>
        /// <param name="withWhitespace">Wether or not to include whitespace in the string (makes it more human readable at the expense of storage)</param>
        /// <returns>The JSON string that represents this field</returns>
        public string GetJSONString(bool withWhitespace = false)
        {
            if (_value == null)
            {
                return "\"" + _name + "\":null";
            }

            return "\"" + _name + "\":" + _value.GetJSONString(withWhitespace);
        }

        public static JSONField FromString(string jsonFieldString)
        {
            int colonPosition = jsonFieldString.IndexOf(':');
            string name = jsonFieldString.Substring(1, colonPosition - 2).Trim();
            JSONValue value = JSONValue.FromString(jsonFieldString.Substring(colonPosition + 1, jsonFieldString.Length - colonPosition - 1).Trim());

            return value == null || value.Equals(null) ? new JSONField(name) : new JSONField(name, value);
        }

    }

}
