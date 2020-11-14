using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomboxItemManager : MonoBehaviour
{
	private static RandomboxItemManager instance;

	private c_Gachapon[] c_Gachapons;

	private Dictionary<string, c_Gachapon> dic;

	public Texture2D[] icons;

	private static string[] types = new string[4]
	{
		"Weapon",
		"Costume",
		"Brick",
		"Etc"
	};

	private bool Loaded;

	public static RandomboxItemManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(RandomboxItemManager)) as RandomboxItemManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the RandomboxItemManager Instance");
				}
			}
			return instance;
		}
	}

	public bool IsLoaded => Loaded;

	public c_Gachapon c_GetGachapon(int id)
	{
		return c_Gachapons[id];
	}

	public c_Gachapon GetGachaponByCode(string code)
	{
		if (dic.ContainsKey(code))
		{
			return dic[code];
		}
		return null;
	}

	public void LoadAll()
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadAllFromWWW());
		}
		else
		{
			Loaded = c_LoadGachaponFromLocalFileSystem();
		}
	}

	private IEnumerator LoadAllFromWWW()
	{
		bool gachaponLoaded = false;
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/c_gachapon.txt.cooked";
		WWW wwwWave = new WWW(url);
		yield return (object)wwwWave;
		using (MemoryStream stream = new MemoryStream(wwwWave.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					c_ParseGachapon(csvLoader);
					gachaponLoaded = true;
				}
			}
		}
		if (!gachaponLoaded)
		{
			Debug.LogError("Fail to download " + url);
		}
		Loaded = gachaponLoaded;
	}

	private bool c_LoadGachaponFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/c_gachapon.txt");
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
		c_ParseGachapon(cSVLoader);
		return true;
	}

	private void c_ParseGachapon(CSVLoader csvLoader)
	{
		c_Gachapons = new c_Gachapon[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			c_Gachapons[i] = new c_Gachapon();
			c_Gachapons[i].items = new string[5];
			c_Gachapons[i].qualities = new int[5];
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			Value.Trim();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value6.Trim();
			c_Gachapons[i].code = Value;
			c_Gachapons[i].itemName = Value2;
			c_Gachapons[i].classType = String2Type(Value3);
			c_Gachapons[i].strtblCode = Value4;
			c_Gachapons[i].icon = FindIcon(Value5);
			c_Gachapons[i].brickPoint = Convert.ToInt32(Value6);
			int num = 6;
			for (int j = 0; j < 5; j++)
			{
				csvLoader.ReadValue(num, i, string.Empty, out string Value7);
				csvLoader.ReadValue(num + 1, i, string.Empty, out string Value8);
				Value7.Trim();
				Value8.Trim();
				c_Gachapons[i].items[j] = Value7;
				c_Gachapons[i].qualities[j] = Convert.ToInt32(Value8);
				num += 2;
			}
			Add(Value, c_Gachapons[i]);
		}
	}

	private void Add(string code, c_Gachapon gacha)
	{
		if (dic.ContainsKey(code))
		{
			Debug.LogError("ERROR, Duplicated gachapon code " + code);
		}
		else
		{
			dic.Add(code, gacha);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Awake()
	{
		dic = new Dictionary<string, c_Gachapon>();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public c_Gachapon[] GetGachaponsByCat(int tab)
	{
		List<c_Gachapon> list = new List<c_Gachapon>();
		foreach (KeyValuePair<string, c_Gachapon> item in dic)
		{
			if (item.Value.classType == tab)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	private Texture2D FindIcon(string iconName)
	{
		if (iconName.Length <= 0)
		{
			return null;
		}
		Texture2D[] array = icons;
		foreach (Texture2D texture2D in array)
		{
			if (texture2D.name == iconName)
			{
				return texture2D;
			}
		}
		return null;
	}

	public static int String2Type(string typeString)
	{
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i] == typeString)
			{
				return i;
			}
		}
		return -1;
	}
}
