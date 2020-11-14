using System;
using UnityEngine;

[Serializable]
public class SquadMode
{
	public Texture2D nonAvailable;

	public Texture2D cover;

	public Texture2D selectedMapFrame;

	private Rect crdMapOutline = new Rect(15f, 242f, 430f, 231f);

	private Rect crdMapView = new Rect(24f, 249f, 410f, 213f);

	private Vector2 crdMapSize = new Vector2(128f, 128f);

	private float crdMapOffset = 6f;

	private Vector2 crdDeveloper = new Vector2(5f, 91f);

	private Vector2 crdAlias = new Vector2(5f, 109f);

	private Vector2 crdModeLabel = new Vector2(570f, 265f);

	private Vector2 crdNumPlayerLabel = new Vector2(570f, 330f);

	private Rect crdModeBg = new Rect(515f, 278f, 110f, 23f);

	private Vector2 crdModeValue = new Vector2(570f, 290f);

	private Vector2 crdNumPlayerValue = new Vector2(570f, 355f);

	private Rect crdNumPlayerBg = new Rect(515f, 345f, 110f, 23f);

	private Rect crdMatchSearch = new Rect(494f, 398f, 150f, 75f);

	private Rect crdModeLeft = new Rect(487f, 277f, 22f, 22f);

	private Rect crdModeRight = new Rect(630f, 277f, 22f, 22f);

	private Rect crdNumPlayerLeft = new Rect(487f, 345f, 22f, 22f);

	private Rect crdNumPlayerRight = new Rect(630f, 345f, 22f, 22f);

	private Rect crdWanted = new Rect(487f, 300f, 22f, 22f);

	private Rect crdFlwrOutline = new Rect(188f, 272f, 335f, 166f);

	private Rect crdFlwrMap = new Rect(200f, 282f, 144f, 144f);

	private Vector2 crdFlwrMapAlias = new Vector2(434f, 314f);

	private Vector2 crdFlwrMode = new Vector2(435f, 368f);

	private Vector2 crdFlwrNumPlayer = new Vector2(434f, 394f);

	private Rect crdFlwrThumbnail = new Rect(207f, 289f, 128f, 128f);

	public Room.ROOM_TYPE[] possibleMode;

	public Room.ROOM_TYPE[] possibleMode_no_ctf;

	private Room.ROOM_TYPE[] playableMode;

	public int minNumPlayer = 4;

	public int maxNumPlayer = 8;

	public bool wanted;

	private Vector2 spMap = Vector2.zero;

	private int ModeToOption(int mode)
	{
		for (int i = 0; i < playableMode.Length; i++)
		{
			if (playableMode[i] == (Room.ROOM_TYPE)mode)
			{
				return i;
			}
		}
		return 0;
	}

	public void Start()
	{
		if (playableMode == null)
		{
			if (!BuildOption.Instance.Props.ctfMatchMode)
			{
				GlobalVars.Instance.allocBattleMode(10);
				int num = 0;
				int[] array = new int[2]
				{
					1,
					4
				};
				playableMode = new Room.ROOM_TYPE[possibleMode_no_ctf.Length];
				for (int i = 0; i < possibleMode_no_ctf.Length; i++)
				{
					playableMode[i] = possibleMode_no_ctf[i];
					GlobalVars.Instance.setBattleMode(num++, Room.modeSelector[array[i]]);
				}
			}
			else
			{
				GlobalVars.Instance.allocBattleMode(10);
				int num3 = 0;
				int[] array2 = new int[3]
				{
					1,
					3,
					4
				};
				playableMode = new Room.ROOM_TYPE[possibleMode.Length];
				for (int j = 0; j < possibleMode.Length; j++)
				{
					playableMode[j] = possibleMode[j];
					GlobalVars.Instance.setBattleMode(num3++, Room.modeSelector[array2[j]]);
				}
			}
		}
	}

	public void Update()
	{
	}

