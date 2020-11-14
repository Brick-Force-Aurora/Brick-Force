using System;
using UnityEngine;

[Serializable]
public class ChangeLangDialog : Dialog
{
	private LangOptManager.LANG_OPT[] languages;

	private Texture2D[] langTex;

	private Vector2 crdButtonSize = new Vector2(211f, 26f);

	public Vector2 crdLeftTop = new Vector2(4f, 50f);

	public Vector2 crdRightBottom = new Vector2(4f, 4f);

	public float offset = 4f;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CHANGE_LANG;
	}

	public override void OnPopup()
	{
	}

	public void InitDialog(float bottom)
	{
		languages = new LangOptManager.LANG_OPT[BuildOption.Instance.Props.supportLanguages.Length];
		langTex = new Texture2D[BuildOption.Instance.Props.supportLanguages.Length];
		for (int i = 0; i < languages.Length; i++)
		{
			languages[i] = BuildOption.Instance.Props.supportLanguages[i];
			langTex[i] = LangOptManager.Instance.languages[(int)languages[i]];
		}
		size = new Vector2(crdLeftTop.x + crdButtonSize.x + crdRightBottom.x, crdLeftTop.y + crdButtonSize.y * (float)languages.Length + crdRightBottom.y);
		if (languages.Length > 0)
		{
			size.y += offset * (float)(languages.Length - 1);
		}
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, bottom - size.y, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool flag = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("LANGUAGE_SELECT"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Vector2 vector = crdLeftTop;
		int num = 0;
		while (!flag && num < langTex.Length)
		{
			if (GUI.Button(new Rect(vector.x, vector.y, crdButtonSize.x, crdButtonSize.y), langTex[num], "BtnBlue"))
			{
				flag = true;
				if (LangOptManager.Instance.LangOpt != (int)languages[num])
				{
					LangOptManager.Instance.LangOpt = (int)languages[num];
					GUISkinFinder.Instance.LanguageChanged();
					GlobalVars.Instance.LoadAbg();
					GlobalVars.Instance.ChangeVoiceByLang(LangOptManager.Instance.LangOpt);
				}
			}
			vector.y += crdButtonSize.y + offset;
			num++;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return flag;
	}
}
