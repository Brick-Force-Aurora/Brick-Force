using UnityEngine;

public class PremiumItemManager : MonoBehaviour
{
	public string[] premiumItems;

	public string[] pcbangItems;

	private Item[] FakePremiumItems;

	private Item[] FakePcbangItems;

	private long StartItemIndex = 9223372036854775800L;

	private static PremiumItemManager _instance;

	public static PremiumItemManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(PremiumItemManager)) as PremiumItemManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the PremiumItemManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	public void SetPremiumItems(string[] items)
	{
		premiumItems = items;
		ResetPremiumItems();
	}

	public void SetPCBangItems(string[] items)
	{
		pcbangItems = items;
		ResetPcbangItems();
	}

	public void ResetPremiumItems()
	{
		if (FakePremiumItems != null)
		{
			for (int i = 0; i < FakePremiumItems.Length; i++)
			{
				MyInfoManager.Instance.Erase(FakePremiumItems[i].Seq);
				FakePremiumItems[i] = null;
			}
		}
		if (MyInfoManager.Instance.IsPremiumAccount())
		{
			FakePremiumItems = new Item[0];
		}
		else
		{
			FakePremiumItems = new Item[premiumItems.Length];
			for (int j = 0; j < premiumItems.Length; j++)
			{
				string code = premiumItems[j];
				MyInfoManager.Instance.SetItem(--StartItemIndex, code, Item.USAGE.DELETED, 100000, 1, 100000);
				FakePremiumItems[j] = MyInfoManager.Instance.GetItemBySequence(StartItemIndex);
			}
		}
	}

	public void ResetPcbangItems()
	{
		if (FakePcbangItems != null)
		{
			for (int i = 0; i < FakePcbangItems.Length; i++)
			{
				MyInfoManager.Instance.Erase(FakePcbangItems[i].Seq);
				FakePcbangItems[i] = null;
			}
		}
		if (BuffManager.Instance.IsPCBangBuff())
		{
			FakePcbangItems = new Item[0];
			if (BuildOption.Instance.Props.usePCBangItem)
			{
				PCBangBenefit[] array = new PCBangBenefit[pcbangItems.Length];
				for (int j = 0; j < pcbangItems.Length; j++)
				{
					string text = pcbangItems[j];
					TItem tItem = TItemManager.Instance.Get<TItem>(text);
					if (tItem == null)
					{
						Debug.LogError("Fail to find item template for " + text);
					}
					else
					{
						array[j] = new PCBangBenefit();
						array[j].texImage = tItem.CurIcon();
						array[j].textKey = tItem.name;
					}
				}
				((PCBangDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.PC_BANG_NOTICE))?.ResetBenerfitList(array);
			}
		}
		else
		{
			FakePcbangItems = new Item[pcbangItems.Length];
			for (int k = 0; k < pcbangItems.Length; k++)
			{
				string code = pcbangItems[k];
				MyInfoManager.Instance.SetItem(--StartItemIndex, code, Item.USAGE.DELETED, 100000, 2, 100000);
				FakePcbangItems[k] = MyInfoManager.Instance.GetItemBySequence(StartItemIndex);
			}
		}
	}

	public Item[] GetPremiumItems()
	{
		if (FakePremiumItems == null)
		{
			ResetPremiumItems();
		}
		return FakePremiumItems;
	}

	public Item[] GetPcbangItems()
	{
		if (FakePcbangItems == null)
		{
			ResetPcbangItems();
		}
		return FakePcbangItems;
	}

	public bool IsPremiumItem(string code)
	{
		for (int i = 0; i < premiumItems.Length; i++)
		{
			if (premiumItems[i].Equals(code))
			{
				return true;
			}
		}
		return false;
	}
}
