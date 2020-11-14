using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class DefenseManager : MonoBehaviour
{
	private static DefenseManager instance;

	public float missionPlayTime;

	private WaveTable[] waveTables;

	private MonTable[] monTables;

	private UpgradeTable[] upgradeAtkTables;

	private UpgradeTable[] upgradeShockTables;

	private UpgradeTable[] upgradeChargeTables;

	private PurcharseItems[] purcharseItems;

	public int totalWaveTime;

	public int totalWave;

	private int addAtkPower;

	public int upgradeAtkVal;

	public int upgradeShockVal = -1;

	public int upgradeChargeVal = -1;

	private int coreLifeRed;

	private int coreLifeBlue;

	private int redPoint;

	private int bluePoint;

	private bool Loaded;

	private int curWave;

	private bool isBrickmode;

	public int shieldCount;

	public static DefenseManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(DefenseManager)) as DefenseManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the DefenseManager Instance");
				}
			}
			return instance;
		}
	}

	public float MissionPlayTime
	{
		get
		{
			return missionPlayTime;
		}
		set
		{
			missionPlayTime = value;
		}
	}

	public int AddAtkPower
	{
		get
		{
			return addAtkPower;
		}
		set
		{
			addAtkPower = value;
		}
	}

	public int CoreLifeRed
	{
		get
		{
			return coreLifeRed;
		}
		set
		{
			coreLifeRed = value;
			if (coreLifeRed < 0)
			{
				coreLifeRed = 0;
			}
		}
	}

	public int CoreLifeBlue
	{
		get
		{
			return coreLifeBlue;
		}
		set
		{
			coreLifeBlue = value;
			if (coreLifeBlue < 0)
			{
				coreLifeBlue = 0;
			}
		}
	}

	public int RedPoint
	{
		get
		{
			return redPoint;
		}
		set
		{
			redPoint = value;
		}
	}

	public int BluePoint
	{
		get
		{
			return bluePoint;
		}
		set
		{
			bluePoint = value;
		}
	}

	public bool IsLoaded => Loaded;

	public int CurWave
	{
		get
		{
			return curWave;
		}
		set
		{
			curWave = value;
		}
	}

	public bool IsBrickmode
	{
		get
		{
			return isBrickmode;
		}
		set
		{
			isBrickmode = value;
		}
	}

	public WaveTable GetWaveTable()
	{
		return waveTables[curWave];
	}

	public MonTable GetMonTable(int tblID)
	{
		return monTables[tblID];
	}

	public UpgradeTable GetUpgradeAtkTable()
	{
		return upgradeAtkTables[upgradeAtkVal];
	}

	public UpgradeTable GetUpgradeShockTable()
	{
		return upgradeShockTables[upgradeShockVal];
	}

	public UpgradeTable GetUpgradeChargeTable()
	{
		return upgradeChargeTables[upgradeChargeVal];
	}

	public UpgradeTable GetNextUpgradeAtkTable()
	{
		return upgradeAtkTables[upgradeAtkVal + 1];
	}

	public UpgradeTable GetNextUpgradeShockTable()
	{
		return upgradeShockTables[upgradeShockVal + 1];
	}

	public UpgradeTable GetNextUpgradeChargeTable()
	{
		return upgradeChargeTables[upgradeChargeVal + 1];
	}

	public int GetMaxMonTable()
	{
		return monTables.Length;
	}

	public int GetMaxAtkTable()
	{
		return upgradeAtkTables.Length;
	}

	public int GetMaxShockTable()
	{
		return upgradeShockTables.Length;
	}

	public int GetMaxChargeTable()
	{
		return upgradeChargeTables.Length;
	}

	public PurcharseItems GetPurcharseItem(int id)
	{
		return purcharseItems[id];
	}

	public void init()
	{
		isBrickmode = false;
		CurWave = 0;
		upgradeAtkVal = 0;
		upgradeShockVal = -1;
		upgradeChargeVal = -1;
		AddAtkPower = 0;
		shieldCount = 0;
		missionPlayTime = 0f;
		coreLifeRed = (coreLifeBlue = RoomManager.Instance.WeaponOption);
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
			Loaded = (LoadWaveTableFromLocalFileSystem() && LoadMonTableFromLocalFileSystem() && LoadUpgradeAtkTableFromLocalFileSystem() && LoadUpgradeShockTableFromLocalFileSystem() && LoadUpgradeChargeTableFromLocalFileSystem() && LoadPurcharseItemsFromLocalFileSystem());
		}
	}

	private IEnumerator LoadAllFromWWW()
	{
		bool waveLoaded = false;
		bool monLoaded = false;
		bool upgradeAtkLoaded = false;
		bool upgradeShockLoaded = false;
		bool upgradeChargeLoaded = false;
		bool purcharseItemsLoaded = false;
		Property prop = BuildOption.Instance.Props;
		string url6 = "http://" + prop.GetResourceServer + "/BfData/Template/wavetable.txt.cooked";
		WWW wwwWave = new WWW(url6);
		yield return (object)wwwWave;
		using (MemoryStream input = new MemoryStream(wwwWave.bytes))
		{
			using (BinaryReader reader2 = new BinaryReader(input))
			{
				CSVLoader csvLoader6 = new CSVLoader();
				if (csvLoader6.SecuredLoadFromBinaryReader(reader2))
				{
					ParseWaveTable(csvLoader6);
					waveLoaded = true;
				}
			}
		}
		if (!waveLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		url6 = "http://" + prop.GetResourceServer + "/BfData/Template/montable.txt.cooked";
		WWW wwwMon = new WWW(url6);
		yield return (object)wwwMon;
		using (MemoryStream input2 = new MemoryStream(wwwMon.bytes))
		{
			using (BinaryReader reader3 = new BinaryReader(input2))
			{
				CSVLoader csvLoader5 = new CSVLoader();
				if (csvLoader5.SecuredLoadFromBinaryReader(reader3))
				{
					ParseMonTable(csvLoader5);
					monLoaded = true;
				}
			}
		}
		if (!monLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		url6 = "http://" + prop.GetResourceServer + "/BfData/Template/upgradeatktable.txt.cooked";
		WWW wwwUpgradeAtk = new WWW(url6);
		yield return (object)wwwUpgradeAtk;
		using (MemoryStream input3 = new MemoryStream(wwwUpgradeAtk.bytes))
		{
			using (BinaryReader reader4 = new BinaryReader(input3))
			{
				CSVLoader csvLoader4 = new CSVLoader();
				if (csvLoader4.SecuredLoadFromBinaryReader(reader4))
				{
					ParseUpgradeAtkTable(csvLoader4);
					upgradeAtkLoaded = true;
				}
			}
		}
		if (!upgradeAtkLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		url6 = "http://" + prop.GetResourceServer + "/BfData/Template/upgradeshocktable.txt.cooked";
		WWW wwwUpgradeShock = new WWW(url6);
		yield return (object)wwwUpgradeShock;
		using (MemoryStream input4 = new MemoryStream(wwwUpgradeShock.bytes))
		{
			using (BinaryReader reader5 = new BinaryReader(input4))
			{
				CSVLoader csvLoader3 = new CSVLoader();
				if (csvLoader3.SecuredLoadFromBinaryReader(reader5))
				{
					ParseUpgradeShockTable(csvLoader3);
					upgradeShockLoaded = true;
				}
			}
		}
		if (!upgradeShockLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		url6 = "http://" + prop.GetResourceServer + "/BfData/Template/upgradechargetable.txt.cooked";
		WWW wwwUpgradeCharge = new WWW(url6);
		yield return (object)wwwUpgradeCharge;
		using (MemoryStream input5 = new MemoryStream(wwwUpgradeCharge.bytes))
		{
			using (BinaryReader reader6 = new BinaryReader(input5))
			{
				CSVLoader csvLoader2 = new CSVLoader();
				if (csvLoader2.SecuredLoadFromBinaryReader(reader6))
				{
					ParseUpgradeChargeTable(csvLoader2);
					upgradeChargeLoaded = true;
				}
			}
		}
		if (!upgradeChargeLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		url6 = "http://" + prop.GetResourceServer + "/BfData/Template/purcharseitemstable.txt.cooked";
		WWW wwwPurcharseItem = new WWW(url6);
		yield return (object)wwwPurcharseItem;
		using (MemoryStream stream = new MemoryStream(wwwPurcharseItem.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParsePurcharseItemsTable(csvLoader);
					purcharseItemsLoaded = true;
				}
			}
		}
		if (!purcharseItemsLoaded)
		{
			Debug.LogError("Fail to download " + url6);
		}
		Loaded = (waveLoaded && monLoaded && upgradeAtkLoaded && upgradeShockLoaded && upgradeChargeLoaded && purcharseItemsLoaded);
	}

	private void ParseWaveTable(CSVLoader csvLoader)
	{
		waveTables = new WaveTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			waveTables[i] = new WaveTable();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			Value.Trim();
			Value2.Trim();
			waveTables[i].numWave = Convert.ToInt32(Value);
			waveTables[i].interval = (float)Convert.ToDouble(Value2);
		}
	}

	private bool LoadWaveTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/wavetable.txt");
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
		ParseWaveTable(cSVLoader);
		return true;
	}

	private void ParseMonTable(CSVLoader csvLoader)
	{
		monTables = new MonTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			monTables[i] = new MonTable();
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
			monTables[i].name = Value;
			monTables[i].str = Value2;
			monTables[i].HP = Convert.ToInt32(Value3);
			monTables[i].MoveSpeed = (float)Convert.ToDouble(Value4);
			monTables[i].toCoreDmg = Convert.ToInt32(Value5);
			monTables[i].Dp = Convert.ToInt32(Value6);
		}
	}

	private bool LoadMonTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/montable.txt");
		CSVLoader cSVLoader = new CSVLoader();
		if (!cSVLoader.SecuredLoad(text2))
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
		ParseMonTable(cSVLoader);
		return true;
	}

	private void ParseUpgradeAtkTable(CSVLoader csvLoader)
	{
		upgradeAtkTables = new UpgradeTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			upgradeAtkTables[i] = new UpgradeTable();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			Value.Trim();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value6.Trim();
			Value7.Trim();
			Value8.Trim();
			upgradeAtkTables[i].Level = Convert.ToInt32(Value);
			upgradeAtkTables[i].AssultAtkVal = (float)Convert.ToDouble(Value2);
			upgradeAtkTables[i].SubmachineAtkVal = (float)Convert.ToDouble(Value3);
			upgradeAtkTables[i].SniperAtkVal = (float)Convert.ToDouble(Value4);
			upgradeAtkTables[i].HeavyAtkVal = (float)Convert.ToDouble(Value5);
			upgradeAtkTables[i].HandgunAtkVal = (float)Convert.ToDouble(Value6);
			upgradeAtkTables[i].SpecialAtkVal = (float)Convert.ToDouble(Value7);
			upgradeAtkTables[i].Price = Convert.ToInt32(Value8);
			if (i > 0)
			{
				upgradeAtkTables[i].AssultAtkVal += upgradeAtkTables[i - 1].AssultAtkVal;
				upgradeAtkTables[i].SubmachineAtkVal += upgradeAtkTables[i - 1].SubmachineAtkVal;
				upgradeAtkTables[i].SniperAtkVal += upgradeAtkTables[i - 1].SniperAtkVal;
				upgradeAtkTables[i].HeavyAtkVal += upgradeAtkTables[i - 1].HeavyAtkVal;
				upgradeAtkTables[i].HandgunAtkVal += upgradeAtkTables[i - 1].HandgunAtkVal;
				upgradeAtkTables[i].SpecialAtkVal += upgradeAtkTables[i - 1].SpecialAtkVal;
			}
		}
	}

	private void ParseUpgradeShockTable(CSVLoader csvLoader)
	{
		upgradeShockTables = new UpgradeTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			upgradeShockTables[i] = new UpgradeTable();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			Value.Trim();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value6.Trim();
			Value7.Trim();
			Value8.Trim();
			upgradeShockTables[i].Level = Convert.ToInt32(Value);
			upgradeShockTables[i].AssultAtkVal = (float)Convert.ToDouble(Value2);
			upgradeShockTables[i].SubmachineAtkVal = (float)Convert.ToDouble(Value3);
			upgradeShockTables[i].SniperAtkVal = (float)Convert.ToDouble(Value4);
			upgradeShockTables[i].HeavyAtkVal = (float)Convert.ToDouble(Value5);
			upgradeShockTables[i].HandgunAtkVal = (float)Convert.ToDouble(Value6);
			upgradeShockTables[i].SpecialAtkVal = (float)Convert.ToDouble(Value7);
			upgradeShockTables[i].Price = Convert.ToInt32(Value8);
			if (i > 0)
			{
				upgradeShockTables[i].AssultAtkVal += upgradeShockTables[i - 1].AssultAtkVal;
				upgradeShockTables[i].SubmachineAtkVal += upgradeShockTables[i - 1].SubmachineAtkVal;
				upgradeShockTables[i].SniperAtkVal += upgradeShockTables[i - 1].SniperAtkVal;
				upgradeShockTables[i].HeavyAtkVal += upgradeShockTables[i - 1].HeavyAtkVal;
				upgradeShockTables[i].HandgunAtkVal += upgradeShockTables[i - 1].HandgunAtkVal;
				upgradeShockTables[i].SpecialAtkVal += upgradeShockTables[i - 1].SpecialAtkVal;
			}
		}
	}

	private void ParseUpgradeChargeTable(CSVLoader csvLoader)
	{
		upgradeChargeTables = new UpgradeTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			upgradeChargeTables[i] = new UpgradeTable();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			csvLoader.ReadValue(7, i, string.Empty, out string Value8);
			Value.Trim();
			Value2.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value6.Trim();
			Value7.Trim();
			Value8.Trim();
			upgradeChargeTables[i].Level = Convert.ToInt32(Value);
			upgradeChargeTables[i].AssultAtkVal = (float)Convert.ToDouble(Value2);
			upgradeChargeTables[i].SubmachineAtkVal = (float)Convert.ToDouble(Value3);
			upgradeChargeTables[i].SniperAtkVal = (float)Convert.ToDouble(Value4);
			upgradeChargeTables[i].HeavyAtkVal = (float)Convert.ToDouble(Value5);
			upgradeChargeTables[i].HandgunAtkVal = (float)Convert.ToDouble(Value6);
			upgradeChargeTables[i].SpecialAtkVal = (float)Convert.ToDouble(Value7);
			upgradeChargeTables[i].Price = Convert.ToInt32(Value8);
		}
	}

	private bool LoadUpgradeAtkTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/upgradeatktable.txt");
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
		ParseUpgradeAtkTable(cSVLoader);
		return true;
	}

	private bool LoadUpgradeShockTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/upgradeshocktable.txt");
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
		ParseUpgradeShockTable(cSVLoader);
		return true;
	}

	private bool LoadUpgradeChargeTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/upgradechargetable.txt");
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
		ParseUpgradeChargeTable(cSVLoader);
		return true;
	}

	private bool LoadPurcharseItemsFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/purcharseitemstable.txt");
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
		ParsePurcharseItemsTable(cSVLoader);
		return true;
	}

	private void ParsePurcharseItemsTable(CSVLoader csvLoader)
	{
		purcharseItems = new PurcharseItems[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			purcharseItems[i] = new PurcharseItems();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string _);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			Value.Trim();
			Value3.Trim();
			Value4.Trim();
			Value5.Trim();
			Value6.Trim();
			Value7.Trim();
			purcharseItems[i].Slot = Convert.ToInt32(Value);
			purcharseItems[i].Price = Convert.ToInt32(Value3);
			purcharseItems[i].BuyUnit = Convert.ToInt32(Value4);
			purcharseItems[i].incAtkSpeed = (float)Convert.ToDouble(Value5);
			purcharseItems[i].incMoveSpeed = (float)Convert.ToDouble(Value6);
			purcharseItems[i].cotinuesTime = (float)Convert.ToDouble(Value7);
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
		UnityEngine.Object.DontDestroyOnLoad(this);
	}
}
