using System;
using UnityEngine;

[Serializable]
public class Brick
{
	public enum DIR
	{
		TOP,
		BOTTOM,
		FRONT,
		BACK,
		LEFT,
		RIGHT
	}

	public enum FUNCTION
	{
		NONE,
		LADDER,
		CANNON,
		SCRIPT
	}

	public enum CATEGORY
	{
		GENERAL,
		COLORBOX,
		ACCESSORY,
		FUNCTIONAL
	}

	public enum SPAWNER_TYPE
	{
		BLUE_TEAM_SPAWNER,
		RED_TEAM_SPAWNER,
		SINGLE_SPAWNER,
		BLUE_FLAG_SPAWNER,
		RED_FLAG_SPAWNER,
		FLAG_SPAWNER,
		BOMB_SPAWNER,
		DEFENCE_SPAWNER,
		NONE
	}

	public enum REPLACE_CHECK
	{
		OK,
		ERR
	}

	public string brickName;

	public int seq;

	public string brickAlias;

	public string brickComment;

	public GameObject brick;

	public GameObject[] bright;

	public GameObject[] dark;

	public Material material;

	public Texture icon;

	public bool destructible;

	public bool directionable;

	public FUNCTION function;

	public bool meshOptimize = true;

	public bool chunkOptimize = true;

	public bool chunkOptimizeOnMatch = true;

	public bool onlyTutor;

	public BuildOption.SEASON season = BuildOption.SEASON.BRICK_STAR;

	public int horz = 1;

	public int vert = 1;

	public Texture2D[] bulletMarks;

	public GameObject[] bulletImpacts;

	public AudioClip[] stepSound;

	public AudioClip addSound;

	public AudioClip delSound;

	public int maxChildrenPerChunk = 255;

	public int maxInstancePerMap = -1;

	public GameObject debris;

	public CATEGORY category;

	public int hitPoint = 1000;

	public bool disable;

	public bool replace = true;

	public bool bnd = true;

	public string[] ticket;

	public Room.ROOM_TYPE gameModeDependent = Room.ROOM_TYPE.NONE;

	public static string[] needFunc = new string[2]
	{
		string.Empty,
		"brickstar_builder"
	};

	public static ushort[] shadowCodeSet = new ushort[6]
	{
		256,
		512,
		1024,
		2048,
		4096,
		8192
	};

	public static ushort[] shadowCodeReset = new ushort[6];

	public static ushort[] meshCodeSet = new ushort[6]
	{
		1,
		2,
		4,
		8,
		16,
		32
	};

	public static ushort[] meshCodeReset = new ushort[6];

	public static DIR[] opposite = new DIR[6]
	{
		DIR.BOTTOM,
		DIR.TOP,
		DIR.BACK,
		DIR.FRONT,
		DIR.RIGHT,
		DIR.LEFT
	};

	public byte index
	{
		get
		{
			return (byte)seq;
		}
		set
		{
			index = value;
		}
	}

	public GameObject[] Bright
	{
		get
		{
			return bright;
		}
		set
		{
			bright = value;
		}
	}

	public GameObject[] Dark
	{
		get
		{
			return dark;
		}
		set
		{
			dark = value;
		}
	}

	public Material Mat
	{
		get
		{
			return material;
		}
		set
		{
			material = value;
		}
	}

