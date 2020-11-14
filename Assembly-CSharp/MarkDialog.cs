using System;
using UnityEngine;

[Serializable]
public class MarkDialog : Dialog
{
	public Texture2D icon;

	public Rect crdIcon = new Rect(2f, 2f, 34f, 34f);

	public Rect crdComment = new Rect(0f, 0f, 0f, 0f);

	public Rect crdCommentLabel = new Rect(0f, 0f, 0f, 0f);

	private Rect crdMainTab = new Rect(14f, 126f, 260f, 31f);

	public Rect crdMarksOutline = new Rect(10f, 120f, 430f, 284f);

	public Rect crdBgOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdBgPosition = new Rect(0f, 0f, 0f, 0f);

	public Rect crdAmblumOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdAmblumPosition = new Rect(0f, 0f, 0f, 0f);

	public Rect crdFrameOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdFramePosition = new Rect(0f, 0f, 0f, 0f);

	public Rect crdClanMarkOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdClanMark = new Rect(0f, 0f, 0f, 0f);

	private int curMainTab;

	private Vector2 spAmblum = Vector2.zero;

	private Vector2 spFrame = Vector2.zero;

	private Vector2 spBg = Vector2.zero;

	private int amblum;

	private int frame;

	private int bg;

	private int curMark;

	public int CurMark
	{
		set
		{
			curMark = value;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MARK;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(int mark)
	{
		curMark = mark;
		if (mark < 0)
		{
			bg = 0;
			frame = 0;
			amblum = 0;
		}
		else
		{
			bg = ClanMarkManager.Instance.MarkToBg(mark);
			frame = ClanMarkManager.Instance.MarkToColor(mark);
			amblum = ClanMarkManager.Instance.MarkToAmblum(mark);
		}
	}

	public override void Update()
	{
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CHANGE_MARK"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoComment()
	{
		GUI.Box(crdComment, string.Empty, "BoxFadeBlue");
		GUI.Label(crdCommentLabel, StringMgr.Instance.Get("CLAN_MARK_COMMENT"), "MiddleLeftLabel");
	}

	private void DoMainTab()
	{
		GUI.Box(crdMarksOutline, string.Empty, "BoxPopLine");
		string[] array = new string[3]
		{
			StringMgr.Instance.Get("CLAN_MARK_BG"),
			StringMgr.Instance.Get("CLAN_MARK_COLOR"),
			StringMgr.Instance.Get("CLAN_MARK_AMBLUM")
		};
		curMainTab = GUI.SelectionGrid(crdMainTab, curMainTab, array, array.Length, "PopTab");
	}

	private void DoAmblums()
	{
		int num = ClanMarkManager.Instance.amblum.Length / 5;
		if (ClanMarkManager.Instance.amblum.Length % 5 != 0)
		{
			num++;
		}
		GUI.Box(crdAmblumOutline, string.Empty, "BoxInnerLine");
		Rect rect = new Rect(0f, 0f, 350f, (float)(70 * num));
		spAmblum = GUI.BeginScrollView(crdAmblumPosition, spAmblum, rect);
		amblum = GUI.SelectionGrid(rect, amblum, ClanMarkManager.Instance.amblum, 5, "SelRect");
		GUI.EndScrollView();
	}

	private void DoFrames()
	{
		int num = ClanMarkManager.Instance.colorPanel.Length / 5;
		if (ClanMarkManager.Instance.colorPanel.Length % 5 != 0)
		{
			num++;
		}
		GUI.Box(crdFrameOutline, string.Empty, "BoxInnerLine");
		Rect rect = new Rect(0f, 0f, 350f, (float)(70 * num));
		spFrame = GUI.BeginScrollView(crdFramePosition, spFrame, rect);
		frame = GUI.SelectionGrid(rect, frame, ClanMarkManager.Instance.colorPanel, 5, "SelRect");
		GUI.EndScrollView();
	}

	private void DoBackgrounds()
	{
		int num = ClanMarkManager.Instance.bg.Length / 5;
		if (ClanMarkManager.Instance.bg.Length % 5 != 0)
		{
			num++;
		}
		GUI.Box(crdBgOutline, string.Empty, "BoxInnerLine");
		Rect rect = new Rect(0f, 0f, 350f, (float)(70 * num));
		spBg = GUI.BeginScrollView(crdBgPosition, spBg, rect);
		bg = GUI.SelectionGrid(rect, bg, ClanMarkManager.Instance.bg, 5, "SelRect");
		GUI.EndScrollView();
	}

	private void DoClanMark()
	{
		Texture2D bgByIndex = ClanMarkManager.Instance.GetBgByIndex(bg);
		Color colorValueByIndex = ClanMarkManager.Instance.GetColorValueByIndex(frame);
		Texture2D amblumByIndex = ClanMarkManager.Instance.GetAmblumByIndex(amblum);
		if (null != bgByIndex)
		{
			TextureUtil.DrawTexture(crdClanMark, bgByIndex, ScaleMode.ScaleToFit);
		}
		Color color = GUI.color;
		GUI.color = colorValueByIndex;
		if (null != amblumByIndex)
		{
			TextureUtil.DrawTexture(crdClanMark, amblumByIndex, ScaleMode.ScaleToFit);
		}
		GUI.color = color;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoComment();
		DoMainTab();
		switch (curMainTab)
		{
		case 0:
			DoBackgrounds();
			break;
		case 1:
			DoFrames();
			break;
		case 2:
			DoAmblums();
			break;
		}
		DoClanMark();
		Rect rc = new Rect(size.x - 90f, size.y - 40f, 80f, 34f);
		int num = ClanMarkManager.Instance.IndexToMark(bg, frame, amblum);
		if (num != curMark && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			TItem specialItem2HaveFunction = TItemManager.Instance.GetSpecialItem2HaveFunction("clan_mark");
			if (specialItem2HaveFunction != null)
			{
				long num2 = MyInfoManager.Instance.HaveFunction("clan_mark");
				if (num2 >= 0)
				{
					CSNetManager.Instance.Sock.SendCS_CHANGE_CLAN_MARK_REQ(MyInfoManager.Instance.ClanSeq, num, num2, specialItem2HaveFunction.code);
				}
				else
				{
					string msg = string.Format(StringMgr.Instance.Get("CLAN_MARK_TICKET_NEED"), specialItem2HaveFunction.Name);
					MessageBoxMgr.Instance.AddMessage(msg);
				}
			}
		}
		Rect rc2 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
