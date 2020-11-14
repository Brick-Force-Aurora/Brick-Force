using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class WordFilter : MonoBehaviour
{
	private List<string>[] badWords;

	private char[] blind = new char[8]
	{
		'!',
		'@',
		'#',
		'$',
		'%',
		'^',
		'&',
		'*'
	};

	private char[] ignore = new char[43]
	{
		'/',
		'`',
		' ',
		'.',
		',',
		';',
		'\'',
		'"',
		'?',
		'\\',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9',
		'0',
		'!',
		'@',
		'#',
		'$',
		'%',
		'^',
		'&',
		'*',
		'(',
		')',
		'~',
		'-',
		'_',
		'+',
		'=',
		'{',
		'}',
		'[',
		']',
		'<',
		'>',
		'|',
		':'
	};

	public bool displayReadString = true;

	private bool isLoaded;

	private static WordFilter _instance;

	public bool IsLoaded => isLoaded;

	public static WordFilter Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(WordFilter)) as WordFilter);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get WordFilter Instance");
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

	private IEnumerator LoadFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/wordfilter.txt.cooked";
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
		badWords = new List<string>[12];
		for (int i = 0; i < badWords.Length; i++)
		{
			badWords[i] = new List<string>();
		}
		for (int j = 0; j < csvLoader.Rows; j++)
		{
			for (int k = 0; k < 12; k++)
			{
				csvLoader.ReadValue(k, j, string.Empty, out string Value);
				Value = Value.Trim();
				if (Value.Length > 0)
				{
					if (displayReadString)
					{
						Debug.Log(" Badword: " + Value);
					}
					badWords[k].Add(Value);
				}
			}
		}
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/wordfilter.txt");
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
		SortByDescendingLength();
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

	private void SortByDescendingLength()
	{
		for (int i = 0; i < badWords.Length; i++)
		{
			for (int j = 0; j < badWords[i].Count; j++)
			{
				for (int k = j + 1; k < badWords[i].Count; k++)
				{
					if (badWords[i][j].Length < badWords[i][k].Length)
					{
						string value = badWords[i][j];
						badWords[i][j] = badWords[i][k];
						badWords[i][k] = value;
					}
				}
			}
		}
	}

	private void Update()
	{
	}

	public string Filter(string input)
	{
		return _Filter(input);
	}

	private string _Filter(string input)
	{
		int langOpt = LangOptManager.Instance.LangOpt;
		for (int i = 0; i < badWords[langOpt].Count; i++)
		{
			string text = input.ToLower();
			string text2 = badWords[langOpt][i].ToLower();
			if (text.Contains(text2))
			{
				string text3 = string.Empty;
				for (int j = 0; j < text2.Length; j++)
				{
					text3 += blind[Random.Range(0, blind.Length)];
				}
				input = text.Replace(text2, text3);
			}
		}
		return input;
	}

	public string IgnoreFilter(string input)
	{
		input = _Filter(input);
		int num = 0;
		List<int> list = new List<int>();
		List<char> list2 = new List<char>();
		while (num >= 0)
		{
			num = input.IndexOfAny(ignore);
			if (num >= 0)
			{
				list.Add(num);
				list2.Add(input[num]);
				input = input.Remove(num, 1);
			}
		}
		input = _Filter(input);
		StringBuilder stringBuilder = new StringBuilder(input);
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			stringBuilder.Insert(list[num2], list2[num2]);
		}
		return stringBuilder.ToString();
	}

	public string CheckBadword(string input)
	{
		string text = _CheckBadword(input);
		if (text.Length > 0)
		{
			return text;
		}
		int num = 0;
		while (num >= 0)
		{
			num = input.IndexOfAny(ignore);
			if (num >= 0)
			{
				input = input.Remove(num, 1);
			}
		}
		text = _CheckBadword(input);
		if (text.Length > 0)
		{
			return text;
		}
		return string.Empty;
	}

	private string _CheckBadword(string input)
	{
		int langOpt = LangOptManager.Instance.LangOpt;
		for (int i = 0; i < badWords[langOpt].Count; i++)
		{
			if (input.Contains(badWords[langOpt][i]))
			{
				return badWords[langOpt][i];
			}
		}
		return string.Empty;
	}
}
