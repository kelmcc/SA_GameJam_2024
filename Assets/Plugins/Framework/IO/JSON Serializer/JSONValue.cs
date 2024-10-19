using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Framework
{

    public abstract class JSONValue
    {
        public abstract string GetJSONString(bool withWhitespace = false);
        public abstract object GetRawObject();

        public static JSONValue FromString(string jsonValueString)
        {
            try
            {
                if (jsonValueString[0] == '{') return new JSONObject(jsonValueString);
                if (jsonValueString[0] == '[') return ParseArray(jsonValueString);
                if (jsonValueString[0] == '\"') return new JSONStringValue(jsonValueString.Substring(1, jsonValueString.Length - 2));
                if (jsonValueString == "true") return new JSONBooleanValue(true);
                if (jsonValueString == "false") return new JSONBooleanValue(false);
                if (jsonValueString == "null") return null;
                if (jsonValueString.Contains(".")) return new JSONFloatValue(JSONFloatValue.Parse(jsonValueString));

                return new JSONIntValue(int.Parse(jsonValueString));
            }
            catch
            {
                Debug.LogError("JSON Deserializer failed to parse json value string:\n" + jsonValueString);
                throw;
            }
        }

        static JSONArrayValue ParseArray(string jsonValueString)
        {

            List<string> elementStrings = new List<string>();
            jsonValueString = jsonValueString.Substring(1, jsonValueString.Length - 2);

            StringBuilder builder = new StringBuilder();


            int depth = 0;
            bool insideQuotes = false;


            for (int i = 0; i < jsonValueString.Length; i++)
            {
                char currentChar = jsonValueString[i];

                if (depth == 0 && currentChar == '"')
                {
                    insideQuotes = !insideQuotes;
                    builder.Append(currentChar);
                }
                else if (insideQuotes)
                {
                    builder.Append(currentChar);
                }
                else if (depth == 0 && currentChar == ',')
                {
                    if (builder.Length > 0)
                    {
                        elementStrings.Add(builder.ToString());
                        builder.Clear();
                    }
                }
                else
                {
                    if (currentChar == '{' || currentChar == '[') depth++;
                    if (currentChar == '}' || currentChar == ']') depth--;

                    if (depth == 0)
                    {
                        if (!char.IsWhiteSpace(currentChar) && !char.IsControl(currentChar))
                        {
                            builder.Append(currentChar);
                        }
                    }
                    else
                    {
                        builder.Append(currentChar);
                    }
                }
            }

            if (builder.Length > 0)
            {
                elementStrings.Add(builder.ToString());
                builder.Clear();
            }

            JSONValue[] elements = new JSONValue[elementStrings.Count];
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = FromString(elementStrings[i]);
            }

            return new JSONArrayValue(elements);
        }
    }

    /// <summary>
    /// A JSON field with a string value.
    /// </summary>
    public class JSONStringValue : JSONValue
    {
        /// <summary>
        /// The string value.
        /// </summary>
        public string StringValue => _value;

        private string _value;

        public JSONStringValue(string value)
        {
            _value = value;
        }

        public override string GetJSONString(bool withWhitespace = false)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"");

            for (int i = 0; i < _value.Length; i++)
            {
                if (_value[i] == '"')
                {
                    builder.Append('\\');
                }

                builder.Append(_value[i]);
            }

            builder.Append("\"");

            return builder.ToString();
        }

        public override object GetRawObject()
        {
            return _value;
        }
    }



    /// <summary>
    /// A JSON field with an integer value.
    /// </summary>
    public class JSONIntValue : JSONValue
    {
        /// <summary>
        /// The integer value.
        /// </summary>
        public int IntValue => _value;

        private int _value;


        public JSONIntValue(int value)
        {
            _value = value;
        }


        public override string GetJSONString(bool withWhitespace = false)
        {
            return _value.ToString();
        }

        public override object GetRawObject()
        {
            return _value;
        }

    }

    /// <summary>
    /// A JSON field with a float value.
    /// </summary>
    public class JSONFloatValue : JSONValue
    {
        /// <summary>
        /// The float value.
        /// </summary>
        public float FloatValue => _value;

        private float _value;

        /// <summary>
        /// Constructs a JSON field with a float value.
        /// </summary>
        /// <param name="value">The float value</param>
        public JSONFloatValue(float value)
        {
            _value = value;
        }

        public override string GetJSONString(bool withWhitespace = false)
        {
            return Format(_value);
        }

        public override object GetRawObject()
        {
            return _value;
        }

        public static float Parse(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string Format(float value)
        {
            if (float.IsNaN(value))
            {
                Debug.LogError("Cannot convert NaN float to JSON");
                return "0.0";
            }


            if (value >= 0)
            {
                if (float.IsPositiveInfinity(value))
                {
                    Debug.LogError("Cannot convert infinity float to JSON");
                    return "340282300000000000000000000000000000000.0";
                }

                float before = Mathf.Floor(value);
                float after = value - before;

                if (after == 0) return before.ToString("F1", CultureInfo.InvariantCulture);

                return before.ToString("F0") + "." + after.ToString("F29").TrimEnd('0').Substring(2);
            }
            else
            {
                if (float.IsNegativeInfinity(value))
                {
                    Debug.LogError("Cannot convert negative infinity float to JSON");
                    return "-340282300000000000000000000000000000000.0";
                }

                value = Mathf.Abs(value);
                float before = Mathf.Floor(value);
                float after = value - before;

                if (after == 0) return "-" + before.ToString("F1", CultureInfo.InvariantCulture);

                return "-" + before.ToString("F0") + "." + after.ToString("F29").TrimEnd('0').Substring(2);
            }
        }

    }

    /// <summary>
    /// A JSON field with a boolean value.
    /// </summary>
    public class JSONBooleanValue : JSONValue
    {
        /// <summary>
        /// The boolean value.
        /// </summary>
        public bool BooleanValue => _value;

        private bool _value;

        /// <summary>
        /// Constructs a JSON field with a boolean value.
        /// </summary>
        /// <param name="value">The boolean value</param>
        public JSONBooleanValue(bool value)
        {
            _value = value;
        }

        public override string GetJSONString(bool withWhitespace = false)
        {
            return _value ? "true" : "false";
        }

        public override object GetRawObject()
        {
            return _value;
        }
    }


    /// <summary>
    /// A JSON field with with an array of JSON objects as its value.
    /// </summary>
    public class JSONArrayValue : JSONValue
    {
        /// <summary>
        /// The number of elements in the array.
        /// </summary>
        public int Length => _elements.Length;

        /// <summary>
        /// The elements in the array.
        /// </summary>
        public JSONValue[] Elements => _elements;

        private JSONValue[] _elements;

        /// <summary>
        /// Constructs a new JSON field with an array of JSON objects as its value.
        /// </summary>
        /// <param name="elements">The array of JSON objects</param>
        public JSONArrayValue(JSONValue[] elements)
        {
            _elements = elements;
        }

        public override string GetJSONString(bool withWhitespace = false)
        {
            StringBuilder elementBuilder = new StringBuilder();
            elementBuilder.Append('[');

            for (int i = 0; i < _elements.Length; i++)
            {
                if (withWhitespace)
                {
                    elementBuilder.Append("\n");
                }

                if (_elements[i] == null)
                {
                    elementBuilder.Append("null");
                }
                else
                {
                    elementBuilder.Append(_elements[i].GetJSONString(withWhitespace));
                }


                if (i < _elements.Length - 1)
                {
                    elementBuilder.Append(',');
                }
            }

            elementBuilder.Append(']');

            return elementBuilder.ToString();
        }

        public override object GetRawObject()
        {
            return _elements;
        }



    }

}