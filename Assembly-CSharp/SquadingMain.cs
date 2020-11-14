using UnityEngine;

public class SquadingMain : MonoBehaviour
{
	public SquadListFrame squadList;

	public ClanMemberListFrame clanMemberList;

	public LobbyChat lobbyChat;

	public SquadingTool squadingTool;

	private Rect crdSquading = new Rect(0f, 0f, 1024f, 768f);

	private Vector2 crdSquadingTitleLabel = new Vector2(512f, 26f);

	private float deltaTime;

	private void Start()
	{
		deltaTime = 0f;
		squadList.Start();
		clanMemberList.Start();
		lobbyChat.Start();
		lobbyChat.SetChatStyle(LOBBYCHAT_STYLE.CLANMATCH);
		squadingTool.Start(squadList);
	}

	private void Update()
	{
		squadList.Update();
		clanMemberList.Update();
		lobbyChat.Update();
		squadingTool.Update();
		deltaTime += Time.deltaTime;
		if (deltaTime > 0.5f)
		{
			deltaTime = 0f;
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
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
			SquadManager.Instance.Clear();
			Application.LoadLevel("Lobby");
		}
	}

	private void OnGUI()
	{
		GlobalVars.Instance.BeginGUI(VersionTextureManager.Instance.seasonTexture.texScreenBg);
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GUI.Box(crdSquading, string.Empty, "BoxPopupBg");
		LabelUtil.TextOut(crdSquadingTitleLabel, StringMgr.Instance.Get("MATCH_TEAM_LIST"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		squadList.OnGUI();
		clanMemberList.OnGUI();
		lobbyChat.OnGUI();
		squadingTool.OnGUI();
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void OnChat(ChatText chat)
	{
		lobbyChat.Enqueue(chat);
	}
}
