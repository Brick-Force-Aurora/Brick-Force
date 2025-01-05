using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserMap
{
	public const int version = 1;

	public bool isLoaded;

	public int map = -1;

	public int skybox = -1;

	public int crc;

	public Dictionary<int, BrickInst> dic;

	private BrickInst[,,] geometry;

	private Dictionary<byte, int> limitedBricks;

	private List<Vector2> randomSpawners;

	private List<SpawnerDesc> redTeamSpawners;

	private List<SpawnerDesc> blueTeamSpawners;

	private List<SpawnerDesc> singleSpawners;

	private List<SpawnerDesc> redFlagSpawners;

	private List<SpawnerDesc> blueFlagSpawners;

	private List<SpawnerDesc> FlagSpawners;

	private List<SpawnerDesc> BombSpawners;

	private List<SpawnerDesc> DefenseSpawners;

	private List<BrickInst> scriptables;

	private List<SpawnerDesc> portalReds;

	private List<SpawnerDesc> portalBlues;

	private List<SpawnerDesc> portalNeutrals;

	private List<SpawnerDesc> railSpawners;

	public static byte xMax = 100;

	public static byte yMax = 100;

	public static byte zMax = 100;

	public float cenX;

	public float cenZ;

	public Vector3 min = Vector3.zero;

	public Vector3 max = Vector3.zero;

	public bool IsPortalMove;

	public UserMap()
	{
		dic = new Dictionary<int, BrickInst>();
		geometry = new BrickInst[xMax, yMax, zMax];
		randomSpawners = new List<Vector2>();
		blueTeamSpawners = new List<SpawnerDesc>();
		redTeamSpawners = new List<SpawnerDesc>();
		singleSpawners = new List<SpawnerDesc>();
		blueFlagSpawners = new List<SpawnerDesc>();
		redFlagSpawners = new List<SpawnerDesc>();
		FlagSpawners = new List<SpawnerDesc>();
		BombSpawners = new List<SpawnerDesc>();
		DefenseSpawners = new List<SpawnerDesc>();
		scriptables = new List<BrickInst>();
		portalReds = new List<SpawnerDesc>();
		portalBlues = new List<SpawnerDesc>();
		portalNeutrals = new List<SpawnerDesc>();
		railSpawners = new List<SpawnerDesc>();
		limitedBricks = new Dictionary<byte, int>();
		Clear();
	}

	private bool IsDefenseBrick(int seq)
	{
		if (seq == 134 || seq == 135 || seq == 136)
		{
			return true;
		}
		return false;
	}

	public BrickInst[] ToBrickInstArray(Brick brick)
	{
		List<BrickInst> list = new List<BrickInst>();
		foreach (KeyValuePair<int, BrickInst> item in dic)
		{
			if (item.Value.Template == (byte)brick.seq)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public bool CanDefensemapSave()
	{
		foreach (KeyValuePair<int, BrickInst> item in dic)
		{
			if (item.Value.Template == 135)
			{
				int num = 0;
				Vector3 vector = new Vector3((float)(int)item.Value.PosX, (float)(int)item.Value.PosY, (float)(int)item.Value.PosZ);
				Vector3 pos = vector;
				pos.x -= 3f;
				BrickInst byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				pos = vector;
				pos.x += 3f;
				byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				pos = vector;
				pos.z -= 3f;
				byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				pos = vector;
				pos.z += 3f;
				byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				pos = vector;
				pos.y -= 3f;
				byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				pos = vector;
				pos.y += 3f;
				byPos = BrickManager.Instance.GetByPos(pos);
				if (byPos != null && IsDefenseBrick(byPos.Template))
				{
					num++;
				}
				if (num < 2)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool Save(string fileName, int mapIndex, int skyboxIndex)
	{
		try
		{
			FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(1);
			binaryWriter.Write(mapIndex);
			binaryWriter.Write(skyboxIndex);
			binaryWriter.Write(dic.Count);
			foreach (KeyValuePair<int, BrickInst> item in dic)
			{
				binaryWriter.Write(item.Value.Seq);
				binaryWriter.Write(item.Value.Template);
				binaryWriter.Write(item.Value.PosX);
				binaryWriter.Write(item.Value.PosY);
				binaryWriter.Write(item.Value.PosZ);
				binaryWriter.Write(item.Value.Code);
				binaryWriter.Write(item.Value.Rot);
				if (item.Value.BrickForceScript != null)
				{
					binaryWriter.Write(item.Value.BrickForceScript.Alias);
					binaryWriter.Write(item.Value.BrickForceScript.EnableOnAwake);
					binaryWriter.Write(item.Value.BrickForceScript.VisibleOnAwake);
					binaryWriter.Write(item.Value.BrickForceScript.GetCommandString());
				}
			}
			binaryWriter.Close();
			fileStream.Close();
			map = mapIndex;
			skybox = skyboxIndex;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0191:;
		}
		return true;
	}

    public void CalcCRC(int seq, byte template)
	{
		crc ^= seq + template;
	}

	public bool LoadFromBinaryReader(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		map = reader.ReadInt32();
		skybox = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		int num3 = 99999;
		int num4 = -99999;
		int num5 = 99999;
		int num6 = -99999;
		int num7 = 99999;
		int num8 = -99999;
		for (int i = 0; i < num2; i++)
		{
			int seq = reader.ReadInt32();
			byte b = reader.ReadByte();
			byte b2 = reader.ReadByte();
			byte b3 = reader.ReadByte();
			byte b4 = reader.ReadByte();
			ushort meshCode = reader.ReadUInt16();
			byte rot = reader.ReadByte();
			if (num3 > b2)
			{
				num3 = b2;
			}
			if (num5 > b2)
			{
				num5 = b3;
			}
			if (num7 > b4)
			{
				num7 = b4;
			}
			if (num4 < b2)
			{
				num4 = b2;
			}
			if (num6 < b3)
			{
				num6 = b3;
			}
			if (num8 < b4)
			{
				num8 = b4;
			}
			CalcCRC(seq, b);
			Brick brick = BrickManager.Instance.GetBrick(b);
			BrickInst brickInst = AddBrickInst(seq, b, b2, b3, b4, meshCode, rot);
			if (brick == null || brickInst == null)
			{
				Debug.LogError("Fail to add a brick instance on load ");
			}
			else if (num >= 1 && brick.function == Brick.FUNCTION.SCRIPT)
			{
				string alias = reader.ReadString();
				bool enableOnAwake = reader.ReadBoolean();
				bool visibleOnAwake = reader.ReadBoolean();
				string commands = reader.ReadString();
				brickInst.UpdateScript(alias, enableOnAwake, visibleOnAwake, commands);
			}
		}
		cenX = (float)(num3 + num4) * 0.5f;
		cenZ = (float)(num7 + num8) * 0.5f;
		min.x = (float)num3;
		min.y = (float)num5;
		min.z = (float)num7;
		max.x = (float)num4;
		max.y = (float)num6;
		max.z = (float)num8;
		return true;
	}

	public bool Load(string fileName, int mapIndex)
	{
		try
		{
			FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			LoadFromBinaryReader(binaryReader);
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0041:;
		}
		PostLoadInit();
		return true;
	}

	public bool Load(int mapIndex)
	{
		string text = Path.Combine(Application.dataPath, "Resources/Cache");
		if (!Directory.Exists(text))
		{
			return false;
		}
		string fileName = Path.Combine(text, "downloaded" + mapIndex + ".geometry");
		return Load(fileName, mapIndex);
	}

	public void Save(int mapIndex, int skyboxIndex)
	{
		string text = Path.Combine(Application.dataPath, "Resources/Cache");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
		}
		else
		{
			string fileName = Path.Combine(text, "downloaded" + mapIndex + ".geometry");
			Save(fileName, mapIndex, skyboxIndex);
        }
	}

	public void CacheDone(int mapIndex, int skyboxIndex)
	{
		PostLoadInit();
		if (mapIndex >= 0)
		{
			Save(mapIndex, skyboxIndex);
		}
		else
		{
			skybox = skyboxIndex;
		}
	}

	private void CalcMeshCodes()
	{
		foreach (KeyValuePair<int, BrickInst> item in dic)
		{
			Brick brick = BrickManager.Instance.GetBrick(item.Value.Template);
			if (brick == null)
			{
				item.Value.Code = 0;
			}
			else
			{
				item.Value.Code = CalcMeshAndShadowCode(item.Value.Seq, item.Value.PosX, item.Value.PosY, item.Value.PosZ);
			}
		}
	}

	public bool IsValidCoord(Vector3 pos)
	{
		if (Mathf.RoundToInt(pos.x) < 0 || Mathf.RoundToInt(pos.y) < 0 || Mathf.RoundToInt(pos.z) < 0)
		{
			return false;
		}
		byte x = (byte)Mathf.RoundToInt(pos.x);
		byte y = (byte)Mathf.RoundToInt(pos.y);
		byte z = (byte)Mathf.RoundToInt(pos.z);
		return IsValidCoord(x, y, z);
	}

	public bool ToCoord(Vector3 pos, ref byte x, ref byte y, ref byte z)
	{
		if (Mathf.RoundToInt(pos.x) < 0 || Mathf.RoundToInt(pos.y) < 0 || Mathf.RoundToInt(pos.z) < 0)
		{
			return false;
		}
		x = (byte)Mathf.RoundToInt(pos.x);
		y = (byte)Mathf.RoundToInt(pos.y);
		z = (byte)Mathf.RoundToInt(pos.z);
		return IsValidCoord(x, y, z);
	}

	public bool IsValidCoord(byte x, byte y, byte z)
	{
		if (x >= xMax || y >= yMax || z >= zMax)
		{
			return false;
		}
		return true;
	}

	public int GetSeqByCoord(Vector3 pos)
	{
		if (pos.x < 0f || pos.y < 0f || pos.z < 0f)
		{
			return -1;
		}
		byte x = (byte)Mathf.RoundToInt(pos.x);
		byte y = (byte)Mathf.RoundToInt(pos.y);
		byte z = (byte)Mathf.RoundToInt(pos.z);
		return GetSeqByCoord(x, y, z);
	}

	public int GetSeqByCoord(byte x, byte y, byte z)
	{
		return GetByCoord(x, y, z)?.Seq ?? (-1);
	}

	public BrickInst Get(int seq)
	{
		if (!dic.ContainsKey(seq))
		{
			return null;
		}
		return dic[seq];
	}

	public BrickInst GetByCoord(Vector3 pos)
	{
		if (pos.x < 0f || pos.y < 0f || pos.z < 0f)
		{
			return null;
		}
		byte x = (byte)Mathf.RoundToInt(pos.x);
		byte y = (byte)Mathf.RoundToInt(pos.y);
		byte z = (byte)Mathf.RoundToInt(pos.z);
		return GetByCoord(x, y, z);
	}

	public BrickInst GetByCoord(byte x, byte y, byte z)
	{
		if (!IsValidCoord(x, y, z))
		{
			return null;
		}
		return geometry[x, y, z];
	}

	public BrickInst GetByCoord(byte x, byte y, byte z, Brick.DIR meshDir)
	{
		MoveTo(meshDir, ref x, ref y, ref z);
		return GetByCoord(x, y, z);
	}

	public void MoveTo(Brick.DIR meshDir, ref byte x, ref byte y, ref byte z)
	{
		switch (meshDir)
		{
		case Brick.DIR.TOP:
			y++;
			break;
		case Brick.DIR.BOTTOM:
			y--;
			break;
		case Brick.DIR.FRONT:
			z++;
			break;
		case Brick.DIR.BACK:
			z--;
			break;
		case Brick.DIR.LEFT:
			x++;
			break;
		case Brick.DIR.RIGHT:
			x--;
			break;
		}
	}

	public int GetSeqByCoord(byte x, byte y, byte z, Brick.DIR meshDir)
	{
		MoveTo(meshDir, ref x, ref y, ref z);
		return GetSeqByCoord(x, y, z);
	}

	public ushort GetMeshCode(int seq)
	{
		if (!dic.ContainsKey(seq))
		{
			return 0;
		}
		return dic[seq].Code;
	}

	private ushort CalcMeshAndShadowCode(int seq, byte x, byte y, byte z)
	{
		ushort num = 0;
		for (Brick.DIR dIR = Brick.DIR.TOP; dIR <= Brick.DIR.RIGHT; dIR++)
		{
			if (NeedMesh(seq, x, y, z, dIR))
			{
				num = (ushort)(num | Brick.meshCodeSet[(int)dIR]);
				if (IsShade(x, y, z, dIR))
				{
					num = (ushort)(num | Brick.shadowCodeSet[(int)dIR]);
				}
			}
		}
		return num;
	}

	private bool IsShade(byte x, byte y, byte z, Brick.DIR meshDir)
	{
		switch (meshDir)
		{
		case Brick.DIR.BOTTOM:
			return false;
		case Brick.DIR.TOP:
			return IsShadeAbove(x, y, z);
		default:
			MoveTo(meshDir, ref x, ref y, ref z);
			return IsShadeAbove(x, y, z);
		}
	}

	private bool IsShadeAbove(byte x, byte y, byte z)
	{
		bool result = false;
		bool flag = true;
		while (flag)
		{
			MoveTo(Brick.DIR.TOP, ref x, ref y, ref z);
			if (!IsValidCoord(x, y, z))
			{
				flag = false;
			}
			else
			{
				BrickInst byCoord = GetByCoord(x, y, z);
				if (byCoord != null && BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
				{
					flag = false;
					result = true;
				}
			}
		}
		return result;
	}

	private bool NeedMesh(int seq, byte x, byte y, byte z, Brick.DIR meshDir)
	{
		if (!IsValidCoord(x, y, z))
		{
			return false;
		}
		if (!dic.ContainsKey(seq))
		{
			return false;
		}
		BrickInst byCoord = GetByCoord(x, y, z, meshDir);
		if (byCoord == null)
		{
			return true;
		}
		if (!BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
		{
			return true;
		}
		return false;
	}

	public BrickInst[] ToArray()
	{
		int num = 0;
		BrickInst[] array = new BrickInst[dic.Count];
		foreach (KeyValuePair<int, BrickInst> item in dic)
		{
			array[num++] = item.Value;
		}
		return array;
	}

	public void PostLoadInit()
	{
		isLoaded = true;
		InitRandomSpawners();
		CalcMeshCodes();
	}

	private void InitRandomSpawners()
	{
		randomSpawners.Clear();
		for (byte b = 0; b < xMax; b = (byte)(b + 1))
		{
			for (byte b2 = 0; b2 < zMax; b2 = (byte)(b2 + 1))
			{
				for (byte b3 = 0; b3 < yMax; b3 = (byte)(b3 + 1))
				{
					if (geometry[b, b3, b2] != null)
					{
						randomSpawners.Add(new Vector2((float)(int)b, (float)(int)b2));
						break;
					}
				}
			}
		}
	}

	public void Clear()
	{
		isLoaded = false;
		crc = 0;
		map = -1;
		skybox = -1;
		dic.Clear();
		for (byte b = 0; b < xMax; b = (byte)(b + 1))
		{
			for (byte b2 = 0; b2 < yMax; b2 = (byte)(b2 + 1))
			{
				for (byte b3 = 0; b3 < zMax; b3 = (byte)(b3 + 1))
				{
					geometry[b, b2, b3] = null;
				}
			}
		}
		redTeamSpawners.Clear();
		blueTeamSpawners.Clear();
		singleSpawners.Clear();
		limitedBricks.Clear();
		blueFlagSpawners.Clear();
		redFlagSpawners.Clear();
		FlagSpawners.Clear();
		BombSpawners.Clear();
		DefenseSpawners.Clear();
		scriptables.Clear();
		railSpawners.Clear();
		randomSpawners.Clear();
	}

	private void ShadeBelow(bool shade, byte x, byte y, byte z, ref List<int> morphes)
	{
		bool flag = true;
		while (flag)
		{
			MoveTo(Brick.DIR.BOTTOM, ref x, ref y, ref z);
			if (!IsValidCoord(x, y, z))
			{
				flag = false;
			}
			else
			{
				BrickInst byCoord = GetByCoord(x, y, z);
				if (byCoord != null && BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
				{
					if (shade)
					{
						if ((byCoord.Code & Brick.meshCodeSet[0]) > 0)
						{
							byCoord.Code |= Brick.shadowCodeSet[0];
							morphes.Add(byCoord.Seq);
							if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
							{
								CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
							}
						}
					}
					else
					{
						byCoord.Code &= Brick.shadowCodeReset[0];
						morphes.Add(byCoord.Seq);
						if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
						{
							CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
						}
					}
					flag = false;
				}
				else
				{
					for (Brick.DIR dIR = Brick.DIR.FRONT; dIR <= Brick.DIR.RIGHT; dIR++)
					{
						byCoord = GetByCoord(x, y, z, dIR);
						if (byCoord != null && BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
						{
							if (shade)
							{
								if ((byCoord.Code & Brick.meshCodeSet[(int)Brick.opposite[(int)dIR]]) > 0)
								{
									byCoord.Code |= Brick.shadowCodeSet[(int)Brick.opposite[(int)dIR]];
									morphes.Add(byCoord.Seq);
									if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
									{
										CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
									}
								}
							}
							else
							{
								byCoord.Code &= Brick.shadowCodeReset[(int)Brick.opposite[(int)dIR]];
								morphes.Add(byCoord.Seq);
								if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
								{
									CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
								}
							}
						}
					}
				}
			}
		}
	}

	public bool AddBrickInst(int seq, byte template, byte x, byte y, byte z, byte rot, ref List<int> morphes)
	{
		Brick brick = BrickManager.Instance.GetBrick(template);
		if (brick == null)
		{
			return false;
		}
		BrickInst brickInst = AddBrickInst(seq, template, x, y, z, 0, rot);
		if (brickInst == null)
		{
			return false;
		}
		brickInst.Code = 0;
		if (brick.meshOptimize)
		{
			brickInst.Code = CalcMeshAndShadowCode(seq, x, y, z);
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(brickInst.Seq, brickInst.Code);
			}
			for (Brick.DIR dIR = Brick.DIR.TOP; dIR <= Brick.DIR.RIGHT; dIR++)
			{
				BrickInst byCoord = GetByCoord(x, y, z, dIR);
				if (byCoord != null && BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
				{
					byCoord.Code &= Brick.meshCodeReset[(int)Brick.opposite[(int)dIR]];
					byCoord.Code &= Brick.shadowCodeReset[(int)Brick.opposite[(int)dIR]];
					morphes.Add(byCoord.Seq);
					if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
					{
						CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
					}
				}
			}
			if ((brickInst.Code & Brick.shadowCodeSet[0]) <= 0)
			{
				ShadeBelow(shade: true, x, y, z, ref morphes);
			}
		}
		return true;
	}

	public BrickInst[] GetAllScriptables()
	{
		return scriptables.ToArray();
	}

	private void RemovePortal(int team, byte x, byte y, byte z)
	{
		Vector3 rhs = new Vector3((float)(int)x, (float)(int)y, (float)(int)z);
		switch (team)
		{
		case 1:
			for (int j = 0; j < portalReds.Count; j++)
			{
				if (portalReds[j].position == rhs)
				{
					portalReds.RemoveAt(j);
					break;
				}
			}
			if (portalReds.Count > 0)
			{
				CheckRedPortalAlphaBlending(portalReds[0].sequence, add: false);
			}
			break;
		case -1:
			for (int k = 0; k < portalBlues.Count; k++)
			{
				if (portalBlues[k].position == rhs)
				{
					portalBlues.RemoveAt(k);
					break;
				}
			}
			if (portalBlues.Count > 0)
			{
				CheckBluePortalAlphaBlending(portalBlues[0].sequence, add: false);
			}
			break;
		default:
			for (int i = 0; i < portalNeutrals.Count; i++)
			{
				if (portalNeutrals[i].position == rhs)
				{
					portalNeutrals.RemoveAt(i);
					break;
				}
			}
			if (portalNeutrals.Count > 0)
			{
				CheckNeutralPortalAlphaBlending(portalNeutrals[0].sequence, add: false);
			}
			break;
		}
	}

	public bool DelBrickInst(int seq, ref List<int> morphes)
	{
		if (!dic.ContainsKey(seq))
		{
			return false;
		}
		Brick brick = BrickManager.Instance.GetBrick(dic[seq].Template);
		if (brick == null)
		{
			return false;
		}
		byte posX = dic[seq].PosX;
		byte posY = dic[seq].PosY;
		byte posZ = dic[seq].PosZ;
		BrickInst item = geometry[posX, posY, posZ];
		geometry[posX, posY, posZ] = null;
		if (!CheckRandomSpawnable(posX, posZ))
		{
			RemoveRandomSpawner(posX, posZ);
		}
		Brick.SPAWNER_TYPE spawnerType = brick.GetSpawnerType();
		if (spawnerType != Brick.SPAWNER_TYPE.NONE)
		{
			RemoveSpawner(spawnerType, posX, posY, posZ);
		}
		GameObject gameObject = GameObject.Find("Main");
		string empty = string.Empty;
		switch (spawnerType)
		{
		case Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER:
			if (null != gameObject)
			{
				empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_02"), StringMgr.Instance.Get("N90_RED_TEAM_SPAWNER"), BrickManager.Instance.CountLimitedBrick(brick.GetIndex()) - 1, brick.maxInstancePerMap);
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
			}
			break;
		case Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER:
			if (null != gameObject)
			{
				empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_02"), StringMgr.Instance.Get("N90_BLUE_TEAM_SPAWNER"), BrickManager.Instance.CountLimitedBrick(brick.GetIndex()) - 1, brick.maxInstancePerMap);
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
			}
			break;
		case Brick.SPAWNER_TYPE.SINGLE_SPAWNER:
			if (null != gameObject)
			{
				empty = string.Format(StringMgr.Instance.Get("NOTICE_SPONER_02"), StringMgr.Instance.Get("N90_SINGLE_SPAWNER"), BrickManager.Instance.CountLimitedBrick(brick.GetIndex()) - 1, brick.maxInstancePerMap);
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, empty));
			}
			break;
		}
		if (dic[seq].Template == 163)
		{
			RemovePortal(1, posX, posY, posZ);
		}
		else if (dic[seq].Template == 164)
		{
			RemovePortal(-1, posX, posY, posZ);
		}
		else if (dic[seq].Template == 178)
		{
			RemovePortal(0, posX, posY, posZ);
		}
		if (brick.function == Brick.FUNCTION.SCRIPT)
		{
			scriptables.Remove(item);
		}
		if (brick.maxInstancePerMap > 0)
		{
			DecreaseLimitedBrick(dic[seq].Template);
		}
		dic.Remove(seq);
		if (brick.meshOptimize)
		{
			bool flag = IsShadeAbove(posX, posY, posZ);
			for (Brick.DIR dIR = Brick.DIR.TOP; dIR <= Brick.DIR.RIGHT; dIR++)
			{
				BrickInst byCoord = GetByCoord(posX, posY, posZ, dIR);
				if (byCoord != null && BrickManager.Instance.GetBrick(byCoord.Template).meshOptimize)
				{
					byCoord.Code |= Brick.meshCodeSet[(int)Brick.opposite[(int)dIR]];
					if (flag && dIR != 0)
					{
						byCoord.Code |= Brick.shadowCodeSet[(int)Brick.opposite[(int)dIR]];
					}
					morphes.Add(byCoord.Seq);
					if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
					{
						CSNetManager.Instance.Sock.SendCS_MORPH_BRICK_REQ(byCoord.Seq, byCoord.Code);
					}
				}
			}
			if (!flag)
			{
				ShadeBelow(shade: false, posX, posY, posZ, ref morphes);
			}
		}
		return true;
	}

	public BrickInst AddBrickInst(int seq, byte template, byte x, byte y, byte z, ushort meshCode, byte rot)
	{
		if (dic.ContainsKey(seq))
		{
			Debug.LogError("Fail to add brick inst for duplicated key " + seq + " template " + template + " x " + x + " y " + y + " z " + z);
			return null;
		}
		if (!IsValidCoord(x, y, z))
		{
			Debug.LogError("Fail to add brick inst for out of range Seq(" + seq + ") template (" + template + ") position(" + x + ", " + y + ", " + z + ")");
			return null;
		}
		Brick brick = BrickManager.Instance.GetBrick(template);
		if (brick == null)
		{
			Debug.LogError("Fail to add brick inst for no template ");
			return null;
		}
		if (template == 136)
		{
			GlobalVars.Instance.vDefenseEnd = new Vector3((float)(int)x, (float)(int)y, (float)(int)z);
		}
		if (template == 191)
		{
			BrickManager.Instance.AddDoorTDic(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z));
		}
		switch (template)
		{
		case 163:
			portalReds.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case 164:
			portalBlues.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case 178:
			portalNeutrals.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		}
		if (template == 196)
		{
			railSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
		}
		BrickInst[,,] array = geometry;
		BrickInst brickInst = new BrickInst(seq, template, x, y, z, meshCode, rot);
		array[x, y, z] = brickInst;
		dic.Add(seq, geometry[x, y, z]);
		Brick.SPAWNER_TYPE spawnerType = brick.GetSpawnerType();
		if (spawnerType != Brick.SPAWNER_TYPE.NONE)
		{
			AddSpawner(seq, spawnerType, x, y, z, rot);
		}
		if (brick.function == Brick.FUNCTION.SCRIPT)
		{
			scriptables.Add(geometry[x, y, z]);
			geometry[x, y, z].UpdateScript(seq.ToString(), enableOnAwake: false, visibleOnAwake: false, string.Empty);
		}
		if (brick.maxInstancePerMap > 0)
		{
			IncreaseLimitedBrick(template);
		}
		return geometry[x, y, z];
	}

	public SpawnerDesc GetPortalPos(bool red, Vector3 p)
	{
		SpawnerDesc result = new SpawnerDesc(0, new Vector3(p.x, p.y, p.z), 0);
		if (red)
		{
			int num = -1;
			for (int i = 0; i < portalReds.Count; i++)
			{
				float num2 = Vector3.Distance(p, portalReds[i].position);
				if (num2 < 1.2f)
				{
					num = i;
					BrickManager.Instance.portalFX(portalReds[i].sequence);
					P2PManager.Instance.SendPEER_PORTAL(portalReds[i].sequence);
				}
			}
			if (num >= 0)
			{
				IsPortalMove = true;
				if (portalReds.Count > 1)
				{
					int index = (num == 0) ? 1 : 0;
					GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
					BrickManager.Instance.portalFX(portalReds[index].sequence);
					P2PManager.Instance.SendPEER_PORTAL(portalReds[index].sequence);
					return portalReds[index];
				}
			}
		}
		else
		{
			int num3 = -1;
			for (int j = 0; j < portalBlues.Count; j++)
			{
				float num4 = Vector3.Distance(p, portalBlues[j].position);
				if (num4 < 1.2f)
				{
					num3 = j;
					BrickManager.Instance.portalFX(portalBlues[j].sequence);
					P2PManager.Instance.SendPEER_PORTAL(portalBlues[j].sequence);
				}
			}
			if (num3 >= 0)
			{
				IsPortalMove = true;
				if (portalBlues.Count > 1)
				{
					int index2 = (num3 == 0) ? 1 : 0;
					GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
					BrickManager.Instance.portalFX(portalBlues[index2].sequence);
					P2PManager.Instance.SendPEER_PORTAL(portalBlues[index2].sequence);
					return portalBlues[index2];
				}
			}
		}
		int num5 = -1;
		for (int k = 0; k < portalNeutrals.Count; k++)
		{
			float num6 = Vector3.Distance(p, portalNeutrals[k].position);
			if (num6 < 1.2f)
			{
				num5 = k;
				BrickManager.Instance.portalFX(portalNeutrals[k].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalNeutrals[k].sequence);
			}
		}
		if (num5 >= 0)
		{
			IsPortalMove = true;
			if (portalNeutrals.Count > 1)
			{
				int index3 = (num5 == 0) ? 1 : 0;
				GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
				BrickManager.Instance.portalFX(portalNeutrals[index3].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalNeutrals[index3].sequence);
				return portalNeutrals[index3];
			}
		}
		return result;
	}

	public SpawnerDesc GetPortalAllPos(Vector3 p)
	{
		SpawnerDesc result = new SpawnerDesc(0, new Vector3(p.x, p.y, p.z), 0);
		int num = -1;
		for (int i = 0; i < portalReds.Count; i++)
		{
			float num2 = Vector3.Distance(p, portalReds[i].position);
			if (num2 < 1.2f)
			{
				num = i;
				BrickManager.Instance.portalFX(portalReds[i].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalReds[i].sequence);
			}
		}
		if (num >= 0)
		{
			IsPortalMove = true;
			if (portalReds.Count > 1)
			{
				int index = (num == 0) ? 1 : 0;
				GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
				BrickManager.Instance.portalFX(portalReds[index].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalReds[index].sequence);
				return portalReds[index];
			}
		}
		int num3 = -1;
		for (int j = 0; j < portalBlues.Count; j++)
		{
			float num4 = Vector3.Distance(p, portalBlues[j].position);
			if (num4 < 1.2f)
			{
				num3 = j;
				BrickManager.Instance.portalFX(portalBlues[j].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalBlues[j].sequence);
			}
		}
		if (num3 >= 0)
		{
			IsPortalMove = true;
			if (portalBlues.Count > 1)
			{
				int index2 = (num3 == 0) ? 1 : 0;
				GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
				BrickManager.Instance.portalFX(portalBlues[index2].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalBlues[index2].sequence);
				return portalBlues[index2];
			}
		}
		int num5 = -1;
		for (int k = 0; k < portalNeutrals.Count; k++)
		{
			float num6 = Vector3.Distance(p, portalNeutrals[k].position);
			if (num6 < 1.2f)
			{
				num5 = k;
				BrickManager.Instance.portalFX(portalNeutrals[k].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalNeutrals[k].sequence);
			}
		}
		if (num5 >= 0)
		{
			IsPortalMove = true;
			if (portalNeutrals.Count > 1)
			{
				int index3 = (num5 == 0) ? 1 : 0;
				GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 1f, 0.5f);
				BrickManager.Instance.portalFX(portalNeutrals[index3].sequence);
				P2PManager.Instance.SendPEER_PORTAL(portalNeutrals[index3].sequence);
				return portalNeutrals[index3];
			}
		}
		return result;
	}

	private void IncreaseLimitedBrick(byte template)
	{
		if (limitedBricks.ContainsKey(template))
		{
			Dictionary<byte, int> dictionary;
			Dictionary<byte, int> dictionary2 = dictionary = limitedBricks;
			byte key;
			byte key2 = key = template;
			int num = dictionary[key];
			dictionary2[key2] = num + 1;
		}
		else
		{
			limitedBricks.Add(template, 1);
		}
	}

	private void DecreaseLimitedBrick(byte template)
	{
		if (limitedBricks.ContainsKey(template))
		{
			Dictionary<byte, int> dictionary;
			Dictionary<byte, int> dictionary2 = dictionary = limitedBricks;
			byte key;
			byte key2 = key = template;
			int num = dictionary[key];
			dictionary2[key2] = num - 1;
		}
	}

	public int CountLimitedBrick(byte template)
	{
		if (limitedBricks.ContainsKey(template))
		{
			return limitedBricks[template];
		}
		return 0;
	}

	private void AddSpawner(int seq, Brick.SPAWNER_TYPE spawnerType, byte x, byte y, byte z, byte rot)
	{
		switch (spawnerType)
		{
		case Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER:
			blueTeamSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER:
			redTeamSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.SINGLE_SPAWNER:
			singleSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER:
			blueFlagSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER:
			redFlagSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.FLAG_SPAWNER:
			FlagSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.BOMB_SPAWNER:
			BombSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		case Brick.SPAWNER_TYPE.DEFENCE_SPAWNER:
			DefenseSpawners.Add(new SpawnerDesc(seq, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), rot));
			break;
		}
	}

	public BrickInst[] ToTeamSpawnersArray()
	{
		List<BrickInst> list = new List<BrickInst>();
		for (int i = 0; i < blueTeamSpawners.Count; i++)
		{
			if (dic.ContainsKey(blueTeamSpawners[i].sequence))
			{
				list.Add(dic[blueTeamSpawners[i].sequence]);
			}
		}
		for (int j = 0; j < redTeamSpawners.Count; j++)
		{
			if (dic.ContainsKey(redTeamSpawners[j].sequence))
			{
				list.Add(dic[redTeamSpawners[j].sequence]);
			}
		}
		return list.ToArray();
	}

	private bool CheckRandomSpawnable(byte x, byte z)
	{
		if (x >= xMax || z >= zMax)
		{
			return false;
		}
		for (byte b = 0; b < yMax; b = (byte)(b + 1))
		{
			if (geometry[x, b, z] != null)
			{
				return true;
			}
		}
		return false;
	}

	private void RemoveRandomSpawner(byte x, byte z)
	{
		Vector2 rhs = new Vector2((float)(int)x, (float)(int)z);
		int num = 0;
		while (true)
		{
			if (num >= randomSpawners.Count)
			{
				return;
			}
			Vector2 lhs = randomSpawners[num];
			if (lhs == rhs)
			{
				break;
			}
			num++;
		}
		randomSpawners.RemoveAt(num);
	}

	private void RemoveSpawner(Brick.SPAWNER_TYPE spawnerType, byte x, byte y, byte z)
	{
		Vector3 rhs = new Vector3((float)(int)x, (float)(int)y, (float)(int)z);
		switch (spawnerType)
		{
		case Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER:
		{
			int num5 = 0;
			while (true)
			{
				if (num5 >= blueTeamSpawners.Count)
				{
					return;
				}
				if (blueTeamSpawners[num5].position == rhs)
				{
					break;
				}
				num5++;
			}
			blueTeamSpawners.RemoveAt(num5);
			break;
		}
		case Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER:
		{
			int num2 = 0;
			while (true)
			{
				if (num2 >= redTeamSpawners.Count)
				{
					return;
				}
				if (redTeamSpawners[num2].position == rhs)
				{
					break;
				}
				num2++;
			}
			redTeamSpawners.RemoveAt(num2);
			break;
		}
		case Brick.SPAWNER_TYPE.SINGLE_SPAWNER:
		{
			int num6 = 0;
			while (true)
			{
				if (num6 >= singleSpawners.Count)
				{
					return;
				}
				if (singleSpawners[num6].position == rhs)
				{
					break;
				}
				num6++;
			}
			singleSpawners.RemoveAt(num6);
			break;
		}
		case Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER:
		{
			int num8 = 0;
			while (true)
			{
				if (num8 >= redFlagSpawners.Count)
				{
					return;
				}
				if (redFlagSpawners[num8].position == rhs)
				{
					break;
				}
				num8++;
			}
			redFlagSpawners.RemoveAt(num8);
			break;
		}
		case Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER:
		{
			int num3 = 0;
			while (true)
			{
				if (num3 >= blueFlagSpawners.Count)
				{
					return;
				}
				if (blueFlagSpawners[num3].position == rhs)
				{
					break;
				}
				num3++;
			}
			blueFlagSpawners.RemoveAt(num3);
			break;
		}
		case Brick.SPAWNER_TYPE.FLAG_SPAWNER:
		{
			int num7 = 0;
			while (true)
			{
				if (num7 >= FlagSpawners.Count)
				{
					return;
				}
				if (FlagSpawners[num7].position == rhs)
				{
					break;
				}
				num7++;
			}
			FlagSpawners.RemoveAt(num7);
			break;
		}
		case Brick.SPAWNER_TYPE.BOMB_SPAWNER:
		{
			int num4 = 0;
			while (true)
			{
				if (num4 >= BombSpawners.Count)
				{
					return;
				}
				if (BombSpawners[num4].position == rhs)
				{
					break;
				}
				num4++;
			}
			BombSpawners.RemoveAt(num4);
			break;
		}
		case Brick.SPAWNER_TYPE.DEFENCE_SPAWNER:
		{
			int num = 0;
			while (true)
			{
				if (num >= DefenseSpawners.Count)
				{
					return;
				}
				if (DefenseSpawners[num].position == rhs)
				{
					break;
				}
				num++;
			}
			DefenseSpawners.RemoveAt(num);
			break;
		}
		}
	}

	public SpawnerDesc GetSpawner(Brick.SPAWNER_TYPE spawnerType, int ticket)
	{
		switch (spawnerType)
		{
		case Brick.SPAWNER_TYPE.BLUE_TEAM_SPAWNER:
			if (blueTeamSpawners.Count > 0)
			{
				int index8 = ticket % blueTeamSpawners.Count;
				return blueTeamSpawners[index8];
			}
			break;
		case Brick.SPAWNER_TYPE.RED_TEAM_SPAWNER:
			if (redTeamSpawners.Count > 0)
			{
				int index4 = ticket % redTeamSpawners.Count;
				return redTeamSpawners[index4];
			}
			break;
		case Brick.SPAWNER_TYPE.SINGLE_SPAWNER:
			if (singleSpawners.Count > 0)
			{
				int index6 = ticket % singleSpawners.Count;
				return singleSpawners[index6];
			}
			break;
		case Brick.SPAWNER_TYPE.RED_FLAG_SPAWNER:
			if (redFlagSpawners.Count > 0)
			{
				int index2 = ticket % redFlagSpawners.Count;
				return redFlagSpawners[index2];
			}
			break;
		case Brick.SPAWNER_TYPE.BLUE_FLAG_SPAWNER:
			if (blueFlagSpawners.Count > 0)
			{
				int index7 = ticket % blueFlagSpawners.Count;
				return blueFlagSpawners[index7];
			}
			break;
		case Brick.SPAWNER_TYPE.FLAG_SPAWNER:
			if (FlagSpawners.Count > 0)
			{
				int index5 = ticket % FlagSpawners.Count;
				return FlagSpawners[index5];
			}
			break;
		case Brick.SPAWNER_TYPE.BOMB_SPAWNER:
			if (BombSpawners.Count > 0)
			{
				int index3 = ticket % BombSpawners.Count;
				return BombSpawners[index3];
			}
			break;
		case Brick.SPAWNER_TYPE.DEFENCE_SPAWNER:
			if (DefenseSpawners.Count > 0)
			{
				int index = ticket % DefenseSpawners.Count;
				return DefenseSpawners[index];
			}
			break;
		}
		return null;
	}

	public Vector2 GetRandomSpawnPos()
	{
		int index;
		while (true)
		{
			if (randomSpawners.Count <= 0)
			{
				return new Vector2(50f, 50f);
			}
			index = UnityEngine.Random.Range(0, randomSpawners.Count);
			if (IsValidSpwanPos(index))
			{
				break;
			}
			randomSpawners.RemoveAt(index);
		}
		return randomSpawners[index];
	}

	private bool IsValidSpwanPos(int index)
	{
		Vector2 vector = randomSpawners[index];
		Vector3 origin = new Vector3(vector.x, 200f, vector.y);
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Ray ray = new Ray(origin, Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit _, 1000f, layerMask))
		{
			return true;
		}
		return false;
	}

	private bool CheckOutputPortalMessage(int slot, bool red)
	{
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND)
		{
			return true;
		}
		if (slot < 8 && red)
		{
			return true;
		}
		if (slot >= 8 && !red)
		{
			return true;
		}
		return false;
	}

	public void CheckRedPortalAlphaBlending(int seq, bool add)
	{
		if (portalReds.Count == 1)
		{
			GameObject brickObject = BrickManager.Instance.GetBrickObject(seq);
			if (!(brickObject == null))
			{
				if (CheckOutputPortalMessage(MyInfoManager.Instance.Slot, red: true))
				{
					GlobalVars.Instance.OutoutPortalMessage = true;
				}
				Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					if (add)
					{
						componentInChildren.Play("potalFX_closed");
					}
					else
					{
						componentInChildren.Play("potalFX_close");
					}
				}
			}
		}
		else if (portalReds.Count > 1)
		{
			for (int i = 0; i < portalReds.Count; i++)
			{
				GameObject brickObject2 = BrickManager.Instance.GetBrickObject(portalReds[i].sequence);
				if (brickObject2 != null)
				{
					Animation componentInChildren2 = brickObject2.GetComponentInChildren<Animation>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play("potalFX_Open");
					}
				}
			}
		}
	}

	public void CheckBluePortalAlphaBlending(int seq, bool add)
	{
		if (portalBlues.Count == 1)
		{
			GameObject brickObject = BrickManager.Instance.GetBrickObject(seq);
			if (!(brickObject == null))
			{
				if (CheckOutputPortalMessage(MyInfoManager.Instance.Slot, red: false))
				{
					GlobalVars.Instance.OutoutPortalMessage = true;
				}
				Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					if (add)
					{
						componentInChildren.Play("potalFX_closed");
					}
					else
					{
						componentInChildren.Play("potalFX_close");
					}
				}
			}
		}
		else if (portalBlues.Count > 1)
		{
			for (int i = 0; i < portalBlues.Count; i++)
			{
				GameObject brickObject2 = BrickManager.Instance.GetBrickObject(portalBlues[i].sequence);
				if (brickObject2 != null)
				{
					Animation componentInChildren2 = brickObject2.GetComponentInChildren<Animation>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play("potalFX_Open");
					}
				}
			}
		}
	}

	public void CheckNeutralPortalAlphaBlending(int seq, bool add)
	{
		if (portalNeutrals.Count == 1)
		{
			GameObject brickObject = BrickManager.Instance.GetBrickObject(seq);
			if (!(brickObject == null))
			{
				if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.BND)
				{
					GlobalVars.Instance.OutoutPortalMessage = true;
				}
				Animation componentInChildren = brickObject.GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					if (add)
					{
						componentInChildren.Play("potalFX_closed");
					}
					else
					{
						componentInChildren.Play("potalFX_close");
					}
				}
			}
		}
		else if (portalNeutrals.Count > 1)
		{
			for (int i = 0; i < portalNeutrals.Count; i++)
			{
				GameObject brickObject2 = BrickManager.Instance.GetBrickObject(portalNeutrals[i].sequence);
				if (brickObject2 != null)
				{
					Animation componentInChildren2 = brickObject2.GetComponentInChildren<Animation>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play("potalFX_Open");
					}
				}
			}
		}
	}

	public SpawnerDesc[] GetRailSpawners()
	{
		if (railSpawners.Count == 0)
		{
			return null;
		}
		return railSpawners.ToArray();
	}
}
