using System.Collections.Generic;
using UnityEngine;

public class ZombieMatchSituation : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	public Texture2D indIcon;

	public Vector2 crdFrame = new Vector2(492f, 450f);

	public Vector2 crdRoomTitle = new Vector2(13f, 9f);

	public Rect crdBoxGreen = new Rect(10f, 30f, 472f, 22f);

	public Rect crdFirstLow = new Rect(9f, 52f, 472f, 16f);

	public Rect crdIndIcon = new Rect(200f, 10f, 57f, 34f);

	public float resultY = 61f;

	public float markX = 55f;

	public float badgeX = 100f;

	public float nickX = 177f;

	public float zombieX = 289f;

	public float scoreX = 381f;

	public float pingX = 440f;

	public Rect crdSituation = new Rect(10f, 69f, 472f, 369f);

	public float yy = 90f;

	public Vector2 clanMarkSize = new Vector2(16f, 16f);

	public Vector2 badgeSize = new Vector2(34f, 17f);

	public float offset = 21f;

	public float myRowX = 10f;

	public Vector2 myRowSize = new Vector2(474f, 22f);

	private bool on;

	private LocalController localController;

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
			GUI.Box(crdBoxGreen, string.Empty, "BoxGreen02");
			GUI.Box(crdFirstLow, string.Empty, "BoxResultText");
			TextureUtil.DrawTexture(crdIndIcon, indIcon, ScaleMode.StretchToFill);
			LabelUtil.TextOut(new Vector2(markX, resultY), StringMgr.Instance.Get("MARK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(badgeX, resultY), StringMgr.Instance.Get("BADGE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(nickX, resultY), StringMgr.Instance.Get("CHARACTER"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(zombieX, resultY), StringMgr.Instance.Get("ZOMBIE") + "/" + StringMgr.Instance.Get("BRICKMAN"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(scoreX, resultY), StringMgr.Instance.Get("SCORE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(new Vector2(pingX, resultY), StringMgr.Instance.Get("PING"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			float num = yy;
			GUI.Box(crdSituation, string.Empty, "BoxResult01");
			bool flag = false;
			List<KeyValuePair<int, BrickManDesc>> list = BrickManManager.Instance.ToSortedList();
			for (int i = 0; i < list.Count; i++)
			{
				BrickManDesc value = list[i].Value;
				if (value != null && value.Slot < 16)
				{
					GameObject gameObject = BrickManManager.Instance.Get(value.Seq);
					if (!(null == gameObject))
					{
						TPController component = gameObject.GetComponent<TPController>();
						if (!(null == component))
						{
							if (!flag && (MyInfoManager.Instance.Score > value.Score || (MyInfoManager.Instance.Score == value.Score && MyInfoManager.Instance.Kill > value.Kill)))
							{
								flag = true;
								float num2 = num;
								num = GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq), MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
								GUI.Box(new Rect(myRowX, num2 - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
							}
							Peer peer = P2PManager.Instance.Get(value.Seq);
							num = GridOut(value.ClanMark, value.Xp, value.Rank, value.Nickname, ZombieVsHumanManager.Instance.IsZombie(value.Seq), value.IsHidePlayer, value.Score, peer?.PingTime ?? float.PositiveInfinity, value.Status, component.IsDead, num, value.Seq == RoomManager.Instance.Master);
						}
					}
				}
			}
			if (!flag)
			{
				GridOut(MyInfoManager.Instance.ClanMark, MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank, MyInfoManager.Instance.Nickname, ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq), MyInfoManager.Instance.ControlMode == MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR, MyInfoManager.Instance.Score, MyInfoManager.Instance.PingTime, MyInfoManager.Instance.Status, localController.IsDead, num, MyInfoManager.Instance.Seq == RoomManager.Instance.Master);
				GUI.Box(new Rect(myRowX, num - myRowSize.y / 2f + 2f, myRowSize.x, myRowSize.y), string.Empty, "BoxMyInfo");
			}
			GUI.EndGroup();
			GUI.enabled = true;
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

	private float GridOut(int clanMark, int xp, int rank, string nickname, bool isZombie, bool isWatching, int score, float avgPing, int status, bool isDead, float y, bool isHost)
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
		string text = StringMgr.Instance.Get("WATCHING");
		Color clrText = Color.gray;
		if (isZombie)
		{
			text = StringMgr.Instance.Get("ZOMBIE");
			clrText = Color.green;
		}
		else if (!isWatching)
		{
			text = StringMgr.Instance.Get("BRICKMAN");
			clrText = Color.white;
		}
		LabelUtil.TextOut(new Vector2(zombieX, y), text, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(scoreX, y), score.ToString(), "MiniLabel", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (status == 4)
		{
			string text2 = "x";
			Color clrText2 = Color.red;
			Color clrOutline = Color.black;
			if (isWatching)
			{
				text2 = StringMgr.Instance.Get("WATCHING");
				clrText2 = Color.gray;
				clrOutline = GlobalVars.txtEmptyColor;
			}
			else if (avgPing < float.PositiveInfinity)
			{
				text2 = avgPing.ToString("0.###");
				clrText2 = ((avgPing > 0.3f) ? Color.red : ((!(avgPing > 0.1f)) ? Color.green : Color.yellow));
				clrOutline = GlobalVars.txtEmptyColor;
			}
			LabelUtil.TextOut(new Vector2(pingX, y), text2, "MiniLabel", clrText2, clrOutline, TextAnchor.MiddleCenter);
		}
		else
		{
			string text3 = StringMgr.Instance.Get("PLAYER_IS_WAITING");
			if (status == 2 || status == 3)
			{
				text3 = StringMgr.Instance.Get("PLAYER_IS_LOADING");
			}
			LabelUtil.TextOut(new Vector2(pingX, y), text3, "MiniLabel", Color.blue, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		return y + offset;
	}

	private void Start()
	{
	}

	private void Update()
	{
		on = (!DialogManager.Instance.IsModal && custom_inputs.Instance.GetButton("K_SITUATION"));
	}
}
