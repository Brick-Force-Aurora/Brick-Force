using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLoadManager : MonoBehaviour
{
	public enum ASS_BUNDLE_TYPE
	{
		FONT,
		VOICE,
		USK,
		BRICK_MAT,
		BRICK_ICON,
		ITEM_MAT,
		ITEM_ICON,
		ITEM_WEAPONBY,
		VOICE2
	}

	public class LoadedInfo
	{
		public ASS_BUNDLE_TYPE abt;

		public string Url;

		public int Version;

		public LoadedInfo(ASS_BUNDLE_TYPE _abt, string _url, int _ver)
		{
			abt = _abt;
			Url = _url;
			Version = _ver;
		}
	}

	private static AssetBundleLoadManager _instance;

	private string resourceServer = string.Empty;

	private List<LoadedInfo> listLoaded;

	public int usk_ver = 1;

	private bool setfont;

	public static AssetBundleLoadManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(AssetBundleLoadManager)) as AssetBundleLoadManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get AssetBundleLoadManager Instance");
				}
			}
			return _instance;
		}
	}

	public bool Setfont
	{
		get
		{
			return setfont;
		}
		set
		{
			setfont = value;
		}
	}

	public AssetBundle getAssetBundle(ASS_BUNDLE_TYPE abt, string assembleStr, int ver)
	{
		string url = string.Empty;
		switch (abt)
		{
		case ASS_BUNDLE_TYPE.FONT:
			url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + assembleStr + ".unity3d";
			break;
		case ASS_BUNDLE_TYPE.VOICE:
		case ASS_BUNDLE_TYPE.USK:
			url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + assembleStr;
			break;
		default:
			Debug.LogError("(load)not support type.");
			break;
		}
		return AssetBundleManager.getAssetBundle(url, ver);
	}

	private void Awake()
	{
		listLoaded = new List<LoadedInfo>();
		Object.DontDestroyOnLoad(this);
	}

	private void AddList(ASS_BUNDLE_TYPE abt, string url, int ver)
	{
		listLoaded.Add(new LoadedInfo(abt, url, ver));
	}

	public void load(ASS_BUNDLE_TYPE abt, string assembleStr, int ver = 1)
	{
		if (resourceServer.Length == 0)
		{
			resourceServer = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/";
		}
		string path = Path.Combine(Application.dataPath, "Resources/");
		string str = Path.Combine(path, assembleStr);
		string empty = string.Empty;
		string empty2 = string.Empty;
		switch (abt)
		{
		case ASS_BUNDLE_TYPE.FONT:
			empty = "file://" + str + ".unity3d";
			empty2 = resourceServer + assembleStr + ".unity3d";
			break;
		case ASS_BUNDLE_TYPE.VOICE:
		case ASS_BUNDLE_TYPE.USK:
		case ASS_BUNDLE_TYPE.BRICK_MAT:
		case ASS_BUNDLE_TYPE.BRICK_ICON:
		case ASS_BUNDLE_TYPE.ITEM_MAT:
		case ASS_BUNDLE_TYPE.ITEM_ICON:
		case ASS_BUNDLE_TYPE.ITEM_WEAPONBY:
		case ASS_BUNDLE_TYPE.VOICE2:
			empty = "file://" + str;
			empty2 = resourceServer + assembleStr;
			break;
		default:
			Debug.LogError("(load)not support type.");
			return;
		}
		StartCoroutine(downloadAB(abt, empty, empty2, ver));
		AddList(abt, assembleStr, ver);
	}

	private IEnumerator downloadAB(ASS_BUNDLE_TYPE abt, string file, string url, int version)
	{
		yield return (object)StartCoroutine(AssetBundleManager.downloadAssetBundle(file, url, version));
		switch (abt)
		{
		case ASS_BUNDLE_TYPE.FONT:
			AssetBundleManager.getAssetBundle(url, version);
			setfont = true;
			break;
		case ASS_BUNDLE_TYPE.VOICE:
		{
			AssetBundle bundle = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs = bundle.LoadAll();
			if (objs.Length > 0)
			{
				VoiceManager.Instance.Clear();
			}
			for (int i = 0; i < objs.Length; i++)
			{
				AudioClip clip = objs[i] as AudioClip;
				if (clip != null)
				{
					VoiceManager.Instance.Add(objs[i].name, clip);
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs[i].name);
				}
			}
			VoiceManager.Instance.bLoaded = true;
			break;
		}
		case ASS_BUNDLE_TYPE.USK:
		{
			AssetBundle bundle2 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs2 = bundle2.LoadAll();
			for (int j = 0; j < objs2.Length; j++)
			{
				Texture texture = objs2[j] as Texture;
				if (texture != null)
				{
					UskManager.Instance.Add(objs2[j].name, texture);
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs2[j].name);
				}
			}
			UskManager.Instance.bLoaded = true;
			break;
		}
		case ASS_BUNDLE_TYPE.BRICK_MAT:
		{
			AssetBundle bundle3 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs3 = bundle3.LoadAll(typeof(Material));
			for (int k = 0; k < objs3.Length; k++)
			{
				Material mat = objs3[k] as Material;
				if (mat != null)
				{
					BrickManager.Instance.DictionaryMatAdd(mat.name, mat);
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs3[k].name);
				}
			}
			break;
		}
		case ASS_BUNDLE_TYPE.BRICK_ICON:
		{
			AssetBundle bundle4 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs4 = bundle4.LoadAll(typeof(Texture2D));
			for (int l = 0; l < objs4.Length; l++)
			{
				Texture2D tex = objs4[l] as Texture2D;
				if (tex != null)
				{
					BrickManager.Instance.DictionaryIconAdd(tex.name, tex);
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs4[l].name);
				}
			}
			break;
		}
		case ASS_BUNDLE_TYPE.ITEM_MAT:
		{
			AssetBundle bundle5 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs5 = bundle5.LoadAll(typeof(Material));
			TItemManager.Instance.materials = new Material[objs5.Length];
			for (int m = 0; m < objs5.Length; m++)
			{
				Material mat2 = objs5[m] as Material;
				if (mat2 != null)
				{
					TItemManager.Instance.materials[m] = mat2;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs5[m].name);
				}
			}
			break;
		}
		case ASS_BUNDLE_TYPE.ITEM_ICON:
		{
			AssetBundle bundle6 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs6 = bundle6.LoadAll();
			TItemManager.Instance.icons = new Texture2D[objs6.Length];
			for (int n = 0; n < objs6.Length; n++)
			{
				Texture2D tex2 = objs6[n] as Texture2D;
				if (tex2 != null)
				{
					TItemManager.Instance.icons[n] = tex2;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs6[n].name);
				}
			}
			break;
		}
		case ASS_BUNDLE_TYPE.ITEM_WEAPONBY:
		{
			AssetBundle bundle7 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs7 = bundle7.LoadAll();
			TItemManager.Instance.weaponBy = new Texture2D[objs7.Length];
			for (int i2 = 0; i2 < objs7.Length; i2++)
			{
				Texture2D tex3 = objs7[i2] as Texture2D;
				if (tex3 != null)
				{
					TItemManager.Instance.weaponBy[i2] = tex3;
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs7[i2].name);
				}
			}
			break;
		}
		case ASS_BUNDLE_TYPE.VOICE2:
		{
			AssetBundle bundle8 = AssetBundleManager.getAssetBundle(url, version);
			Object[] objs8 = bundle8.LoadAll();
			if (objs8.Length > 0)
			{
				VoiceManager.Instance.Clear2();
			}
			for (int i3 = 0; i3 < objs8.Length; i3++)
			{
				AudioClip clip2 = objs8[i3] as AudioClip;
				if (clip2 != null)
				{
					VoiceManager.Instance.Add2(objs8[i3].name, clip2);
				}
				else
				{
					Debug.LogError("local bundle load error: " + objs8[i3].name);
				}
			}
			VoiceManager.Instance.bLoaded2 = true;
			break;
		}
		default:
			Debug.LogError("(downloadAB)not support type.");
			break;
		}
	}

	private void OnDestroy()
	{
		string empty = string.Empty;
		foreach (LoadedInfo item in listLoaded)
		{
			empty = ((item.abt != 0) ? (resourceServer + item.Url) : (resourceServer + item.Url + ".unity3d"));
			AssetBundleManager.Unload(empty, item.Version, allObjects: false);
		}
	}
}
