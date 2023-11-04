using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TItemManager : MonoBehaviour
{
	public enum WEAPON_CATEGORY
	{
		HEAVY,
		ASSAULT,
		SNIPER,
		SUB_MACHINE,
		HAND_GUN,
		MELEE,
		SPECIAL
	}

	private bool isLoaded;

	private bool isIconLoaded;

	private bool isWeaponByLoaded;

	private bool isMaterialLoaded;

	public int icnVersion = 1;

	public int wpnbyVersion = 1;

	public int mtlVersion = 1;

	public Material[] materials;

	public Texture2D[] icons;

	public Texture2D[] weaponBy;

	public Dictionary<string, TItem> dic;

	public GameObject[] prefabs;

	public string[] coatBodyCodes;

	public string[] onlyOneCountingCodes;

	private Queue<IconReq> qIconReq;

	private bool iconRequesting;

	private Dictionary<int, int> wpnBy2Slot;

	private Dictionary<int, int> wpnBy2Category;

	private static TItemManager instance;

	public bool IsLoaded => isLoaded;

	public bool IsIconLoaded => isIconLoaded;

	public bool IsWeaponByLoaded => isWeaponByLoaded;

	public bool IsMaterialLoaded => isMaterialLoaded;

	public static TItemManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(TItemManager)) as TItemManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the TItemManager Instance");
				}
			}
			return instance;
		}
	}

	public void getIconBundle(string fileName)
	{
		StartCoroutine(downloadICONS(fileName));
	}

	public void getWeaponByBundle(string fileName)
	{
		StartCoroutine(downloadWEAPONBY(fileName));
	}

	public void getMaterialBundle(string fileName)
	{
		StartCoroutine(downloadMATERIALS(fileName));
	}

	private IEnumerator downloadICONS(string fileName)
	{
		while (!Caching.ready)
		{
			yield return (object)null;
		}
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		using (WWW www = WWW.LoadFromCacheOrDownload(url, icnVersion))
		{
			yield return (object)www;
			AssetBundle bundle = www.assetBundle;
			UnityEngine.Object[] objs = bundle.LoadAll();
			icons = new Texture2D[objs.Length];
			for (int i = 0; i < objs.Length; i++)
			{
				Texture2D tex = objs[i] as Texture2D;
				if (tex != null)
				{
					icons[i] = tex;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs[i].name);
				}
			}
			isIconLoaded = true;
		}
	}

	private IEnumerator downloadWEAPONBY(string fileName)
	{
		while (!Caching.ready)
		{
			yield return (object)null;
		}
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		using (WWW www = WWW.LoadFromCacheOrDownload(url, wpnbyVersion))
		{
			yield return (object)www;
			AssetBundle bundle = www.assetBundle;
			UnityEngine.Object[] objs = bundle.LoadAll();
			weaponBy = new Texture2D[objs.Length];
			for (int i = 0; i < objs.Length; i++)
			{
				Texture2D tex = objs[i] as Texture2D;
				if (tex != null)
				{
					weaponBy[i] = tex;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs[i].name);
				}
			}
			isWeaponByLoaded = true;
		}
	}

	private IEnumerator downloadMATERIALS(string fileName)
	{
		while (!Caching.ready)
		{
			yield return (object)null;
		}
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		using (WWW www = WWW.LoadFromCacheOrDownload(url, mtlVersion))
		{
			yield return (object)www;
			AssetBundle bundle = www.assetBundle;
			UnityEngine.Object[] objs = bundle.LoadAll(typeof(Material));
			materials = new Material[objs.Length];
			for (int i = 0; i < objs.Length; i++)
			{
				Material mat = objs[i] as Material;
				if (mat != null)
				{
					materials[i] = mat;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs[i].name);
				}
			}
			isMaterialLoaded = true;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public T Get<T>(string key) where T : class
	{
		if (!dic.ContainsKey(key))
		{
			return (T)null;
		}
		return dic[key] as T;
	}

	public TItem GetSpecialItem2HaveFunction(string func)
	{
		int num = TItem.String2FunctionMask(func);
		if (num < 0)
		{
			return null;
		}
		foreach (KeyValuePair<string, TItem> item in dic)
		{
			if (item.Value.type == TItem.TYPE.SPECIAL)
			{
				TSpecial tSpecial = (TSpecial)item.Value;
				if (tSpecial.functionMask == num)
				{
					return item.Value;
				}
			}
			else if (item.Value.type == TItem.TYPE.ACCESSORY)
			{
				TAccessory tAccessory = (TAccessory)item.Value;
				if (tAccessory.functionMask == num)
				{
					return item.Value;
				}
			}
		}
		return null;
	}

	private void Add(string code, TItem tItem)
	{
		if (dic.ContainsKey(code))
		{
			Debug.LogError("ERROR, Duplicated item code " + code);
		}
		else
		{
			dic.Add(code, tItem);
		}
	}

	private void Start()
	{
		dic = new Dictionary<string, TItem>();
		wpnBy2Slot = new Dictionary<int, int>();
		wpnBy2Category = new Dictionary<int, int>();
		qIconReq = new Queue<IconReq>();
		iconRequesting = false;
	}

	private IEnumerator LoadIcon(IconReq req)
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/" + req.iconPath;
		req.CDN = new WWW(url);
		yield return (object)req.CDN;
		TItem tItem = Get<TItem>(req.code);
		if (tItem != null)
		{
			Texture2D icon = new Texture2D(167, 91, TextureFormat.RGBA32, mipmap: false)
			{
				wrapMode = TextureWrapMode.Clamp
			};
			if (icon.LoadImage(req.CDN.bytes))
			{
				tItem.SetIcon(icon);
				tItem.CurIcon().Apply();
			}
		}
		req.CDN.Dispose();
		req.CDN = null;
		iconRequesting = false;
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
			isLoaded = (LoadCostumeFromLocalFileSystem() && LoadWeaponFromLocalFileSystem() && LoadAccessoryFromLocalFileSystem() && LoadCharacterFromLocalFileSystem() && LoadSpecialFromLocalFileSystem() && LoadUpgradeFromLocalFileSystem() && LoadBundleFromLocalFileSystem());
		}
	}

	public void ExportItems()
	{
	}

	private IEnumerator LoadAllFromWWW()
	{
		bool characterLoaded = false;
		bool accessoryLoaded = false;
		bool costumeLoaded = false;
		bool weaponLoaded = false;
		bool specialLoaded = false;
		bool upgradeLoaded = false;
		bool bundleLoaded = false;
		Property prop = BuildOption.Instance.Props;
		string url7 = "http://" + prop.GetResourceServer + "/BfData/Template/character.txt.cooked";
		WWW wwwCharacter = new WWW(url7);
		yield return (object)wwwCharacter;
		using (MemoryStream input = new MemoryStream(wwwCharacter.bytes))
		{
			using (BinaryReader reader2 = new BinaryReader(input))
			{
				CSVLoader csvLoader7 = new CSVLoader();
				if (csvLoader7.SecuredLoadFromBinaryReader(reader2))
				{
					ParseCharacter(csvLoader7);
					characterLoaded = true;
				}
			}
		}
		if (!characterLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/costume.txt.cooked";
		WWW wwwCostume = new WWW(url7);
		yield return (object)wwwCostume;
		using (MemoryStream input2 = new MemoryStream(wwwCostume.bytes))
		{
			using (BinaryReader reader3 = new BinaryReader(input2))
			{
				CSVLoader csvLoader6 = new CSVLoader();
				if (csvLoader6.SecuredLoadFromBinaryReader(reader3))
				{
					ParseCostume(csvLoader6);
					costumeLoaded = true;
				}
			}
		}
		if (!costumeLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/accessory.txt.cooked";
		WWW wwwAccessory = new WWW(url7);
		yield return (object)wwwAccessory;
		using (MemoryStream input3 = new MemoryStream(wwwAccessory.bytes))
		{
			using (BinaryReader reader4 = new BinaryReader(input3))
			{
				CSVLoader csvLoader5 = new CSVLoader();
				if (csvLoader5.SecuredLoadFromBinaryReader(reader4))
				{
					ParseAccessory(csvLoader5);
					accessoryLoaded = true;
				}
			}
		}
		if (!accessoryLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/weapon.txt.cooked";
		WWW wwwWeapon = new WWW(url7);
		yield return (object)wwwWeapon;
		using (MemoryStream input4 = new MemoryStream(wwwWeapon.bytes))
		{
			using (BinaryReader reader5 = new BinaryReader(input4))
			{
				CSVLoader csvLoader4 = new CSVLoader();
				if (csvLoader4.SecuredLoadFromBinaryReader(reader5))
				{
					ParseWeapon(csvLoader4);
					weaponLoaded = true;
				}
			}
		}
		if (!weaponLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/special.txt.cooked";
		WWW wwwSpecial = new WWW(url7);
		yield return (object)wwwSpecial;
		using (MemoryStream input5 = new MemoryStream(wwwSpecial.bytes))
		{
			using (BinaryReader reader6 = new BinaryReader(input5))
			{
				CSVLoader csvLoader3 = new CSVLoader();
				if (csvLoader3.SecuredLoadFromBinaryReader(reader6))
				{
					ParseSpecial(csvLoader3);
					specialLoaded = true;
				}
			}
		}
		if (!specialLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/upgrade.txt.cooked";
		WWW wwwUpgrade = new WWW(url7);
		yield return (object)wwwUpgrade;
		using (MemoryStream input6 = new MemoryStream(wwwUpgrade.bytes))
		{
			using (BinaryReader reader7 = new BinaryReader(input6))
			{
				CSVLoader csvLoader2 = new CSVLoader();
				if (csvLoader2.SecuredLoadFromBinaryReader(reader7))
				{
					ParseUpgrade(csvLoader2);
					upgradeLoaded = true;
				}
			}
		}
		if (!upgradeLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		url7 = "http://" + prop.GetResourceServer + "/BfData/Template/bundle.txt.cooked";
		WWW wwwBundle = new WWW(url7);
		yield return (object)wwwBundle;
		using (MemoryStream stream = new MemoryStream(wwwBundle.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParseBundle(csvLoader);
					bundleLoaded = true;
				}
			}
		}
		if (!bundleLoaded)
		{
			Debug.LogError("Fail to download " + url7);
		}
		isLoaded = (costumeLoaded && characterLoaded && weaponLoaded && accessoryLoaded && specialLoaded && upgradeLoaded && bundleLoaded);
	}

	private void ParseBundle(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, def: false, out bool Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, 100, out int Value8);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value7.Trim();
			int ct = TItem.String2Type(Value4);
			Add(Value, new TBundle(Value, Value2, FindIcon(Value3), ct, Value5, Value6, Convert.ToInt32(Value7), Value8));
		}
	}

	public void UpdateBundle(string code, string nameKey, string iconPath, string descKey)
	{
		TBundle tBundle = Get<TBundle>(code);
		if (tBundle == null)
		{
			Debug.LogError("Fail to find generic bundle item with " + code);
		}
		else
		{
			tBundle.name = nameKey;
			tBundle.comment = descKey;
			qIconReq.Enqueue(new IconReq(code, iconPath));
		}
	}

	private bool LoadBundleFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/bundle.txt");
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
		ParseBundle(cSVLoader);
		return true;
	}

	private void ParseUpgrade(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, -1, out int Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, -1, out int Value7);
			csvLoader.ReadValue(7, i, -1, out int Value8);
			csvLoader.ReadValue(8, i, -1, out int Value9);
			csvLoader.ReadValue(9, i, string.Empty, out string Value10);
			csvLoader.ReadValue(10, i, 100, out int Value11);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value6.Trim();
			Value6.ToLower();
			Value10.Trim();
			int ct = TItem.String2Type(Value4);
			Add(Value, new TUpgrade(Value, Value2, FindIcon(Value3), ct, Value5, Value6, Value7, Value8, Value9, Value10, Value11));
		}
	}

	private bool LoadUpgradeFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/upgrade.txt");
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
		ParseUpgrade(cSVLoader);
		return true;
	}

	private void ParseSpecial(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, def: false, out bool Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, def: false, out bool Value8);
			csvLoader.ReadValue(8, i, string.Empty, out string Value9);
			csvLoader.ReadValue(9, i, def: false, out bool Value10);
			csvLoader.ReadValue(10, i, string.Empty, out string Value11);
			csvLoader.ReadValue(11, i, string.Empty, out string Value12);
			csvLoader.ReadValue(12, i, 100, out int Value13);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value6 = Value6.Trim();
			Value6 = Value6.ToLower();
			Value7.Trim();
			Value11.Trim();
			int ct = TItem.String2Type(Value4);
			Add(Value, new TSpecial(Value, Value2, FindIcon(Value3), ct, Value5, TItem.String2FunctionMask(Value6), Value7, Value8, Value9, Value10, Convert.ToInt32(Value11), Value12, Value13));
		}
	}

	private bool LoadSpecialFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/special.txt");
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
		ParseSpecial(cSVLoader);
		return true;
	}

	private void ParseCharacter(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, def: true, out bool Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			csvLoader.ReadValue(8, i, string.Empty, out string Value9);
			csvLoader.ReadValue(9, i, def: false, out bool Value10);
			csvLoader.ReadValue(10, i, string.Empty, out string Value11);
			csvLoader.ReadValue(11, i, string.Empty, out string Value12);
			csvLoader.ReadValue(12, i, string.Empty, out string Value13);
			csvLoader.ReadValue(13, i, string.Empty, out string Value14);
			csvLoader.ReadValue(14, i, string.Empty, out string Value15);
			csvLoader.ReadValue(15, i, string.Empty, out string Value16);
			csvLoader.ReadValue(16, i, 100, out int Value17);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value6.Trim();
			Value7.Trim();
			Value8.Trim();
			Value9.Trim();
			Value12.Trim();
			Value13.Trim();
			Value14.Trim();
			Value15.Trim();
			Value16.Trim();
			int ct = TItem.String2Type(Value4);
			Add(Value, new TCharacter(Value, Value2, FindIcon(Value3), ct, Value5, Value6, Value7, Value8, BuffManager.Instance.Get(Value9), Value10, Value11, Convert.ToInt32(Value12), FindMaterial(Value13), Value14, Value15, Value16, Value17));
		}
	}

	private bool LoadCharacterFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/character.txt");
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
		ParseCharacter(cSVLoader);
		return true;
	}

	private void ParseCostume(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, def: true, out bool Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			csvLoader.ReadValue(8, i, string.Empty, out string Value9);
			csvLoader.ReadValue(9, i, string.Empty, out string Value10);
			csvLoader.ReadValue(10, i, string.Empty, out string Value11);
			csvLoader.ReadValue(11, i, string.Empty, out string Value12);
			csvLoader.ReadValue(12, i, string.Empty, out string Value13);
			csvLoader.ReadValue(13, i, string.Empty, out string Value14);
			csvLoader.ReadValue(14, i, string.Empty, out string Value15);
			csvLoader.ReadValue(15, i, 0f, out float Value16);
			csvLoader.ReadValue(16, i, def: false, out bool Value17);
			csvLoader.ReadValue(17, i, string.Empty, out string Value18);
			csvLoader.ReadValue(18, i, 0, out int Value19);
			csvLoader.ReadValue(19, i, string.Empty, out string Value20);
			csvLoader.ReadValue(20, i, def: false, out bool Value21);
			csvLoader.ReadValue(21, i, string.Empty, out string Value22);
			csvLoader.ReadValue(22, i, string.Empty, out string Value23);
			csvLoader.ReadValue(23, i, string.Empty, out string Value24);
			csvLoader.ReadValue(24, i, string.Empty, out string Value25);
			csvLoader.ReadValue(25, i, string.Empty, out string Value26);
			csvLoader.ReadValue(26, i, string.Empty, out string Value27);
			csvLoader.ReadValue(27, i, 100, out int Value28);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value5.Trim();
			Value5.ToLower();
			Value6.Trim();
			Value6.ToLower();
			Value8.Trim();
			Value9.Trim();
			Value10.Trim();
			Value11.Trim();
			Value12.Trim();
			Value13.Trim();
			Value14.Trim();
			Value15.Trim();
			Value14 = Value14.ToLower();
			Value24.Trim();
			Value25.Trim();
			Value26.Trim();
			Value27.Trim();
			Value20 = Value20.ToLower();
			int num = TItem.String2Type(Value4);
			int ck = TItem.String2Kind(num, Value5);
			int upCat = TItem.String2UpgradeCategory(Value20);
			Add(Value, new TCostume(Value, Value2, Value10, Value11, FindMaterial(Value8), FindMaterial(Value9), FindIcon(Value3), num, ck, Value7, TItem.String2Slot(Value6), Value12, BuffManager.Instance.Get(Value13), Value17, Value18, Value19, upCat, Value21, Value23, FindMaterial(Value22), Convert.ToInt32(Value24), Value25, Value26, Value27, TItem.String2FunctionMask(Value14), Value16, FindIcon(Value15), Value28));
		}
	}

	private bool LoadCostumeFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/costume.txt");
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
		ParseCostume(cSVLoader);
		return true;
	}

	private void ParseWeapon(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			csvLoader.ReadValue(8, i, def: true, out bool Value9);
			csvLoader.ReadValue(9, i, string.Empty, out string Value10);
			csvLoader.ReadValue(10, i, string.Empty, out string Value11);
			csvLoader.ReadValue(11, i, string.Empty, out string Value12);
			csvLoader.ReadValue(12, i, string.Empty, out string Value13);
			csvLoader.ReadValue(13, i, string.Empty, out string Value14);
			csvLoader.ReadValue(14, i, def: false, out bool Value15);
			csvLoader.ReadValue(15, i, string.Empty, out string Value16);
			csvLoader.ReadValue(16, i, -1, out int Value17);
			csvLoader.ReadValue(17, i, string.Empty, out string Value18);
			csvLoader.ReadValue(18, i, def: false, out bool Value19);
			csvLoader.ReadValue(19, i, string.Empty, out string Value20);
			csvLoader.ReadValue(20, i, string.Empty, out string Value21);
			csvLoader.ReadValue(21, i, string.Empty, out string Value22);
			csvLoader.ReadValue(22, i, string.Empty, out string Value23);
			csvLoader.ReadValue(23, i, def: false, out bool Value24);
			csvLoader.ReadValue(24, i, 100, out int Value25);
			Value8 = Value8.Trim();
			Value8 = Value8.ToLower();
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value5.ToLower();
			Value6.Trim();
			Value6.ToLower();
			Value7.Trim();
			Value7.ToLower();
			Value10.Trim();
			Value11.Trim();
			Value12.Trim();
			Value13.Trim();
			Value14.Trim();
			Value20.Trim();
			Value21.Trim();
			Value22.Trim();
			Value23.Trim();
			Value18 = Value18.ToLower();
			int num = TItem.String2Type(Value5);
			int ck = TItem.String2Kind(num, Value6);
			int num2 = TWeapon.String2WeaponCategory(Value8);
			int upCat = TItem.String2UpgradeCategory(Value18);
			TWeapon tWeapon = new TWeapon(Value, Value2, Value12, FindPrefab(Value10), FindPrefab(Value11), FindIcon(Value3), FindIcon(Value4), num, ck, num2, Value9, TItem.String2Slot(Value7), Value13, BuffManager.Instance.Get(Value14), Value15, Value16, Value17, upCat, Value19, Convert.ToInt32(Value20), Value21, Value22, Value23, Value24, Value25);
			Add(Value, tWeapon);
			if (tWeapon.CurPrefab() != null)
			{
				WeaponFunction component = tWeapon.CurPrefab().GetComponent<WeaponFunction>();
				if (null == component)
				{
					Debug.LogError(Value + " weapon does not have WeaponFunction");
				}
				else
				{
					if (!wpnBy2Slot.ContainsKey((int)component.weaponBy))
					{
						wpnBy2Slot.Add((int)component.weaponBy, (int)tWeapon.slot);
					}
					if (!wpnBy2Category.ContainsKey((int)component.weaponBy))
					{
						wpnBy2Category.Add((int)component.weaponBy, num2);
					}
				}
			}
		}
	}

	public int WeaponBy2Slot(int weaponBy)
	{
		if (wpnBy2Slot.ContainsKey(weaponBy))
		{
			return wpnBy2Slot[weaponBy];
		}
		return -1;
	}

	public int WeaponBy2Category(int weaponBy)
	{
		if (wpnBy2Category.ContainsKey(weaponBy))
		{
			return wpnBy2Category[weaponBy];
		}
		return -1;
	}

	private bool LoadWeaponFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/weapon.txt");
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

		cSVLoader.Save("weapon.txt");
		ParseWeapon(cSVLoader);
		return true;
	}

	private void ParseAccessory(CSVLoader csvLoader)
	{
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, def: true, out bool Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			csvLoader.ReadValue(8, i, string.Empty, out string Value9);
			csvLoader.ReadValue(9, i, string.Empty, out string Value10);
			csvLoader.ReadValue(10, i, string.Empty, out string Value11);
			csvLoader.ReadValue(11, i, def: false, out bool Value12);
			csvLoader.ReadValue(12, i, string.Empty, out string Value13);
			csvLoader.ReadValue(13, i, string.Empty, out string Value14);
			csvLoader.ReadValue(14, i, string.Empty, out string Value15);
			csvLoader.ReadValue(15, i, 0, out int Value16);
			csvLoader.ReadValue(16, i, 0f, out float Value17);
			csvLoader.ReadValue(17, i, string.Empty, out string Value18);
			csvLoader.ReadValue(18, i, string.Empty, out string Value19);
			csvLoader.ReadValue(19, i, string.Empty, out string Value20);
			csvLoader.ReadValue(20, i, string.Empty, out string Value21);
			csvLoader.ReadValue(21, i, string.Empty, out string Value22);
			csvLoader.ReadValue(22, i, 100, out int Value23);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value4.ToLower();
			Value5.Trim();
			Value5.ToLower();
			Value6.Trim();
			Value6.ToLower();
			Value8.Trim();
			Value9.Trim();
			Value10.Trim();
			Value11.Trim();
			Value19.Trim();
			Value20.Trim();
			Value21.Trim();
			Value22.Trim();
			Value14 = Value14.Trim();
			Value14 = Value14.ToLower();
			Value15.Trim();
			Value18 = Value18.ToLower();
			int num = TItem.String2Type(Value4);
			int ck = TItem.String2Kind(num, Value5);
			int upCat = TItem.String2UpgradeCategory(Value18);
			Add(Value, new TAccessory(Value, Value2, Value9, FindPrefab(Value8), FindIcon(Value3), num, ck, Value7, TItem.String2Slot(Value6), Value10, BuffManager.Instance.Get(Value11), Value12, Value13, TItem.String2FunctionMask(Value14), Value16, Value17, upCat, Convert.ToInt32(Value19), Value20, Value21, Value22, FindIcon(Value15), Value23));
		}
	}

	private bool LoadAccessoryFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/accessory.txt");
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
		ParseAccessory(cSVLoader);
		return true;
	}

	public Texture2D GetWeaponBy(int wpn)
	{
		if (0 > wpn || wpn >= weaponBy.Length)
		{
			return null;
		}
		return weaponBy[wpn];
	}

	private Texture2D FindIcon(string iconName)
	{
		if (iconName.Length <= 0)
		{
			return null;
		}
		if (BuildOption.Instance.Props.useUskWeaponIcon)
		{
			Texture2D texture2D = UskManager.Instance.Get(iconName) as Texture2D;
			if (texture2D != null)
			{
				return texture2D;
			}
		}
		Texture2D[] array = icons;
		foreach (Texture2D texture2D2 in array)
		{
			if (texture2D2.name == iconName)
			{
				return texture2D2;
			}
		}
		return null;
	}

	public GameObject FindPrefab(string prefabName)
	{
		if (prefabName.Length <= 0)
		{
			return null;
		}
		GameObject[] array = prefabs;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name == prefabName)
			{
				return gameObject;
			}
		}
		return null;
	}

	private Material FindMaterial(string matName)
	{
		if (matName.Length <= 0)
		{
			return null;
		}
		Material[] array = materials;
		foreach (Material material in array)
		{
			if (material.name == matName)
			{
				return material;
			}
		}
		Debug.LogError("Fail to find material " + matName);
		return null;
	}

	private void Update()
	{
		if (!iconRequesting && qIconReq.Count > 0)
		{
			iconRequesting = true;
			StartCoroutine(LoadIcon(qIconReq.Dequeue()));
		}
	}

	public TWeapon[] GetCompleteWeaponArray()
	{
		List<TWeapon> list = new List<TWeapon>();
		foreach (KeyValuePair<string, TItem> item2 in dic)
		{
			if (item2.Value.type == TItem.TYPE.WEAPON && item2.Value.CurIcon() != null)
			{
				list.Add((TWeapon)item2.Value);
			}
		}
		foreach (KeyValuePair<string, TItem> item3 in dic)
		{
			if (item3.Value.code == "s09" && item3.Value.CurIcon() != null)
			{
				TWeapon item = new TWeapon(item3.Value.code, item3.Value.name, string.Empty, null, null, item3.Value.CurIcon(), item3.Value.CurIcon(), 0, 0, 0, itemTakeoffable: false, TItem.SLOT.UPPER, string.Empty, null, itemDiscomposable: false, string.Empty, 0, 0, basic: false, 0, "none", "none", "none", twohands: false, item3.Value._StarRate);
				list.Add(item);
			}
		}
		return list.ToArray();
	}
}
