using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Globalization;
using Framework;
using UnityEngine.Assertions;

namespace Framework
{

    /// <summary>
    /// A JSON object, basically just a collection of JSON fields.
    /// </summary>
    public class JSONObject : JSONValue
    {
        /// <summary>
        /// The number of JSON fields this object contains.
        /// </summary>
        public int FieldCount => _fields.Length;

        /// <summary>
        /// The JSON fields that this object contains.
        /// </summary>
        public JSONField[] Fields => _fields;

        private JSONField[] _fields;

        /// <summary>
        /// Constructs a new JSON object from an array of JSON fields.
        /// </summary>
        /// <param name="fields">The field array</param>
        public JSONObject(JSONField[] fields)
        {
            _fields = fields;
        }

        /// <summary>
        /// Constructs a new JSON object from a JSON string.
        /// </summary>
        /// <param name="jsonString">The JSON string</param>
        public JSONObject(string jsonString)
        {

            jsonString = StripFormattingFromJSONString(jsonString);
            List<string> fieldStrings = new List<string>();

            int depth = 0;
            int startIndex = 1;
            bool insideValue = false;
            for (int i = 0; i < jsonString.Length; i++)
            {
                char currentChar = jsonString[i];


                if (depth == 1 && currentChar == '"')
                {
                    insideValue = !insideValue;
                }

                if (!insideValue)
                {

                    if (depth == 1 && currentChar == ',')
                    {
                        fieldStrings.Add(jsonString.Substring(startIndex, i - startIndex).Trim());
                        startIndex = i + 1;
                    }

                    if (currentChar == '{' || currentChar == '[')
                    {
                        depth++;
                    }

                    if (currentChar == '}' || currentChar == ']')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            fieldStrings.Add(jsonString.Substring(startIndex, i - startIndex).Trim());
                            startIndex = i + 1;
                        }
                    }
                }
            }

            List<JSONField> fields = new List<JSONField>();
            for (int i = 0; i < fieldStrings.Count; i++)
            {
                if (fieldStrings[i].Length > 0)
                {
                    fields.Add(JSONField.FromString(fieldStrings[i]));
                }
            }

            _fields = fields.ToArray();
        }

        public void AppendField(JSONField field)
        {
            _fields = _fields.WithAddedElement(field);
        }

        public void InsertField(JSONField field, int index)
        {
            _fields = _fields.WithInsertedElement(field, index);
        }

        public JSONField GetField(string fieldName)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    return _fields[i];
                }
            }

            throw new ArgumentException("JSON field not found: " + fieldName);
        }

        public bool TryGetField(string fieldName, out JSONField field)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    field = _fields[i];
                    return true;
                }
            }

            field = null;
            return false;
        }

        public bool TryReplaceField(JSONField newField)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == newField.Name)
                {
                    _fields[i] = newField;
                    return true;
                }
            }

            return false;
        }

        public void ReplaceField(int fieldIndex, JSONField newField)
        {
            _fields[fieldIndex] = newField;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T GetFieldValue<T>(string fieldName)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    try
                    {
                        return (T)_fields[i].JSONValue.GetRawObject();
                    }
                    catch (InvalidCastException)
                    {
                        Debug.LogError("JSON field is not " + typeof(T) + ": " + fieldName);
                        return default(T);
                    }
                }
            }

            Debug.LogError("JSON field was not found: " + fieldName + "");
            return default(T);
        }

        public bool TryGetFieldValue<T>(string fieldName, out T result)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    try
                    {
                        result = (T)_fields[i].JSONValue.GetRawObject();
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                        Debug.LogError("JSON field is not " + typeof(T) + ": " + fieldName);

                    }
                }
            }

            result = default(T);
            return false;
        }

        public bool HasField(string fieldName)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the JSON string that represents this JSON object.
        /// </summary>
        /// <param name="withWhitespace">Wether or not to include whitespace in the string (makes it more human readable at the expense of storage)</param>
        /// <returns>The JSON string that represents this object</returns>
        public override string GetJSONString(bool withWhitespace = true)
        {
            StringBuilder fieldBuilder = new StringBuilder("{");

            for (int i = 0; i < _fields.Length; i++)
            {
                fieldBuilder.Append(_fields[i].GetJSONString());
                if (i < _fields.Length - 1)
                {
                    fieldBuilder.Append(",");
                }
            }

            fieldBuilder.Append("}");

            return withWhitespace ? FormatJSONString(fieldBuilder.ToString()) : fieldBuilder.ToString();

        }

        public override object GetRawObject()
        {
            return _fields;
        }

        static string StripFormattingFromJSONString(string jsonString)
        {
            bool insideQuote = false;

            StringBuilder unformatted = new StringBuilder();

            for (int i = 0; i < jsonString.Length; i++)
            {
                char c = jsonString[i];

                if (c == '"')
                {
                    insideQuote = !insideQuote;
                }

                if (!insideQuote)
                {
                    if (c == '\n' || c == '\t' || c == ' ')
                    {
                        continue;
                    }
                }

                unformatted.Append(c);
            }

            return unformatted.ToString();
        }

        static string FormatJSONString(string jsonString)
        {
            StringBuilder formatted = new StringBuilder();

            int depth = 0;
            bool insideQuote = false;

            for (int i = 0; i < jsonString.Length; i++)
            {
                char c = jsonString[i];

                if (c == '"')
                {
                    insideQuote = !insideQuote;
                }

                if (insideQuote)
                {
                    formatted.Append(c);
                    continue;
                }

                if (c == '{' || c == '[')
                {
                    formatted.Append(c);
                    formatted.Append("\n");

                    depth++;
                    for (int j = 0; j < depth; j++)
                    {
                        formatted.Append("\t");
                    }

                }
                else if (c == ',')
                {
                    formatted.Append(c);
                    formatted.Append("\n");
                    for (int j = 0; j < depth; j++)
                    {
                        formatted.Append("\t");
                    }


                }
                else if (c == '}' || c == ']')
                {
                    formatted.Append("\n");

                    depth--;
                    for (int j = 0; j < depth; j++)
                    {
                        formatted.Append("\t");
                    }

                    formatted.Append(c);


                }
                else if (c == ':')
                {
                    formatted.Append(" ");
                    formatted.Append(c);
                    formatted.Append(" ");

                    if (i < jsonString.Length - 1)
                    {
                        if (jsonString[i + 1] == '{' || jsonString[i + 1] == '[')
                        {
                            formatted.Append("\n");
                            for (int j = 0; j < depth; j++)
                            {
                                formatted.Append("\t");
                            }
                        }
                    }
                }
                else
                {
                    formatted.Append(c);
                }


            }

            return formatted.ToString();
        }







    }

}
