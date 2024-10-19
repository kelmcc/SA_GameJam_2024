using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Framework
{

    public static class AssetImporterExtensions
    {
        private static JSONDeserializer _deserializer;
        private static JSONSerializer _serializer;
        private static JSONSerializer.Config _serializerConfig;

        public static void SetUserData(this AssetImporter importer, string key, object value)
        {
            Dictionary<string, JSONValue> dictionary = ReadFromImporter(importer);
            if (dictionary == null) dictionary = new Dictionary<string, JSONValue>();

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = Serialize(value);
            }
            else
            {
                dictionary.Add(key, Serialize(value));
            }


            WriteToImporter(importer, dictionary);
        }

        public static bool HasUserData(this AssetImporter importer, string key)
        {
            Dictionary<string, JSONValue> dictionary = ReadFromImporter(importer);
            return dictionary != null && dictionary.ContainsKey(key);
        }

        public static bool TryGetUserData<T>(this AssetImporter importer, string key, out T result)
        {
            Dictionary<string, JSONValue> dictionary = ReadFromImporter(importer);
            if (dictionary != null && dictionary.TryGetValue(key, out JSONValue value))
            {
                result = Deserialize<T>(value);
                return true;
            }

            result = default;
            return false;
        }

        public static T GetUserData<T>(this AssetImporter importer, string key)
        {
            Dictionary<string, JSONValue> dictionary = ReadFromImporter(importer);
            if (dictionary != null && dictionary.TryGetValue(key, out JSONValue value))
            {
                return Deserialize<T>(value);
            }

            return default;
        }

        public static bool ClearUserData(this AssetImporter importer, string key)
        {
            Dictionary<string, JSONValue> dictionary = ReadFromImporter(importer);
            if (dictionary != null)
            {
                if (dictionary.Remove(key))
                {
                    WriteToImporter(importer, dictionary);
                    return true;
                }
            }

            return false;
        }

        public static void ClearAllUserData(this AssetImporter importer)
        {
            importer.userData = null;
        }

        static JSONValue Serialize(object value)
        {
            if (_serializer == null)
            {
                _serializer = new JSONSerializer();
                _serializerConfig = new JSONSerializer.Config
                {
                    SerializeObjectReferences = false
                };
            }

            _serializer.BeginSerialization(_serializerConfig);
            JSONValue result = _serializer.Serialize(value);
            _serializer.EndSerialization();

            return result;
        }

        static T Deserialize<T>(JSONValue value)
        {
            if (value == null) return default;

            if (_deserializer == null)
            {
                _deserializer = new JSONDeserializer();
            }

            _deserializer.BeginDeserialization();
            T result = (T)_deserializer.Deserialize(typeof(T), value);
            _deserializer.EndDeserialization();

            return result;
        }

        static Dictionary<string, JSONValue> ReadFromImporter(AssetImporter importer)
        {

            if (string.IsNullOrEmpty(importer.userData)) return null;

            try
            {
                Dictionary<string, JSONValue> dictionary = new Dictionary<string, JSONValue>();

                JSONObject json = new JSONObject(importer.userData);
                for (int i = 0; i < json.FieldCount; i++)
                {
                    dictionary.Add(json.Fields[i].Name, json.Fields[i].JSONValue);
                }

                return dictionary;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        static void WriteToImporter(AssetImporter importer, Dictionary<string, JSONValue> dictionary)
        {
            List<JSONField> fields = new List<JSONField>();

            foreach (KeyValuePair<string, JSONValue> kvp in dictionary)
            {
                fields.Add(new JSONField(kvp.Key, kvp.Value));
            }

            importer.userData = new JSONObject(fields.ToArray()).GetJSONString(false);
        }
    }
}
