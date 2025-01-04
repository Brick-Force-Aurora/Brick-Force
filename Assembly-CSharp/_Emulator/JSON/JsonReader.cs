using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator.JSON
{
    /*
     * This is a JSON Reader class, this class only supports the following datatypes:
     * String, Boolean, Float, Integer, Null
     * This JSON implementation can currently not use any arrays.
     * This JSON implementation can currently not use any nested objects.
     */
    class JsonReader
    {
        private readonly TextReader _reader;

        public JsonReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public JsonObject ReadObject()
        {
            var result = new JsonObject();
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("{") || line.StartsWith("}")) // Skip empty lines or comments
                    continue;

                int separatorIndex = line.IndexOf(":");
                if (separatorIndex < 0)
                    throw new FormatException("Invalid format: Missing ':' separator.");

                string key = ParseKey(line.Substring(0, separatorIndex));
                object value = ParseValue(line.Substring(separatorIndex + 1).Trim());
                result.Add(key, value);
            }
            return result;
        }

        private string ParseKey(string value)
        {
            return value.Trim().Trim('"');
        }

        private object ParseValue(string value)
        {
            if (value.StartsWith("\"") && value.EndsWith("\"")) // String
            {
                return value.Substring(1, value.Length - 2);
            }
            else if (int.TryParse(value, out int intValue)) // Integer
            {
                return intValue;
            }
            else if (float.TryParse(value, out float doubleValue)) // Float
            {
                return doubleValue;
            }
            else if (bool.TryParse(value, out bool boolValue)) // Boolean
            {
                return boolValue;
            }
            else if (value.Equals("null", StringComparison.OrdinalIgnoreCase)) // Null
            {
                return null;
            }
            else
            {
                throw new FormatException("Unsupported value type: " + value);
            }
        }
    }
}
