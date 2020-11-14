using System;
using UnityEngine;

[Serializable]
public class RecvMemoDialog : Dialog
{
	private Memo memo;

	private MemoDialog parent;

	private Vector2 spContents = Vector2.zero;

	public Texture2D icon;

	public Rect crdIcon = new Rect(0f, 0f, 24f, 24f);

	public Rect crdPresent = new Rect(19f, 380f, 80f, 34f);

	public Vector2 crdItemName = new Vector2(280f, 425f);

	public Vector2 crdOption = new Vector2(280f, 448f);

	public Vector2 crdSenderLabel = new Vector2(104f, 46f);

	public Vector2 crdSender = new Vector2(106f, 46f);

	public Vector2 crdMemoTitleLabel = new Vector2(104f, 62f);

	public Vector2 crdMemoTitle = new Vector2(104f, 62f);

	private Vector2 crdTitleSize = new Vector2(220f, 26f);

	public Rect crdArea = new Rect(26f, 120f, 300f, 365f);

	public Rect crdOutline = new Rect(26f, 120f, 300f, 365f);

	public Rect crdAttachedArea = new Rect(26f, 120f, 300f, 240f);

	public Rect crdAttachedOutline = new Rect(26f, 120f, 300f, 240f);

	public Rect crdPresentOutline = new Rect(26f, 250f, 300f, 120f);

	public Vector2 crdItemIcon = new Vector2(26f, 250f);

	private Rect crdReply = new Rect(134f, 515f, 100f, 34f);

	private Rect crdDelete = new Rect(234f, 515f, 100f, 34f);

	public Rect crdInvitedOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdInvitationAnswerOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdClanAccept = new Rect(0f, 0f, 77f, 26f);

	public Rect crdClanRefuse = new Rect(0f, 0f, 77f, 26f);

	private bool attached;

	private string memoContents;

	private void ShowClanInvitation()
	{
		if (memo != null && memo.attached == "000" && memo.option >= 0)
		{
			GUI.Box(crdInvitationAnswerOutline, string.Empty, "BoxPopLine");
			if (GlobalVars.Instance.MyButton(crdClanAccept, StringMgr.Instance.Get("ACCEPT_CLAN_INVITATION"), "BtnChat"))
			{
				string contents = "CLAN_INVITATION_ACCEPTED" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.Nickname;
				CSNetManager.Instance.Sock.SendCS_ANSWER_CLAN_INVITATION_REQ(memo.seq, memo.option, accept: true, "REPLY_CLAN_INVITATION", contents);
			}
			if (GlobalVars.Instance.MyButton(crdClanRefuse, StringMgr.Instance.Get("REFUSE"), "BtnChat"))
			{
				string contents2 = "CLAN_INVITATION_REFUSED" + GlobalVars.DELIMITER + "n" + MyInfoManager.Instance.Nickname;
				CSNetManager.Instance.Sock.SendCS_ANSWER_CLAN_INVITATION_REQ(memo.seq, memo.option, accept: false, "REPLY_CLAN_INVITATION", contents2);
			}
		}
	}

	private void ShowPresent()
	{
		if (memo != null && memo.attached != "000")
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(memo.attached);
			if (tItem != null && null != tItem.CurIcon())
			{
				GUI.Box(crdAttachedOutline, string.Empty, "BoxPopLine");
				GUI.Box(crdPresentOutline, string.Empty, "BoxFadeBlue");
				GUI.DrawTexture(new Rect(crdItemIcon.x, crdItemIcon.y, (float)tItem.CurIcon().width, (float)tItem.CurIcon().height), tItem.CurIcon());
				if (GlobalVars.Instance.MyButton(crdPresent, StringMgr.Instance.Get("RECEIVING"), "BtnAction"))
				{
					CSNetManager.Instance.Sock.SendCS_RCV_PRESENT_REQ(memo.seq, memo.attached, memo.option, tItem.IsAmount);
				}
				LabelUtil.TextOut(crdItemName, tItem.Name, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(crdOption, tItem.GetOptionStringByOption(memo.option), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				attached = true;
			}
			else
			{
				attached = false;
			}
		}
	}

	private bool IsInvitation()
	{
		return memo.attached == "000";
	}

	private bool HaveAttachedItem()
	{
		TItem tItem = null;
		if (memo != null)
		{
			Good good = ShopManager.Instance.Get(memo.attached);
			if (good != null)
			{
				tItem = good.tItem;
			}
		}
		return tItem != null;
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RECV_MEMO;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("RECV_MEMO"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdSenderLabel, StringMgr.Instance.Get("MEMO_SND"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		if (memo.needReply())
		{
			LabelUtil.TextOut(crdSender, memo.sender, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			LabelUtil.TextOut(crdSender, StringMgr.Instance.Get("GAME_TITLE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		LabelUtil.TextOut(crdMemoTitleLabel, StringMgr.Instance.Get("MEMO_TITLE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		if (memo.needStringKey())
		{
			memoContents = GlobalVars.Instance.DelimiterProcess(memo.title);
			LabelUtil.TextOut(crdMemoTitle, crdTitleSize, memoContents, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			LabelUtil.TextOut(crdMemoTitle, crdTitleSize, memo.title, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		Rect screenRect = crdAttachedArea;
		if (IsInvitation())
		{
			GUI.Box(crdInvitedOutline, string.Empty, "BoxPopLine");
		}
		else if (HaveAttachedItem())
		{
			GUI.Box(crdAttachedOutline, string.Empty, "BoxPopLine");
		}
		else
		{
			GUI.Box(crdOutline, string.Empty, "BoxPopLine");
			screenRect = crdArea;
		}
		GUILayout.BeginArea(screenRect);
		spContents = GUILayout.BeginScrollView(spContents, GUILayout.Width(screenRect.width), GUILayout.Height(screenRect.height));
		if (memo.needStringKey())
		{
			memoContents = GlobalVars.Instance.DelimiterProcess(memo.contents);
			GUILayout.Label(memoContents);
		}
		else
		{
			GUILayout.Label(memo.contents);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		ShowPresent();
		ShowClanInvitation();
		if (memo.needReply() && GlobalVars.Instance.MyButton(crdReply, StringMgr.Instance.Get("DO_REPLY"), "BtnAction"))
		{
			parent.Reply(memo.sender, StringMgr.Instance.Get("REPLY") + ":" + memo.title);
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdDelete, StringMgr.Instance.Get("DELETE"), "BtnAction"))
		{
			if (attached)
			{
				MessageBoxMgr.Instance.AddSelectMessage(StringMgr.Instance.Get("ITEM_DELETE_CONFIRM"));
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_DEL_MEMO_REQ(memo.seq);
				result = true;
			}
		}
		if (MyInfoManager.Instance.MsgBoxConfirm)
		{
			CSNetManager.Instance.Sock.SendCS_DEL_MEMO_REQ(memo.seq);
			result = true;
			MyInfoManager.Instance.MsgBoxConfirm = false;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	public void InitDialog(Vector2 pos, Memo mm, MemoDialog prnt)
	{
		rc.x = pos.x;
		rc.y = pos.y;
		memo = mm;
		parent = prnt;
		spContents = Vector2.zero;
		if (!memo.IsRead)
		{
			memo.IsRead = true;
			CSNetManager.Instance.Sock.SendCS_READ_MEMO_REQ(memo.seq);
		}
	}
}
