using System.Collections;
using System.IO;
using UnityEngine;

public class UpgradePropManager : MonoBehaviour
{
	private static UpgradePropManager instance;

	private UpgradeCategoryPropTable[] upgradeCatTable;

	private bool Loaded;

	public static UpgradePropManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (Object.FindObjectOfType(typeof(UpgradePropManager)) as UpgradePropManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the UpgradePropManager Instance");
				}
			}
			return instance;
		}
	}

	public bool IsLoaded => Loaded;

	public bool UseProp(int cat, int prop)
	{
		return upgradeCatTable[prop].props[cat];
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
			Loaded = LoadUpgradePropTableFromLocalFileSystem();
		}
	}

	private IEnumerator LoadAllFromWWW()
	{
		bool propLoaded = false;
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Template/upgradeprops.txt.cooked";
		WWW wwwProp = new WWW(url);
		yield return (object)wwwProp;
		using (MemoryStream stream = new MemoryStream(wwwProp.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					ParseUpgradePropTable(csvLoader);
					propLoaded = true;
				}
			}
		}
		if (!propLoaded)
		{
			Debug.LogError("Fail to download " + url);
		}
		Loaded = propLoaded;
	}

	private void ParseUpgradePropTable(CSVLoader csvLoader)
	{
		upgradeCatTable = new UpgradeCategoryPropTable[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			upgradeCatTable[i] = new UpgradeCategoryPropTable();
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, def: false, out bool Value2);
			csvLoader.ReadValue(2, i, def: false, out bool Value3);
			csvLoader.ReadValue(3, i, def: false, out bool Value4);
			csvLoader.ReadValue(4, i, def: false, out bool Value5);
			csvLoader.ReadValue(5, i, def: false, out bool Value6);
			csvLoader.ReadValue(6, i, def: false, out bool Value7);
			csvLoader.ReadValue(7, i, def: false, out bool Value8);
			csvLoader.ReadValue(8, i, def: false, out bool Value9);
			csvLoader.ReadValue(9, i, def: false, out bool Value10);
			csvLoader.ReadValue(10, i, def: false, out bool Value11);
			csvLoader.ReadValue(11, i, def: false, out bool Value12);
			csvLoader.ReadValue(12, i, def: false, out bool Value13);
			csvLoader.ReadValue(13, i, def: false, out bool Value14);
			Value.Trim();
			upgradeCatTable[i].name = Value;
			upgradeCatTable[i].props = new bool[13];
			upgradeCatTable[i].props[0] = Value5;
			upgradeCatTable[i].props[1] = Value2;
			upgradeCatTable[i].props[2] = Value4;
			upgradeCatTable[i].props[3] = Value3;
			upgradeCatTable[i].props[4] = Value6;
			upgradeCatTable[i].props[5] = Value7;
			upgradeCatTable[i].props[6] = Value8;
			upgradeCatTable[i].props[7] = Value9;
			upgradeCatTable[i].props[8] = Value10;
			upgradeCatTable[i].props[9] = Value11;
			upgradeCatTable[i].props[10] = Value12;
			upgradeCatTable[i].props[11] = Value13;
			upgradeCatTable[i].props[12] = Value14;
		}
	}

	private bool LoadUpgradePropTableFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string text2 = Path.Combine(text, "Template/upgradeprops.txt");
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
		ParseUpgradePropTable(cSVLoader);
		return true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}
}