	private int DoRegMap(int wannaPlayMap, int wannaPlayMode)
	{
		RegMap[] array = RegMapManager.Instance.ToArray(wannaPlayMode, (Channel.MODE)ChannelManager.Instance.CurChannel.Mode);
		int num = -1;
		int num2 = 0;
		while (num < 0 && num2 < array.Length)
		{
			if (array[num2].Map == wannaPlayMap)
			{
				num = num2;
			}
			num2++;
		}
		int num3 = array.Length / 3;
		if (array.Length % 3 > 0)
		{
			num3++;
		}
		GUI.Box(crdMapOutline, string.Empty, "LineBoxBlue");
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 3f + crdMapOffset * 2f, crdMapSize.y * (float)num3);
		if (num3 > 1)
		{
			viewRect.height += crdMapOffset * (float)(num3 - 1);
		}
		spMap = GUI.BeginScrollView(crdMapView, spMap, viewRect);
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int num4 = 3 * i + j;
				if (num4 < array.Length)
				{
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset), (float)i * (crdMapSize.y + crdMapOffset), crdMapSize.x, crdMapSize.y);
					if (GlobalVars.Instance.MyButton(rect, (!(array[num4].Thumbnail == null)) ? array[num4].Thumbnail : nonAvailable, "InvisibleButton"))
					{
						num = num4;
					}
					TextureUtil.DrawTexture(rect, cover);
					LabelUtil.TextOut(new Vector2(rect.x + crdDeveloper.x, rect.y + crdDeveloper.y), array[num4].Developer, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), array[num4].Alias, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					if (num == num4)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.ScaleToFit);
					}
				}
			}
		}
		GUI.EndScrollView();
		return (0 > num || num >= array.Length) ? (-1) : array[num].Map;
	}

	private void DoLeader()
	{
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null)
		{
			int wannaPlayMode = curSquad.WannaPlayMode;
			int num = ModeToOption(curSquad.WannaPlayMode);
			LabelUtil.TextOut(crdModeLabel, StringMgr.Instance.Get("ROOM_TYPE"), "Label", new Color(0.54f, 0.54f, 0.54f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdModeLeft, string.Empty, "Left"))
			{
				num--;
				if (num < 0)
				{
					num = 0;
				}
			}
			GUI.Box(crdModeBg, string.Empty, "BoxTextBg");
			LabelUtil.TextOut(crdModeValue, Room.Type2String(playableMode[num]), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdModeRight, string.Empty, "Right"))
			{
				num++;
				if (num >= playableMode.Length)
				{
					num = playableMode.Length - 1;
				}
			}
			wannaPlayMode = (int)playableMode[num];
			int num2 = DoRegMap(curSquad.WannaPlayMap, num);
			int num3 = curSquad.MaxMember;
			LabelUtil.TextOut(crdNumPlayerLabel, StringMgr.Instance.Get("NUM_PLAYERS"), "Label", new Color(0.54f, 0.54f, 0.54f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdNumPlayerLeft, string.Empty, "Left"))
			{
				num3--;
				if (num3 < minNumPlayer)
				{
					num3 = minNumPlayer;
				}
			}
			GUI.Box(crdNumPlayerBg, string.Empty, "BoxTextBg");
			LabelUtil.TextOut(crdNumPlayerValue, num3.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			if (GlobalVars.Instance.MyButton(crdNumPlayerRight, string.Empty, "Right"))
			{
				num3++;
				if (num3 > maxNumPlayer)
				{
					num3 = maxNumPlayer;
				}
			}
			if (num3 < curSquad.MemberCount)
			{
				num3 = curSquad.MemberCount;
			}
			if (BuildOption.Instance.Props.UseWanted && curSquad.WannaPlayMode == 1)
			{
				wanted = GUI.Toggle(crdWanted, wanted, StringMgr.Instance.Get("ROOM_SET_WANTED"));
			}
			if (GlobalVars.Instance.MyButton(crdMatchSearch, StringMgr.Instance.Get("MATCH_SEARCH"), "BtnAction"))
			{
				if (num3 > curSquad.MemberCount)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MATCH_MORE_PEOPLE_NEED"));
				}
				else
				{
					RegMap regMap = RegMapManager.Instance.Get(curSquad.WannaPlayMap);
					if (regMap == null)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_PLAY_WITHOUT_MAP"));
					}
					else if (curSquad.WannaPlayMode == 4)
					{
						CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_START_REQ(curSquad.WannaPlayMode, curSquad.MaxMember, curSquad.WannaPlayMap, 5, 180, 0, regMap.Alias, wanted: false);
					}
					else
					{
						CSNetManager.Instance.Sock.SendCS_MATCH_TEAM_START_REQ(curSquad.WannaPlayMode, curSquad.MaxMember, curSquad.WannaPlayMap, 50, 480, 0, regMap.Alias, curSquad.WannaPlayMode == 1 && wanted);
					}
				}
			}
			if (wannaPlayMode != curSquad.WannaPlayMode || num2 != curSquad.WannaPlayMap || num3 != curSquad.MaxMember)
			{
				curSquad.WannaPlayMode = wannaPlayMode;
				curSquad.WannaPlayMap = num2;
				curSquad.MaxMember = num3;
				CSNetManager.Instance.Sock.SendCS_CHG_SQUAD_OPTION_REQ(num2, wannaPlayMode, num3);
			}
		}
	}

	private void DoFollower()
	{
		Squad curSquad = SquadManager.Instance.CurSquad;
		GUI.Box(crdFlwrOutline, string.Empty, "LineBoxBlue");
		GUI.Box(crdFlwrMap, string.Empty, "BoxFadeBlue");
		if (curSquad != null)
		{
			RegMap regMap = RegMapManager.Instance.Get(curSquad.WannaPlayMap);
			if (regMap != null)
			{
				Texture2D thumbnail = regMap.Thumbnail;
				if (null == thumbnail)
				{
					TextureUtil.DrawTexture(crdFlwrThumbnail, nonAvailable);
				}
				else
				{
					TextureUtil.DrawTexture(crdFlwrThumbnail, thumbnail);
				}
				LabelUtil.TextOut(crdFlwrMapAlias, regMap.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			LabelUtil.TextOut(crdFlwrMode, Room.Type2String((Room.ROOM_TYPE)curSquad.WannaPlayMode), "Label", new Color(0.54f, 0.54f, 0.54f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdFlwrNumPlayer, curSquad.MaxMember.ToString() + "vs" + curSquad.MaxMember.ToString(), "Label", new Color(0.54f, 0.54f, 0.54f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	public void OnGUI()
	{
		if (SquadManager.Instance.CurSquad != null)
		{
			if (SquadManager.Instance.CurSquad.Leader == MyInfoManager.Instance.Seq)
			{
				DoLeader();
			}
			else
			{
				DoFollower();
			}
		}
	}
}
