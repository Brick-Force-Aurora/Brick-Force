using System;
using UnityEngine;

[Serializable]
public class CreateClanDialog : Dialog
{
	public Texture2D icon;

	public int maxClanName = 10;

	public Rect crdIcon = new Rect(10f, 10f, 34f, 34f);

	public Rect crdClanNameLabelBox = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdClanNameLabel = new Vector2(14f, 130f);

	private Rect crdClanName = new Rect(14f, 50f, 276f, 28f);

	private Rect crdCheckAvailability = new Rect(298f, 48f, 135f, 34f);

	private Vector2 crdCommentLabel = new Vector2(20f, 84f);

	private string clanName = string.Empty;

	private bool clanNameIsAvailable;

	private string availableName = string.Empty;

	private Vector2 crdClanCreatePoint = new Vector2(33f, 216f);

	private Vector2 crdClanHasPoint = new Vector2(33f, 236f);

	public void SetClanNameAvailability(string clanName, bool available)
	{
		clanNameIsAvailable = available;
		availableName = clanName;
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CREATE_CLAN;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		clanName = string.Empty;
		clanNameIsAvailable = false;
		availableName = string.Empty;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CLAN_CREATE"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private void DoComment()
	{
		GUI.SetNextControlName("ClanName");
		clanName = GUI.TextField(crdClanName, clanName, maxClanName);
		clanName = clanName.Replace(" ", string.Empty);
		clanName = clanName.Replace("\t", string.Empty);
		clanName = clanName.Replace("\n", string.Empty);
		if (GlobalVars.Instance.MyButton(crdCheckAvailability, StringMgr.Instance.Get("CHECK_AVAILABILITY"), "BtnAction") && CheckInput())
		{
			CSNetManager.Instance.Sock.SendCS_CHECK_CLAN_NAME_AVAILABILITY_REQ(clanName);
		}
		string text = StringMgr.Instance.Get("CLAN_CREATE_COMMENT");
		if (availableName.Length > 0)
		{
			text = ((!clanNameIsAvailable) ? string.Format(StringMgr.Instance.Get("NAME_IS_NOT_AVAILABLE"), availableName) : string.Format(StringMgr.Instance.Get("NAME_IS_AVAILABLE"), availableName));
		}
		Dialog top = DialogManager.Instance.GetTop();
		if (top != null && top.ID == id)
		{
			GUI.FocusWindow((int)id);
			GUI.FocusControl("ClanName");
		}
		LabelUtil.TextOut(crdCommentLabel, text, "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private bool CheckInput()
	{
		clanName = clanName.Trim();
		if (clanName.Length <= 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_CLAN_NAME"));
			return false;
		}
		if (clanName.Length < 2)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TOO_SHORT_CLAN_NAME"));
			return false;
		}
		string text = WordFilter.Instance.CheckBadword(clanName);
		if (text.Length > 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), text));
			return false;
		}
		return true;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoComment();
		Rect rc = new Rect(size.x - 115f, size.y - 44f, 100f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("CLAN_CREATE"), "BtnAction"))
		{
			if (MyInfoManager.Instance.Point < 30000)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY4CREATE_CLAN"));
			}
			else if (MyInfoManager.Instance.ClanSeq >= 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MEMBER_CANT_CREATE_CLAN"));
			}
			else if (!clanNameIsAvailable)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHECK_NAME_AVAILABILITY"));
			}
			else if (availableName != clanName)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHECK_NAME_AVAILABILITY"));
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_CREATE_CLAN_REQ(clanName);
				result = true;
			}
		}
		string text = StringMgr.Instance.Get("CLAN_CREATE_POINT") + " : 30000";
		string text2 = StringMgr.Instance.Get("MYPOINT") + " : " + MyInfoManager.Instance.Point.ToString();
		LabelUtil.TextOut(crdClanCreatePoint, text, "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdClanHasPoint, text2, "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
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