	public Texture Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
		}
	}

	public bool climbable => function == FUNCTION.LADDER;

	public bool shootable => function == FUNCTION.CANNON;

	public bool IsUnit => horz == 1 && vert == 1;

	public bool IsTutor()
	{
		if (!Application.loadedLevelName.Contains("Tutor"))
		{
			return true;
		}
		if (onlyTutor)
		{
			return true;
		}
		return false;
	}

	public bool UseAbleSeason()
	{
		if (BuildOption.Instance.Props.season >= season)
		{
			return true;
		}
		return false;
	}

	public bool UseAbleGameMode()
	{
		switch (gameModeDependent)
		{
		case Room.ROOM_TYPE.TEAM_MATCH:
			return BuildOption.Instance.Props.teamMatchMode;
		case Room.ROOM_TYPE.INDIVIDUAL:
			return BuildOption.Instance.Props.individualMatchMode;
		case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
			return BuildOption.Instance.Props.ctfMatchMode;
		case Room.ROOM_TYPE.EXPLOSION:
			return BuildOption.Instance.Props.explosionMatchMode;
		case Room.ROOM_TYPE.MISSION:
			return BuildOption.Instance.Props.defenseMatchMode;
		case Room.ROOM_TYPE.BND:
			return BuildOption.Instance.Props.bndMatchMode;
		case Room.ROOM_TYPE.BUNGEE:
			return BuildOption.Instance.Props.bungeeMode;
		case Room.ROOM_TYPE.ESCAPE:
			return BuildOption.Instance.Props.escapeMode;
		default:
			return true;
		}
	}

	public bool IsEnable(Room.ROOM_TYPE roomType)
	{
		if (disable)
		{
			return false;
		}
		if (roomType == Room.ROOM_TYPE.BND)
		{
			return bnd;
		}
		return true;
	}

	public REPLACE_CHECK CheckReplace()
	{
		if (!IsUnit || directionable || !replace)
		{
			return REPLACE_CHECK.ERR;
		}
		return REPLACE_CHECK.OK;
	}

	public AudioClip GetStepSound()
	{
		if (stepSound.Length <= 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, stepSound.Length);
		return stepSound[num];
	}

	public Texture2D GetBulletMark()
	{
		if (bulletMarks.Length <= 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, bulletMarks.Length);
		return bulletMarks[num];
	}

	public GameObject GetBulletImpact()
	{
		if (bulletImpacts.Length <= 0)
		{
			return null;
		}
		int num = UnityEngine.Random.Range(0, bulletImpacts.Length);
		return bulletImpacts[num];
	}

	public static Vector3 ToBrickCoord(Vector3 normal, Vector3 point)
	{
		point += -0.01f * normal;
		return new Vector3((float)Mathf.RoundToInt(point.x), (float)Mathf.RoundToInt(point.y), (float)Mathf.RoundToInt(point.z));
	}

	public bool NeedChunkOptimize()
	{
		if (chunkOptimize)
		{
			return true;
		}
		return chunkOptimizeOnMatch && RoomManager.Instance.IsBattleScene;
	}

	public SPAWNER_TYPE GetSpawnerType()
	{
		if (brickName.Contains("blue_team_spawner"))
		{
			return SPAWNER_TYPE.BLUE_TEAM_SPAWNER;
		}
		if (brickName.Contains("red_team_spawner"))
		{
			return SPAWNER_TYPE.RED_TEAM_SPAWNER;
		}
		if (brickName.Contains("single_spawner"))
		{
			return SPAWNER_TYPE.SINGLE_SPAWNER;
		}
		if (brickName.Contains("red_flag_spawner"))
		{
			return SPAWNER_TYPE.RED_FLAG_SPAWNER;
		}
		if (brickName.Contains("blue_flag_spawner"))
		{
			return SPAWNER_TYPE.BLUE_FLAG_SPAWNER;
		}
		if (brickName.Contains("flag_spawner"))
		{
			return SPAWNER_TYPE.FLAG_SPAWNER;
		}
		if (brickName.Contains("bomb_spawner"))
		{
			return SPAWNER_TYPE.BOMB_SPAWNER;
		}
		if (brickName.Contains("df_spawner_start"))
		{
			return SPAWNER_TYPE.DEFENCE_SPAWNER;
		}
		return SPAWNER_TYPE.NONE;
	}

	public GameObject Instantiate(ushort code, Vector3 position, byte rot)
	{
		return Instantiate(code, position, Rot.ToQuaternion(rot));
	}

	public GameObject Instantiate(ushort code, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate((UnityEngine.Object)brick, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		if (meshOptimize)
		{
			for (DIR dIR = DIR.TOP; dIR <= DIR.RIGHT; dIR++)
			{
				if ((code & meshCodeSet[(int)dIR]) > 0)
				{
					GameObject gameObject2 = bright[(int)dIR];
					if ((code & shadowCodeSet[(int)dIR]) > 0)
					{
						gameObject2 = dark[(int)dIR];
					}
					GameObject gameObject3 = UnityEngine.Object.Instantiate((UnityEngine.Object)gameObject2, position, gameObject2.transform.rotation) as GameObject;
					gameObject3.transform.parent = gameObject.transform;
					gameObject.transform.rotation = rotation;
				}
			}
		}
		else
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate((UnityEngine.Object)bright[0], position, bright[0].transform.rotation) as GameObject;
			if (NeedChunkOptimize())
			{
				Collider[] componentsInChildren = gameObject4.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i]);
				}
				Recursively.SetLayer(gameObject4.transform, LayerMask.NameToLayer("Default"));
			}
			gameObject4.transform.parent = gameObject.transform;
			gameObject.transform.rotation = rotation;
		}
		return gameObject;
	}

	public byte GetIndex()
	{
		return index;
	}
}
