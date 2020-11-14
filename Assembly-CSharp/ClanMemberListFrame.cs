using System;
using UnityEngine;

[Serializable]
public class ClanMemberListFrame
{
	private Rect crdOutline = new Rect(714f, 86f, 295f, 670f);

	private Rect crdClanMemberLabel = new Rect(721f, 91f, 280f, 22f);

	private Vector2 crdClanMemberText = new Vector2(867f, 102f);

	private Rect crdUserList = new Rect(721f, 125f, 280f, 616f);

	private Vector2 crdUser = new Vector2(260f, 27f);

	private float firstClanMemberY;

	private float clanMemberOffset = 28f;

	private Vector2 crdBadge = new Vector2(3f, 4f);

	private Vector2 crdBadgeSize = new Vector2(34f, 17f);

	private Vector2 crdNickname = new Vector2(38f, 3f);

	private Vector2 spClan = Vector2.zero;

	private int curClanMember;

	private float onceASecond;

	public void Start()
	{
		onceASecond = 0f;
	}

	public void Update()
	{
		onceASecond += Time.deltaTime;
		if (onceASecond > 1f)
		{
			onceASecond = 0f;
			CSNetManager.Instance.Sock.SendCS_WHATSUP_FELLA_REQ();
		}
	}

	private float aPlayer(NameCard clan, float y)
	{
		Texture2D badge = XpManager.Instance.GetBadge(clan.Lv, clan.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(crdBadge.x, y + crdBadge.y, crdBadgeSize.x, crdBadgeSize.y), badge);
		}
		LabelUtil.TextOut(new Vector2(crdNickname.x, crdNickname.y + y), clan.Nickname, "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		return y + clanMemberOffset;
	}

	public void OnGUI()
	{
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		GUI.Box(crdClanMemberLabel, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdClanMemberText, StringMgr.Instance.Get("CLAN_MEMBERS"), "Label", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
		NameCard[] claneesIncludeMe = MyInfoManager.Instance.GetClaneesIncludeMe(connectedOnly: true);
		string[] array = new string[claneesIncludeMe.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = string.Empty;
		}
		Rect rect = new Rect(0f, 0f, crdUser.x, (float)claneesIncludeMe.Length * (crdUser.y + 1f));
		spClan = GUI.BeginScrollView(crdUserList, spClan, rect);
		curClanMember = GUI.SelectionGrid(rect, curClanMember, array, 1, "BoxGridStyle");
		float y = firstClanMemberY;
		for (int j = 0; j < claneesIncludeMe.Length; j++)
		{
			y = aPlayer(claneesIncludeMe[j], y);
		}
		GUI.EndScrollView();
	}
}
