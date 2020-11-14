using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StringMgr : MonoBehaviour
{
	public bool displayReadString;

	private bool isLoaded;

	private Dictionary<string, string>[] _dicString;

	private static StringMgr _instance;

	public bool IsLoaded => isLoaded;

	public static StringMgr Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(StringMgr)) as StringMgr);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get StringMgr Instance");
				}
			}
			return _instance;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
	}

	private IEnumerator LoadFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/string.txt.cooked";
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParseData(csvLoader);
					isLoaded = true;
				}
			}
		}
		if (!isLoaded)
		{
			Debug.LogError("Fail to download " + url);
		}
	}

	private void ParseData(CSVLoader csvLoader)
	{
		_dicString = new Dictionary<string, string>[12];
		for (int i = 0; i < _dicString.Length; i++)
		{
			_dicString[i] = new Dictionary<string, string>();
		}
		for (int j = 0; j < csvLoader.Rows; j++)
		{
			csvLoader.ReadValue(0, j, string.Empty, out string Value);
			Value.Trim();
			for (int k = 0; k < 12; k++)
			{
				csvLoader.ReadValue(k + 1, j, string.Empty, out string Value2);
				Value2 = Value2.Trim();
				if (Value.Length <= 0)
				{
					Debug.LogError("ERROR, Empty key at row:" + j + " Col: " + k + " Key: " + Value + " Value: " + Value2);
				}
				if (_dicString[k].ContainsKey(Value))
				{
					Debug.LogError("ERROR, Duplicate string key: " + Value + " at row: " + j + " Col: " + k + " Key: " + Value + " Value: " + Value2);
				}
				else
				{
					Value2 = Value2.Replace("\\0", "\n");
					_dicString[k].Add(Value, Value2);
					if (displayReadString)
					{
						Debug.Log(" Key: " + Value + " Val: " + Value2);
					}
				}
			}
		}
	}

	private void UpdateString(string key, LangOptManager.LANG_OPT lang, string val)
	{
		if (val.Length > 0)
		{
			val = val.Trim();
			val = val.Replace("\\0", "\n");
			if (_dicString[(int)lang].ContainsKey(key))
			{
				_dicString[(int)lang][key] = val;
			}
			else
			{
				_dicString[(int)lang].Add(key, val);
			}
		}
	}

	public void UpdateStrings(string key, string korean, string english, string simplifiedChinese, string japanese, string german, string traditionalChinse, string french, string spanish, string polish, string turkish, string indonesian, string spanishAx)
	{
		UpdateString(key, LangOptManager.LANG_OPT.KOREAN, korean);
		UpdateString(key, LangOptManager.LANG_OPT.ENGLISH, english);
		UpdateString(key, LangOptManager.LANG_OPT.SIMPLIFIED_CHINESE, simplifiedChinese);
		UpdateString(key, LangOptManager.LANG_OPT.JAPANESE, japanese);
		UpdateString(key, LangOptManager.LANG_OPT.GERMAN, german);
		UpdateString(key, LangOptManager.LANG_OPT.TRADITIONAL_CHINESE, traditionalChinse);
		UpdateString(key, LangOptManager.LANG_OPT.FRENCH, french);
		UpdateString(key, LangOptManager.LANG_OPT.SPANISH, spanish);
		UpdateString(key, LangOptManager.LANG_OPT.POLISH, polish);
		UpdateString(key, LangOptManager.LANG_OPT.TURKISH, turkish);
		UpdateString(key, LangOptManager.LANG_OPT.INDONESIAN, indonesian);
		UpdateString(key, LangOptManager.LANG_OPT.SPANISH_AXESO5, spanishAx);
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/string.txt");
		CSVLoader cSVLoader = new CSVLoader();
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader.SecuredLoad(text2))
		{
			if (!cSVLoader.Load(text2))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text2);
				return false;
			}
			if (!cSVLoader.SecuredSave(text2))
			{
				Debug.LogError("ERROR, Load success " + text2 + " but save secured failed");
			}
		}
		ParseData(cSVLoader);
		return true;
	}

	private void Start()
	{
	}

	public void Load()
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadFromWWW());
		}
		else
		{
			isLoaded = LoadFromLocalFileSystem();
		}
	}

	public string GetEx(string key)
	{
		if (!_dicString[LangOptManager.Instance.LangOpt].ContainsKey(key))
		{
			return string.Empty;
		}
		return _dicString[LangOptManager.Instance.LangOpt][key];
	}

	public string Get(string key)
	{
		if (!_dicString[LangOptManager.Instance.LangOpt].ContainsKey(key))
		{
			if (key != "FIRE_BOMB")
				Debug.LogError("ERROR, Fail to find string : " + key);
			return string.Empty;
		}
		return _dicString[LangOptManager.Instance.LangOpt][key];
	}

	public string Get(string key, string defValue)
	{
		if (!_dicString[LangOptManager.Instance.LangOpt].ContainsKey(key))
		{
			return defValue;
		}
		return Get(key);
	}

	public string Get(string key, LangOptManager.LANG_OPT opt)
	{
		if (!_dicString[(int)opt].ContainsKey(key))
		{
			Debug.LogError("ERROR, Fail to find string : " + key);
			return string.Empty;
		}
		return _dicString[(int)opt][key];
	}
}
