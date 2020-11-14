using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelUpCompensationManager : MonoBehaviour
{
	private static LevelUpCompensationManager instance;

	private List<LevelUpCompensation> levelupCompens;

	public static LevelUpCompensationManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(LevelUpCompensationManager)) as LevelUpCompensationManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the LevelUpCompensation Instance");
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		levelupCompens = new List<LevelUpCompensation>();
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
			LoadFromLocalFileSystem();
		}
	}

	private IEnumerator LoadFromWWW()
	{
		bool Loaded = false;
		Property prop = BuildOption.Instance.Props;
		string tempName = "/BfData/Template/LevelupCompensation.txt.cooked";
		string url = "http://" + prop.GetResourceServer + tempName;
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					Parse(csvLoader);
					Loaded = true;
				}
			}
		}
		if (!Loaded)
		{
			Debug.LogError("Fail to download " + url);
		}
	}

	private bool LoadFromLocalFileSystems()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string path = "Template/LevelupCompensation.txt";
		string text2 = Path.Combine(text, path);
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
		Parse(cSVLoader);
		return true;
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string path = "Template/LevelupCompensation.txt";
		string text2 = Path.Combine(text, path);
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
		Parse(cSVLoader);
		return true;
	}

	private void Parse(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value2.ToLower();
			Value3.Trim();
			Value3.ToLower();
			Value4.Trim();
			Value4.ToLower();
			int evt = Convert.ToInt32(Value);
			int opt = Convert.ToInt32(Value3);
			int amount = Convert.ToInt32(Value4);
			levelupCompens.Add(new LevelUpCompensation(evt, Value2, opt, amount));
		}
	}

	public LevelUpCompensation getCurCompensation(int lvl)
	{
		for (int i = 0; i < levelupCompens.Count; i++)
		{
			if (levelupCompens[i].evt == lvl)
			{
				return levelupCompens[i];
			}
		}
		return null;
	}

	public void Clear()
	{
		levelupCompens.Clear();
	}

	public void Add(int _evt, string _code, int _opt, int _isAmount)
	{
		levelupCompens.Add(new LevelUpCompensation(_evt, _code, _opt, _isAmount));
	}
}
