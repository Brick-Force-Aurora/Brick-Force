using UnityEngine;

public class SquadMain : MonoBehaviour
{
	public ClanMemberListFrame clanMemberList;

	public LobbyChat lobbyChat;

	public SquadMemberListFrame squadMemberList;

	public SquadMode squadMode;

	public SquadTool squadTool;

	private Rect crdSquading = new Rect(0f, 0f, 1024f, 768f);

	private Vector2 crdSquadingTitleLabel = new Vector2(512f, 26f);

	private Rect crdSquadInfo = new Rect(20f, 91f, 668f, 22f);

	private Vector2 crdName = new Vector2(41f, 91f);

	private Vector2 crdMemberCount = new Vector2(356f, 91f);

	private Vector2 crdRecord = new Vector2(670f, 91f);

	private Vector2 crdMatchingProgress = new Vector2(300f, 160f);

	private Vector2 crdMatching = new Vector2(150f, 82f);

	private Vector2 crdMatchingTitle = new Vector2(12f, 8f);

	private Rect crdSearchingCancel = new Rect(60f, 120f, 180f, 34f);

	private float deltaTime;

	private int dotCount;

	public Texture2D texBg;

	private void Start()
	{
		deltaTime = 0f;
		clanMemberList.Start();
		lobbyChat.Start();
		lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.CLANMATCH);
		squadMemberList.Start();
		squadMode.Start();
		squadTool.Start(squadMemberList);
	}

	private void Update()
	{
		clanMemberList.Update();
		lobbyChat.Update();
		squadMemberList.Update();
		squadMode.Update();
		squadTool.Update();
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.5f)
		{
			deltaTime = 0f;
			dotCount++;
			if (dotCount > 3)
			{
				dotCount = 0;
			}
			if (MyInfoManager.Instance.ClanSeq < 0)
			{
				OnClanExiled();
			}
		}
	}

	private void OnClanExiled()
	{
		if (!Application.isLoadingLevel)
		{
			DialogManager.Instance.CloseAll();
			ContextMenuManager.Instance.CloseAll();
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_EXILED_FROM_CLAN"));
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
			SquadManager.Instance.Leave();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
			SquadManager.Instance.Clear();
			Application.LoadLevel("Lobby");
		}
	}

	private void MatchingProgress(int id)
	{
		string text = StringMgr.Instance.Get("CLAN_MATCH_SEARCING");
		for (int i = 0; i < dotCount; i++)
		{
			text += ".";
		}
		LabelUtil.TextOut(crdMatchingTitle, StringMgr.Instance.Get("CLAN_MATCH_SEARCH"), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMatching, text, "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleCenter);
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null && SquadManager.Instance.CurSquad.Leader == MyInfoManager.Instance.Seq && GlobalVars.Instance.MyButton(crdSearchingCancel, StringMgr.Instance.Get("CLAN_MATCH_CANCEL"), "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_CANCEL_REQ();
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		if (SquadManager.Instance.IsMatching)
		{
			Rect clientRect = new Rect(((float)Screen.width - crdMatchingProgress.x) / 2f, ((float)Screen.height - crdMatchingProgress.y) / 2f, crdMatchingProgress.x, crdMatchingProgress.y);
			GUI.Window(1024, clientRect, MatchingProgress, string.Empty);
		}
		GUI.enabled = (!SquadManager.Instance.IsMatching && !DialogManager.Instance.IsModal);
		TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), texBg, ScaleMode.StretchToFill);
		GUI.Box(crdSquading, string.Empty, "BoxPopupBg");
		LabelUtil.TextOut(crdSquadingTitleLabel, StringMgr.Instance.Get("MY_MATCH_TEAM"), "BigLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		GUI.Box(crdSquadInfo, string.Empty, "BoxFadeBlue");
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null)
		{
			LabelUtil.TextOut(crdName, curSquad.Name, "Label", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
			LabelUtil.TextOut(crdMemberCount, curSquad.MemberCountString, "Label", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
			LabelUtil.TextOut(crdRecord, curSquad.Record, "Label", txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperRight);
		}
		clanMemberList.OnGUI();
		lobbyChat.OnGUI();
		squadMemberList.OnGUI();
		squadMode.OnGUI();
		squadTool.OnGUI();
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void OnChat(ChatText chat)
	{
		lobbyChat.Enqueue(chat);
	}
}
