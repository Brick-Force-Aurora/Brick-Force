using System.Collections.Generic;
using UnityEngine;

public class MonManager : MonoBehaviour
{
	private Dictionary<int, MonDesc> dicDescriptor;

	private Dictionary<int, GameObject> dicMon;

	public GameObject bee01;

	public GameObject bee02;

	public GameObject bomber01;

	public GameObject bomber02;

	public GameObject champ01;

	public GameObject champ02;

	public GameObject destroyer;

	public GameObject ghost;

	public GameObject intruder01;

	public GameObject intruder02;

	public Texture2D iconBee2;

	public Texture2D iconBomber;

	public Texture2D iconChampion;

	public Texture2D iconIntruder;

	public GameObject m_expFX;

	public GameObject m_expFX2x;

	public GameObject m_expFX4x;

	public GameObject m_smokeFX;

	public GameObject m_scratchFX;

	public GameObject m_spwanFX;

	public GameObject healEff;

	public GameObject healerEff;

	private MonController monContoller;

	private float flightHeight = 0.5f;

	private int monGenID;

	private bool bGened150R;

	private bool bGened100R;

	private bool bGened50R;

	private bool bGened20R;

	private bool bGened150B;

	private bool bGened100B;

	private bool bGened50B;

	private bool bGened20B;

	public Queue<BossUiInfo> BossUiQ;

	private static MonManager _instance;

	public float FlightHeight
	{
		get
		{
			return flightHeight;
		}
		set
		{
			flightHeight = value;
		}
	}

