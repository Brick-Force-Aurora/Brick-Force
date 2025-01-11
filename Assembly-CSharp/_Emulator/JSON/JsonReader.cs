using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator.JSON
{
    /*
     * This is a JSON Reader class, this class supports the following datatypes:
     * String, Boolean, Float, Integer, Null, Arrays, Nested Objects
     */
    class JsonReader
    {
        private readonly TextReader _reader;

        public JsonReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public T ReadObject<T>() where T : class, new()
        {
            T result = new T();

            if (result is JsonObject jsonObject)
            {
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
                    jsonObject.Add(key, value);
                }

                return result;
            }
            else if (result is JsonArray jsonArray)
            {
                string line;
                while ((line = _reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("[") || line.StartsWith("]")) // Skip comments or start/end of array
                        continue;

                    if (line.StartsWith("{"))
                    {
                        // Parse nested JsonObject
                        StringBuilder nestedObjectBuilder = new StringBuilder();
                        nestedObjectBuilder.AppendLine(line); // Add the opening brace

                        while ((line = _reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            nestedObjectBuilder.AppendLine(line);

                            if (line.StartsWith("}")) // End of JsonObject
                                break;
                        }

                        using (var nestedReader = new StringReader(nestedObjectBuilder.ToString()))
                        {
                            var nestedJsonReader = new JsonReader(nestedReader);
                            JsonObject nestedObject = nestedJsonReader.ReadObject<JsonObject>();
                            jsonArray.Add(nestedObject);
                        }
                    }
                }

                return result;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported type: {typeof(T).Name}");
            }
        }



        private string ParseKey(string value)
        {
            return value.Trim().Trim('"');
        }

        private object ParseValue(string value)
        {
            if (value.EndsWith(","))
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (value.StartsWith("[") && value.EndsWith("]")) // JSON Array
            {
                return ParseArray(value);
            }
            else if (value.StartsWith("{") && value.EndsWith("}")) // Nested JSON Object
            {
                return ParseNestedObject(value);
            }
            else if (value.StartsWith("\"") && value.EndsWith("\"")) // String
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

        private JsonArray ParseArray(string value)
        {
            var jsonArray = new JsonArray();
            // Remove the square brackets
            value = value.Substring(1, value.Length - 2).Trim();

            var elements = SplitJsonArrayElements(value);
            foreach (var element in elements)
            {
                jsonArray.Add((JsonObject)ParseValue(element.Trim()));
            }

            return jsonArray;
        }

        private JsonObject ParseNestedObject(string value)
        {
            var nestedReader = new StringReader(value);
            var nestedJsonReader = new JsonReader(nestedReader);
            return nestedJsonReader.ReadObject<JsonObject>();
        }

        private List<string> SplitJsonArrayElements(string value)
        {
            var elements = new List<string>();
            var currentElement = new StringBuilder();
            int depth = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (c == '[' || c == '{')
                {
                    depth++;
                }
                else if (c == ']' || c == '}')
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    elements.Add(currentElement.ToString());
                    currentElement = new StringBuilder();
                    continue;
                }

                currentElement.Append(c);
            }

            if (currentElement.Length > 0)
            {
                elements.Add(currentElement.ToString());
            }

            return elements;
        }
    }
}
