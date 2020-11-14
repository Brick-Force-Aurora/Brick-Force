using UnityEngine;

public class TosMain : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.BOTTOM;

	private LangOptManager.LANG_OPT[] languages;

	private Texture2D[] langTex;

	private Vector2 scrollPosition = Vector2.zero;

	private float txtsHeight;

	private bool bAgree;

	private bool isAgreeing;

	public Texture2D texPopupBg;

	private Rect crdPopupBg = new Rect(0f, 0f, 1024f, 768f);

	private float grbWidth = 930f;

	private Rect crdTosRect = new Rect(32f, 60f, 960f, 618f);

	private Rect crdOkBtn = new Rect(904f, 715f, 100f, 34f);

	private Rect crdCloseBtn = new Rect(971f, 10f, 34f, 34f);

	private Rect crdCurLangBtn = new Rect(400f, 708f, 223f, 34f);

	private Rect crdAgree = new Rect(32f, 715f, 21f, 22f);

	private void Start()
	{
		bAgree = false;
		isAgreeing = false;
		languages = new LangOptManager.LANG_OPT[BuildOption.Instance.Props.supportLanguages.Length];
		langTex = new Texture2D[BuildOption.Instance.Props.supportLanguages.Length];
		for (int i = 0; i < languages.Length; i++)
		{
			languages[i] = BuildOption.Instance.Props.supportLanguages[i];
			langTex[i] = LangOptManager.Instance.languages[(int)languages[i]];
		}
		if (BuildOption.Instance.Props.ShowAgb)
		{
			GlobalVars.Instance.LoadAbg();
		}
	}

	private void OnGUI()
	{
		GUI.depth = (int)guiDepth;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GlobalVars.Instance.BeginGUIWithBox("BoxBg");
		CalculateHeight();
		TextureUtil.DrawTexture(crdPopupBg, texPopupBg, ScaleMode.StretchToFill);
		scrollPosition = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, grbWidth, txtsHeight), position: crdTosRect, scrollPosition: scrollPosition);
		float num = 60f;
		foreach (string item in GlobalVars.Instance.arrTextAgb)
		{
			GUIStyle style = GUI.skin.GetStyle("Label");
			float num2 = style.CalcHeight(new GUIContent(item), grbWidth);
			GUI.Label(new Rect(10f, num, grbWidth, num2), item, "Label");
			num += num2;
		}
		GUI.EndScrollView();
		bAgree = GUI.Toggle(crdAgree, bAgree, StringMgr.Instance.Get("AGREE"));
		bool enabled = GUI.enabled;
		GUI.enabled = bAgree;
		if (GlobalVars.Instance.MyButton(crdOkBtn, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (MyInfoManager.Instance.NeedPlayerInfo)
			{
				Application.LoadLevel("PlayerInfo");
			}
			else if (!isAgreeing)
			{
				CSNetManager.Instance.Sock.SendCS_I_AGREE_TOS_REQ();
				isAgreeing = true;
			}
		}
		GUI.enabled = enabled;
		if (BuildOption.Instance.Props.LanguageSelectable)
		{
			int num3 = -1;
			int num4 = 0;
			while (num3 < 0 && num4 < languages.Length)
			{
				if (languages[num4] == (LangOptManager.LANG_OPT)LangOptManager.Instance.LangOpt)
				{
					num3 = num4;
				}
				num4++;
			}
			if (num3 < 0 || num3 >= langTex.Length)
			{
				Debug.LogError("Fail to find language options");
			}
			else if (GlobalVars.Instance.MyButton(crdCurLangBtn, langTex[num3], "BtnBlue"))
			{
				((ChangeLangDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CHANGE_LANG, exclusive: true))?.InitDialog(crdCurLangBtn.y - 5f);
			}
		}
		if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			Application.LoadLevel("Login");
		}
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void Update()
	{
	}

	private void CalculateHeight()
	{
		if (!GlobalVars.Instance.bOnceCalcHeight)
		{
			float num = 0f;
			GUIStyle style = GUI.skin.GetStyle("Label");
			foreach (string item in GlobalVars.Instance.arrTextAgb)
			{
				float num2 = style.CalcHeight(new GUIContent(item), grbWidth);
				num += num2;
			}
			txtsHeight = num;
			GlobalVars.Instance.bOnceCalcHeight = true;
		}
	}
}
