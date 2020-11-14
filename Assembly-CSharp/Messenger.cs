using System;
using UnityEngine;

[Serializable]
public class Messenger
{
	public Texture2D textBg;

	public string[] tabKey;

	private string[] tab;

	private int selected;

	private Vector2 spChannel;

	private Vector2 spFriend;

	private Vector2 spClan;

	private float onceASecond;

	private Rect crdFrame = new Rect(806f, 488f, 216f, 274f);

	private Rect crdUserList = new Rect(809f, 526f, 196f, 230f);

	private Rect crdGrid = new Rect(809f, 489f, 196f, 32f);

	private Rect crdFrameTemp = new Rect(806f, 488f, 216f, 274f);

	private Rect crdUserListTemp = new Rect(809f, 526f, 196f, 230f);

	private Rect crdGridTemp = new Rect(809f, 489f, 196f, 34f);

	private Vector2 crdUser = new Vector2(145f, 18f);

	private Vector2 crdBadge = new Vector2(2f, 2f);

	private Vector2 crdBadgeSize = new Vector2(28f, 13f);

	private Vector2 crdNick = new Vector2(30f, -3f);

	private Vector2 crdPopupOffset = new Vector2(195f, 0f);

	private bool isBriefing;

	private float fChangeHeight;

	public bool IsBriefing
	{
		set
		{
			isBriefing = value;
		}
	}

	public void Start()
	{
	}

	private void UpdateChannelTab()
	{
		tab = new string[tabKey.Length];
		for (int i = 0; i < tabKey.Length; i++)
		{
			tab[i] = StringMgr.Instance.Get(tabKey[i]);
		}
	}

	public void ToggleLeftTop()
	{
		if (isBriefing)
		{
			crdFrame.x -= crdPopupOffset.x;
			crdFrame.y -= crdPopupOffset.y;
			crdUserList.x -= crdPopupOffset.x;
			crdUserList.y -= crdPopupOffset.y;
			crdGrid.x -= crdPopupOffset.x;
			crdGrid.y -= crdPopupOffset.y;
		}
		else
		{
			crdFrame = crdFrameTemp;
			crdUserList = crdUserListTemp;
			crdGrid = crdGridTemp;
		}
	}

	public void Update()
	{
		onceASecond += Time.deltaTime;
		if (onceASecond > 1f)
		{
			onceASecond = 0f;
			if (selected >= 1)
			{
				CSNetManager.Instance.Sock.SendCS_WHATSUP_FELLA_REQ();
			}
		}
		ChannelUserManager.Instance.Refresh();
	}

	public void ChangeHeight(float h)
	{
		fChangeHeight = h;
		if (h > 0.001f)
		{
			crdFrame.y -= h;
			crdFrame.height += h;
		}
		else
		{
			crdFrame = crdFrameTemp;
		}
	}

	public void OnGUI()
	{
		Rect position = new Rect(crdFrame);
		GUI.Box(position, string.Empty, "BoxChatBase");
		UpdateChannelTab();
		if (isBriefing)
		{
			GUI.Box(crdFrame, string.Empty);
		}
		Vector2 pos = new Vector2(0f, 0f);
		Rect position2 = new Rect(crdUserList.x, crdUserList.y, crdUserList.width, crdUserList.height);
		position2.y -= fChangeHeight;
		position2.height += fChangeHeight;
		Rect position3 = new Rect(crdGrid);
		position3.y -= fChangeHeight;
		selected = GUI.SelectionGrid(position3, selected, tab, tab.Length, "BtnChat");
		if (selected == 0)
		{
			NameCard[] array = ChannelUserManager.Instance.ToArray();
			spChannel = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdUser.x, crdUser.y * (float)array.Length), position: position2, scrollPosition: spChannel);
			float y = spChannel.y;
			float num = spChannel.y + position2.height;
			for (int i = 0; i < 7 || i < array.Length; i++)
			{
				float y2 = pos.y;
				float num2 = pos.y + crdUser.y;
				if (num2 >= y && y2 <= num && i < array.Length)
				{
					aPlayer(pos, array[i]);
				}
				pos.y += crdUser.y;
			}
			GUI.EndScrollView();
		}
		else if (selected == 1)
		{
			NameCard[] friends = MyInfoManager.Instance.GetFriends(connectedOnly: true);
			spFriend = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdUser.x, crdUser.y * (float)friends.Length), position: position2, scrollPosition: spFriend);
			float y3 = spFriend.y;
			float num3 = spFriend.y + position2.height;
			for (int j = 0; j < 7 || j < friends.Length; j++)
			{
				float y4 = pos.y;
				float num4 = pos.y + crdUser.y;
				if (num4 >= y3 && y4 <= num3 && j < friends.Length)
				{
					aPlayer(pos, friends[j]);
				}
				pos.y += crdUser.y;
			}
			GUI.EndScrollView();
		}
		else if (selected == 2)
		{
			NameCard[] clanees = MyInfoManager.Instance.GetClanees(connectedOnly: true);
			spClan = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdUser.x, crdUser.y * (float)clanees.Length), position: position2, scrollPosition: spClan);
			float y5 = spClan.y;
			float num5 = spClan.y + position2.height;
			for (int k = 0; k < 7 || k < clanees.Length; k++)
			{
				float y6 = pos.y;
				float num6 = pos.y + crdUser.y;
				if (num6 >= y5 && y6 <= num5 && k < clanees.Length)
				{
					aPlayer(pos, clanees[k]);
				}
				pos.y += crdUser.y;
			}
			GUI.EndScrollView();
		}
	}

	private void aPlayer(Vector2 pos, NameCard player)
	{
		Texture2D badge = XpManager.Instance.GetBadge(player.Lv, player.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(pos.x + crdBadge.x, pos.y + crdBadge.y, crdBadgeSize.x, crdBadgeSize.y), badge);
		}
		if (GlobalVars.Instance.MyButton(new Rect(pos.x, pos.y, crdUser.x, crdUser.y), string.Empty, "InvisibleButton") && Event.current.button == 1)
		{
			UserMenu userMenu = ContextMenuManager.Instance.Popup();
			if (userMenu != null)
			{
				bool flag = MyInfoManager.Instance.IsClanee(player.Seq);
				userMenu.InitDialog(MouseUtil.ScreenToPixelPoint(Input.mousePosition), player.Seq, player.Nickname, !flag, masterAssign: false);
			}
		}
		LabelUtil.TextOut(new Vector2(pos.x + crdNick.x, pos.y + crdNick.y), player.Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}
}
