using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveItemManager : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth;

	private static ActiveItemManager _instance;

	public GameObject activeItemObject;

	public AudioClip sndItemGet;

	public GameObject itemGetEffect;

	public ActiveItemData[] activeItems;

	private UIImageSizeChange itemIconEffect = new UIImageSizeChange();

	private float elapsedTime;

	public float START_ITEM_TIME = 5f;

	public int MAX_ITEM_COUNT = 10;

	public float ITEM_CREATE_TIME = 10f;

	private float createTime;

	private Dictionary<int, GameObject> dicActiveItem;

	private int lastSeq;

	public Vector2[] itemIconEffectTime;

	public static ActiveItemManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(ActiveItemManager)) as ActiveItemManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ActiveItemManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		elapsedTime = 0f;
		createTime = 0f;
		lastSeq = 0;
		for (int i = 0; i < itemIconEffectTime.Length; i++)
		{
			itemIconEffect.AddStep(itemIconEffectTime[i].x, itemIconEffectTime[i].y);
		}
		itemIconEffect.position = new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2 - 200));
		for (int j = 0; j < activeItems.Length; j++)
		{
			activeItems[j].SetItemType(j);
		}
	}

	private void Start()
	{
		dicActiveItem = new Dictionary<int, GameObject>();
	}

	private void Update()
	{
		itemIconEffect.Update();
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			elapsedTime += Time.deltaTime;
			if (dicActiveItem.Count < MAX_ITEM_COUNT)
			{
				createTime += Time.deltaTime;
				if (elapsedTime > 1f && createTime > ITEM_CREATE_TIME)
				{
					createTime = 0f;
					CreateActiveItem();
				}
			}
		}
	}

	private bool CreateActiveItem()
	{
		if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			return false;
		}
		Vector3 randomSpawnPos = BrickManager.Instance.GetRandomSpawnPos();
		int layerMask = (1 << LayerMask.NameToLayer("Chunk")) | (1 << LayerMask.NameToLayer("Brick"));
		Ray ray = new Ray(randomSpawnPos, Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, layerMask))
		{
			Vector3 point = hitInfo.point;
			randomSpawnPos.y = point.y + 0.5f;
		}
		else
		{
			Debug.Log("No Active Item Position");
		}
		while (!IsValidPosition(randomSpawnPos))
		{
			randomSpawnPos.y += 1f;
		}
		int seq = CreateActiveItem(0, randomSpawnPos);
		P2PManager.Instance.SendPEER_CREATE_ACTIVE_ITEM(seq, randomSpawnPos);
		return false;
	}

	public int CreateActiveItem(int seq, Vector3 pos)
	{
		if (dicActiveItem.ContainsKey(seq))
		{
			return lastSeq;
		}
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			lastSeq++;
		}
		else
		{
			lastSeq = seq;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate((UnityEngine.Object)activeItemObject, pos, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		ActiveItemTrigger component = gameObject.GetComponent<ActiveItemTrigger>();
		component.seq = lastSeq;
		dicActiveItem.Add(lastSeq, gameObject);
		return lastSeq;
	}

	private bool IsValidPosition(Vector3 pos)
	{
		foreach (KeyValuePair<int, GameObject> item in dicActiveItem)
		{
			if (Vector3.Distance(pos, item.Value.transform.position) < 0.1f)
			{
				return false;
			}
		}
		return true;
	}

	public void EatItem(int seq, int userSeq)
	{
		if (dicActiveItem.ContainsKey(seq))
		{
			if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
			{
				P2PManager.Instance.SendPEER_EAT_ACTIVE_ITEM_REQ(seq, MyInfoManager.Instance.Seq);
			}
			else
			{
				int choiceItemType = GetChoiceItemType();
				DeleteItem(seq, userSeq, choiceItemType);
				P2PManager.Instance.SendPEER_EAT_ACTIVE_ITEM_ACK(seq, userSeq, choiceItemType);
			}
		}
	}

	public void DeleteItem(int seq, int userSeq, int useItemType)
	{
		if (dicActiveItem != null && dicActiveItem.Count != 0 && dicActiveItem.ContainsKey(seq))
		{
			Vector3 position = dicActiveItem[seq].transform.position;
			UnityEngine.Object.Instantiate((UnityEngine.Object)itemGetEffect, position, Quaternion.Euler(0f, 0f, 0f));
			UnityEngine.Object.Destroy(dicActiveItem[seq]);
			dicActiveItem.Remove(seq);
			if (userSeq == MyInfoManager.Instance.Seq)
			{
				GameObject gameObject = GameObject.Find("Me");
				if (null != gameObject)
				{
					AudioSource component = gameObject.GetComponent<AudioSource>();
					if (component != null)
					{
						component.PlayOneShot(sndItemGet);
					}
				}
				if (BungeeTools.Instance.AddActiveItem(activeItems[useItemType]))
				{
					ItemGetIconEffect(useItemType);
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get(activeItems[useItemType].itemText));
				}
			}
		}
	}

	public void UseItem(int userSeq, int useItemType)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate((UnityEngine.Object)activeItems[useItemType].itemPrefap, Vector3.zero, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		ActiveItemBase component = gameObject.GetComponent<ActiveItemBase>();
		if (component != null)
		{
			component.UseItem(userSeq);
		}
	}

	private int GetChoiceItemType()
	{
		int num = 0;
		for (int i = 0; i < activeItems.Length; i++)
		{
			num += activeItems[i].chance;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		num = 0;
		for (int j = 0; j < activeItems.Length; j++)
		{
			num += activeItems[j].chance;
			if (num2 < num)
			{
				return j;
			}
		}
		return 0;
	}

	private void ItemGetIconEffect(int itemType)
	{
		itemIconEffect.texImage = activeItems[itemType].icon;
		itemIconEffect.Reset();
	}

	public void OnGUI()
	{
		GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
		if (null != gUISkin)
		{
			GUI.skin = gUISkin;
			GUI.depth = (int)guiDepth;
			itemIconEffect.Draw();
		}
	}

	public Dictionary<int, GameObject> GetActiveItemDictionary()
	{
		return dicActiveItem;
	}
}
