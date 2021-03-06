using System.Collections.Generic;
using UnityEngine;

public class ExplosionMatchSituation : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public Texture2D redTeam;

	public Texture2D blueTeam;

	private Vector2 crdFrame = new Vector2(492f, 520f);

	private Vector2 crdRoomTitle = new Vector2(10f, 0f);

	private Rect crdRedTeamTitle = new Rect(11f, 26f, 472f, 22f);

	private Rect crdBlueTeamTitle = new Rect(11f, 273f, 472f, 22f);

	private Rect crdRedTeamIcon = new Rect(200f, 21f, 86f, 34f);

	private Rect crdBlueTeamIcon = new Rect(200f, 267f, 86f, 34f);

	private Rect crdRedFirstLow = new Rect(11f, 48f, 472f, 19f);

	private Rect crdBlueFirstLow = new Rect(11f, 295f, 472f, 19f);

	private float redResultY = 57f;

	private float blueResultY = 304f;

	private Vector2 clanMarkSize = new Vector2(16f, 16f);

	private Vector2 badgeSize = new Vector2(34f, 17f);

	private float redY = 80f;

	private float blueY = 325f;

	private float markX = 53f;

	private float badgeX = 97f;

	private float nickX = 176f;

	private float killX = 280f;

	private float missionX = 350f;

	private float scoreX = 395f;

	private float pingX = 439f;

	private float offset = 21f;

	private float myRowX = 11f;

	private Vector2 myRowSize = new Vector2(472f, 20f);

	private Rect crdRedTeamSituation = new Rect(11f, 67f, 472f, 192f);

	private Rect crdBlueTeamSituation = new Rect(11f, 314f, 472f, 192f);

	private bool on;

	public UIFlickerColor redTeamMyTeam;

	public UIFlickerColor blueTeamMyTeam;

	private LocalController localController;

	private void Start()
	{
	}

	private void VerifyLocalController()
	{
		if (null == localController)
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				localController = gameObject.GetComponent<LocalController>();
			}
		}
	}

	private void DrawClanMark(Rect rc, int mark)
	{
		if (mark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(mark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(mark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(mark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(rc, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(rc, amblum);
			}
			GUI.color = color;
		}
	}

	private float GridOut(int clanMark, int xp, int rank, string nickname, int kill, int death, int assist, int mission, bool isWatching, int score, float avgPing, int status, bool isDead, float y, bool isHost)
	{
		DrawClanMark(new Rect(markX - clanMarkSize.x / 2f, y - clanMarkSize.y / 2f + 1f, clanMarkSize.x, clanMarkSize.y), clanMark);
		Texture2D badge = XpManager.Instance.GetBadge(XpManager.Instance.GetLevel(xp), rank);
		TextureUtil.DrawTexture(new Rect(badgeX - badgeSize.x / 2f, y - badgeSize.y / 2f + 1f, badgeSize.x, badgeSize.y), badge);
		if (isDead)
		{
			LabelUtil.TextOut(new Vector2(nickX, y), nickname, "MiniLabel", Color.grey, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(nickX, y), nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		LabelUtil.TextOut(new Vector2(killX, y), kill.ToString() + "/" + assist.ToString() + "/" + death.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(missionX, y), mission.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(scoreX, y), score.ToString(), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (status == 4)
		{
			string text = "x";
			Color clrText = Color.red;
			Color clrOutline = Color.black;
			if (isWatching)
			{
				text = StringMgr.Instance.Get("WATCHING");
				clrText = Color.gray;
				clrOutline = GlobalVars.txtEmptyColor;
			}
			else if (avgPing < float.PositiveInfinity)
			{
				text = avgPing.ToString("0.###");
				clrText = ((avgPing > 0.3f) ? Color.red : ((!(avgPing > 0.1f)) ? Color.green : Color.yellow));
				clrOutline = GlobalVars.txtEmptyColor;
			}
			LabelUtil.TextOut(new Vector2(pingX, y), text, "MiniLabel", clrText, clrOutline, TextAnchor.MiddleCenter);
		}
		else
		{
			string text2 = StringMgr.Instance.Get("PLAYER_IS_WAITING");
			if (status == 2 || status == 3)
			{
				text2 = StringMgr.Instance.Get("PLAYER_IS_LOADING");
			}
			LabelUtil.TextOut(new Vector2(pingX, y), text2, "MiniLabel", Color.blue, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		return y + offset;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn && on)
		{
			VerifyLocalController();
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			GUI.BeginGroup(new Rect(((float)Screen.width - crdFrame.x) / 2f, ((float)Screen.height - crdFrame.y) / 2f, crdFrame.x, crdFrame.y));
			GUI.Box(new Rect(0f, 0f, crdFrame.x, crdFrame.y), string.Empty, "BoxResult");
			Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
			if (room != null)
			{
				LabelUtil.TextOut(crdRoomTitle, room.GetString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			GUI.Box(crdRedTeamTitle, string.Empty, "BoxRed02");
			GUI.Box(crdRedFirstLow, string.Empty, "BoxResultText");
			TextureUtil.DrawTexture(crdRedTeamIcon, redTeam, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(markX, redResultY), StringMgr.Instance.Get("MARK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(badgeX, redResultY), StringMgr.Instance.Get("BADGE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(nickX, redResultY), StringMgr.Instance.Get("CHARACTER"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(killX, redResultY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(missionX, redResultY), StringMgr.Instance.Get("MISSION"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(scoreX, redResultY), StringMgr.Instance.Get("SCORE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(pingX, redResultY), StringMgr.Instance.Get("PING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			float num = redY;
			GUI.Box(crdRedTeamSituation, string.Empty, "BoxResult01");
			bool flag = false;
			List<KeyValuePair<int, BrickManDesc>> list = BrickManManager.Instance.ToSortedList();
			for (int i = 0; i < list.Count; i++)
			{
				BrickManDesc value = list[i].Value;
				if (value != null && value.Slot < 8)
				{
					GameObject gameObject = BrickManManager.Instance.Get(value.Seq);
					if (!(null == gameObject))
					{
						TPController component = gameObject.GetComponent<TPController>();
						if (!(null == component))
						{
							if (MyInfoManager.Instance.Slot < 8 && !flag && (MyInfoManager.Instance.Score > value.Score || (MyInfoManager.Instance.Score == value.Score && MyInfoManager.Instance.Kill > value.Kill)))
							{
								flag = true;
								float num2 = num;
								num = GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.Kill, MyInfoManager.Instance.Death, MyInfoManager.Instance.Assist, MyInfoManager.Instance.Mission, MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
								GUI.Box(new Rect(myRowX, num2 - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
							}
							Peer peer = P2PManager.Instance.Get(value.Seq);
							num = GridOut(value.ClanMark, value.Xp, value.Rank, value.Nickname, value.Kill, value.Death, value.Assist, value.Mission, value.IsHidePlayer, value.Score, peer?.PingTime ?? float.PositiveInfinity, value.Status, component.IsDead, num, value.Seq == RoomManager.Instance.Master);
						}
					}
				}
			}
			if (MyInfoManager.Instance.Slot < 8 && !flag)
			{
				GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.Kill, MyInfoManager.Instance.Death, MyInfoManager.Instance.Assist, MyInfoManager.Instance.Mission, MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
				GUI.Box(new Rect(myRowX, num - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
			}
			GUI.Box(crdBlueTeamTitle, string.Empty, "BoxBlue02");
			GUI.Box(crdBlueFirstLow, string.Empty, "BoxResultText");
			TextureUtil.DrawTexture(crdBlueTeamIcon, blueTeam, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(markX, blueResultY), StringMgr.Instance.Get("MARK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(badgeX, blueResultY), StringMgr.Instance.Get("BADGE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(nickX, blueResultY), StringMgr.Instance.Get("CHARACTER"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(killX, blueResultY), StringMgr.Instance.Get("KILL") + "/" + StringMgr.Instance.Get("ASSIST") + "/" + StringMgr.Instance.Get("DEATH"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(missionX, blueResultY), StringMgr.Instance.Get("MISSION"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(scoreX, blueResultY), StringMgr.Instance.Get("SCORE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(pingX, blueResultY), StringMgr.Instance.Get("PING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			num = blueY;
			GUI.Box(crdBlueTeamSituation, string.Empty, "BoxResult01");
			for (int j = 0; j < list.Count; j++)
			{
				BrickManDesc value2 = list[j].Value;
				if (value2 != null && value2.Slot >= 8)
				{
					GameObject gameObject2 = BrickManManager.Instance.Get(value2.Seq);
					if (!(null == gameObject2))
					{
						TPController component2 = gameObject2.GetComponent<TPController>();
						if (!(null == component2))
						{
							if (MyInfoManager.Instance.Slot >= 8 && !flag && (MyInfoManager.Instance.Score > value2.Score || (MyInfoManager.Instance.Score == value2.Score && MyInfoManager.Instance.Kill > value2.Kill)))
							{
								flag = true;
								float num3 = num;
								num = GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.Kill, MyInfoManager.Instance.Death, MyInfoManager.Instance.Assist, MyInfoManager.Instance.Mission, MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
								GUI.Box(new Rect(myRowX, num3 - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
							}
							Peer peer2 = P2PManager.Instance.Get(value2.Seq);
							num = GridOut(value2.ClanMark, value2.Xp, value2.Rank, value2.Nickname, value2.Kill, value2.Death, value2.Assist, value2.Mission, value2.IsHidePlayer, value2.Score, peer2?.PingTime ?? float.PositiveInfinity, value2.Status, component2.IsDead, num, value2.Seq == RoomManager.Instance.Master);
						}
					}
				}
			}
			if (MyInfoManager.Instance.Slot >= 8 && !flag)
			{
				GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, MyInfoManager.Instance.Kill, MyInfoManager.Instance.Death, MyInfoManager.Instance.Assist, MyInfoManager.Instance.Mission, MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
				GUI.Box(new Rect(myRowX, num - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
			}
			if (MyInfoManager.Instance.IsRedTeam())
			{
				redTeamMyTeam.Draw();
			}
			else
			{
				blueTeamMyTeam.Draw();
			}
			GUI.EndGroup();
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		on = (!DialogManager.Instance.IsModal && custom_inputs.Instance.GetButton("K_SITUATION"));
		redTeamMyTeam.Update();
		blueTeamMyTeam.Update();
	}
}