	public static MonManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(MonManager)) as MonManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the MonManager Instance");
				}
			}
			return _instance;
		}
	}

	public GameObject[] ToGameObjectArray()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (KeyValuePair<int, GameObject> item in dicMon)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public MonDesc[] ToDescriptorArray()
	{
		List<MonDesc> list = new List<MonDesc>();
		foreach (KeyValuePair<int, MonDesc> item in dicDescriptor)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public MonDesc GetDesc(int seq)
	{
		if (dicDescriptor.ContainsKey(seq))
		{
			return dicDescriptor[seq];
		}
		return null;
	}

	public GameObject Get(int seq)
	{
		if (!dicMon.ContainsKey(seq))
		{
			return null;
		}
		return dicMon[seq];
	}

	public void ResetGameStuff()
	{
		foreach (KeyValuePair<int, MonDesc> item in dicDescriptor)
		{
			item.Value.ResetGameStuff();
		}
	}

	public void DisableAll()
	{
		foreach (KeyValuePair<int, GameObject> item in dicMon)
		{
			MonProperty component = item.Value.GetComponent<MonProperty>();
			component.Desc.Xp = 0;
			SkinnedMeshRenderer componentInChildren = item.Value.GetComponentInChildren<SkinnedMeshRenderer>();
			componentInChildren.enabled = false;
		}
	}

	public void ClearMonEtc()
	{
		ClearMonDictionary();
	}

	private void ClearMonDictionary()
	{
		dicMon.Clear();
	}

	public MonAI GetAIClass(int Seq, int tblID)
	{
		GameObject gameObject = Get(Seq);
		if (gameObject == null)
		{
			return null;
		}
		MonAI result = null;
		switch (tblID)
		{
		case 0:
			result = gameObject.GetComponent<aiBee>();
			break;
		case 1:
			result = gameObject.GetComponent<aiBee2>();
			break;
		case 2:
			result = gameObject.GetComponent<aiIntruder>();
			break;
		case 3:
			result = gameObject.GetComponent<aiBomber>();
			break;
		case 4:
			result = gameObject.GetComponent<aiChampion>();
			break;
		default:
			Debug.LogError("GetAIClass Not Found: " + tblID);
			break;
		}
		return result;
	}

	public MonAI GetAIClass(GameObject m, int tblID)
	{
		MonAI result = null;
		switch (tblID)
		{
		case 0:
			result = m.GetComponent<aiBee>();
			break;
		case 1:
			result = m.GetComponent<aiBee2>();
			break;
		case 2:
			result = m.GetComponent<aiIntruder>();
			break;
		case 3:
			result = m.GetComponent<aiBomber>();
			break;
		case 4:
			result = m.GetComponent<aiChampion>();
			break;
		default:
			Debug.LogError("GetAIClass Not Found: " + tblID);
			break;
		}
		return result;
	}

	private int GetMonTblID(string gen_mon_name)
	{
		string[] array = new string[5]
		{
			"Bee01",
			"Bee02",
			"Intruder01",
			"Bomber01",
			"Champion01"
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (gen_mon_name == array[i])
			{
				return i;
			}
		}
		Debug.LogError("not found mon name: " + gen_mon_name);
		return -1;
	}

	private int Birth(int genID)
	{
		int num = 0;
		string empty = string.Empty;
		if (genID % 2 == 0)
		{
			if (DefenseManager.Instance.RedPoint >= 150 && !bGened150R)
			{
				empty = "Champion01";
				bGened150R = true;
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
				{
					DefenseManager.Instance.RedPoint -= 150;
					CSNetManager.Instance.Sock.SendCS_MISSION_POINT_REQ(DefenseManager.Instance.RedPoint, DefenseManager.Instance.BluePoint);
				}
				BossUiInfo bossUiInfo = new BossUiInfo();
				bossUiInfo.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo.tex2d = iconChampion;
				num = GetMonTblID(empty);
				bossUiInfo.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
				bossUiInfo.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo);
			}
			else if (DefenseManager.Instance.RedPoint >= 100 && !bGened100R)
			{
				empty = "Bomber01";
				bGened100R = true;
				BossUiInfo bossUiInfo2 = new BossUiInfo();
				bossUiInfo2.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo2.tex2d = iconBomber;
				num = GetMonTblID(empty);
				bossUiInfo2.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
				bossUiInfo2.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo2);
			}
			else if (DefenseManager.Instance.RedPoint >= 50 && !bGened50R)
			{
				empty = "Intruder01";
				bGened50R = true;
				BossUiInfo bossUiInfo3 = new BossUiInfo();
				bossUiInfo3.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo3.tex2d = iconIntruder;
				num = GetMonTblID(empty);
				bossUiInfo3.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
				bossUiInfo3.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo3);
			}
			else if (DefenseManager.Instance.RedPoint >= 20 && !bGened20R)
			{
				empty = "Bee02";
				bGened20R = true;
				BossUiInfo bossUiInfo4 = new BossUiInfo();
				bossUiInfo4.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo4.tex2d = iconBee2;
				num = GetMonTblID(empty);
				bossUiInfo4.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
				bossUiInfo4.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo4);
			}
			else
			{
				empty = "Bee01";
			}
		}
		else if (DefenseManager.Instance.BluePoint >= 150 && !bGened150B)
		{
			empty = "Champion01";
			bGened150B = true;
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				DefenseManager.Instance.BluePoint -= 150;
				CSNetManager.Instance.Sock.SendCS_MISSION_POINT_REQ(DefenseManager.Instance.RedPoint, DefenseManager.Instance.BluePoint);
			}
			BossUiInfo bossUiInfo5 = new BossUiInfo();
			bossUiInfo5.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo5.tex2d = iconChampion;
			num = GetMonTblID(empty);
			bossUiInfo5.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
			bossUiInfo5.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo5);
		}
		else if (DefenseManager.Instance.BluePoint >= 100 && !bGened100B)
		{
			empty = "Bomber01";
			bGened100B = true;
			BossUiInfo bossUiInfo6 = new BossUiInfo();
			bossUiInfo6.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo6.tex2d = iconBomber;
			num = GetMonTblID(empty);
			bossUiInfo6.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
			bossUiInfo6.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo6);
		}
		else if (DefenseManager.Instance.BluePoint >= 50 && !bGened50B)
		{
			empty = "Intruder01";
			bGened50B = true;
			BossUiInfo bossUiInfo7 = new BossUiInfo();
			bossUiInfo7.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo7.tex2d = iconIntruder;
			num = GetMonTblID(empty);
			bossUiInfo7.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
			bossUiInfo7.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo7);
		}
		else if (DefenseManager.Instance.BluePoint >= 20 && !bGened20B)
		{
			empty = "Bee02";
			bGened20B = true;
			BossUiInfo bossUiInfo8 = new BossUiInfo();
			bossUiInfo8.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo8.tex2d = iconBee2;
			num = GetMonTblID(empty);
			bossUiInfo8.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(num).str);
			bossUiInfo8.dmg = DefenseManager.Instance.GetMonTable(num).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo8);
		}
		else
		{
			empty = "Bee01";
		}
		return num;
	}

	private void checkBossP2P(int genID, int tblID)
	{
		if (genID % 2 == 0)
		{
			if (DefenseManager.Instance.RedPoint >= 150 && !bGened150R)
			{
				bGened150R = true;
				BossUiInfo bossUiInfo = new BossUiInfo();
				bossUiInfo.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo.tex2d = iconChampion;
				bossUiInfo.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
				bossUiInfo.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo);
			}
			else if (DefenseManager.Instance.RedPoint >= 100 && !bGened100R)
			{
				bGened100R = true;
				BossUiInfo bossUiInfo2 = new BossUiInfo();
				bossUiInfo2.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo2.tex2d = iconBomber;
				bossUiInfo2.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
				bossUiInfo2.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo2);
			}
			else if (DefenseManager.Instance.RedPoint >= 50 && !bGened50R)
			{
				bGened50R = true;
				BossUiInfo bossUiInfo3 = new BossUiInfo();
				bossUiInfo3.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo3.tex2d = iconIntruder;
				bossUiInfo3.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
				bossUiInfo3.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo3);
			}
			else if (DefenseManager.Instance.RedPoint >= 20 && !bGened20R)
			{
				bGened20R = true;
				BossUiInfo bossUiInfo4 = new BossUiInfo();
				bossUiInfo4.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("RED"));
				bossUiInfo4.tex2d = iconBee2;
				bossUiInfo4.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
				bossUiInfo4.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
				BossUiQ.Enqueue(bossUiInfo4);
			}
		}
		else if (DefenseManager.Instance.BluePoint >= 150 && !bGened150B)
		{
			bGened150B = true;
			BossUiInfo bossUiInfo5 = new BossUiInfo();
			bossUiInfo5.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo5.tex2d = iconChampion;
			bossUiInfo5.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
			bossUiInfo5.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo5);
		}
		else if (DefenseManager.Instance.BluePoint >= 100 && !bGened100B)
		{
			bGened100B = true;
			BossUiInfo bossUiInfo6 = new BossUiInfo();
			bossUiInfo6.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo6.tex2d = iconBomber;
			bossUiInfo6.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
			bossUiInfo6.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo6);
		}
		else if (DefenseManager.Instance.BluePoint >= 50 && !bGened50B)
		{
			bGened50B = true;
			BossUiInfo bossUiInfo7 = new BossUiInfo();
			bossUiInfo7.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo7.tex2d = iconIntruder;
			bossUiInfo7.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
			bossUiInfo7.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo7);
		}
		else if (DefenseManager.Instance.BluePoint >= 20 && !bGened20B)
		{
			bGened20B = true;
			BossUiInfo bossUiInfo8 = new BossUiInfo();
			bossUiInfo8.msg = string.Format(StringMgr.Instance.Get("MON_OUT_MSG"), StringMgr.Instance.Get("BLUE"));
			bossUiInfo8.tex2d = iconBee2;
			bossUiInfo8.name = StringMgr.Instance.Get(DefenseManager.Instance.GetMonTable(tblID).str);
			bossUiInfo8.dmg = DefenseManager.Instance.GetMonTable(tblID).toCoreDmg;
			BossUiQ.Enqueue(bossUiInfo8);
		}
	}

	public void MonGenerateP2P(int tblID, int typeID, int seq, float x, float y, float z, float vx, float vy, float vz)
	{
		if (MyInfoManager.Instance.Status == 4 && !dicDescriptor.ContainsKey(seq))
		{
			monGenID = seq;
			checkBossP2P(monGenID, tblID);
			int hP = DefenseManager.Instance.GetMonTable(tblID).HP;
			int dp = DefenseManager.Instance.GetMonTable(tblID).Dp;
			Vector3 startPosition = new Vector3(x, y, z);
			OnEnter(tblID, typeID, monGenID, hP, bP2P: true, dp);
			GameObject gameObject = Get(monGenID);
			if (gameObject != null)
			{
				MonAI aIClass = Instance.GetAIClass(gameObject, tblID);
				if (aIClass == null)
				{
					Debug.LogError("tblID error: " + tblID);
				}
				else
				{
					aIClass.StartPosition = startPosition;
				}
			}
		}
	}

	public void BossUnVisibleAll(bool bRed)
	{
		if (bRed)
		{
			bGened150R = false;
			bGened100R = false;
			bGened50R = false;
			bGened20R = false;
		}
		else
		{
			bGened150B = false;
			bGened100B = false;
			bGened50B = false;
			bGened20B = false;
		}
	}

	public void MonGenerateNew()
	{
		int num = Birth(monGenID);
		int type = 0;
		int hP = DefenseManager.Instance.GetMonTable(num).HP;
		int dp = DefenseManager.Instance.GetMonTable(num).Dp;
		OnEnter(num, type, monGenID, hP, bP2P: false, dp);
		monGenID++;
	}

	public GameObject AddMon(MonDesc desc)
	{
		SpawnerDesc spawner = BrickManager.Instance.GetSpawner(Brick.SPAWNER_TYPE.DEFENCE_SPAWNER, 0);
		if (spawner == null)
		{
			return null;
		}
		Vector3 position = spawner.position;
		GameObject gameObject = null;
		if (desc.tblID == 0)
		{
			gameObject = (Object.Instantiate((Object)bee01, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
		else if (desc.tblID == 1)
		{
			gameObject = (Object.Instantiate((Object)bee02, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
		else if (desc.tblID == 2)
		{
			gameObject = (Object.Instantiate((Object)intruder01, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
		else if (desc.tblID == 3)
		{
			gameObject = (Object.Instantiate((Object)bomber01, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
		else if (desc.tblID == 4)
		{
			gameObject = (Object.Instantiate((Object)champ01, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject);
		}
		if (null == gameObject)
		{
			Debug.LogError("Fail to instantiate a monster: " + desc.tblID);
			return null;
		}
		MonProperty component = gameObject.GetComponent<MonProperty>();
		if (null == component)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError("New monster doesnt have MonProperty: " + desc.tblID);
			return null;
		}
		component.Desc = desc;
		component.Desc.coreToDmg = DefenseManager.Instance.GetMonTable(desc.tblID).toCoreDmg;
		component.Desc.InitLog();
		component.InvisiblePosition = position;
		MonAI aIClass = Instance.GetAIClass(gameObject, desc.tblID);
		if (null == aIClass)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError("New monster doesnt have monAI: " + desc.tblID);
			return null;
		}
		aIClass.StartPosition = position;
		aIClass.moveSpeed = DefenseManager.Instance.GetMonTable(desc.typeID).MoveSpeed;
		aIClass.MonType = (MonAI.MON_TYPE)desc.typeID;
		dicMon.Add(desc.Seq, gameObject);
		aIClass.changeTexture();
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			P2PManager instance = P2PManager.Instance;
			int tblID = desc.tblID;
			int typeID = desc.typeID;
			int seq = desc.Seq;
			float x = position.x;
			float y = position.y;
			float z = position.z;
			Vector3 forward = gameObject.transform.forward;
			float x2 = forward.x;
			Vector3 forward2 = gameObject.transform.forward;
			float y2 = forward2.y;
			Vector3 forward3 = gameObject.transform.forward;
			instance.SendPEER_MON_GEN(tblID, typeID, seq, x, y, z, x2, y2, forward3.z);
		}
		return gameObject;
	}

	public void SendMonAll()
	{
		if (dicDescriptor != null && dicDescriptor.Count != 0)
		{
			foreach (KeyValuePair<int, MonDesc> item in dicDescriptor)
			{
				MonDesc value = item.Value;
				if (value != null)
				{
					GameObject gameObject = Get(value.Seq);
					if (gameObject != null)
					{
						Transform transform = gameObject.transform;
						P2PManager instance = P2PManager.Instance;
						int tblID = value.tblID;
						int typeID = value.typeID;
						int seq = value.Seq;
						Vector3 position = transform.position;
						float x = position.x;
						Vector3 position2 = transform.position;
						float y = position2.y;
						Vector3 position3 = transform.position;
						float z = position3.z;
						Vector3 forward = transform.forward;
						float x2 = forward.x;
						Vector3 forward2 = transform.forward;
						float y2 = forward2.y;
						Vector3 forward3 = transform.forward;
						instance.SendPEER_MON_GEN(tblID, typeID, seq, x, y, z, x2, y2, forward3.z);
					}
				}
			}
		}
	}

	private bool NeedGameObjectOnEnter()
	{
		return Application.loadedLevelName != "Bootstrap" && Application.loadedLevelName != "Lobby" && Application.loadedLevelName != "Login" && !Application.loadedLevelName.Contains("Result");
	}

	public void OnEnter(int tbl, int type, int seq, int xp, bool bP2P, int dp)
	{
		if (!dicDescriptor.ContainsKey(seq))
		{
			MonDesc monDesc = new MonDesc(tbl, type, seq, xp, bP2P, dp);
			dicDescriptor.Add(seq, monDesc);
			if (NeedGameObjectOnEnter())
			{
				AddMon(monDesc);
			}
		}
	}

	public void Remove(int seq)
	{
		GameObject gameObject = Get(seq);
		if (gameObject != null)
		{
			Object.DestroyImmediate(gameObject);
			dicMon.Remove(seq);
			dicDescriptor.Remove(seq);
		}
	}

	public void Clear()
	{
		foreach (KeyValuePair<int, GameObject> item in dicMon)
		{
			Object.DestroyImmediate(item.Value);
		}
		dicDescriptor.Clear();
		ClearMonEtc();
		monGenID = 0;
		DefenseManager.Instance.RedPoint = 0;
		DefenseManager.Instance.BluePoint = 0;
		BossUnVisibleAll(bRed: true);
		BossUnVisibleAll(bRed: false);
	}

	private void DividePoint(int seq, int Hp, int totPoint)
	{
		MonDesc desc = GetDesc(seq);
		if (desc != null)
		{
			foreach (KeyValuePair<int, int> item in desc.dicDamageLog)
			{
				float num = (float)item.Value / (float)Hp;
				if (MyInfoManager.Instance.Seq != item.Key)
				{
					P2PManager.Instance.SendPEER_MON_ADDPOINT(item.Key, (int)((float)totPoint * num));
				}
				else
				{
					Defense component = GameObject.Find("Main").GetComponent<Defense>();
					if (component != null)
					{
						component.AddDefensePoint(bRed: true, (int)((float)totPoint * num));
					}
				}
			}
		}
	}

	public void Hit(int seq, int damage, float rigidFactor, int weaponBy, Vector3 ammopos, Vector3 ammodir, int curammo)
	{
		if (damage > 0)
		{
			MonDesc desc = GetDesc(seq);
			if (desc != null)
			{
				desc.rigidity = rigidFactor;
				desc.Xp -= damage;
				desc.LogAttacker(MyInfoManager.Instance.Seq, damage);
				desc.IsHit = true;
				if (!GlobalVars.Instance.applyNewP2P)
				{
					P2PManager.Instance.SendPEER_MON_HIT(MyInfoManager.Instance.Seq, desc.Seq, damage, rigidFactor, ammopos, ammodir, curammo);
				}
				if (desc.Xp <= 0)
				{
					P2PManager.Instance.SendPEER_MON_DIE(desc.Seq, arrived: false);
					CSNetManager.Instance.Sock.SendCS_INFLICTED_DAMAGE_REQ(desc.dicInflictedDamage);
					CSNetManager.Instance.Sock.SendCS_KILL_LOG_REQ(0, MyInfoManager.Instance.Seq, 1, desc.Seq, weaponBy, -1, -1, 0, desc.dicDamageLog);
					desc.clearLog();
				}
			}
		}
	}

	public void AiReset()
	{
		foreach (KeyValuePair<int, GameObject> item in dicMon)
		{
			MonAI component = item.Value.GetComponent<MonAI>();
			if (component != null)
			{
				component.ResetMove();
			}
			else
			{
				Debug.LogError("ai == null");
			}
		}
	}

	public void OnStart()
	{
		ClearMonEtc();
		foreach (KeyValuePair<int, MonDesc> item in dicDescriptor)
		{
			AddMon(item.Value);
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		dicDescriptor = new Dictionary<int, MonDesc>();
		dicMon = new Dictionary<int, GameObject>();
		BossUiQ = new Queue<BossUiInfo>();
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			foreach (KeyValuePair<int, MonDesc> item in dicDescriptor)
			{
				item.Value.ReportInflictedDamage();
			}
		}
	}
}
