using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeChannel : MonoBehaviour
{
	public Texture2D userCountGauge;

	public Texture2D[] texPopupBg;

	public Texture2D texScrollbar;

	private bool waiting;

	public float doubleClickTimeout = 0.2f;

	private float lastClickTime;

	private Vector2 scrollPosition = Vector2.zero;

	private Channel targetChannel;

	private float deltaTime;

	private ComboBox cbox;

	private GUIContent select;

	private GUIContent[] listContent;

	private int selID;

	private int[] flags;

	private bool bGuiEnable;

	private Vector2 size = new Vector2(1024f, 768f);

	private Vector2 crdTitle = new Vector2(512f, 40f);

	private Rect crdCloseBtn = new Rect(977f, 5f, 34f, 34f);

	private Rect crdCombo = new Rect(40f, 30f, 180f, 30f);

	private Rect crdRealRect = new Rect(40f, 90f, 960f, 580f);

	private int channelPerRow = 7;

	private Vector2 crdChannel = new Vector2(111f, 98f);

	private Vector2 crdOffset = new Vector2(20f, 10f);

	private Vector2 crdChannelName = new Vector2(107f, 80f);

	private Vector2 crdTitleOffset = new Vector2(5f, 5f);

	private float titleHeight = 32f;

	private float tooltipWidth = 200f;

	private int overchannel = -1;

	private void Start()
	{
		selID = 0;
		targetChannel = null;
		GlobalVars.Instance.ApplyAudioSource();
		Compass.Instance.LobbyType = LOBBY_TYPE.ROOMS;
		waiting = false;
		select = new GUIContent();
		cbox = new ComboBox();
		cbox.Initialize(bImage: true, new Vector2(180f, 30f));
		cbox.setBackground(Color.white, GlobalVars.Instance.txtMainColor);
		SortedDictionary<int, Texture> sortedDictionary = new SortedDictionary<int, Texture>();
		sortedDictionary.Add(-1, BuildOption.Instance.defaultCountryFilter);
		Channel[] array = ChannelManager.Instance.ToArraySortedByMode();
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (!sortedDictionary.ContainsKey(array[i].Country) && 0 <= array[i].Country && array[i].Country < BuildOption.Instance.CountryIcons.Length)
				{
					sortedDictionary.Add(array[i].Country, BuildOption.Instance.CountryIcons[array[i].Country]);
				}
			}
			int num = 0;
			flags = new int[sortedDictionary.Count];
			listContent = new GUIContent[sortedDictionary.Count];
			foreach (KeyValuePair<int, Texture> item in sortedDictionary)
			{
				listContent[num] = new GUIContent(StringMgr.Instance.Get((0 > item.Key || item.Key >= BuildOption.Instance.CountryNames.Length) ? "ALL_SERVERS" : BuildOption.Instance.CountryNames[item.Key]), item.Value);
				flags[num] = item.Key;
				num++;
			}
		}
	}

	private void CheckGUIEnable()
	{
		if (!cbox.IsClickedComboButton())
		{
			bGuiEnable = true;
		}
		else
		{
			bGuiEnable = false;
		}
	}

	private void Update()
	{
		CheckGUIEnable();
		if (targetChannel != null)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 5f)
			{
				deltaTime = 0f;
				if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (!CSNetManager.Instance.SwitchAfter.Open(targetChannel.Ip, targetChannel.Port))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_FAIL"));
					targetChannel = null;
					BuildOption.Instance.Exit();
				}
			}
		}
	}

	private void Roamin()
	{
		waiting = true;
		targetChannel = null;
		RoomManager.Instance.Clear();
		SquadManager.Instance.Clear();
		CSNetManager.Instance.Sock.SendCS_ROAMIN_REQ(MyInfoManager.Instance.Seq, 0, BuildOption.Instance.Props.isWebPlayer, LangOptManager.Instance.LangOpt, NoCheat.Instance.GenCode);
	}

	private void MoveChannel(Channel dst)
	{
		if (dst.Mode == 1 && !MyInfoManager.Instance.IsNewbie)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NEWBIE_ONLY"));
		}
		else if (dst.UserCount >= dst.MaxUserCount)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANNEL_IS_CROWDED"));
		}
		else if (ChannelManager.Instance.CurChannelId == dst.Id)
		{
			Roamin();
		}
		else
		{
			CSNetManager.Instance.Sock.SendCS_ROAMOUT_REQ(dst.Id);
			waiting = true;
		}
	}

	private void DrawChannel(int startx, int starty, Channel chl)
	{
		Texture2D tex = BuildOption.Instance.defaultCountryFilter;
		if (0 <= chl.Country && chl.Country < BuildOption.Instance.CountryIcons.Length)
		{
			tex = BuildOption.Instance.CountryIcons[chl.Country];
		}
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		bool flag = false;
		if (!chl.IsUseAbleLevel(levelMixLank))
		{
			flag = true;
		}
		Color color = GUI.color;
		if (flag)
		{
			GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
		}
		if (GlobalVars.Instance.MyButton(new Rect((float)startx, (float)starty, crdChannel.x, crdChannel.y), tex, new GUIContent(string.Empty, chl.Id.ToString()), "BtnChannel"))
		{
			if (Application.CanStreamedLevelBeLoaded("Lobby"))
			{
				if (!flag)
				{
					MoveChannel(chl);
				}
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
		}
		if (flag)
		{
			GUI.color = color;
		}
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		LabelUtil.TextOut(new Vector2((float)startx + crdChannelName.x, (float)starty + crdChannelName.y), chl.Name, "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		float num = (chl.MaxUserCount > 0) ? ((float)chl.UserCount / (float)chl.MaxUserCount) : 0f;
		if (num > 1f)
		{
			num = 1f;
		}
		TextureUtil.DrawTexture(new Rect((float)(startx + 3), (float)(starty + 88), 105f * num, 6f), texScrollbar, ScaleMode.StretchToFill);
	}

	private string GetChannelName(int roommode)
	{
		string empty = string.Empty;
		switch (roommode)
		{
		case 3:
			return StringMgr.Instance.Get("BUILD_CHANNELS");
		case 4:
			return StringMgr.Instance.Get("CLAN_CHANNELS");
		case 1:
			return StringMgr.Instance.Get("NEWBIE_CHANNELS");
		case 2:
			return StringMgr.Instance.Get("PLAY_CHANNELS");
		default:
			return StringMgr.Instance.Get("ALL_SERVERS");
		}
	}

	private void DrawChannelGroup(Channel[] channels, int yOffset)
	{
		for (int i = 0; i < channels.Length; i++)
		{
			int num = i % channelPerRow;
			int num2 = i / channelPerRow;
			int num3 = (int)(crdChannel.x * (float)num + crdOffset.x * (float)(num + 1));
			int num4 = (int)((float)yOffset + crdChannel.y * (float)num2 + crdOffset.y * (float)(num2 + 1));
			DrawChannel(num3, num4, channels[i]);
			if (ChannelManager.Instance.CurChannelId == channels[i].Id)
			{
				GUI.Box(new Rect((float)num3, (float)num4, crdChannel.x, crdChannel.y), string.Empty, "BtnSelectF");
			}
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		bool flag = false;
		if (GUI.enabled)
		{
			GUI.enabled = bGuiEnable;
			flag = !bGuiEnable;
		}
		if (GUI.enabled && waiting)
		{
			GUI.enabled = false;
		}
		GlobalVars.Instance.BeginGUIWithBox("BoxBg");
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		int num = 0;
		TextureUtil.DrawTexture(new Rect(0f, 0f, size.x, size.y), texPopupBg[num], ScaleMode.StretchToFill);
		string channelName = GetChannelName(-1);
		LabelUtil.TextOut(crdTitle, channelName, "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (BuildOption.Instance.IsNetmarble && MyInfoManager.Instance.Nickname != null && (MyInfoManager.Instance.Nickname.Equals("은빛늑대7".ToString()) || MyInfoManager.Instance.Nickname.Equals("Yasitaca".ToString()) || MyInfoManager.Instance.Nickname.Equals("상남자_레이".ToString())))
		{
			LabelUtil.TextOut(new Vector2(crdTitle.x + 200f, crdTitle.y), ChannelManager.Instance.GetAllUserCount().ToString(), "BigBtnLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || (!GlobalVars.Instance.IsModalAll() && GlobalVars.Instance.IsEscapePressed()))
		{
			Application.LoadLevel(ChannelManager.Instance.PrevScene);
		}
		Channel[] array = null;
		Channel[] array2 = null;
		Channel[] array3 = null;
		if (0 <= selID && selID < flags.Length)
		{
			Channel[] array4 = null;
			Channel[] array5 = null;
			array4 = ChannelManager.Instance.ToArray(1, flags[selID]);
			array5 = ChannelManager.Instance.ToArray(2, flags[selID]);
			array2 = ChannelManager.Instance.ToArray(3, flags[selID]);
			array3 = ChannelManager.Instance.ToArray(4, flags[selID]);
			List<Channel> list = new List<Channel>();
			int num2 = 0;
			while (array4 != null && num2 < array4.Length)
			{
				list.Add(array4[num2]);
				num2++;
			}
			int num3 = 0;
			while (array5 != null && num3 < array5.Length)
			{
				list.Add(array5[num3]);
				num3++;
			}
			array = list.ToArray();
		}
		int num4 = 0;
		int num5 = 0;
		int num6 = array.Length / channelPerRow;
		if (array.Length % channelPerRow > 0)
		{
			num6++;
		}
		if (num6 > 0)
		{
			num4 = (int)titleHeight;
			num5 = (int)(crdOffset.y * (float)(num6 + 1) + crdChannel.y * (float)num6);
		}
		int num7 = 0;
		int num8 = 0;
		int num9 = array2.Length / channelPerRow;
		if (array2.Length % channelPerRow > 0)
		{
			num9++;
		}
		if (num9 > 0)
		{
			num7 = (int)titleHeight;
			num8 = (int)(crdOffset.y * (float)(num9 + 1) + crdChannel.y * (float)num9);
		}
		int num10 = 0;
		int num11 = 0;
		int num12 = array3.Length / channelPerRow;
		if (array3.Length % channelPerRow > 0)
		{
			num12++;
		}
		if (num12 > 0)
		{
			num10 = (int)titleHeight;
			num11 = (int)(crdOffset.y * (float)(num12 + 1) + crdChannel.y * (float)num12);
		}
		scrollPosition = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdRealRect.width - 20f, (float)(num4 + num7 + num10 + num11 + num8 + num5)), position: crdRealRect, scrollPosition: scrollPosition);
		int num13 = 0;
		if (num6 > 0 && array != null && array.Length > 0)
		{
			LabelUtil.TextOut(new Vector2(crdTitleOffset.x, (float)num13 + crdTitleOffset.y), StringMgr.Instance.Get("PLAY"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			num13 += num4;
			GUI.Box(new Rect(0f, (float)num13, crdRealRect.width - 20f, (float)num5), string.Empty, "BoxPopLine");
			DrawChannelGroup(array, num13);
			num13 += num5;
		}
		if (num7 > 0 && array2 != null && array2.Length > 0)
		{
			LabelUtil.TextOut(new Vector2(crdTitleOffset.x, (float)num13 + crdTitleOffset.y), StringMgr.Instance.Get("BUILD"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			num13 += num7;
			GUI.Box(new Rect(0f, (float)num13, crdRealRect.width - 20f, (float)num8), string.Empty, "BoxPopLine");
			DrawChannelGroup(array2, num13);
			num13 += num8;
		}
		if (num10 > 0 && array3 != null && array3.Length > 0)
		{
			LabelUtil.TextOut(new Vector2(crdTitleOffset.x, (float)num13 + crdTitleOffset.y), StringMgr.Instance.Get("CLAN"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			num13 += num10;
			GUI.Box(new Rect(0f, (float)num13, crdRealRect.width - 20f, (float)num11), string.Empty, "BoxPopLine");
			DrawChannelGroup(array3, num13);
		}
		GUI.EndScrollView();
		if (flag)
		{
			GUI.enabled = true;
		}
		if (listContent != null && listContent.Length > 0)
		{
			selID = cbox.List(crdCombo, select, listContent);
			select = listContent[selID];
		}
		if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
		{
			Channel channel = ChannelManager.Instance.Get(Convert.ToInt32(GUI.tooltip));
			if (channel != null)
			{
				Vector2 tooltipsize = Vector2.zero;
				CalcTooltipSize(channel, ref tooltipsize);
				Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				float num14 = ((float)Screen.width - size.x) / 2f;
				if (vector.x + tooltipsize.x > size.x + num14)
				{
					vector.x = size.x - tooltipsize.x + num14;
				}
				float num15 = ((float)Screen.height - size.y) / 2f;
				if (vector.y + tooltipsize.y > size.y + num15)
				{
					vector.y = size.y - tooltipsize.y + num15;
				}
				overchannel = Convert.ToInt32(GUI.tooltip);
				GUI.Window(1108, new Rect(vector.x, vector.y, tooltipsize.x, tooltipsize.y), ShowTooltip, string.Empty, "LineWindow");
			}
		}
		GUI.enabled = true;
		GlobalVars.Instance.EndGUI();
	}

	private void CalcTooltipSize(Channel chl, ref Vector2 tooltipsize)
	{
		float num = 10f;
		float num2 = 40f;
		string rank = XpManager.Instance.GetRank(chl.MinLvRank);
		string rank2 = XpManager.Instance.GetRank(chl.MaxLvRank);
		string text = string.Format(StringMgr.Instance.Get("CHANNEL_LIMIT_MSG01"), rank, rank2);
		float num3 = num2;
		Vector2 vector = LabelUtil.CalcSize("MiniLabel", text, tooltipWidth);
		num2 = num3 + (vector.y + num);
		if (chl.IsLimitStarRate)
		{
			string text2 = string.Format(StringMgr.Instance.Get("WEAPON_STAR_LIMIT_2"), chl.LimitStarRate);
			float num4 = num2;
			Vector2 vector2 = LabelUtil.CalcSize("MiniLabel", text2, tooltipWidth);
			num2 = num4 + (vector2.y + num);
		}
		string mapHint = chl.GetMapHint();
		if (mapHint.Length > 0)
		{
			float num5 = num2;
			Vector2 vector3 = LabelUtil.CalcSize("MiniLabel", mapHint, tooltipWidth);
			num2 = num5 + (vector3.y + num);
		}
		if (chl.XpBonus > 0)
		{
			string text3 = string.Format(StringMgr.Instance.Get("XP_UP"), chl.XpBonus);
			float num6 = num2;
			Vector2 vector4 = LabelUtil.CalcSize("MiniLabel", text3, tooltipWidth);
			num2 = num6 + (vector4.y + num);
		}
		if (chl.FpBonus > 0)
		{
			string text4 = string.Format(StringMgr.Instance.Get("POINT_UP"), chl.FpBonus);
			float num7 = num2;
			Vector2 vector5 = LabelUtil.CalcSize("MiniLabel", text4, tooltipWidth);
			num2 = num7 + (vector5.y + num);
		}
		tooltipsize.x = tooltipWidth + 20f;
		tooltipsize.y = num2;
	}

	private void ShowTooltip(int id)
	{
		if (id == 1108)
		{
			Channel channel = ChannelManager.Instance.Get(overchannel);
			Vector2 vector = new Vector2(10f, 10f);
			Texture2D badge = XpManager.Instance.GetBadge(channel.MinLvRank);
			if (null != badge)
			{
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)badge.width, (float)badge.height), badge, ScaleMode.StretchToFill);
			}
			vector = new Vector2(56f, 7f);
			LabelUtil.TextOut(vector, "~", "BigLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			vector = new Vector2(80f, 10f);
			badge = XpManager.Instance.GetBadge(channel.MaxLvRank);
			if (null != badge)
			{
				TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)badge.width, (float)badge.height), badge, ScaleMode.StretchToFill);
			}
			Rect position = new Rect(10f, 40f, tooltipWidth, 0f);
			float num = 10f;
			string rank = XpManager.Instance.GetRank(channel.MinLvRank);
			string rank2 = XpManager.Instance.GetRank(channel.MaxLvRank);
			string text = string.Format(StringMgr.Instance.Get("CHANNEL_LIMIT_MSG01"), rank, rank2);
			Vector2 vector2 = LabelUtil.CalcSize("MiniLabel", text, tooltipWidth);
			position.height = vector2.y;
			GUI.Label(position, text, "MiniLabel");
			position.y += position.height + num;
			if (channel.IsLimitStarRate)
			{
				float num2 = (float)channel.LimitStarRate / 20f;
				string text2 = string.Format(StringMgr.Instance.Get("WEAPON_STAR_LIMIT_2"), num2);
				Vector2 vector3 = LabelUtil.CalcSize("MiniLabel", text2, tooltipWidth);
				position.height = vector3.y;
				GUI.Label(position, text2, "MiniLabel");
				position.y += position.height + num;
			}
			string mapHint = channel.GetMapHint();
			if (mapHint.Length > 0)
			{
				Vector2 vector4 = LabelUtil.CalcSize("MiniLabel", mapHint, tooltipWidth);
				position.height = vector4.y;
				GUI.Label(position, mapHint, "MiniLabel");
				position.y += position.height + num;
			}
			if (channel.XpBonus > 0)
			{
				string text3 = string.Format(StringMgr.Instance.Get("XP_UP"), channel.XpBonus);
				Vector2 vector5 = LabelUtil.CalcSize("MiniLabel", text3, tooltipWidth);
				position.height = vector5.y;
				GUI.Label(position, text3, "MiniLabel");
				position.y += position.height + num;
			}
			if (channel.FpBonus > 0)
			{
				string text4 = string.Format(StringMgr.Instance.Get("POINT_UP"), channel.FpBonus);
				Vector2 vector6 = LabelUtil.CalcSize("MiniLabel", text4, tooltipWidth);
				position.height = vector6.y;
				GUI.Label(position, text4, "MiniLabel");
				position.y += position.height + num;
			}
		}
	}

	private void OnRoamIn(int ret)
	{
		if (ret < 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANNEL_MOVE_FAILED"));
			BuildOption.Instance.Exit();
		}
		else
		{
			MyInfoManager.Instance.VerifyMustEquipSlots();
			Application.LoadLevel("Lobby");
		}
	}

	private void OnRoamOut(int ret)
	{
		if (ret < 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANNEL_MOVE_FAILED"));
			BuildOption.Instance.Exit();
		}
		else
		{
			Channel channel = ChannelManager.Instance.Get(ret);
			if (channel == null)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANNEL_MOVE_FAILED"));
				BuildOption.Instance.Exit();
			}
			else
			{
				if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (CSNetManager.Instance.SwitchAfter.Open(channel.Ip, channel.Port))
				{
					targetChannel = channel;
				}
				else
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NETWORK_FAIL"));
					BuildOption.Instance.Exit();
				}
			}
		}
	}

	private void OnSeed()
	{
		ChannelUserManager.Instance.Clear();
		Roamin();
	}
}
