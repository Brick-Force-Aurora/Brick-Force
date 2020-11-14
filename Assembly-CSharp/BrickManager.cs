using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
	public enum DRUM_BOOM
	{
		EXP = 115,
		TOXIC = 116,
		TNT = 193
	}

	public enum GRAVITY_BRICK_SEQ
	{
		MINUS = 155,
		PLUS,
		MINUS_DESTROY,
		PLUS_DESTROY
	}

	public enum BOOSTER_SEQ
	{
		Y_UP = 159,
		Z_MINUS
	}

	public enum DAMAGE_SEQ
	{
		FIRE = 133,
		DOOR = 165,
		TRAP = 166,
		CACTUS = 190,
		DOOR_T = 191
	}

	public enum PORTAL_SEQ
	{
		RED = 163,
		BLUE = 164,
		NEUTRAL = 178
	}

	public enum ETC_SPAWNER_SEQ
	{
		RAIL = 196
	}

	public enum SPECIAL_SEQ
	{
		RAIL = 197,
		RAILLINK_R = 198,
		RAILSLOPE_UP = 199,
		HIGHGRASS = 200,
		BEARTRAP = 201,
		RAILLINK_L = 202,
		RAILSLOPE_DN = 203,
		DEFENSE_END = 136
	}

	public enum SYSTEM_MAP
	{
		TEAM_MATCH_AWARDS,
		LOBBY,
		WAITING,
		BATTLE_TUTOR,
		INDIVIDUAL_MATCH_AWARDS,
		DEFENSE_MODE_AWARDS
	}

	public GameObject bulletMark;

	public GameObject chunk;

	public GameObject brickCreator;

	public GameObject shrinkBrick;

	public GameObject bungeeFeedbackEffects;

	public Brick[] bricks;

	public Dictionary<byte, Brick> dicTBrick;

	private List<CannonController> listCannonControllers;

	public float Bungee = -50f;

	public ArrayList[,,,] chunks;

	private Dictionary<int, GameObject> dicBricks;

	private Dictionary<int, GameObject> dicBrickCreators;

	public UserMap userMap;

	public UserMap result4TeamMatch;

	public UserMap lobby;

	public UserMap tutorMap;

	private bool isBattleMapLoad = true;

	private float deltaTime;

	public Material[] skyboxMaterial;

	private bool isLoaded;

	private int specialCount;

	private int gravityValue;

	private int numPlusBrick;

	private int numMiusBrick;

	private static BrickManager _instance;

	private Vector3[] offset55 = new Vector3[25]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, 2f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, -2f),
		new Vector3(1f, 0f, 0f),
		new Vector3(2f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(-2f, 0f, 0f),
		new Vector3(1f, 0f, 1f),
		new Vector3(1f, 0f, 2f),
		new Vector3(2f, 0f, 1f),
		new Vector3(2f, 0f, 2f),
		new Vector3(1f, 0f, -1f),
		new Vector3(1f, 0f, -2f),
		new Vector3(2f, 0f, -1f),
		new Vector3(2f, 0f, -2f),
		new Vector3(-1f, 0f, -1f),
		new Vector3(-1f, 0f, -2f),
		new Vector3(-2f, 0f, -1f),
		new Vector3(-2f, 0f, -2f),
		new Vector3(-1f, 0f, 1f),
		new Vector3(-1f, 0f, 2f),
		new Vector3(-2f, 0f, 1f),
		new Vector3(-2f, 0f, 2f)
	};

	private Vector3[] offset33 = new Vector3[9]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, -1f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 1f),
		new Vector3(1f, 0f, -1f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(-1f, 0f, 1f),
		new Vector3(-1f, 0f, -1f)
	};

	private Dictionary<int, Vector3> dicDoorT;

	public AudioClip audiclipTrap;

	public GameObject fxPortalOn;

	private Material[] materials;

	private Texture[] icons;

	private Dictionary<string, Texture> dicIcon;

	private Dictionary<string, Material> dicMat;

	private Dictionary<string, GameObject> dicBright;

	private Dictionary<string, GameObject> dicDark;

	private bool isIconLoaded;

	private bool isMaterialLoaded;

	private bool isBrightLoaded;

	private bool isDarkLoaded;

	private WWW wwwIconBundle;

	private WWW wwwMaterialBundle;

	private WWW wwwBrightBundle;

	private WWW wwwDarkBundle;

	public bool IsLoaded => isLoaded;

	public int Count => dicBricks.Count;

	public int SpecialCount => specialCount;

	public int GravityValue
	{
		get
		{
			return gravityValue;
		}
		set
		{
			gravityValue = value;
		}
	}

	public int NumPlusBrick
	{
		get
		{
			return numPlusBrick;
		}
		set
		{
			numPlusBrick = value;
		}
	}

	public int NumMiusBrick
	{
		get
		{
			return numMiusBrick;
		}
		set
		{
			numMiusBrick = value;
		}
	}

	public static BrickManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BrickManager)) as BrickManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BrickManager Instance");
				}
			}
			return _instance;
		}
	}

	public bool IsIconLoaded => isIconLoaded;

	public bool IsMaterialLoaded => isMaterialLoaded;

	public bool IsBrightLoaded => isBrightLoaded;

	public bool IsDarkLoaded => isDarkLoaded;

	private void OnApplicationQuit()
	{
	}

	private void Update()
	{
		if (userMap != null && userMap.isLoaded && !isLoaded)
		{
			OnUserMapLoaded();
		}
		if (isLoaded)
		{
			Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
			if (room != null && room.Status == Room.ROOM_STATUS.PLAYING && RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				deltaTime += Time.deltaTime;
				if (deltaTime > 1f)
				{
					deltaTime = 0f;
					foreach (CannonController listCannonController in listCannonControllers)
					{
						if (listCannonController.Shooter < 0)
						{
							listCannonController.NotifyMove();
						}
					}
				}
			}
		}
	}

	public void LoadTutorMap(bool isBattle)
	{
		tutorMap = null;
		isBattleMapLoad = isBattle;
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadBattleTutorFromWWW());
		}
		else
		{
			LoadBattleTutorFromLocalFileSystem();
		}
	}

	private IEnumerator LoadBattleTutorFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string battleGeom = "/BfData/BattleTutor.geometry";
		string editGeom = "/BfData/MapeditTutor.geometry";
		string url2 = "http://" + prop.GetResourceServer;
		url2 = ((!isBattleMapLoad) ? (url2 + editGeom) : (url2 + battleGeom));
		WWW www = new WWW(url2);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				tutorMap = new UserMap();
				if (tutorMap.LoadFromBinaryReader(reader))
				{
					tutorMap.PostLoadInit();
				}
				else
				{
					Debug.LogError("Fail to load system map for BattleTutor result ");
				}
			}
		}
	}

	private void LoadBattleTutorFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("Fail to find Resources directory");
		}
		else
		{
			string text2 = "BattleTutor.geometry";
			string text3 = "MapeditTutor.geometry";
			string empty = string.Empty;
			empty = ((!isBattleMapLoad) ? text3 : text2);
			string text4 = Path.Combine(text, empty);
			tutorMap = new UserMap();
			if (!tutorMap.Load(text4, 311))
			{
				Debug.LogError("Fail to load system map for BattleTutor " + text4);
			}
		}
	}

	private void LoadResult4TeamMatch()
	{
		result4TeamMatch = null;
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadResult4TeamMatchFromWWW());
		}
		else
		{
			LoadResult4TeamMatchFromLocalFileSystem();
		}
	}

	private IEnumerator LoadResult4TeamMatchFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Result4TeamMatch.geometry";
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				result4TeamMatch = new UserMap();
				if (result4TeamMatch.LoadFromBinaryReader(reader))
				{
					result4TeamMatch.PostLoadInit();
				}
				else
				{
					Debug.LogError("Fail to load system map for team match result ");
				}
			}
		}
	}

	private void LoadResult4TeamMatchFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("Fail to find Resources directory");
		}
		else
		{
			string text2 = Path.Combine(text, "Result4TeamMatch.geometry");
			result4TeamMatch = new UserMap();
			if (!result4TeamMatch.Load(text2, 36))
			{
				Debug.LogError("Fail to load system map for team match result " + text2);
			}
		}
	}

	private void LoadLobby()
	{
		lobby = null;
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadLobbyFromWWW());
		}
		else
		{
			LoadLobbyFromLocalFileSystem();
		}
	}

	private IEnumerator LoadLobbyFromWWW()
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/Lobby.geometry";
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				lobby = new UserMap();
				if (lobby.LoadFromBinaryReader(reader))
				{
					lobby.PostLoadInit();
				}
				else
				{
					Debug.LogError("Fail to load system map for lobby ");
				}
			}
		}
	}

	private void LoadLobbyFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("Fail to find Resources directory");
		}
		else
		{
			string text2 = Path.Combine(text, "Lobby.geometry");
			lobby = new UserMap();
			if (!lobby.Load(text2, 67))
			{
				Debug.LogError("Fail to load system map for team match result " + text2);
			}
		}
	}

	public Brick[] ToBrickArray(Brick.CATEGORY category)
	{
		List<Brick> list = new List<Brick>();
		foreach (KeyValuePair<byte, Brick> item in dicTBrick)
		{
			if (item.Value.category == category && item.Value.IsEnable(RoomManager.Instance.CurrentRoomType) && item.Value.IsTutor() && item.Value.UseAbleSeason() && item.Value.UseAbleGameMode())
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public Texture[] ToBrickIconArray(Brick.CATEGORY category)
	{
		List<Texture> list = new List<Texture>();
		foreach (KeyValuePair<byte, Brick> item in dicTBrick)
		{
			if (item.Value.category == category && item.Value.IsEnable(RoomManager.Instance.CurrentRoomType) && item.Value.IsTutor() && item.Value.UseAbleSeason() && item.Value.UseAbleGameMode())
			{
				list.Add(item.Value.Icon);
			}
		}
		return list.ToArray();
	}

	public Brick[] ToReplaceBrickArray(Brick.CATEGORY category)
	{
		List<Brick> list = new List<Brick>();
		foreach (KeyValuePair<byte, Brick> item in dicTBrick)
		{
			if (item.Value.category == category && !item.Value.disable && item.Value.CheckReplace() == Brick.REPLACE_CHECK.OK && (item.Value.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(item.Value.ticket) >= 0))
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public Texture[] ToReplaceBrickIconArray(Brick.CATEGORY category)
	{
		List<Texture> list = new List<Texture>();
		foreach (KeyValuePair<byte, Brick> item in dicTBrick)
		{
			if (item.Value.category == category && !item.Value.disable && item.Value.CheckReplace() == Brick.REPLACE_CHECK.OK && (item.Value.ticket.Length <= 0 || MyInfoManager.Instance.HaveFunction(item.Value.ticket) >= 0))
			{
				list.Add(item.Value.Icon);
			}
		}
		return list.ToArray();
	}

	public BrickInst[] ToBrickInstArray(Brick brick)
	{
		if (userMap == null)
		{
			return null;
		}
		return userMap.ToBrickInstArray(brick);
	}

	public BrickInst[] ToBrickInstArray()
	{
		if (userMap == null)
		{
			return null;
		}
		return userMap.ToArray();
	}

	public void ChangeUskDecals()
	{
		if (BuildOption.Instance.Props.useUskDecal)
		{
			for (int i = 0; i < bricks.Length; i++)
			{
				if (bricks[i].bulletMarks.Length > 0)
				{
					bricks[i].bulletMarks[0] = (UskManager.Instance.Get(bricks[i].bulletMarks[0].name) as Texture2D);
				}
			}
		}
	}

	private void Start()
	{
		LoadResult4TeamMatch();
	}

	public void ExportBricks()
	{
	}

	public int CountLimitedBrick(byte template)
	{
		if (userMap == null)
		{
			return 0;
		}
		return userMap.CountLimitedBrick(template);
	}

	public int GetStepOnBrick(Vector3 position)
	{
		Vector3 start = new Vector3(position.x, position.y + 0.2f, position.z);
		Vector3 end = new Vector3(position.x, position.y - 0.2f, position.z);
		int layerMask = 1 << LayerMask.NameToLayer("Chunk");
		if (!Physics.Linecast(start, end, out RaycastHit hitInfo, layerMask))
		{
			return -1;
		}
		BrickProperty[] componentsInChildren = hitInfo.transform.GetComponentsInChildren<BrickProperty>(includeInactive: true);
		if (componentsInChildren.Length <= 0)
		{
			return -1;
		}
		return componentsInChildren[0].Index;
	}

	private void Awake()
	{
		for (int i = 0; i < 6; i++)
		{
			Brick.meshCodeReset[i] = (ushort)(~Brick.meshCodeSet[i]);
			Brick.shadowCodeReset[i] = (ushort)(~Brick.shadowCodeSet[i]);
		}
		dicIcon = new Dictionary<string, Texture>();
		dicMat = new Dictionary<string, Material>();
		dicBright = new Dictionary<string, GameObject>();
		dicDark = new Dictionary<string, GameObject>();
		dicTBrick = new Dictionary<byte, Brick>();
		for (int j = 0; j < bricks.Length; j++)
		{
			if (bricks[j].index >= bricks.Length)
			{
				Debug.LogError("Invalid brick index found : " + bricks[j].index + " on " + j + "th");
			}
			if (dicTBrick.ContainsKey(bricks[j].index))
			{
				Debug.LogError("Duplicate brick index found : " + bricks[j].index);
			}
			else
			{
				dicTBrick.Add(bricks[j].index, bricks[j]);
			}
		}
		dicBricks = new Dictionary<int, GameObject>();
		dicBrickCreators = new Dictionary<int, GameObject>();
		listCannonControllers = new List<CannonController>();
		dicDoorT = new Dictionary<int, Vector3>();
		int num = (int)UserMap.xMax / 10;
		int num2 = (int)UserMap.yMax / 10;
		int num3 = (int)UserMap.zMax / 10;
		if ((int)UserMap.xMax % 10 > 0)
		{
			num++;
		}
		if ((int)UserMap.yMax % 10 > 0)
		{
			num2++;
		}
		if ((int)UserMap.zMax % 10 > 0)
		{
			num3++;
		}
		chunks = new ArrayList[num, num2, num3, bricks.Length];
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < num2; l++)
			{
				for (int m = 0; m < num3; m++)
				{
					for (int n = 0; n < bricks.Length; n++)
					{
						ArrayList[,,,] array = chunks;
						int num4 = k;
						int num5 = l;
						int num6 = m;
						int num7 = n;
						ArrayList arrayList = new ArrayList();
						array[num4, num5, num6, num7] = arrayList;
					}
				}
			}
		}
		Object.DontDestroyOnLoad(this);
	}

	public SpawnerDesc GetAwardSpawner4TeamMatch(Brick.SPAWNER_TYPE spawnerType, int rank)
	{
		return result4TeamMatch.GetSpawner(spawnerType, rank);
	}

	public SpawnerDesc GetSpawner(Brick.SPAWNER_TYPE spawnerType, int ticket)
	{
		if (userMap == null)
		{
			return null;
		}
		return userMap.GetSpawner(spawnerType, ticket);
	}

	public Transform GetSpawnerTransform(Brick.SPAWNER_TYPE spawnerType, int ticket)
	{
		GameObject gameObject = null;
		SpawnerDesc spawner = GetSpawner(spawnerType, ticket);
		if (spawner != null && dicBricks.ContainsKey(spawner.sequence))
		{
			gameObject = dicBricks[spawner.sequence];
		}
		if (null == gameObject)
		{
			return null;
		}
		return gameObject.transform;
	}

	public Vector3 GetRandomSpawnPos()
	{
		if (userMap == null)
		{
			return new Vector3(50f, 100f, 50f);
		}
		Vector2 randomSpawnPos = userMap.GetRandomSpawnPos();
		return new Vector3(randomSpawnPos.x, 100f, randomSpawnPos.y);
	}

	private bool MakeMapInstance(UserMap _userMap)
	{
		if (_userMap == null || !_userMap.isLoaded)
		{
			return false;
		}
		BrickInst[] array = _userMap.ToArray();
		gravityValue = 0;
		numMiusBrick = 0;
		numPlusBrick = 0;
		int num = 99999;
		int num2 = -99999;
		int num3 = 99999;
		int num4 = -99999;
		int num5 = 99999;
		int num6 = -99999;
		foreach (BrickInst brickInst in array)
		{
			if (num > brickInst.PosX)
			{
				num = brickInst.PosX;
			}
			if (num3 > brickInst.PosY)
			{
				num3 = brickInst.PosY;
			}
			if (num5 > brickInst.PosZ)
			{
				num5 = brickInst.PosZ;
			}
			if (num2 < brickInst.PosX)
			{
				num2 = brickInst.PosX;
			}
			if (num4 < brickInst.PosY)
			{
				num4 = brickInst.PosY;
			}
			if (num6 < brickInst.PosZ)
			{
				num6 = brickInst.PosZ;
			}
			Create(brickInst.Seq, _userMap.GetMeshCode(brickInst.Seq), brickInst.Template, new Vector3((float)(int)brickInst.PosX, (float)(int)brickInst.PosY, (float)(int)brickInst.PosZ), brickInst.Rot, combineMesh: false);
		}
		_userMap.cenX = (float)(num + num2) * 0.5f;
		_userMap.cenZ = (float)(num5 + num6) * 0.5f;
		_userMap.min.x = (float)num;
		_userMap.min.y = (float)num3;
		_userMap.min.z = (float)num5;
		_userMap.max.x = (float)num2;
		_userMap.max.y = (float)num4;
		_userMap.max.z = (float)num6;
		MergeAllChunks();
		RenderSettings.skybox = skyboxMaterial[_userMap.skybox];
		return true;
	}

	public void MakeSystemMapInstance(SYSTEM_MAP systemMap)
	{
		isLoaded = false;
		_Clear();
		switch (systemMap)
		{
		case SYSTEM_MAP.LOBBY:
		case SYSTEM_MAP.WAITING:
			MakeMapInstance(lobby);
			break;
		case SYSTEM_MAP.TEAM_MATCH_AWARDS:
		case SYSTEM_MAP.INDIVIDUAL_MATCH_AWARDS:
		case SYSTEM_MAP.DEFENSE_MODE_AWARDS:
			MakeMapInstance(result4TeamMatch);
			break;
		case SYSTEM_MAP.BATTLE_TUTOR:
			userMap = tutorMap;
			MakeMapInstance(tutorMap);
			isLoaded = true;
			break;
		}
	}

	private void OnUserMapLoaded()
	{
		_Clear();
		if (MakeMapInstance(userMap))
		{
			isLoaded = true;
			CSNetManager.Instance.Sock.SendCS_LOAD_COMPLETE_REQ(userMap.crc);
			if (RoomManager.Instance.CurrentRoom >= 0)
			{
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					gameObject.BroadcastMessage("OnLoadComplete");
				}
			}
		}
	}

	public void CacheBrick(int seq, byte template, byte x, byte y, byte z, ushort meshCode, byte rot)
	{
		userMap.CalcCRC(seq, template);
		if (userMap.AddBrickInst(seq, template, x, y, z, meshCode, rot) == null)
		{
			Debug.LogError("CacheBrick Fail ");
		}
	}

	public void CacheDone(int mapIndex, int skyboxIndex)
	{
		userMap.CacheDone(mapIndex, skyboxIndex);
	}

	public bool IsEmpty2(Brick newBrick, Vector3 org, bool brickOnly)
	{
		for (int i = 0; i < newBrick.vert; i++)
		{
			for (int j = 0; j < newBrick.horz * newBrick.horz; j++)
			{
				Vector3 b = (newBrick.horz != 3) ? offset55[j] : offset33[j];
				Vector3 pos = org + b;
				if (!_isEmpty(pos, brickOnly))
				{
					return false;
				}
			}
			org.y += 1f;
		}
		return true;
	}

	public bool IsBoxIn(Brick newBrick, Vector3 hitpos, Vector3 myPos)
	{
		float num = (float)newBrick.horz / 2f;
		float num2 = (float)newBrick.vert + 0.1f;
		if (myPos.x < hitpos.x - num || myPos.x > hitpos.x + num)
		{
			return true;
		}
		if (myPos.z < hitpos.z - num || myPos.z > hitpos.z + num)
		{
			return true;
		}
		if (myPos.y > hitpos.y + num2)
		{
			return true;
		}
		return false;
	}

	public bool IsEmpty(Brick newBrick, Vector3 org, bool brickOnly)
	{
		for (int i = 0; i < newBrick.vert; i++)
		{
			for (int j = 0; j < newBrick.horz * newBrick.horz; j++)
			{
				Vector3 b = (newBrick.horz != 3) ? offset55[j] : offset33[j];
				Vector3 pos = org + b;
				if (!_isEmpty(pos, brickOnly))
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool _isEmpty(Vector3 pos, bool brickOnly)
	{
		if (!brickOnly && Physics.CheckSphere(pos, 0.48f))
		{
			return false;
		}
		return 0 > GetSeqByPos(pos);
	}

	private bool _isEmpty2(Vector3 pos, bool brickOnly)
	{
		if (!brickOnly && Physics.CheckSphere(pos, 0.48f))
		{
			return false;
		}
		return 0 > GetSeqByPos(pos);
	}

	public bool ToCoord(Vector3 pos, ref byte x, ref byte y, ref byte z)
	{
		if (userMap == null)
		{
			return false;
		}
		return userMap.ToCoord(pos, ref x, ref y, ref z);
	}

	public bool IsValidPos(Vector3 pos)
	{
		if (userMap == null)
		{
			return false;
		}
		return userMap.IsValidCoord(pos);
	}

	public int GetSeqByPos(Vector3 pos)
	{
		if (userMap == null)
		{
			return -1;
		}
		return userMap.GetSeqByCoord(pos);
	}

	public BrickInst GetByPos(Vector3 pos)
	{
		if (userMap == null)
		{
			return null;
		}
		return userMap.GetByCoord(pos);
	}

	public Brick GetHitBrickTemplate(GameObject hit, Vector3 normal, Vector3 point)
	{
		BrickProperty hitBrickProperty = GetHitBrickProperty(hit, normal, point);
		if (null != hitBrickProperty)
		{
			return GetBrick(hitBrickProperty.Index);
		}
		if (hit.layer == LayerMask.NameToLayer("Chunk"))
		{
			BrickCreator component = hit.GetComponent<BrickCreator>();
			if (null != component)
			{
				return GetBrick(component.Brick);
			}
		}
		return null;
	}

	public BrickProperty GetHitBrickProperty(GameObject hit, Vector3 normal, Vector3 point)
	{
		if (hit.layer == LayerMask.NameToLayer("Chunk"))
		{
			GameObject brickObjectByPos = GetBrickObjectByPos(Brick.ToBrickCoord(normal, point));
			if (null != brickObjectByPos)
			{
				return brickObjectByPos.GetComponent<BrickProperty>();
			}
		}
		else
		{
			if (hit.layer == LayerMask.NameToLayer("Edit Layer"))
			{
				return hit.GetComponent<BrickProperty>();
			}
			if (hit.layer == LayerMask.NameToLayer("Brick"))
			{
				return hit.transform.parent.gameObject.GetComponent<BrickProperty>();
			}
		}
		return null;
	}

	public BrickInst GetBrickInst(int seq)
	{
		if (userMap == null)
		{
			return null;
		}
		return userMap.Get(seq);
	}

	public BrickInst GetHitBrickInst(GameObject hit, Vector3 normal, Vector3 point)
	{
		BrickProperty hitBrickProperty = GetHitBrickProperty(hit, normal, point);
		if (null == hitBrickProperty || userMap == null)
		{
			return null;
		}
		return userMap.Get(hitBrickProperty.Seq);
	}

	public GameObject GetBrickObject(int seq)
	{
		if (!dicBricks.ContainsKey(seq))
		{
			return null;
		}
		return dicBricks[seq];
	}

	public GameObject GetBrickObjectByPos(Vector3 pos)
	{
		int seqByPos = GetSeqByPos(pos);
		if (seqByPos < 0)
		{
			return null;
		}
		return GetBrickObject(seqByPos);
	}

	public void AddDoorTDic(int seq, Vector3 p)
	{
		if (!dicDoorT.ContainsKey(seq))
		{
			dicDoorT.Add(seq, p);
		}
	}

	public void CheckDistanceDoorT(Vector3 p)
	{
		if (dicDoorT.Count != 0)
		{
			foreach (KeyValuePair<int, Vector3> item in dicDoorT)
			{
				float num = Vector3.Distance(p, item.Value);
				if (num < 1.5f)
				{
					GameObject brickObject = GetBrickObject(item.Key);
					if (null != brickObject)
					{
						BrickProperty component = brickObject.GetComponent<BrickProperty>();
						if (null != component)
						{
							AnimationPlay(component.Index, component.Seq, "fire");
						}
					}
				}
			}
		}
	}

	public void Clear()
	{
		isLoaded = false;
		dicDoorT.Clear();
		if (userMap != null)
		{
			if (userMap != tutorMap)
			{
				userMap.Clear();
			}
			userMap = null;
		}
		_Clear();
	}

	public void _Clear()
	{
		specialCount = 0;
		dicBricks.Clear();
		dicBrickCreators.Clear();
		listCannonControllers.Clear();
		ArrayList[,,,] array = chunks;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		int length3 = array.GetLength(2);
		int length4 = array.GetLength(3);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length3; k++)
				{
					for (int l = 0; l < length4; l++)
					{
						ArrayList arrayList = array[i, j, k, l];
						foreach (GameObject item in arrayList)
						{
							Object.Destroy(item);
						}
						arrayList.Clear();
					}
				}
			}
		}
	}

	public BrickProperty[] GetAllScriptables()
	{
		List<BrickProperty> list = new List<BrickProperty>();
		if (userMap != null)
		{
			BrickInst[] allScriptables = userMap.GetAllScriptables();
			for (int i = 0; i < allScriptables.Length; i++)
			{
				GameObject brickObject = GetBrickObject(allScriptables[i].Seq);
				if (null != brickObject)
				{
					BrickProperty component = brickObject.GetComponent<BrickProperty>();
					if (null != component)
					{
						Brick brick = GetBrick(component.Index);
						if (brick != null && brick.function == Brick.FUNCTION.SCRIPT)
						{
							list.Add(component);
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	public Trigger[] GetEnabledScriptables()
	{
		List<Trigger> list = new List<Trigger>();
		if (userMap != null)
		{
			BrickInst[] allScriptables = userMap.GetAllScriptables();
			for (int i = 0; i < allScriptables.Length; i++)
			{
				if (!Application.loadedLevelName.Contains("Tutor") || allScriptables[i].Template != 162)
				{
					GameObject brickObject = GetBrickObject(allScriptables[i].Seq);
					if (null != brickObject)
					{
						Trigger componentInChildren = brickObject.GetComponentInChildren<Trigger>();
						if (null != componentInChildren && componentInChildren.enabled)
						{
							list.Add(componentInChildren);
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	public Vector3 GetNearstDefenseBrickPos(Vector3 curpos)
	{
		if (userMap == null)
		{
			return Vector3.zero;
		}
		Vector3 result = Vector3.zero;
		float num = 99999f;
		BrickInst[] array = userMap.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Template == 135)
			{
				Vector3 vector = new Vector3((float)(int)array[i].PosX, (float)(int)array[i].PosY, (float)(int)array[i].PosZ);
				float num2 = Vector3.Distance(vector, curpos);
				if (num > num2)
				{
					result = vector;
					num = num2;
				}
			}
		}
		return result;
	}

	public Vector3 GetNearstBrickPos(Vector3 curpos)
	{
		if (userMap != null)
		{
			Vector3 result = Vector3.zero;
			float num = 999999f;
			BrickInst[] array = userMap.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Template == 134 || array[i].Template == 135 || array[i].Template == 136)
				{
					GameObject brickObject = GetBrickObject(array[i].Seq);
					if (null != brickObject)
					{
						float num2 = Vector3.Distance(brickObject.transform.position, curpos);
						if (num > num2)
						{
							result = brickObject.transform.position;
							num = num2;
						}
					}
				}
			}
			return result;
		}
		return Vector3.zero;
	}

	private Vector3 Destroy(int seq)
	{
		if (!dicBricks.ContainsKey(seq))
		{
			Debug.LogError("Fail to destroy a gameObject");
			return Vector3.zero;
		}
		Vector3 result = Vector3.zero;
		GameObject gameObject = dicBricks[seq];
		if (null != gameObject)
		{
			result = gameObject.transform.position;
			BrickProperty component = gameObject.GetComponent<BrickProperty>();
			if (null != component && dicTBrick.ContainsKey(component.Index))
			{
				if (component.Index == 155 || component.Index == 157)
				{
					Instance.GravityValue++;
					Instance.NumMiusBrick--;
					GameObject gameObject2 = GameObject.Find("Me");
					if (null != gameObject2)
					{
						LocalController component2 = gameObject2.GetComponent<LocalController>();
						component2.ResetGravity();
					}
				}
				else if (component.Index == 156 || component.Index == 158)
				{
					Instance.GravityValue--;
					Instance.NumPlusBrick--;
					GameObject gameObject3 = GameObject.Find("Me");
					if (null != gameObject3)
					{
						LocalController component3 = gameObject3.GetComponent<LocalController>();
						component3.ResetGravity();
					}
				}
				if (!dicTBrick[component.Index].NeedChunkOptimize())
				{
					Object.DestroyImmediate(gameObject);
				}
				else
				{
					int num = (int)(result.x / 10f);
					int num2 = (int)(result.y / 10f);
					int num3 = (int)(result.z / 10f);
					GameObject gameObject4 = (GameObject)chunks[num, num2, num3, component.Index][component.Chunk];
					BrickChunk component4 = gameObject4.GetComponent<BrickChunk>();
					if (null != component4)
					{
						Object.DestroyImmediate(gameObject);
						component4.Merge();
					}
				}
				if (dicTBrick[component.Index].category == Brick.CATEGORY.ACCESSORY || dicTBrick[component.Index].category == Brick.CATEGORY.FUNCTIONAL)
				{
					specialCount--;
					if (specialCount < 0)
					{
						specialCount = 0;
					}
				}
			}
		}
		dicBricks.Remove(seq);
		return result;
	}

	public void DestroyBrick(int seq)
	{
		if (dicBricks.ContainsKey(seq))
		{
			GameObject gameObject = dicBricks[seq];
			BrickProperty component = gameObject.GetComponent<BrickProperty>();
			if (null != component && dicTBrick.ContainsKey(component.Index))
			{
				if (component.Index == 116)
				{
					Object.Instantiate((Object)GlobalVars.Instance.drum_poison_explosion, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
					Object.Instantiate((Object)GlobalVars.Instance.drum_poison_eff, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
					GameObject gameObject2 = GameObject.Find("Me");
					if (null != gameObject2)
					{
						LocalController component2 = gameObject2.GetComponent<LocalController>();
						component2.PosionSpot = gameObject.transform.position;
					}
				}
				else if (component.Index == 115 || component.Index == 193)
				{
					Object.Instantiate((Object)GlobalVars.Instance.drum_bomp_explosion, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
				}
				if (null != dicTBrick[component.Index].debris)
				{
					Object.Instantiate((Object)dicTBrick[component.Index].debris, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f));
				}
			}
		}
		DelBrick(seq, shrink: false);
	}

	public void SetBrickHitpoint(int seq, int hp)
	{
		if (dicBricks.ContainsKey(seq))
		{
			GameObject gameObject = dicBricks[seq];
			BrickProperty component = gameObject.GetComponent<BrickProperty>();
			if (null != component && component.HitPoint > hp)
			{
				component.HitPoint = hp;
			}
		}
	}

	public void DelBrick(int seq, bool shrink)
	{
		if (userMap != null)
		{
			List<int> morphes = new List<int>();
			byte rot = 0;
			Vector3 size = new Vector3(1f, 1f, 1f);
			Vector3 center = Vector3.zero;
			BrickInst brickInst = userMap.Get(seq);
			if (brickInst != null)
			{
				rot = brickInst.Rot;
			}
			if (dicBricks.ContainsKey(seq))
			{
				BoxCollider component = dicBricks[seq].GetComponent<BoxCollider>();
				if (null != component)
				{
					size = component.size;
					center = component.center;
				}
			}
			if (userMap.DelBrickInst(seq, ref morphes))
			{
				Vector3 position = Destroy(seq);
				List<GameObject> modifiedChunks = new List<GameObject>();
				foreach (int item in morphes)
				{
					Morph(item, ref modifiedChunks);
				}
				foreach (GameObject item2 in modifiedChunks)
				{
					BrickChunk component2 = item2.GetComponent<BrickChunk>();
					if (null != component2)
					{
						component2.Merge();
					}
				}
				if (shrink)
				{
					GameObject gameObject = Object.Instantiate((Object)shrinkBrick, position, Rot.ToQuaternion(rot)) as GameObject;
					if (null != gameObject)
					{
						gameObject.GetComponent<ShrinkBrick>().CenterAndSize(center, size);
					}
				}
				for (int i = 0; i < listCannonControllers.Count; i++)
				{
					if (listCannonControllers[i].BrickSeq == seq)
					{
						listCannonControllers.RemoveAt(i);
						break;
					}
				}
			}
			if (dicBrickCreators.ContainsKey(seq))
			{
				Object.DestroyImmediate(dicBrickCreators[seq]);
				dicBrickCreators.Remove(seq);
			}
		}
	}

	public void FetchCenterAndSize(byte index, ref Vector3 center, ref Vector3 size)
	{
		center = new Vector3(0f, 0f, 0f);
		size = new Vector3(1f, 1f, 1f);
		if (dicTBrick.ContainsKey(index))
		{
			BoxCollider component = dicTBrick[index].brick.GetComponent<BoxCollider>();
			if (null != component)
			{
				center = component.center;
				size = component.size;
			}
		}
	}

	public void AddBrickCreator(int seq, byte index, Vector3 position, byte rot)
	{
		GameObject gameObject = Object.Instantiate((Object)brickCreator, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		if (null != gameObject)
		{
			BrickCreator component = gameObject.GetComponent<BrickCreator>();
			if (null != component)
			{
				component.Seq = seq;
				component.Brick = index;
				component.Rotation = rot;
			}
			dicBrickCreators.Add(seq, gameObject);
		}
	}

	public void DelBrickCreator(int seq)
	{
		if (dicBrickCreators.ContainsKey(seq))
		{
			dicBrickCreators.Remove(seq);
		}
	}

	public void AddBrick(int seq, byte index, Vector3 position, byte rot)
	{
		if (userMap != null)
		{
			byte x = (byte)Mathf.RoundToInt(position.x);
			byte y = (byte)Mathf.RoundToInt(position.y);
			byte z = (byte)Mathf.RoundToInt(position.z);
			List<int> morphes = new List<int>();
			if (userMap.AddBrickInst(seq, index, x, y, z, rot, ref morphes) && Create(seq, userMap.GetMeshCode(seq), index, position, rot, combineMesh: true))
			{
				List<GameObject> modifiedChunks = new List<GameObject>();
				foreach (int item in morphes)
				{
					Morph(item, ref modifiedChunks);
				}
				foreach (GameObject item2 in modifiedChunks)
				{
					BrickChunk component = item2.GetComponent<BrickChunk>();
					if (null != component)
					{
						component.Merge();
					}
				}
			}
		}
	}

	private void Morph(int seq, ref List<GameObject> modifiedChunks)
	{
		if (dicBricks.ContainsKey(seq))
		{
			GameObject gameObject = dicBricks[seq];
			if (!(null == gameObject))
			{
				BrickProperty component = gameObject.GetComponent<BrickProperty>();
				if (!(null == component))
				{
					byte index = component.Index;
					int num = component.Chunk;
					Vector3 position = gameObject.transform.position;
					Quaternion rotation = gameObject.transform.rotation;
					ushort meshCode = userMap.GetMeshCode(seq);
					int hitPoint = component.HitPoint;
					Object.DestroyImmediate(gameObject);
					gameObject = dicTBrick[index].Instantiate(meshCode, position, rotation);
					if (num < 0)
					{
						Debug.LogError("chunkIndex is invalid");
					}
					int num2 = (int)(position.x / 10f);
					int num3 = (int)(position.y / 10f);
					int num4 = (int)(position.z / 10f);
					GameObject gameObject2 = (GameObject)chunks[num2, num3, num4, index][num];
					BrickChunk component2 = gameObject2.GetComponent<BrickChunk>();
					component2.AddBrick(gameObject, merge: false);
					component = gameObject.GetComponent<BrickProperty>();
					if (null != component)
					{
						component.Seq = seq;
						component.Index = index;
						component.Chunk = num;
						component.HitPoint = hitPoint;
					}
					dicBricks[seq] = gameObject;
					bool flag = false;
					foreach (GameObject modifiedChunk in modifiedChunks)
					{
						if (modifiedChunk == gameObject2)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						modifiedChunks.Add(gameObject2);
					}
				}
			}
		}
	}

	private bool Create(int seq, ushort meshCode, byte index, Vector3 position, byte rot, bool combineMesh)
	{
		if (!dicTBrick.ContainsKey(index))
		{
			Debug.LogError("Invalid brick index " + index);
			return false;
		}
		if (index == 135 && RoomManager.Instance.CurrentRoomType != 0)
		{
			return false;
		}
		if ((index == 134 || index == 136) && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.MISSION && RoomManager.Instance.CurrentRoomType != 0)
		{
			return false;
		}
		if (index == 196 && RoomManager.Instance.CurrentRoomType != 0)
		{
			return false;
		}
		if (index == 181 && RoomManager.Instance.CurrentRoomType != 0 && RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.ESCAPE)
		{
			return false;
		}
		GameObject gameObject = null;
		gameObject = dicTBrick[index].Instantiate(meshCode, position, rot);
		if (gameObject == null)
		{
			return false;
		}
		bool flag = true;
		int num = -1;
		if (dicTBrick[index].NeedChunkOptimize())
		{
			int num2 = (int)(position.x / 10f);
			int num3 = (int)(position.y / 10f);
			int num4 = (int)(position.z / 10f);
			int num5 = 0;
			while (num < 0 && num5 < chunks[num2, num3, num4, index].Count)
			{
				GameObject gameObject2 = (GameObject)chunks[num2, num3, num4, index][num5];
				BrickChunk component = gameObject2.GetComponent<BrickChunk>();
				if (component != null && component.AddBrick(gameObject, combineMesh))
				{
					num = num5;
				}
				num5++;
			}
			if (num < 0)
			{
				GameObject gameObject3 = Object.Instantiate((Object)chunk) as GameObject;
				if (null != gameObject3)
				{
					chunks[num2, num3, num4, index].Add(gameObject3);
					BrickChunk component2 = gameObject3.GetComponent<BrickChunk>();
					component2.Init(dicTBrick[index].Mat, dicTBrick[index].maxChildrenPerChunk);
					if (component2.AddBrick(gameObject, combineMesh))
					{
						num = chunks[num2, num3, num4, index].Count - 1;
					}
				}
			}
			if (num < 0)
			{
				Object.DestroyImmediate(gameObject);
				flag = false;
			}
		}
		if (flag)
		{
			if (dicTBrick[index].category == Brick.CATEGORY.ACCESSORY || dicTBrick[index].category == Brick.CATEGORY.FUNCTIONAL)
			{
				specialCount++;
			}
			BrickProperty component3 = gameObject.GetComponent<BrickProperty>();
			if (null != component3)
			{
				component3.Seq = seq;
				component3.Index = index;
				component3.Chunk = num;
				component3.HitPoint = dicTBrick[index].hitPoint;
			}
			dicBricks.Add(seq, gameObject);
			CannonController[] componentsInChildren = gameObject.GetComponentsInChildren<CannonController>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				listCannonControllers.Add(componentsInChildren[0]);
			}
			bool flag2 = dicTBrick[index].GetSpawnerType() == Brick.SPAWNER_TYPE.NONE;
			bool flag3 = dicTBrick[index].GetSpawnerType() == Brick.SPAWNER_TYPE.DEFENCE_SPAWNER;
			if (!flag2 && !flag3 && RoomManager.Instance.CurrentRoomType != 0)
			{
				gameObject.SetActive(value: false);
			}
			switch (index)
			{
			case 155:
			case 157:
			{
				Instance.GravityValue--;
				Instance.NumMiusBrick++;
				GameObject gameObject5 = GameObject.Find("Me");
				if (null != gameObject5)
				{
					LocalController component5 = gameObject5.GetComponent<LocalController>();
					component5.ResetGravity();
				}
				break;
			}
			case 156:
			case 158:
			{
				Instance.GravityValue++;
				Instance.NumPlusBrick++;
				GameObject gameObject4 = GameObject.Find("Me");
				if (null != gameObject4)
				{
					LocalController component4 = gameObject4.GetComponent<LocalController>();
					component4.ResetGravity();
				}
				break;
			}
			}
			switch (index)
			{
			case 163:
				userMap.CheckRedPortalAlphaBlending(seq, add: true);
				break;
			case 164:
				userMap.CheckBluePortalAlphaBlending(seq, add: true);
				break;
			case 178:
				userMap.CheckNeutralPortalAlphaBlending(seq, add: true);
				break;
			}
			BrickCollisionTest componentInChildren = gameObject.GetComponentInChildren<BrickCollisionTest>();
			if (componentInChildren != null)
			{
				componentInChildren.Index = index;
				componentInChildren.Seq = seq;
			}
		}
		return flag;
	}

	public void ShowTeamSpawners(bool visible)
	{
		if (userMap != null)
		{
			BrickInst[] array = userMap.ToTeamSpawnersArray();
			int num = 0;
			while (array != null && num < array.Length)
			{
				GameObject gameObject = null;
				if (dicBricks.ContainsKey(array[num].Seq))
				{
					gameObject = dicBricks[array[num].Seq];
				}
				if (null != gameObject)
				{
					gameObject.SetActive(visible);
				}
				num++;
			}
		}
	}

	private void MergeAllChunks()
	{
		ArrayList[,,,] array = chunks;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		int length3 = array.GetLength(2);
		int length4 = array.GetLength(3);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length3; k++)
				{
					for (int l = 0; l < length4; l++)
					{
						ArrayList arrayList = array[i, j, k, l];
						foreach (GameObject item in arrayList)
						{
							BrickChunk component = item.GetComponent<BrickChunk>();
							component.Merge();
						}
					}
				}
			}
		}
	}

	public Texture GetIcon(int index)
	{
		if (!dicTBrick.ContainsKey((byte)index))
		{
			return null;
		}
		return dicTBrick[(byte)index].Icon;
	}

	public AudioClip GetStepSound(int index)
	{
		return GetBrick(index)?.GetStepSound();
	}

	public Texture2D GetBulletMark(int index)
	{
		return GetBrick(index)?.GetBulletMark();
	}

	public GameObject GetBulletImpact(int index)
	{
		return GetBrick(index)?.GetBulletImpact();
	}

	public Brick GetBrick(int index)
	{
		if (!dicTBrick.ContainsKey((byte)index))
		{
			Debug.LogError("Caller requests brick with invalid index " + index);
			return null;
		}
		return dicTBrick[(byte)index];
	}

	public void UpdateScript(int seq, string alias, bool enableOnAwake, bool visibleOnAwake, string commands)
	{
		BrickInst brickInst = GetBrickInst(seq);
		if (brickInst != null)
		{
			Brick brick = GetBrick(brickInst.Template);
			if (brick != null && brick.function == Brick.FUNCTION.SCRIPT)
			{
				brickInst.UpdateScript(alias, enableOnAwake, visibleOnAwake, commands);
			}
			else
			{
				Debug.LogError("Fail to update script, because there is no such template or the brick is not scriptable");
			}
		}
	}

	public void EnableColliderBox(int seq, bool enable)
	{
		if (enable)
		{
			GameObject brickObject = GetBrickObject(seq);
			if (brickObject == null)
			{
				Debug.LogError("brickObj == null");
			}
			else
			{
				BoxCollider[] componentsInChildren = brickObject.GetComponentsInChildren<BoxCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (!(componentsInChildren[i].name == "collider_pillaL") && !(componentsInChildren[i].name == "collider_pillaR"))
					{
						componentsInChildren[i].enabled = true;
					}
				}
			}
		}
		else
		{
			GameObject brickObject2 = GetBrickObject(seq);
			if (brickObject2 == null)
			{
				Debug.LogError("brickObj == null: " + seq);
			}
			else
			{
				BoxCollider[] componentsInChildren2 = brickObject2.GetComponentsInChildren<BoxCollider>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if (!(componentsInChildren2[j].name == "collider_pillaL") && !(componentsInChildren2[j].name == "collider_pillaR"))
					{
						componentsInChildren2[j].enabled = false;
					}
				}
			}
		}
	}

	public void AnimationPlay(int index, int seq, string animName, bool IsP2P = false)
	{
		GameObject brickObject = GetBrickObject(seq);
		if (!(brickObject == null))
		{
			Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
			if (!(componentInChildren == null) && !componentInChildren.IsPlaying("fire") && !componentInChildren.IsPlaying("fired"))
			{
				if (index == 166)
				{
					AudioSource componentInChildren2 = brickObject.GetComponentInChildren<AudioSource>();
					if (componentInChildren2 != null && audiclipTrap != null)
					{
						componentInChildren2.PlayOneShot(audiclipTrap);
					}
				}
				if (!IsP2P)
				{
					P2PManager.Instance.SendPEER_BRICK_ANIM(index, seq, animName);
				}
				componentInChildren.Play(animName);
			}
		}
	}

	public void AnimationCrossFade(int seq, string animName, bool IsP2P = false)
	{
		GameObject brickObject = GetBrickObject(seq);
		if (!(brickObject == null))
		{
			Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
			if (componentInChildren == null)
			{
				Debug.LogError("anim == null");
			}
			if (!componentInChildren.IsPlaying("fire") && !componentInChildren.IsPlaying("fired"))
			{
				if (!IsP2P)
				{
					P2PManager.Instance.SendPEER_BRICK_ANIM_CROSSFADE(seq, animName);
				}
				componentInChildren.CrossFade(animName, 1f);
			}
		}
	}

	public bool checkAddMinMaxGravity(int idx)
	{
		if ((idx == 155 || idx == 157) && Instance.NumMiusBrick >= 10)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("OVER_NUM"));
			return false;
		}
		if ((idx == 156 || idx == 158) && Instance.NumPlusBrick >= 10)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("OVER_NUM"));
			return false;
		}
		return true;
	}

	public void portalFX(int brickSeq)
	{
		BrickInst brickInst = GetBrickInst(brickSeq);
		if (brickInst != null)
		{
			Object.Instantiate(position: new Vector3((float)(int)brickInst.PosX, (float)(int)brickInst.PosY, (float)(int)brickInst.PosZ), original: fxPortalOn, rotation: Quaternion.Euler(0f, 0f, 0f));
		}
	}

	public Material[] saveBundleBrickMaterials()
	{
		materials = new Material[bricks.Length];
		for (int i = 0; i < bricks.Length; i++)
		{
			materials[i] = bricks[i].Mat;
		}
		return materials;
	}

	public Texture[] saveBundleBrickIcons()
	{
		icons = new Texture[bricks.Length];
		for (int i = 0; i < bricks.Length; i++)
		{
			icons[i] = bricks[i].Icon;
		}
		return icons;
	}

	public GameObject[] saveBundleBrightBricks()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < bricks.Length; i++)
		{
			for (int j = 0; j < bricks[i].Bright.Length; j++)
			{
				list.Add(bricks[i].Bright[j]);
			}
		}
		return list.ToArray();
	}

	public GameObject[] saveBundleDarkBricks()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < bricks.Length; i++)
		{
			for (int j = 0; j < bricks[i].Dark.Length; j++)
			{
				list.Add(bricks[i].Dark[j]);
			}
		}
		return list.ToArray();
	}

	public void getIconBundle(string fileName)
	{
		string path = Path.Combine(Application.dataPath, "Resources/");
		string str = Path.Combine(path, fileName);
		wwwIconBundle = new WWW("file://" + str);
		if (wwwIconBundle.bytes.Length > 0)
		{
			AssetBundle assetBundle = wwwIconBundle.assetBundle;
			Object[] array = assetBundle.LoadAll();
			for (int i = 0; i < array.Length; i++)
			{
				Texture2D texture2D = array[i] as Texture2D;
				if (texture2D != null)
				{
					dicIcon.Add(texture2D.name, texture2D);
				}
				else
				{
					Debug.LogError("local bundle load error: " + array[i].name);
				}
			}
			isIconLoaded = true;
		}
		else
		{
			StartCoroutine(downloadICONS(fileName));
		}
	}

	public void getMaterialBundle(string fileName)
	{
		string path = Path.Combine(Application.dataPath, "Resources/");
		string str = Path.Combine(path, fileName);
		wwwMaterialBundle = new WWW("file://" + str);
		if (wwwMaterialBundle.bytes.Length > 0)
		{
			AssetBundle assetBundle = wwwMaterialBundle.assetBundle;
			Object[] array = assetBundle.LoadAll(typeof(Material));
			for (int i = 0; i < array.Length; i++)
			{
				Material material = array[i] as Material;
				if (material != null)
				{
					dicMat.Add(material.name, material);
				}
				else
				{
					Debug.LogError("local bundle load error: " + array[i].name);
				}
			}
			isMaterialLoaded = true;
		}
		else
		{
			StartCoroutine(downloadMATERIALS(fileName));
		}
	}

	public void DictionaryMatAdd(string n, Material m)
	{
		dicMat.Add(n, m);
	}

	public void DictionaryIconAdd(string n, Texture2D t)
	{
		dicIcon.Add(n, t);
	}

	public void getBrightBundle(string fileName)
	{
		string path = Path.Combine(Application.dataPath, "Resources/");
		string str = Path.Combine(path, fileName);
		wwwBrightBundle = new WWW("file://" + str);
		if (wwwBrightBundle.bytes.Length > 0)
		{
			AssetBundle assetBundle = wwwBrightBundle.assetBundle;
			Object[] array = assetBundle.LoadAll(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i] as GameObject;
				if (gameObject != null)
				{
					if (!dicBright.ContainsKey(gameObject.name))
					{
						dicBright.Add(gameObject.name, gameObject);
					}
					else
					{
						Debug.Log("(bright)already exist: " + gameObject.name);
					}
				}
				else
				{
					Debug.LogError("local bundle load error: " + array[i].name);
				}
			}
			isBrightLoaded = true;
		}
		else
		{
			StartCoroutine(downloadBRIGHTS(fileName));
		}
	}

	public void getDarkBundle(string fileName)
	{
		string path = Path.Combine(Application.dataPath, "Resources/");
		string str = Path.Combine(path, fileName);
		wwwDarkBundle = new WWW("file://" + str);
		if (wwwDarkBundle.bytes.Length > 0)
		{
			AssetBundle assetBundle = wwwDarkBundle.assetBundle;
			Object[] array = assetBundle.LoadAll(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i] as GameObject;
				if (gameObject != null)
				{
					if (!dicDark.ContainsKey(gameObject.name))
					{
						dicDark.Add(gameObject.name, gameObject);
					}
					else
					{
						Debug.Log("(dark)already exist: " + gameObject.name);
					}
				}
			}
			isDarkLoaded = true;
		}
		else
		{
			StartCoroutine(downloadDARKS(fileName));
		}
	}

	private IEnumerator downloadICONS(string fileName)
	{
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		wwwIconBundle = new WWW(url);
		yield return (object)wwwIconBundle;
		AssetBundle bundle = wwwIconBundle.assetBundle;
		Object[] objs = bundle.LoadAll();
		icons = new Texture2D[objs.Length];
		for (int i = 0; i < objs.Length; i++)
		{
			Texture2D tex = objs[i] as Texture2D;
			if (tex != null)
			{
				icons[i] = tex;
			}
			else
			{
				Debug.LogError("local bundle load error: " + objs[i].name);
			}
		}
		isIconLoaded = true;
	}

	private IEnumerator downloadMATERIALS(string fileName)
	{
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		wwwMaterialBundle = new WWW(url);
		yield return (object)wwwMaterialBundle;
		AssetBundle bundle = wwwMaterialBundle.assetBundle;
		Object[] objs = bundle.LoadAll(typeof(Material));
		materials = new Material[objs.Length];
		for (int i = 0; i < objs.Length; i++)
		{
			Material mat = objs[i] as Material;
			if (mat != null)
			{
				materials[i] = mat;
			}
			else
			{
				Debug.LogError("local bundle load error: " + objs[i].name);
			}
		}
		isMaterialLoaded = true;
	}

	private IEnumerator downloadBRIGHTS(string fileName)
	{
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		wwwMaterialBundle = new WWW(url);
		yield return (object)wwwMaterialBundle;
		AssetBundle bundle = wwwMaterialBundle.assetBundle;
		Object[] objs = bundle.LoadAll(typeof(Material));
		for (int i = 0; i < objs.Length; i++)
		{
			GameObject obj = objs[i] as GameObject;
			if (obj != null)
			{
				dicBright.Add(obj.name, obj);
			}
			else
			{
				Debug.LogError("local bundle load error: " + objs[i].name);
			}
		}
		isBrightLoaded = true;
	}

	private IEnumerator downloadDARKS(string fileName)
	{
		string url = "http://" + BuildOption.Instance.Props.GetResourceServer + "/BfData/" + fileName;
		wwwDarkBundle = new WWW(url);
		yield return (object)wwwDarkBundle;
		AssetBundle bundle = wwwDarkBundle.assetBundle;
		Object[] objs = bundle.LoadAll(typeof(Material));
		for (int i = 0; i < objs.Length; i++)
		{
			GameObject obj = objs[i] as GameObject;
			if (obj != null)
			{
				dicDark.Add(obj.name, obj);
			}
			else
			{
				Debug.LogError("local bundle load error: " + objs[i].name);
			}
		}
		isDarkLoaded = true;
	}

	public ushort GetPossibleModeMask()
	{
		ushort num = 0;
		int num2 = Instance.CountLimitedBrick(23);
		int num3 = Instance.CountLimitedBrick(22);
		int num4 = Instance.CountLimitedBrick(24);
		int num5 = Instance.CountLimitedBrick(121);
		int num6 = Instance.CountLimitedBrick(122);
		int num7 = Instance.CountLimitedBrick(123);
		int num8 = Instance.CountLimitedBrick(124);
		int num9 = Instance.CountLimitedBrick(134);
		int num10 = Instance.CountLimitedBrick(135);
		int num11 = Instance.CountLimitedBrick(136);
		int num12 = Instance.CountLimitedBrick(181);
		num = 0;
		if (num3 >= 8 && num2 >= 8 && BuildOption.Instance.Props.teamMatchMode)
		{
			num = (ushort)(num | 1);
		}
		if (num4 >= 16)
		{
			if (BuildOption.Instance.Props.individualMatchMode)
			{
				num = (ushort)(num | 2);
			}
			if (BuildOption.Instance.Props.zombieMode)
			{
				num = (ushort)(num | 0x100);
			}
		}
		if (num3 >= 8 && num2 >= 8 && num5 >= 1 && num6 >= 1 && num7 >= 1 && BuildOption.Instance.Props.ctfMatchMode)
		{
			num = (ushort)(num | 4);
		}
		if (num8 >= 2 && num3 >= 8 && num2 >= 8 && BuildOption.Instance.Props.explosionMatchMode)
		{
			num = (ushort)(num | 8);
		}
		if (num3 >= 8 && num2 >= 8 && num9 > 0 && num10 > 0 && num11 > 0 && BuildOption.Instance.Props.defenseMatchMode && Instance.userMap.CanDefensemapSave())
		{
			num = (ushort)(num | 0x10);
		}
		if (num4 >= 16 && num12 >= 1 && BuildOption.Instance.Props.escapeMode)
		{
			num = (ushort)(num | 0x80);
		}
		return num;
	}
}
