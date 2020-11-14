using UnityEngine;

public class LangOptManager : MonoBehaviour
{
	public enum LANG_OPT
	{
		KOREAN,
		ENGLISH,
		SIMPLIFIED_CHINESE,
		JAPANESE,
		GERMAN,
		TRADITIONAL_CHINESE,
		FRENCH,
		SPANISH,
		POLISH,
		TURKISH,
		INDONESIAN,
		SPANISH_AXESO5,
		COUNT
	}

	public string[] langNames;

	private LANG_OPT langOpt;

	public Texture2D[] languages;

	public string[] levels;

	public string[] skinHolders;

	private Font[] fonts;

	public string[] fontNames;

	public int[] fontVersions;

	public string[] toss;

	private bool onceStreamed;

	private float streamingProgress;

	private static LangOptManager _instance;

	private bool isFontReady;

	public int LangOpt
	{
		get
		{
			return (int)langOpt;
		}
		set
		{
			langOpt = (LANG_OPT)value;
			PlayerPrefs.SetInt("BfLanguage", value);
		}
	}

	public Font CurFont => fonts[(int)langOpt];

	public static LangOptManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(LangOptManager)) as LangOptManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get LangOptManager Instance");
				}
			}
			return _instance;
		}
	}

	public bool IsFontReady => isFontReady;

	public string GetAgbById(int id)
	{
		return toss[id];
	}

	public string GetAgbCurrent()
	{
		return toss[(int)langOpt];
	}

	public string GetLangName(int id)
	{
		return langNames[id];
	}

	private void Awake()
	{
		onceStreamed = false;
		Object.DontDestroyOnLoad(this);
	}

	public void SetFont()
	{
		if (fonts[(int)langOpt] != null)
		{
			GUISkinFinder.Instance.UpdateFont(CurFont);
		}
		else
		{
			int num = AlreadyLoadedFont((int)langOpt);
			if (num >= 0)
			{
				GUISkinFinder.Instance.UpdateFont(fonts[num]);
			}
			else
			{
				isFontReady = false;
				AssetBundleLoadManager.Instance.Setfont = false;
				AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.FONT, fontNames[(int)langOpt], fontVersions[(int)langOpt]);
			}
		}
	}

	private void OnDestroy()
	{
	}

	private int AlreadyLoadedFont(int lang)
	{
		string b = fontNames[lang];
		for (int i = 0; i < fontNames.Length; i++)
		{
			if (fontNames[i] == b && fonts[i] != null)
			{
				return i;
			}
		}
		return -1;
	}

	private void Start()
	{
		langOpt = BuildOption.Instance.Props.DefaultLanguage;
		if (BuildOption.Instance.Props.LanguageSelectable)
		{
			langOpt = (LANG_OPT)PlayerPrefs.GetInt("BfLanguage", (int)langOpt);
			bool flag = false;
			int num = 0;
			while (!flag && num < BuildOption.Instance.Props.supportLanguages.Length)
			{
				if (langOpt == BuildOption.Instance.Props.supportLanguages[num])
				{
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				langOpt = BuildOption.Instance.Props.DefaultLanguage;
			}
		}
		fonts = new Font[fontNames.Length];
		for (int i = 0; i < fontNames.Length; i++)
		{
			fonts[i] = null;
		}
		AssetBundleLoadManager.Instance.load(AssetBundleLoadManager.ASS_BUNDLE_TYPE.FONT, fontNames[(int)langOpt], fontVersions[(int)langOpt]);
	}

	private void readyFont()
	{
		if (AssetBundleLoadManager.Instance.Setfont)
		{
			fonts[(int)langOpt] = (AssetBundleLoadManager.Instance.getAssetBundle(AssetBundleLoadManager.ASS_BUNDLE_TYPE.FONT, fontNames[(int)langOpt], fontVersions[(int)langOpt]).mainAsset as Font);
			GUISkinFinder.Instance.UpdateFont(CurFont);
			SetFont();
			isFontReady = true;
		}
	}

	private void Update()
	{
		if (!isFontReady)
		{
			readyFont();
		}
		if (isFontReady && BuildOption.Instance.Props.isWebPlayer && levels.Length > 0)
		{
			float num = 0f;
			for (int i = 0; i < levels.Length; i++)
			{
				num += Application.GetStreamProgressForLevel(levels[i]);
			}
			streamingProgress = 100f * (num / (float)levels.Length);
			if (!onceStreamed && streamingProgress >= 99.99999f)
			{
				onceStreamed = true;
			}
		}
	}

	private void OnGUI()
	{
		if (BuildOption.Instance.Props.isWebPlayer && streamingProgress < 99.99999f)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 100)), streamingProgress.ToString("0.#") + "%", "LoadingLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
			LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 101)), streamingProgress.ToString("0.#") + "%", "LoadingLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
		}
	}
}
