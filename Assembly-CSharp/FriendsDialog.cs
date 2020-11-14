using System;
using UnityEngine;

[Serializable]
public class FriendsDialog : Dialog
{
	public Texture2D selectedBox;

	public Texture2D friendsTextBg;

	public Texture2D icon;

	public Texture2D grayFoot;

	public Texture2D colorFoot;

	public string[] tabKey;

	private string[] tab;

	private int selected;

	private Vector2 spFriend = Vector2.zero;

	private Vector2 spBan = Vector2.zero;

	private int currentFriend = -1;

	private int currentBan = -1;

	private float onceASecond;

	public Rect crdIcon = new Rect(0f, 0f, 12f, 12f);

	private Rect crdTab = new Rect(27f, 53f, 347f, 27f);

	private Rect crdOutline = new Rect(23f, 79f, 355f, 311f);

	public Rect crdView = new Rect(34f, 92f, 335f, 279f);

	public Vector2 crdItemSize = new Vector2(304f, 31f);

	public Rect crdBadge = new Rect(4f, 7f, 34f, 17f);

	public Vector2 crdNickname = new Vector2(40f, 15f);

	public Vector2 crdCon = new Vector2(300f, 15f);

	private Rect crdAddFriend = new Rect(114f, 415f, 130f, 34f);

	private Rect crdDelFriend = new Rect(250f, 415f, 130f, 34f);

	private Rect crdAddBan = new Rect(114f, 415f, 130f, 34f);

	private Rect crdDelBan = new Rect(250f, 415f, 130f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.FRIENDS;
		tab = new string[tabKey.Length];
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		for (int i = 0; i < tabKey.Length; i++)
		{
			tab[i] = StringMgr.Instance.Get(tabKey[i]);
		}
	}

	public void InitDialog()
	{
	}

	public override void Update()
	{
		onceASecond += Time.deltaTime;
		if (onceASecond > 1f)
		{
			onceASecond = 0f;
			if (selected == 0)
			{
				CSNetManager.Instance.Sock.SendCS_WHATSUP_FELLA_REQ();
			}
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("FRIEND_MANAGEMENT"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "BoxPopLine");
		selected = GUI.SelectionGrid(crdTab, selected, tab, 2, "PopTab");
		if (selected == 0)
		{
			NameCard[] friends = MyInfoManager.Instance.GetFriends(connectedOnly: false);
			if (currentFriend >= friends.Length)
			{
				currentFriend = friends.Length - 1;
			}
			spFriend = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdItemSize.x, crdItemSize.y * (float)friends.Length), position: crdView, scrollPosition: spFriend);
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < 9 || i < friends.Length; i++)
			{
				if (i % 2 == 0)
				{
					GUI.Box(new Rect(num, num2, crdItemSize.x, crdItemSize.y), string.Empty, "BoxFadeBlue");
				}
				if (i < friends.Length)
				{
					Texture2D badge = XpManager.Instance.GetBadge(friends[i].Lv, friends[i].Rank);
					if (null != badge)
					{
						TextureUtil.DrawTexture(new Rect(num + crdBadge.x, num2 + crdBadge.y, crdBadge.width, crdBadge.height), badge, ScaleMode.StretchToFill);
					}
					LabelUtil.TextOut(new Vector2(num + crdNickname.x, num2 + crdNickname.y), friends[i].Nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
					if (GlobalVars.Instance.MyButton(new Rect(num, num2, crdItemSize.x, crdItemSize.y), string.Empty, "InvisibleButton"))
					{
						currentFriend = i;
						if (Event.current.button == 1)
						{
							UserMenu userMenu = ContextMenuManager.Instance.Popup();
							if (userMenu != null)
							{
								bool flag = MyInfoManager.Instance.IsClanee(friends[i].Seq);
								userMenu.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), friends[i].Seq, friends[i].Nickname, !flag, masterAssign: false);
							}
						}
					}
					if (!friends[i].IsConnected)
					{
						Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(150, 150, 150);
						LabelUtil.TextOut(new Vector2(num + crdCon.x, num2 + crdCon.y), StringMgr.Instance.Get("LOGOFF"), "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					}
					else
					{
						Channel channel = ChannelManager.Instance.Get(friends[i].SvrId);
						if (channel != null)
						{
							LabelUtil.TextOut(new Vector2(num + crdCon.x, num2 + crdCon.y), channel.Name, "Label", Color.green, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
						}
					}
					if (currentFriend == i)
					{
						TextureUtil.DrawTexture(new Rect(num, num2, crdItemSize.x, crdItemSize.y), selectedBox, ScaleMode.StretchToFill);
					}
				}
				num2 += crdItemSize.y;
			}
			GUI.EndScrollView();
			if (GlobalVars.Instance.MyButton(crdAddFriend, StringMgr.Instance.Get("ADD_FRIEND"), "BtnAction"))
			{
				((AddFriendDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ADD_FRIEND, exclusive: false))?.InitDialog();
			}
			if (GlobalVars.Instance.MyButton(crdDelFriend, StringMgr.Instance.Get("DEL_FRIEND"), "BtnAction") && 0 <= currentFriend && currentFriend < friends.Length)
			{
				CSNetManager.Instance.Sock.SendCS_DEL_FRIEND_REQ(friends[currentFriend].Seq);
			}
		}
		else if (selected == 1)
		{
			NameCard[] bans = MyInfoManager.Instance.GetBans();
			if (currentBan >= bans.Length)
			{
				currentBan = bans.Length - 1;
			}
			spBan = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdItemSize.x, crdItemSize.y * (float)bans.Length), position: crdView, scrollPosition: spBan);
			float num3 = 0f;
			float num4 = 0f;
			for (int j = 0; j < 9 || j < bans.Length; j++)
			{
				if (j % 2 == 0)
				{
					GUI.Box(new Rect(num3, num4, crdItemSize.x, crdItemSize.y), string.Empty, "BoxFadeBlue");
				}
				if (j < bans.Length)
				{
					LabelUtil.TextOut(new Vector2(num3 + 5f, num4 + crdNickname.y), bans[j].Nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
					if (GlobalVars.Instance.MyButton(new Rect(num3, num4, crdItemSize.x, crdItemSize.y), string.Empty, "InvisibleButton"))
					{
						currentBan = j;
						if (Event.current.button == 1)
						{
							ContextMenuManager.Instance.Popup()?.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), bans[j].Seq, bans[j].Nickname, clanInvitable: false, masterAssign: false);
						}
					}
					if (j == currentBan)
					{
						TextureUtil.DrawTexture(new Rect(num3, num4, crdItemSize.x, crdItemSize.y), selectedBox, ScaleMode.ScaleToFit);
					}
				}
				num4 += crdItemSize.y;
			}
			GUI.EndScrollView();
			if (GlobalVars.Instance.MyButton(crdAddBan, StringMgr.Instance.Get("ADD_BAN"), "BtnAction"))
			{
				((AddBanDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ADD_BAN, exclusive: false))?.InitDialog();
			}
			if (GlobalVars.Instance.MyButton(crdDelBan, StringMgr.Instance.Get("DEL_BAN"), "BtnAction") && 0 <= currentBan && currentBan < bans.Length)
			{
				CSNetManager.Instance.Sock.SendCS_DEL_BAN_REQ(bans[currentBan].Seq);
			}
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		bool flag2 = DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.ADD_FRIEND) || DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.ADD_BAN);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || (!flag2 && GlobalVars.Instance.IsEscapePressed()))
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
