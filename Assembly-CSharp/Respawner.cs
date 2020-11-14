using UnityEngine;

public class Respawner : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture2D gaugeFrame;

	public Texture2D gaugeBar;

	public Texture2D gaugeText;

	private SecureFloat respawnTimeSecure;

	private bool outOnce = true;

	private float resTime = 5f;

	private LocalController localController;

	private bool changedRespawnTime;

	private float respawnTime
	{
		get
		{
			return respawnTimeSecure.Get();
		}
		set
		{
			respawnTimeSecure.Set(value);
		}
	}

	public float ResTime
	{
		set
		{
			resTime = value;
		}
	}

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			localController = gameObject.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
		if (localController != null)
		{
			localController.CheckRespawnItemHave();
			resTime -= resTime * localController.RespwanTimeDec;
			if (resTime < 0f)
			{
				resTime = 0f;
			}
		}
		respawnTimeSecure.Init(resTime);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.RESPAWMTIME, resTime);
	}

	private void OnDestroy()
	{
		respawnTimeSecure.Release();
	}

	public bool Resurrect2()
	{
		if (null == localController || localController.HitPoint > 0)
		{
			return false;
		}
		if (Application.loadedLevelName.Contains("Tutor"))
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && !ZombieVsHumanManager.Instance.AmIRespawnable)
		{
			return false;
		}
		respawnTimeSecure.Init(0.5f);
		localController.DeltaFromDeath = 0.05f;
		changedRespawnTime = true;
		return true;
	}

	public bool Resurrect()
	{
		if (null == localController || localController.HitPoint > 0)
		{
			return false;
		}
		if (Application.loadedLevelName.Contains("Tutor") || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.EXPLOSION)
		{
			return false;
		}
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE && !ZombieVsHumanManager.Instance.AmIRespawnable)
		{
			return false;
		}
		localController.DeltaFromDeath = respawnTime;
		return true;
	}

	private void DoSpawn(SpawnerDesc spawner)
	{
		if (null != localController)
		{
			if (changedRespawnTime)
			{
				respawnTimeSecure.Init(resTime);
				changedRespawnTime = false;
			}
			respawnTimeSecure.Reset();
			if (spawner != null)
			{
				localController.Spawn(spawner.position, Rot.ToQuaternion(spawner.rotation));
			}
			else
			{
				localController.Spawn(BrickManager.Instance.GetRandomSpawnPos(), Rot.ToQuaternion((byte)Random.Range(0, 4)));
			}
		}
	}

	private bool CheckJustRespawn()
	{
		GameObject gameObject = GameObject.Find("Main");
		ShooterTools component = gameObject.GetComponent<ShooterTools>();
		if (null != component && component.find("just_respawn"))
		{
			return true;
		}
		return false;
	}

	private void OnGUI()
	{
		if (null != localController && localController.HitPoint <= 0)
		{
			float num = localController.DeltaFromDeath / respawnTime;
			if (num >= 1f)
			{
				if (Application.loadedLevelName.Contains("Tutor"))
				{
					DoSpawn(BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, 0));
				}
				else
				{
					switch (RoomManager.Instance.CurrentRoomType)
					{
					case Room.ROOM_TYPE.MAP_EDITOR:
						DoSpawn(null);
						break;
					case Room.ROOM_TYPE.TEAM_MATCH:
					case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
					case Room.ROOM_TYPE.BND:
						DoSpawn(BrickManager.Instance.GetSpawner(MyInfoManager.Instance.GetTeamSpawnerType(), MyInfoManager.Instance.Ticket));
						break;
					case Room.ROOM_TYPE.EXPLOSION:
					{
						ClanMatchRounding component = GetComponent<ClanMatchRounding>();
						if (null != component && !component.Rounding)
						{
							localController.GetComponent<WoundFx>().ClearScreen();
							MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR;
							if (outOnce)
							{
								outOnce = false;
								GameObject gameObject2 = GameObject.Find("Main");
								if (null != gameObject2)
								{
									string text2 = StringMgr.Instance.Get("WATCHING_USER_CHANGE");
									gameObject2.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text2));
								}
							}
						}
						break;
					}
					case Room.ROOM_TYPE.MISSION:
						DoSpawn(BrickManager.Instance.GetSpawner((MyInfoManager.Instance.Slot < 4) ? Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER : Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER, MyInfoManager.Instance.Ticket));
						break;
					case Room.ROOM_TYPE.INDIVIDUAL:
						DoSpawn(BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, MyInfoManager.Instance.Ticket));
						break;
					case Room.ROOM_TYPE.BUNGEE:
						DoSpawn(null);
						break;
					case Room.ROOM_TYPE.ESCAPE:
						DoSpawn(BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, MyInfoManager.Instance.Ticket));
						break;
					case Room.ROOM_TYPE.ZOMBIE:
						if (ZombieVsHumanManager.Instance.AmIRespawnable)
						{
							ZombieVsHumanManager.Instance.AmIRespawnable = false;
							DoSpawn(BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.SINGLE_SPAWNER, MyInfoManager.Instance.Ticket));
						}
						else
						{
							localController.GetComponent<WoundFx>().ClearScreen();
							MyInfoManager.Instance.ControlMode = MyInfoManager.CONTROL_MODE.PLAYING_SPECTATOR;
							if (outOnce)
							{
								outOnce = false;
								GameObject gameObject = GameObject.Find("Main");
								if (null != gameObject)
								{
									string text = StringMgr.Instance.Get("WATCHING_USER_CHANGE");
									gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
								}
							}
						}
						break;
					}
				}
			}
			else if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.EXPLOSION)
			{
				if (MyInfoManager.Instance.isGuiOn && BuildOption.Instance.IsNetmarbleOrDev && (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ZOMBIE || ZombieVsHumanManager.Instance.AmIRespawnable))
				{
					GUI.skin = GUISkinFinder.Instance.GetGUISkin();
					GUI.depth = (int)guiDepth;
					GUI.enabled = !DialogManager.Instance.IsModal;
					Rect position = new Rect((float)((Screen.width - 586) / 2), (float)((Screen.height - 37) / 2), 586f, 37f);
					Rect position2 = new Rect((float)((Screen.width - 558) / 2), (float)((Screen.height - 23) / 2), 558f, 23f);
					Rect position3 = new Rect((float)((Screen.width - 558) / 2), (float)((Screen.height - 23) / 2), 558f * num, 23f);
					TextureUtil.DrawTexture(position, gaugeFrame, ScaleMode.StretchToFill, alphaBlend: true);
					TextureUtil.DrawTexture(position3, gaugeBar, ScaleMode.StretchToFill, alphaBlend: true);
					TextureUtil.DrawTexture(position2, gaugeText, ScaleMode.StretchToFill, alphaBlend: true);
					if (RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BUNGEE)
					{
						if (!localController.CheckJustRespawnItemHave())
						{
							LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)((Screen.height - 86) / 2)), StringMgr.Instance.Get("RESPAWN_BUY_DES"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
						}
						else if (!CheckJustRespawn())
						{
							LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)((Screen.height - 86) / 2)), StringMgr.Instance.Get("RESPAWN_EQUIP_DES"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
						}
					}
					GUI.enabled = true;
				}
			}
			else
			{
				outOnce = true;
			}
		}
	}

	private void Update()
	{
		NoCheat.Instance.KillCheater(NoCheat.WATCH_DOG.RESPAWMTIME, resTime);
	}
}
