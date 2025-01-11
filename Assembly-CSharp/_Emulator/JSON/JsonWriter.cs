using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _Emulator.JSON
{
    /*
     * This is a JSON Writer class, this class only supports the following datatypes:
     * String, Boolean, Float, Integer, Null
     * This JSON implementation can currently not use any arrays.
     * This JSON implementation can currently not use any nested objects.
     */
    class JsonWriter
    {
        private readonly TextWriter _writer;

        public JsonWriter(TextWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void WriteObject(JsonObject data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _writer.Write(data.ToString());
        }

        public void WriteObject(JsonArray data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _writer.Write(data.ToString());
        }

        private string SerializeValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is string stringValue)
            {
                return $"\"{stringValue}\"";
            }
            else if (value is bool boolValue)
            {
                return boolValue.ToString().ToLower();
            }
            else if (value is int)
            {
                return value.ToString();
            }
            else if (value is float floatValue)
            {
                return floatValue.ToString("0.0");
            }
            else
            {
                throw new InvalidOperationException("Unsupported value type: " + value.GetType().Name);
            }
        }
    }
}
