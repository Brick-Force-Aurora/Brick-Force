using System.Collections.Generic;
using UnityEngine;

public class BrickManManager : MonoBehaviour
{
	public Dictionary<int, BrickManDesc> dicDescriptor;

	public Dictionary<int, GameObject> dicBrickMan;

	public string markGMName;

	public GameObject brickMan;

	public RenderTexture[] overlayArray;

	private Queue<RenderTexture> freeOverlayQ;

	public Vector3[] invisiblePosition;

	private Queue<Vector3> freeInvisiblePositionQ;

	public int[] ids;

	public int haveFlagSeq = -1;

	public bool bSendTcpCheckOnce;

	public bool bSuccessFlagCapture;

	public Vector3 vFlag = Vector3.zero;

	private static BrickManManager _instance;

	public static BrickManManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BrickManManager)) as BrickManManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BrickManManager Instance");
				}
			}
			return _instance;
		}
	}

	public void setId(int slot, int id)
	{
		if (ids.Length <= 0)
		{
			ids[slot] = id;
		}
	}

	public void InitFlagVars()
	{
		haveFlagSeq = -1;
		bSendTcpCheckOnce = false;
		bSuccessFlagCapture = false;
		vFlag = Vector3.zero;
	}

	private int SortList(BrickManDesc p1, BrickManDesc p2)
	{
		if (p1.Score < p2.Score)
		{
			return 1;
		}
		if (p1.Score > p2.Score)
		{
			return -1;
		}
		return 0;
	}

	public List<KeyValuePair<int, BrickManDesc>> ToSortedList()
	{
		List<KeyValuePair<int, BrickManDesc>> list = new List<KeyValuePair<int, BrickManDesc>>(dicDescriptor);
		list.Sort((KeyValuePair<int, BrickManDesc> firstPair, KeyValuePair<int, BrickManDesc> nextPair) => firstPair.Value.Compare(nextPair.Value));
		return list;
	}

	public List<KeyValuePair<int, BrickManDesc>> ToEscapeSortedList()
	{
		List<KeyValuePair<int, BrickManDesc>> list = new List<KeyValuePair<int, BrickManDesc>>(dicDescriptor);
		list.Sort((KeyValuePair<int, BrickManDesc> firstPair, KeyValuePair<int, BrickManDesc> nextPair) => firstPair.Value.EscapeCompare(nextPair.Value));
		return list;
	}

	public GameObject[] ToGameObjectArray()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (KeyValuePair<int, GameObject> item in dicBrickMan)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public Dictionary<int, GameObject> GetDicBrickMan()
	{
		return dicBrickMan;
	}

	public BrickManDesc[] ToDescriptorArray()
	{
		List<BrickManDesc> list = new List<BrickManDesc>();
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public BrickManDesc[] ToDescriptorArrayWhoTookTooLongToWait()
	{
		List<BrickManDesc> list = new List<BrickManDesc>();
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			if (item.Value.IsTooLong4Init)
			{
				list.Add(item.Value);
			}
		}
		return list.ToArray();
	}

	public BrickManDesc[] ToDescriptorArrayBySlot()
	{
		List<BrickManDesc> list = new List<BrickManDesc>();
		for (int i = 0; i < 16; i++)
		{
			list.Add(null);
		}
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			if (0 <= item.Value.Slot && item.Value.Slot < 16)
			{
				list[item.Value.Slot] = item.Value;
			}
		}
		return list.ToArray();
	}

	public BrickManDesc GetDesc(int seq)
	{
		if (dicDescriptor.ContainsKey(seq))
		{
			return dicDescriptor[seq];
		}
		return null;
	}

	public int GetDescCount()
	{
		return dicDescriptor.Count;
	}

	public GameObject Get(int seq)
	{
		if (!dicBrickMan.ContainsKey(seq))
		{
			return null;
		}
		return dicBrickMan[seq];
	}

	private BrickManDesc[] ToPlayingDescArray(bool friendlyOnly)
	{
		List<BrickManDesc> list = new List<BrickManDesc>();
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			if (item.Value.Status == 4 && (!friendlyOnly || !item.Value.IsHostile()) && !item.Value.IsHidePlayer)
			{
				list.Add(item.Value);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list.ToArray();
	}

	public GameObject GetRandomPlayer(bool friendlyOnly)
	{
		BrickManDesc[] array = ToPlayingDescArray(friendlyOnly);
		if (array == null)
		{
			return null;
		}
		return Get(array[Random.Range(0, array.Length)].Seq);
	}

	public GameObject GetNextPlayer(GameObject obj, bool excludingMe, bool friendlyOnly)
	{
		PlayerProperty component = obj.GetComponent<PlayerProperty>();
		if (null != component)
		{
			BrickManDesc[] array = ToPlayingDescArray(friendlyOnly);
			if (array != null && array.Length > 0)
			{
				bool flag = false;
				List<BrickManDesc> list = new List<BrickManDesc>();
				List<BrickManDesc> list2 = new List<BrickManDesc>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Seq == component.Desc.Seq)
					{
						flag = true;
					}
					else if (flag)
					{
						list2.Add(array[i]);
					}
					else
					{
						list.Add(array[i]);
					}
				}
				if (list2.Count > 0)
				{
					return Get(list2[0].Seq);
				}
				if (list.Count > 0)
				{
					return Get(list[0].Seq);
				}
			}
		}
		if (excludingMe)
		{
			return null;
		}
		return obj;
	}

	public void ResetGameStuff()
	{
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			item.Value.ResetGameStuff();
		}
	}

	public void ClearBrickManEtc()
	{
		ClearBrickManDictionary();
		FreeAllOverlays();
		FreeAllInvisiblePositions();
	}

	private void ClearBrickManDictionary()
	{
		dicBrickMan.Clear();
	}

	public GameObject AddBrickMan(BrickManDesc desc)
	{
		Vector3 position = new Vector3(0f, 11000f, 0f);
		if (freeInvisiblePositionQ.Count <= 0)
		{
			Debug.LogError("No more free Invisible Position ");
		}
		else
		{
			position = freeInvisiblePositionQ.Dequeue();
		}
		GameObject gameObject = Object.Instantiate((Object)brickMan, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		if (null == gameObject)
		{
			Debug.LogError("Fail to instantiate a brick man ");
			return null;
		}
		PlayerProperty component = gameObject.GetComponent<PlayerProperty>();
		if (null == component)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError("New brickman doesnt have PlayerProperty ");
			return null;
		}
		component.Desc = desc;
		component.InvisiblePosition = position;
		TPController component2 = gameObject.GetComponent<TPController>();
		if (null == component2)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError("New brickman doesnt have ThirdPersonController");
			return null;
		}
		Camera camera = null;
		Camera[] componentsInChildren = gameObject.GetComponentsInChildren<Camera>();
		if (componentsInChildren != null)
		{
			string value = "Near";
			if (Application.loadedLevelName == "Result4Defense")
			{
				value = "Far";
			}
			int num = 0;
			while (camera == null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].name.Contains(value))
				{
					camera = componentsInChildren[num];
				}
				num++;
			}
		}
		if (null == camera)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError(" New brickman doesnt have Overlay camera ");
			return null;
		}
		if (freeOverlayQ.Count <= 0)
		{
			Debug.LogError("No more freeOverlay ");
		}
		camera.enabled = true;
		camera.targetTexture = freeOverlayQ.Dequeue();
		Weapon.isInitialize = true;
		LookCoordinator component3 = gameObject.GetComponent<LookCoordinator>();
		if (null == component3)
		{
			Object.DestroyImmediate(gameObject);
			Debug.LogError("New brickman doesnt have LookCoordinator");
			return null;
		}
		component3.Init(mirror: false);
		for (int i = 0; i < desc.Equipment.Length; i++)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(desc.Equipment[i]);
			if (tItem != null)
			{
				string itemCode = desc.Equipment[i];
				if (tItem.type == TItem.TYPE.WEAPON)
				{
					TWeapon tWeapon = (TWeapon)tItem;
					int num2 = 0;
					while (desc.WpnChg != null && num2 < desc.WpnChg.Length)
					{
						TItem tItem2 = TItemManager.Instance.Get<TItem>(desc.WpnChg[num2]);
						if (tItem2 != null && tItem2.type == TItem.TYPE.WEAPON)
						{
							TWeapon tWeapon2 = (TWeapon)tItem2;
							if (tWeapon.slot == tWeapon2.slot)
							{
								itemCode = desc.WpnChg[num2];
							}
						}
						num2++;
					}
					int num3 = 0;
					while (desc.DrpItm != null && num3 < desc.DrpItm.Length)
					{
						TItem tItem3 = TItemManager.Instance.Get<TItem>(desc.DrpItm[num3]);
						if (tItem3 != null && tItem3.type == TItem.TYPE.WEAPON)
						{
							TWeapon tWeapon3 = (TWeapon)tItem3;
							if (tWeapon.slot == tWeapon3.slot)
							{
								itemCode = desc.DrpItm[num3];
							}
						}
						num3++;
					}
				}
				component3.Equip(itemCode);
			}
		}
		component3.ChangeWeapon(RoomManager.Instance.DefaultWeaponType);
		dicBrickMan.Add(desc.Seq, gameObject);
		Weapon.isInitialize = false;
		return gameObject;
	}

	public void Reinit()
	{
		foreach (KeyValuePair<int, GameObject> item in dicBrickMan)
		{
			LookCoordinator component = item.Value.GetComponent<LookCoordinator>();
			if (null != component)
			{
				component.ChangeWeapon(RoomManager.Instance.DefaultWeaponType);
			}
		}
	}

	private bool NeedGameObjectOnEnter()
	{
		return Application.loadedLevelName != "Bootstrap" && Application.loadedLevelName != "Lobby" && Application.loadedLevelName != "Login" && !Application.loadedLevelName.Contains("Result");
	}

	public void OnEnter(int seq, string nickname, string[] equip, int status, int xp, int clan, string clanName, int clanMark, int rank, string[] wpnChg, string[] drpItm)
	{
		if (!dicDescriptor.ContainsKey(seq))
		{
			BrickManDesc brickManDesc = new BrickManDesc(seq, nickname, equip, status, xp, clan, clanName, clanMark, rank, wpnChg, drpItm);
			dicDescriptor.Add(seq, brickManDesc);
			if (NeedGameObjectOnEnter())
			{
				AddBrickMan(brickManDesc);
			}
		}
	}

	public void Remove(int seq)
	{
		GameObject gameObject = Get(seq);
		if (gameObject != null)
		{
			LetMeKnowABrickManIsBeingRemoved(gameObject);
			Camera[] componentsInChildren = gameObject.GetComponentsInChildren<Camera>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (null != componentsInChildren[i])
					{
						componentsInChildren[i].enabled = false;
						if (null != componentsInChildren[i].targetTexture)
						{
							freeOverlayQ.Enqueue(componentsInChildren[i].targetTexture);
						}
					}
				}
			}
			TPController component = gameObject.GetComponent<TPController>();
			if (component != null)
			{
				component.destroyCongratulation();
			}
			PlayerProperty componentInChildren = gameObject.GetComponentInChildren<PlayerProperty>();
			if (null != componentInChildren)
			{
				freeInvisiblePositionQ.Enqueue(componentInChildren.InvisiblePosition);
			}
			InvincibleArmor component2 = gameObject.GetComponent<InvincibleArmor>();
			if (null != component2)
			{
				component2.Destroy();
			}
			Object.Destroy(gameObject);
			dicBrickMan.Remove(seq);
		}
		if (dicDescriptor.ContainsKey(seq))
		{
			dicDescriptor.Remove(seq);
		}
	}

	private void LetMeKnowABrickManIsBeingRemoved(GameObject obj)
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnRemoveBrickMan", obj);
		}
	}

	public void Clear()
	{
		foreach (KeyValuePair<int, GameObject> item in dicBrickMan)
		{
			Object.DestroyImmediate(item.Value);
		}
		dicDescriptor.Clear();
		ClearBrickManEtc();
	}

	public void OnStart()
	{
		ClearBrickManEtc();
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			AddBrickMan(item.Value);
		}
	}

	private void FreeAllOverlays()
	{
		freeOverlayQ.Clear();
		for (int i = 0; i < overlayArray.Length; i++)
		{
			freeOverlayQ.Enqueue(overlayArray[i]);
		}
	}

	private void FreeAllInvisiblePositions()
	{
		freeInvisiblePositionQ.Clear();
		for (int i = 0; i < invisiblePosition.Length; i++)
		{
			freeInvisiblePositionQ.Enqueue(invisiblePosition[i]);
		}
	}

	public void ClearAllInvisibility()
	{
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			BrickManDesc value = item.Value;
			GameObject gameObject = Instance.Get(item.Key);
			if (gameObject != null && value != null)
			{
				TPController component = gameObject.GetComponent<TPController>();
				if (null != component && !component.IsLocallyControlled)
				{
					value.IsInvisibilityOn = false;
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		dicDescriptor = new Dictionary<int, BrickManDesc>();
		dicBrickMan = new Dictionary<int, GameObject>();
		freeOverlayQ = new Queue<RenderTexture>();
		freeInvisiblePositionQ = new Queue<Vector3>();
		FreeAllOverlays();
		FreeAllInvisiblePositions();
		Object.DontDestroyOnLoad(this);
	}

	public int GetPlayingPlayerCount()
	{
		int num = 0;
		foreach (KeyValuePair<int, BrickManDesc> item in dicDescriptor)
		{
			if (item.Value.Status == 4)
			{
				num++;
			}
		}
		return num;
	}
}
