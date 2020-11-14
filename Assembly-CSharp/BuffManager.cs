using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
	public const int BF_NON_NMCAFE_USER = 0;

	public const int BF_NMCAFE_USER_CHECKIN = 1;

	public BuffDesc[] why;

	public Texture2D[] icons;

	private bool isLoaded;

	private Dictionary<int, TBuff> dic;

	private Dictionary<string, TBuff> dicByName;

	private static BuffManager _instance;

	public int netCafeCode;

	public bool isPcBangShowDialog;

	public bool IsLoaded => isLoaded;

	public static BuffManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BuffManager)) as BuffManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BuffManager Instance");
				}
			}
			return _instance;
		}
	}

	public Texture2D getPointUpTex()
	{
		return icons[0];
	}

	public Texture2D getXpUpTex()
	{
		return icons[1];
	}

	public Texture2D getLuckUpTex()
	{
		return icons[2];
	}

	private bool Add(int index, string name, TBuff buff)
	{
		if (dic.ContainsKey(index) || dicByName.ContainsKey(name))
		{
			return false;
		}
		dic.Add(index, buff);
		dicByName.Add(name, buff);
		return true;
	}

	public BuffDesc[] ToWhyArray(long mask)
	{
		List<BuffDesc> list = new List<BuffDesc>();
		long num = 1L;
		for (int i = 0; i < 64; i++)
		{
			if ((num & mask) != 0L && !BuildOption.Instance.Props.Name.Contains("Infernum") && !why[i].icon.name.Contains("premium"))
			{
				list.Add(why[i]);
			}
			num <<= 1;
		}
		return list.ToArray();
	}

	public TBuff Get(int index)
	{
		if (!dic.ContainsKey(index))
		{
			return null;
		}
		return dic[index];
	}

	public TBuff Get(string name)
	{
		if (!dicByName.ContainsKey(name))
		{
			return null;
		}
		return dicByName[name];
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		dic = new Dictionary<int, TBuff>();
		dicByName = new Dictionary<string, TBuff>();
	}

	public void Load()
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadAllFromWWW());
		}
		else
		{
			isLoaded = LoadFromLocalFileSystem();
		}
	}

	private void Update()
	{
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/buff.txt");
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

	public void ExportBuffs()
	{
	}

	private IEnumerator LoadAllFromWWW()
	{
		bool loaded = false;
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/buff.txt.cooked";
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
					loaded = true;
				}
			}
		}
		if (!loaded)
		{
			Debug.LogError("Fail to download " + url);
		}
		isLoaded = loaded;
	}

	private void Parse(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, -1, out int Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, def: false, out bool Value3);
			csvLoader.ReadValue(3, i, def: false, out bool Value4);
			csvLoader.ReadValue(4, i, def: false, out bool Value5);
			csvLoader.ReadValue(5, i, 0f, out float Value6);
			Value2.Trim();
			if (!Add(Value, Value2, new TBuff(Value, Value3, Value4, Value5, Value6)))
			{
				Debug.LogError("Fail to add buff : " + Value.ToString());
			}
		}
	}

	public bool IsChannelBuff()
	{
		if (ChannelManager.Instance.CurChannel.XpBonus != 0)
		{
			return true;
		}
		if (ChannelManager.Instance.CurChannel.FpBonus != 0)
		{
			return true;
		}
		return false;
	}

	public BuffDesc GetBuffDesc(BuffDesc.WHY bufftype)
	{
		if (BuffDesc.WHY.ITEM > bufftype)
		{
			return null;
		}
		if (BuffDesc.WHY.PC_BANG < bufftype)
		{
			return null;
		}
		return why[(int)bufftype];
	}

	public bool IsPCBangBuff()
	{
		return netCafeCode == 1;
	}
}
