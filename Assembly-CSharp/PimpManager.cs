using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PimpManager : MonoBehaviour
{
	private static PimpManager instance;

	private float[,,] pimpVals;

	public static PimpManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(PimpManager)) as PimpManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the PimpManager Instance");
				}
			}
			return instance;
		}
	}

	private void Start()
	{
		int num = 13;
		int num2 = 13;
		pimpVals = new float[num, num2, 10];
		Load();
	}

	private void Update()
	{
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void updateValue(int cat, int prop, int lv, float val)
	{
		pimpVals[cat, prop, lv] = val;
	}

	public float getValue(int cat, int prop, int lv)
	{
		return pimpVals[cat, prop, lv];
	}

	public void Load()
	{
		Property props = BuildOption.Instance.Props;
		if (props.loadShopTxt)
		{
			if (props.isWebPlayer)
			{
				StartCoroutine(LoadFromWWW());
			}
			else
			{
				LoadFromLocalFileSystem();
			}
		}
	}

	private IEnumerator LoadFromWWW()
	{
		bool Loaded = false;
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/pimp.txt.cooked";
		WWW wwwPimp = new WWW(url);
		yield return (object)wwwPimp;
		using (MemoryStream stream = new MemoryStream(wwwPimp.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParsePimp(csvLoader);
					Loaded = true;
				}
			}
		}
		if (!Loaded)
		{
			Debug.LogError("Fail to download " + url);
		}
	}

	public bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/pimp.txt");
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
		ParsePimp(cSVLoader);
		return true;
	}

	private void ParsePimp(CSVLoader csvLoader)
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
			int cat = Convert.ToInt32(Value);
			int prop = Convert.ToInt32(Value2);
			int num = Convert.ToInt32(Value3);
			float val = (float)Convert.ToDouble(Value4);
			updateValue(cat, prop, num - 1, val);
		}
	}
}
