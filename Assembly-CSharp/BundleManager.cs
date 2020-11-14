using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundleManager : MonoBehaviour
{
	private Dictionary<string, BundleDesc> dic;

	private bool isLoaded;

	private static BundleManager _instance;

	public bool IsLoaded => isLoaded;

	public static BundleManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BundleManager)) as BundleManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BundleManager Instance");
				}
			}
			return _instance;
		}
	}

	public BundleUnit[] Unpack(string bundle)
	{
		if (dic.ContainsKey(bundle))
		{
			return dic[bundle].Unpack();
		}
		return null;
	}

	public void Pack(string bundle, string code, int opt)
	{
		bundle = bundle.ToLower();
		code = code.ToLower();
		TItem tItem = TItemManager.Instance.Get<TItem>(code);
		if (tItem != null)
		{
			if (!dic.ContainsKey(bundle))
			{
				dic.Add(bundle, new BundleDesc());
			}
			dic[bundle].Pack(tItem, opt);
		}
	}

	public void Clear()
	{
		dic.Clear();
	}

	public void Remove(string bundle)
	{
		bundle = bundle.ToLower();
		if (dic.ContainsKey(bundle))
		{
			dic.Remove(bundle);
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		dic = new Dictionary<string, BundleDesc>();
	}

	private IEnumerator LoadFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string urlBundleItem = "http://" + prop.GetResourceServer + "/BfData/Template/bundle_item.txt.cooked";
		WWW wwwBundleItem = new WWW(urlBundleItem);
		yield return (object)wwwBundleItem;
		using (MemoryStream msBundleItem = new MemoryStream(wwwBundleItem.bytes))
		{
			using (BinaryReader rdBundleItem = new BinaryReader(msBundleItem))
			{
				CSVLoader csvBundleItem = new CSVLoader();
				if (csvBundleItem.SecuredLoadFromBinaryReader(rdBundleItem))
				{
					ParseData(csvBundleItem);
					isLoaded = true;
				}
			}
		}
		if (!isLoaded)
		{
			Debug.LogError("Fail to download " + urlBundleItem);
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
		string text2 = Path.Combine(text, "Template/bundle_item.txt");
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

	private void ParseData(CSVLoader csvBuldleItem)
	{
		for (int i = 0; i < csvBuldleItem.Rows; i++)
		{
			csvBuldleItem.ReadValue(0, i, string.Empty, out string Value);
			csvBuldleItem.ReadValue(1, i, string.Empty, out string Value2);
			csvBuldleItem.ReadValue(2, i, 0, out int Value3);
			csvBuldleItem.ReadValue(3, i, 0, out int _);
			Value2 = Value2.Trim().ToLower();
			Pack(Value, Value2, Value3);
		}
	}

	public void Load()
	{
		dic.Clear();
		Property props = BuildOption.Instance.Props;
		if (!props.loadShopTxt && !Application.isEditor)
		{
			isLoaded = true;
		}
		else if (props.isWebPlayer)
		{
			StartCoroutine(LoadFromWWW());
		}
		else
		{
			isLoaded = LoadFromLocalFileSystem();
		}
	}
}
