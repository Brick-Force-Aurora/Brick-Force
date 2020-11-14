using System;
using UnityEngine;

[Serializable]
public class TutorHelpDialog : Dialog
{
	public Texture2D iconHelp;

	public Texture2D iconPoint;

	public Texture2D[] tutos;

	private static int subHeadID;

	private static int articleID;

	private static int artID;

	private float articley;

	private string strArticle;

	private int keyindex1;

	private int keyindex2;

	private int keyindex3;

	private int keyindex4;

	private int subPage;

	private Color clrMain = Color.white;

	private string[] mainHeads;

	private string[] subHeads;

	private string[] subArticles;

	private int[] articlesPageStartId = new int[6]
	{
		0,
		4,
		6,
		8,
		10,
		12
	};

	private int[] endsPerPage = new int[6]
	{
		4,
		2,
		2,
		2,
		2,
		3
	};

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TUTORHELP;
		clrMain = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 125);
	}

	public override void OnPopup()
	{
		size.x = 806f;
		size.y = 736f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		mainHeads = new string[6];
		mainHeads[0] = StringMgr.Instance.Get("TUTO_HELP_POPUP01");
		mainHeads[1] = StringMgr.Instance.Get("TUTO_HELP_POPUP12");
		mainHeads[2] = StringMgr.Instance.Get("TUTO_HELP_POPUP17");
		mainHeads[3] = StringMgr.Instance.Get("TUTO_HELP_POPUP22");
		mainHeads[4] = StringMgr.Instance.Get("TUTO_HELP_POPUP27");
		mainHeads[5] = StringMgr.Instance.Get("TUTO_HELP_POPUP32");
		subHeads = new string[15];
		subHeads[0] = StringMgr.Instance.Get("TUTO_HELP_POPUP02");
		subHeads[1] = StringMgr.Instance.Get("TUTO_HELP_POPUP06");
		subHeads[2] = StringMgr.Instance.Get("TUTO_HELP_POPUP08");
		subHeads[3] = StringMgr.Instance.Get("TUTO_HELP_POPUP10");
		subHeads[4] = StringMgr.Instance.Get("TUTO_HELP_POPUP13");
		subHeads[5] = StringMgr.Instance.Get("TUTO_HELP_POPUP15");
		subHeads[6] = StringMgr.Instance.Get("TUTO_HELP_POPUP18");
		subHeads[7] = StringMgr.Instance.Get("TUTO_HELP_POPUP20");
		subHeads[8] = StringMgr.Instance.Get("TUTO_HELP_POPUP23");
		subHeads[9] = StringMgr.Instance.Get("TUTO_HELP_POPUP25");
		subHeads[10] = StringMgr.Instance.Get("TUTO_HELP_POPUP28");
		subHeads[11] = StringMgr.Instance.Get("TUTO_HELP_POPUP30");
		subHeads[12] = StringMgr.Instance.Get("TUTO_HELP_POPUP33");
		subHeads[13] = StringMgr.Instance.Get("TUTO_HELP_POPUP35");
		subHeads[14] = StringMgr.Instance.Get("TUTO_HELP_POPUP37");
		subArticles = new string[15];
		subArticles[0] = StringMgr.Instance.Get("TUTO_HELP_POPUP03");
		subArticles[1] = StringMgr.Instance.Get("TUTO_HELP_POPUP07");
		subArticles[2] = StringMgr.Instance.Get("TUTO_HELP_POPUP09");
		subArticles[3] = StringMgr.Instance.Get("TUTO_HELP_POPUP11");
		subArticles[4] = StringMgr.Instance.Get("TUTO_HELP_POPUP14");
		subArticles[5] = StringMgr.Instance.Get("TUTO_HELP_POPUP16");
		subArticles[6] = StringMgr.Instance.Get("TUTO_HELP_POPUP19");
		subArticles[7] = StringMgr.Instance.Get("TUTO_HELP_POPUP21");
		subArticles[8] = StringMgr.Instance.Get("TUTO_HELP_POPUP24");
		subArticles[9] = StringMgr.Instance.Get("TUTO_HELP_POPUP26");
		subArticles[10] = StringMgr.Instance.Get("TUTO_HELP_POPUP29");
		subArticles[11] = StringMgr.Instance.Get("TUTO_HELP_POPUP31");
		subArticles[12] = StringMgr.Instance.Get("TUTO_HELP_POPUP34");
		subArticles[13] = StringMgr.Instance.Get("TUTO_HELP_POPUP36");
		subArticles[14] = StringMgr.Instance.Get("TUTO_HELP_POPUP38");
	}

	public void InitDialog()
	{
		articleID = 0;
		subPage = 0;
		if (!GlobalVars.Instance.isLoadBattleTutor)
		{
			GlobalVars.Instance.opened = 3;
		}
		else
		{
			GlobalVars.Instance.opened = 0;
		}
	}

	public void OpenNext(bool inc)
	{
		if (inc)
		{
			GlobalVars.Instance.opened++;
			articleID = articlesPageStartId[GlobalVars.Instance.opened];
			subPage = 0;
		}
	}

	private string GetArticle(int cur)
	{
		switch (cur)
		{
		case 0:
			keyindex1 = custom_inputs.Instance.KeyIndex("K_FORWARD");
			keyindex2 = custom_inputs.Instance.KeyIndex("K_BACKWARD");
			keyindex3 = custom_inputs.Instance.KeyIndex("K_RIGHT");
			keyindex4 = custom_inputs.Instance.KeyIndex("K_LEFT");
			return string.Format(subArticles[cur], custom_inputs.Instance.InputKey[keyindex1].ToString(), custom_inputs.Instance.InputKey[keyindex2].ToString(), custom_inputs.Instance.InputKey[keyindex3].ToString(), custom_inputs.Instance.InputKey[keyindex4].ToString());
		case 3:
			keyindex1 = custom_inputs.Instance.KeyIndex("K_JUMP");
			return string.Format(subArticles[cur], custom_inputs.Instance.InputKey[keyindex1].ToString());
		case 4:
			keyindex1 = custom_inputs.Instance.KeyIndex("K_SIT");
			return string.Format(subArticles[cur], custom_inputs.Instance.InputKey[keyindex1].ToString());
		default:
			return subArticles[cur];
		}
	}

	private string GetArticle(int page, int subHead)
	{
		switch (page)
		{
		case 0:
			switch (subHead)
			{
			case 0:
				keyindex1 = custom_inputs.Instance.KeyIndex("K_FORWARD");
				keyindex2 = custom_inputs.Instance.KeyIndex("K_BACKWARD");
				keyindex3 = custom_inputs.Instance.KeyIndex("K_RIGHT");
				keyindex4 = custom_inputs.Instance.KeyIndex("K_LEFT");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString(), custom_inputs.Instance.InputKey[keyindex2].ToString(), custom_inputs.Instance.InputKey[keyindex3].ToString(), custom_inputs.Instance.InputKey[keyindex4].ToString());
			case 1:
				keyindex1 = custom_inputs.Instance.KeyIndex("K_JUMP");
				keyindex2 = custom_inputs.Instance.KeyIndex("K_SIT");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString(), custom_inputs.Instance.InputKey[keyindex2].ToString());
			}
			break;
		case 1:
			switch (subHead)
			{
			case 0:
				keyindex1 = custom_inputs.Instance.KeyIndex("K_WPNCHG1");
				keyindex2 = custom_inputs.Instance.KeyIndex("K_WPNCHG2");
				keyindex3 = custom_inputs.Instance.KeyIndex("K_WPNCHG3");
				keyindex4 = custom_inputs.Instance.KeyIndex("K_WPNCHG4");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString(), custom_inputs.Instance.InputKey[keyindex2].ToString(), custom_inputs.Instance.InputKey[keyindex3].ToString(), custom_inputs.Instance.InputKey[keyindex4].ToString());
			case 1:
				keyindex1 = custom_inputs.Instance.KeyIndex("K_WPNCHG5");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString());
			}
			break;
		case 2:
			if (subHead == 1)
			{
				keyindex1 = custom_inputs.Instance.KeyIndex("K_RELOAD");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString());
			}
			break;
		case 4:
			if (subHead == 2)
			{
				keyindex1 = custom_inputs.Instance.KeyIndex("K_BRICK_MENU");
				return string.Format(subArticles[articleID + subHead], custom_inputs.Instance.InputKey[keyindex1].ToString());
			}
			break;
		}
		return subArticles[articleID + subHead];
	}

	private float calcBoxHeight(int article)
	{
		float num = 15f;
		num = (articley = num + 20f);
		strArticle = GetArticle(article);
		GUIStyle style = GUI.skin.GetStyle("ArticleLabel");
		float num2 = style.CalcHeight(new GUIContent(strArticle), 720f);
		num += num2;
		return num + 15f;
	}

	private float calcBoxHeight(int page, int subHead)
	{
		float num = 15f;
		num = (articley = num + 20f);
		strArticle = GetArticle(page, subHead);
		GUIStyle style = GUI.skin.GetStyle("ArticleLabel");
		float num2 = style.CalcHeight(new GUIContent(strArticle), 720f);
		num += num2;
		return num + 15f;
	}

	private float calcSimpleBoxHeight(int page, int subHead)
	{
		float num = 15f;
		articley = num + 4f;
		strArticle = GetArticle(page, subHead);
		GUIStyle style = GUI.skin.GetStyle("ArticleLabel");
		float num2 = style.CalcHeight(new GUIContent(strArticle), 720f);
		num += num2;
		return num + 15f;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Rect position = new Rect(16f, 12f, 774f, 38f);
		GUI.Box(position, string.Empty, "BoxFadeBlue");
		TextureUtil.DrawTexture(new Rect(33f, 16f, (float)iconHelp.width, (float)iconHelp.height), iconHelp);
		LabelUtil.TextOut(new Vector2(80f, 30f), mainHeads[GlobalVars.Instance.opened], "MissionTitleLabel", clrMain, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		float num = calcBoxHeight(articleID);
		Rect position2 = new Rect(16f, 63f, 764f, num);
		GUI.Box(position2, string.Empty, "LineBoxBlue");
		TextureUtil.DrawTexture(new Rect(35f, 79f, (float)iconPoint.width, (float)iconPoint.height), iconPoint);
		LabelUtil.TextOut(new Vector2(55f, 81f), subHeads[articleID], "MissionTitleLabel", clrMain, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		GUI.Label(new Rect(position2.x + 20f, position2.y + articley, 720f, 60f), strArticle, "ArticleLabel");
		TextureUtil.DrawTexture(new Rect(45f, position2.y + num + 10f, (float)tutos[articleID].width, (float)tutos[articleID].height), tutos[articleID]);
		if (endsPerPage[GlobalVars.Instance.opened] > 0 && subPage < endsPerPage[GlobalVars.Instance.opened] - 1)
		{
			Rect rc = new Rect(size.x - 160f, size.y - 50f, 140f, 34f);
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("BTN_NEXT"), "BtnAction"))
			{
				subPage++;
				articleID++;
			}
		}
		else
		{
			Rect rc2 = new Rect(size.x - 160f, size.y - 50f, 140f, 34f);
			if (GlobalVars.Instance.MyButton(rc2, StringMgr.Instance.Get("OK"), "BtnAction") || GlobalVars.Instance.IsEscapePressed() || GlobalVars.Instance.IsReturnPressed())
			{
				GlobalVars.Instance.SetForceClosed(set: true);
				result = true;
			}
		}
		if (endsPerPage[GlobalVars.Instance.opened] > 0 && subPage > 0)
		{
			Rect rc3 = new Rect(20f, size.y - 50f, 140f, 34f);
			if (GlobalVars.Instance.MyButton(rc3, StringMgr.Instance.Get("BTN_PREVIOUS"), "BtnAction"))
			{
				subPage--;
				articleID--;
			}
		}
		Rect rc4 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc4, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			GlobalVars.Instance.resetMenuEx();
			result = true;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}
}
