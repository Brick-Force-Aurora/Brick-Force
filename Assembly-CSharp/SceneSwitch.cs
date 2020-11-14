using UnityEngine;

public class SceneSwitch : MonoBehaviour
{
	public enum STEP
	{
		DEPARTURE,
		CHANGE_CHANNEL,
		LOBBY,
		DESTINATION
	}

	public const int CHANNEL_MOVE_FAIL = 0;

	public const int FAIL_TO_JOIN_ROOM = -1;

	public const int EXCEED = -2;

	public const int WRONG_PSWD = -3;

	public const int LOADING_WAIT = -5;

	public const int CANT_BREAK_INTO = -6;

	public const int RENDEZVOUS_NOT_COMPLETED = -7;

	public const int NETWORK_FAIL = -4;

	public Texture loadingImage;

	public Vector2 logoSize = new Vector2(343f, 120f);

	private STEP step;

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 vector = new Vector2((float)((Screen.width - loadingImage.width) / 2), (float)((Screen.height - loadingImage.height) / 2));
		TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)loadingImage.width, (float)loadingImage.height), loadingImage);
		Texture2D logo = BuildOption.Instance.Props.logo;
		if (null != logo)
		{
			Vector2 vector2 = new Vector2((float)((Screen.width - logo.width) / 2), vector.y + (float)loadingImage.height - logoSize.y);
			TextureUtil.DrawTexture(new Rect(vector2.x, vector2.y, (float)logo.width, (float)logo.height), logo, ScaleMode.StretchToFill);
		}
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 32)), StringMgr.Instance.Get("LOADING"), "BigLabel", new Color(0.9f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.LowerCenter);
	}

	private void Start()
	{
		step = STEP.DEPARTURE;
	}

	private void OnJoin(int ret)
	{
		Room room = RoomManager.Instance.GetRoom(ret);
		if (room != null)
		{
			RoomManager.Instance.CurrentRoom = ret;
			Room.ROOM_TYPE type = room.Type;
			if (type == Room.ROOM_TYPE.MAP_EDITOR)
			{
				OnSuccess("MapEditor");
			}
			else if (Room.ROOM_TYPE.MAP_EDITOR < type && type < Room.ROOM_TYPE.NUM_TYPE)
			{
				OnSuccess("Briefing4TeamMatch");
			}
		}
		if (step != STEP.DESTINATION)
		{
			int causedBy = -1;
			switch (ret)
			{
			case -2:
				causedBy = -2;
				break;
			case -3:
				causedBy = -3;
				break;
			case -5:
				causedBy = -5;
				break;
			case -6:
				causedBy = -6;
				break;
			}
			OnFail(causedBy);
		}
	}

	private void OnRoamIn(int ret)
	{
		if (ret < 0)
		{
			OnFail(0);
		}
		else
		{
			MyInfoManager.Instance.VerifyMustEquipSlots();
			OnLobby();
		}
	}

	private void SendCS_JOIN_REQ()
	{
		if (GlobalVars.Instance.clanSendJoinREQ == 1)
		{
			GlobalVars.Instance.ENTER_SQUADING_ACK();
		}
		else
		{
			CSNetManager.Instance.Sock.SendCS_JOIN_REQ(Compass.Instance.RoomIdx, Compass.Instance.RoomPswd, invite: true);
		}
	}

	private bool UpdateJoinRoom()
	{
		if (GlobalVars.Instance.clanSendJoinREQ == 2)
		{
			GlobalVars.Instance.clanSendJoinREQ = -1;
			return true;
		}
		return false;
	}

	private void OnLobby()
	{
		step = STEP.LOBBY;
		switch (Compass.Instance.Dst)
		{
		case Compass.DESTINATION_LEVEL.LOBBY:
			OnSuccess(Compass.Instance.GetDestinationLevel());
			break;
		case Compass.DESTINATION_LEVEL.BATTLE_TUTOR:
			OnSuccess(Compass.Instance.GetDestinationLevel());
			break;
		case Compass.DESTINATION_LEVEL.ROOM:
			SendCS_JOIN_REQ();
			break;
		case Compass.DESTINATION_LEVEL.SQUAD:
			GlobalVars.Instance.ENTER_SQUADING_ACK();
			break;
		}
	}

	private void OnSeed()
	{
		ChannelUserManager.Instance.Clear();
		RoamIn();
	}

	private void RoamIn()
	{
		RoomManager.Instance.Clear();
		SquadManager.Instance.Clear();
		CSNetManager.Instance.Sock.SendCS_ROAMIN_REQ(MyInfoManager.Instance.Seq, 0, BuildOption.Instance.Props.isWebPlayer, LangOptManager.Instance.LangOpt, NoCheat.Instance.GenCode);
	}

	private void OnSuccess(string levelName)
	{
		step = STEP.DESTINATION;
		Application.LoadLevel(levelName);
	}

	private void OnFail(int causedBy)
	{
		string msg = StringMgr.Instance.Get("CHANNEL_MOVE_FAILED");
		switch (causedBy)
		{
		case -1:
			msg = StringMgr.Instance.Get("FAIL_TO_JOIN_ROOM");
			break;
		case -2:
			msg = StringMgr.Instance.Get("EXCEED");
			break;
		case -3:
			msg = StringMgr.Instance.Get("WRONG_PSWD");
			break;
		case -5:
			msg = StringMgr.Instance.Get("LOADING_WAIT");
			break;
		case -6:
			msg = StringMgr.Instance.Get("CANT_BREAK_INTO");
			break;
		case -4:
			msg = StringMgr.Instance.Get("NETWORK_FAIL");
			break;
		}
		MessageBoxMgr.Instance.AddMessage(msg);
		if (step == STEP.LOBBY)
		{
			step = STEP.DESTINATION;
			Application.LoadLevel("Lobby");
		}
		else
		{
			BuildOption.Instance.Exit();
		}
	}

	private void OnRoamOut(int ret)
	{
		if (ret < 0)
		{
			OnFail(0);
		}
		else
		{
			Channel channel = ChannelManager.Instance.Get(ret);
			if (channel == null)
			{
				OnFail(0);
			}
			else
			{
				if (CSNetManager.Instance.Sock != null)
				{
					CSNetManager.Instance.Sock.Close();
				}
				CSNetManager.Instance.SwitchAfter = new SockTcp();
				if (!CSNetManager.Instance.SwitchAfter.Open(channel.Ip, channel.Port))
				{
					OnFail(-4);
				}
			}
		}
	}

	private void Update()
	{
		if (!UpdateJoinRoom() || CSNetManager.Instance.Sock.SendCS_JOIN_REQ(Compass.Instance.RoomIdx, Compass.Instance.RoomPswd, invite: true))
		{
		}
		if (step == STEP.DEPARTURE)
		{
			Squad curSquad = SquadManager.Instance.CurSquad;
			Channel channel = ChannelManager.Instance.Get(Compass.Instance.Channel);
			if (channel == null)
			{
				OnFail(0);
			}
			else
			{
				bool flag = false;
				switch (Compass.Instance.SrcLevel)
				{
				case "ChangeChannel":
					flag = true;
					break;
				case "Briefing4TeamMatch":
					P2PManager.Instance.Shutdown();
					CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
					if (curSquad != null)
					{
						CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
						SquadManager.Instance.Leave();
						CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
						SquadManager.Instance.Clear();
					}
					break;
				case "Squad":
					CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
					SquadManager.Instance.Leave();
					CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
					SquadManager.Instance.Clear();
					break;
				case "Squading":
					CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
					SquadManager.Instance.Clear();
					break;
				}
				if (channel.Id == ChannelManager.Instance.CurChannelId || (ChannelManager.Instance.CurChannelId < 0 && channel.Id == ChannelManager.Instance.LoginChannelId))
				{
					if (!flag)
					{
						step = STEP.LOBBY;
						RoamIn();
					}
					else
					{
						step = STEP.CHANGE_CHANNEL;
						RoamIn();
					}
				}
				else
				{
					step = STEP.CHANGE_CHANNEL;
					CSNetManager.Instance.Sock.SendCS_ROAMOUT_REQ(channel.Id);
				}
			}
		}
	}
}
