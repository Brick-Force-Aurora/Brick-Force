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

        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                throw new InvalidCastException($"Value for key '{key}' is not of type {typeof(T).Name}.");
            }
            throw new KeyNotFoundException($"Key '{key}' not found in JsonObject.");
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
    }
}
