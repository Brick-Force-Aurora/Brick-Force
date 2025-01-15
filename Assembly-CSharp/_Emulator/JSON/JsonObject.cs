using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator.JSON
{
    class JsonObject: IEnumerable
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Add(string key, object value)
        {
            _data[key] = value;
        }

        public T Get<T>(string key, T fallback = default)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }

                throw new InvalidCastException($"Value for key '{key}' is not of type {typeof(T).Name}.");
            }
            return fallback;
        }


        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(_data);
        }

        public void LoadFromDictionary(Dictionary<string, object> dictionary)
        {
            _data.Clear();
            foreach (var kvp in dictionary)
            {
                _data[kvp.Key] = kvp.Value;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            int count = 0;
            foreach (var kvp in _data)
            {
                Serialize(sb, kvp);

                if (count < _data.Count - 1)
                {
                    sb.Append(",\n");
                } else if (count == _data.Count -1)
                {
                    sb.Append("\n");
                }
                count++;
            }
            sb.Append("}");
            return sb.ToString();
        }

        public string ToString(string prefix)
        {
            var sb = new StringBuilder();
            sb.Append(prefix + "{\n");
            int count = 0;
            foreach (var kvp in _data)
            {
                Serialize(sb, kvp, prefix);

                if (count < _data.Count - 1)
                {
                    sb.Append(",\n");
                }
                else if (count == _data.Count - 1)
                {
                    sb.Append("\n");
                }
                count++;
            }
            sb.Append(prefix + "}");
            return sb.ToString();
        }

        public void Serialize(StringBuilder sb, KeyValuePair<string, object> kvp, string prefix = "")
        {
            sb.Append($"{prefix}\t\"{kvp.Key}\": ");
            if (kvp.Value is string)
            {
                sb.Append($"\"{kvp.Value}\"");
            }
            else if (kvp.Value is JsonObject)
            {
                JsonObject val = (JsonObject)kvp.Value;
                sb.Append(val.ToString("\t"));
            }
            else if (kvp.Value is JsonArray)
            {
                JsonArray val = (JsonArray)kvp.Value;
                sb.Append(val.ToString("\t"));
            }
            else if (kvp.Value is bool)
            {
                sb.Append(kvp.Value.ToString().ToLower());
            }
            else if (kvp.Value is float || kvp.Value is double)
            {
                string value = kvp.Value.ToString();
                if (!value.Contains("."))
                {
                    value += ".0"; // Add .0 if not present
                }
                sb.Append(value);
            }
            else if (kvp.Value is Item.USAGE)
            {
                sb.Append($"\"{kvp.Value}\"");
            }
            else if (kvp.Value is TItem.SLOT)
            {
                sb.Append($"\"{kvp.Value}\"");
            }
            else
            {
                sb.Append(kvp.Value);
            }
        }
    }
}
