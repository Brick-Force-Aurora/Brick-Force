using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace _Emulator.JSON
{
    class JsonArray : IEnumerable
    {
        private readonly List<JsonObject> _items = new List<JsonObject>();

        public JsonArray() { }

        public JsonArray(object[] array) {
            foreach (var item in array)
            {
                if (item is JsonObject)
                {
                    _items.Add((JsonObject) item); // Add directly if it's already a JsonObject or JsonArray
                }
                else if (item is UpgradeProp)
                {
                    // Wrap primitive types in a JsonObject for consistency
                    UpgradeProp prop = (UpgradeProp)item;
                    var jsonObject = new JsonObject
                    {
                        {"use", prop.use },
                        {"grade", prop.grade }
                    };
                    _items.Add(jsonObject);
                }
            }
        }

        public void Add(JsonObject item)
        {
            _items.Add(item);
        }

        public JsonObject Get(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                return _items[index];
            }
            throw new IndexOutOfRangeException($"Index {index} is out of range.");
        }

        public List<Dictionary<string, object>> ToList()
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var jsonObject in _items)
            {
                result.Add(jsonObject.ToDictionary());
            }
            return result;
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[\n");
            for (int i = 0; i < _items.Count; i++)
            {
                sb.Append(_items[i].ToString("\t"));
                if (i < _items.Count - 1)
                {
                    sb.Append(",\n");
                } else if (i == _items.Count - 1)
                {
                    sb.Append("\n");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        public string ToString(string prefix)
        {
            var sb = new StringBuilder();
            sb.Append("[\n");
            for (int i = 0; i < _items.Count; i++)
            {
                sb.Append(_items[i].ToString(prefix + "\t"));
                if (i < _items.Count - 1)
                {
                    sb.Append(",\n");
                }
                else if (i == _items.Count - 1)
                {
                    sb.Append("\n");
                }
            }
            sb.Append(prefix + "]");
            return sb.ToString();
        }
    }
}
