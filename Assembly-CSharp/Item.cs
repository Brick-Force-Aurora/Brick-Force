using UnityEngine;

public class Item
{
	public enum USAGE
	{
		NOT_USING = -1,
		UNEQUIP,
		EQUIP,
		DELETED
	}

	public const int maxUpgradeLevel = 10;

	private long seq;

	private TItem tItem;

	private string code;

	private USAGE usage;

	private int remain;

	private int amount;

	private int amountBuf;

	private sbyte premium;

	private int durability;

	public sbyte toolSlot = -1;

	public UpgradeProp[] upgradeProps;

	public long Seq => seq;

	public TItem Template => tItem;

	public string Code
	{
		get
		{
			return code;
		}
		set
		{
			code = value;
		}
	}

	public USAGE Usage
	{
		get
		{
			return usage;
		}
		set
		{
			usage = value;
		}
	}

	public int Remain
	{
		get
		{
			return remain;
		}
		set
		{
			remain = value;
		}
	}

	public int Amount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
		}
	}

	public int AmountBuf
	{
		get
		{
			return amountBuf;
		}
		set
		{
			amountBuf = value;
		}
	}

	public bool IsPremium
	{
		get
		{
			if (premium != 0 && premium != 1 && premium != 2)
			{
				Debug.LogError("Invalid Premium value");
			}
			return premium == 1;
		}
	}

	public bool IsPCBang
	{
		get
		{
			if (premium != 0 && premium != 1 && premium != 2)
			{
				Debug.LogError("Invalid Premium value");
			}
			return premium == 2;
		}
	}

	public bool IsPremiumOrPCBang
	{
		get
		{
			if (premium != 0 && premium != 1 && premium != 2)
			{
				Debug.LogError("Invalid Premium value");
			}
			return premium != 0;
		}
	}

	public float starRate => tItem.StarRate;

	public int _StarRate => tItem._StarRate;

	public bool IsLimitedByStarRate
	{
		get
		{
			if (tItem.type != 0)
			{
				return false;
			}
			if (ChannelManager.Instance.CurChannel.LimitStarRate <= 0 || ChannelManager.Instance.CurChannel.LimitStarRate >= 10000)
			{
				return false;
			}
			return tItem._StarRate > ChannelManager.Instance.CurChannel.LimitStarRate;
		}
	}

	public int Durability
	{
		get
		{
			return durability;
		}
		set
		{
			durability = value;
		}
	}

	public bool EnoughToConsume
	{
		get
		{
			if (tItem == null || !tItem.IsAmount)
			{
				return false;
			}
			if (premium == 1)
			{
				long num = MyInfoManager.Instance.HaveFunction("premium_account");
				if (num < 0)
				{
					return false;
				}
			}
			return amount < 0 || amount >= 1;
		}
	}

	public bool IsDecomposable
	{
		get
		{
			Good good = ShopManager.Instance.Get(tItem.code);
			if (good == null)
			{
				good = ShopManager.Instance.Get(tItem.bpBackCode);
			}
			if (good == null)
			{
				return false;
			}
			return tItem.discomposable;
		}
	}

	public bool IsTakeoffable => tItem.takeoffable;

	public bool IsEquipable
	{
		get
		{
			if (premium == 1)
			{
				long num = MyInfoManager.Instance.HaveFunction("premium_account");
				if (num < 0)
				{
					return false;
				}
			}
			return tItem.IsEquipable;
		}
	}

	public bool IsAmount => tItem.IsAmount;

	public int Opt
	{
		get
		{
			if (usage != USAGE.NOT_USING)
			{
				return -1;
			}
			return Mathf.RoundToInt((float)remain / 86400f);
		}
	}

	public int BpBack
	{
		get
		{
			Good good = ShopManager.Instance.Get(tItem.code);
			if (good == null)
			{
				good = ShopManager.Instance.Get(tItem.bpBackCode);
			}
			if (good == null)
			{
				return 0;
			}
			return good.GetPriceByOpt(Opt, Good.BUY_HOW.GENERAL_POINT) / 20;
		}
	}

	public string BpBackCode => tItem.bpBackCode;

	public bool IsPimped
	{
		get
		{
			for (int i = 0; i < upgradeProps.Length; i++)
			{
				if (upgradeProps[i].grade > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsWeaponSlotAble => Template.type == TItem.TYPE.WEAPON && (usage == USAGE.EQUIP || usage == USAGE.UNEQUIP);

	public bool IsShooterSlotAble
	{
		get
		{
			if (Template.type == TItem.TYPE.SPECIAL && EnoughToConsume)
			{
				TSpecial tSpecial = (TSpecial)Template;
				ConsumableDesc consumableDesc = ConsumableManager.Instance.Get(TItem.FunctionMaskToString(tSpecial.functionMask));
				if (consumableDesc != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public Item(long itemSeq, TItem itemTemplate, string itemCode, USAGE itemUsage, int itemRemain, sbyte itemPremium, int itemDurability)
	{
		seq = itemSeq;
		tItem = itemTemplate;
		code = itemCode;
		usage = itemUsage;
		premium = itemPremium;
		durability = itemDurability;
		if (tItem.IsAmount)
		{
			remain = -1;
			amount = itemRemain;
		}
		else
		{
			remain = itemRemain;
			amount = -1;
		}
		int num = 13;
		upgradeProps = new UpgradeProp[num];
		for (int i = 0; i < num; i++)
		{
			upgradeProps[i] = new UpgradeProp();
			upgradeProps[i].use = false;
			upgradeProps[i].grade = 0;
		}
	}

	public void Refresh(USAGE itemUsage, int itemRemain, sbyte itemPremium, int itemDurability)
	{
		if (tItem.IsAmount)
		{
			amount = itemRemain;
		}
		else
		{
			remain = itemRemain;
		}
		usage = itemUsage;
		if (itemPremium == 0 || itemPremium == 1)
		{
			premium = itemPremium;
		}
		durability = itemDurability;
	}

	public void Buy(int itemRemain, USAGE initialUsage, int itemDurability)
	{
		if (tItem.IsAmount)
		{
			amount = itemRemain;
			if (amount > 0)
			{
				usage = USAGE.UNEQUIP;
			}
		}
		else
		{
			remain = itemRemain;
			usage = initialUsage;
		}
		durability = itemDurability;
	}

	public void TickTok()
	{
		if ((remain >= 0 || amount >= 0) && !tItem.IsAmount && usage != USAGE.DELETED && usage != USAGE.NOT_USING)
		{
			remain--;
			if (remain < 0)
			{
				remain = 0;
			}
		}
	}

	public bool IsExpiring()
	{
		if (remain < 0 && amount < 0)
		{
			return false;
		}
		if (tItem.IsAmount)
		{
			return false;
		}
		if (usage == USAGE.DELETED)
		{
			return false;
		}
		return remain == 0;
	}

	private string _GetRemainString4AmountItem()
	{
		return amount.ToString() + StringMgr.Instance.Get("TIMES_UNIT");
	}

	private string _GetRemainString4TermsItem()
	{
		if (usage == USAGE.DELETED)
		{
			return StringMgr.Instance.Get("EXPIRED_ITEM");
		}
		int num = Mathf.RoundToInt((float)remain / 86400f);
		int num2 = Mathf.RoundToInt((float)remain / 3600f);
		int num3 = Mathf.RoundToInt((float)remain / 60f);
		if (remain > 315360000)
		{
			return StringMgr.Instance.Get("REMAIN_OVERFLOW");
		}
		if (num > 0)
		{
			return num.ToString() + StringMgr.Instance.Get("DAYS");
		}
		if (num2 > 0)
		{
			return num2.ToString() + StringMgr.Instance.Get("HOURS");
		}
		if (num3 > 0)
		{
			return num3.ToString() + StringMgr.Instance.Get("MINUTES");
		}
		return StringMgr.Instance.Get("UNDER_ONE_MINUTE");
	}

	public string GetAmountString()
	{
		if (tItem == null || !tItem.IsAmount)
		{
			return string.Empty;
		}
		if (IsPremium)
		{
			return StringMgr.Instance.Get("JUST_PREMIUM");
		}
		if (IsPCBang)
		{
			return StringMgr.Instance.Get("PCBANG_INVEN_TAB");
		}
		if (usage == USAGE.DELETED)
		{
			return string.Empty;
		}
		if (remain < 0 && amount < 0)
		{
			return StringMgr.Instance.Get("INFINITE");
		}
		return amount.ToString();
	}

	public string GetRemainString()
	{
		if (IsPremium)
		{
			return StringMgr.Instance.Get("JUST_PREMIUM");
		}
		if (IsPCBang)
		{
			return StringMgr.Instance.Get("PCBANG_INVEN_TAB");
		}
		if (usage == USAGE.DELETED)
		{
			return string.Empty;
		}
		if (remain < 0 && amount < 0)
		{
			return StringMgr.Instance.Get("INFINITE");
		}
		if (tItem.IsAmount)
		{
			return _GetRemainString4AmountItem();
		}
		return _GetRemainString4TermsItem();
	}

	public bool CanBeMerged()
	{
		if (tItem.IsAmount)
		{
			return false;
		}
		if (IsPremium)
		{
			return false;
		}
		if (IsPCBang)
		{
			return false;
		}
		if (usage == USAGE.DELETED)
		{
			return true;
		}
		if (remain < 0 && amount < 0)
		{
			return false;
		}
		return true;
	}

	public bool CanSpecialUse()
	{
		if (!tItem.IsAmount || amount <= 0 || usage == USAGE.DELETED)
		{
			return false;
		}
		if (tItem.type != TItem.TYPE.SPECIAL)
		{
			return false;
		}
		TSpecial tSpecial = (TSpecial)tItem;
		return tSpecial.functionMask == 24 || tSpecial.functionMask == 25 || tSpecial.functionMask == 26 || tSpecial.functionMask == 80 || tSpecial.functionMask == 87 || tSpecial.functionMask == 92 || tSpecial.functionMask == 93 || tSpecial.functionMask == 94 || tSpecial.functionMask == 95 || tSpecial.functionMask == 96 || tSpecial.functionMask == 97 || tSpecial.functionMask == 98 || tSpecial.functionMask == 99 || tSpecial.functionMask == 100;
	}

	public void SpecialUse()
	{
		if (tItem.IsAmount && amount > 0 && usage != USAGE.DELETED && tItem.type == TItem.TYPE.SPECIAL)
		{
			TSpecial tSpecial = (TSpecial)tItem;
			switch (tSpecial.functionMask)
			{
			case 24:
				CSNetManager.Instance.Sock.SendCS_CHARGE_PICKNWIN_COIN_REQ(seq, tSpecial.code);
				break;
			case 25:
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(this, AreYouSure.SURE.ADD_MAP_SLOT);
				break;
			case 26:
				((SelectUserMapDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SELECT_USER_MAP, exclusive: true))?.InitDialog(this);
				break;
			case 80:
				CSNetManager.Instance.Sock.SendCS_CHARGE_FORCE_POINT_REQ(seq, tSpecial.code);
				break;
			case 87:
				((ChangeNickDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.CHANGE_NICK, exclusive: true))?.InitDialog();
				break;
			case 92:
			case 93:
			case 94:
			case 95:
			case 96:
			case 97:
			case 98:
			case 99:
			case 100:
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(this, AreYouSure.SURE.RECORD_INIT, tSpecial.functionMask);
				break;
			}
		}
	}

	public int Compare(Item a)
	{
		int num = Template.season;
		int num2 = a.Template.season;
		if (num <= 0)
		{
			num = 1;
		}
		if (num2 <= 0)
		{
			num2 = 1;
		}
		if (num == num2)
		{
			return Template.Name.CompareTo(a.Template.Name);
		}
		return -Template.season.CompareTo(a.Template.season);
	}

	public bool IsFiltered(int filter)
	{
		if (filter == 0)
		{
			return true;
		}
		bool result = false;
		switch (filter)
		{
		case 1:
			result = (usage == USAGE.EQUIP || usage == USAGE.UNEQUIP);
			break;
		case 2:
			result = (usage == USAGE.NOT_USING);
			break;
		case 3:
			result = (usage != USAGE.DELETED && usage != USAGE.NOT_USING && tItem.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE);
			break;
		case 4:
			result = CanBeMerged();
			break;
		}
		return result;
	}

	public bool CanUpgradeAble()
	{
		int num = 0;
		int num2 = 13;
		for (int i = 0; i < num2; i++)
		{
			if (UpgradePropManager.Instance.UseProp((int)Template.upgradeCategory, i))
			{
				num++;
			}
		}
		int num3 = 0;
		for (int j = 0; j < num2; j++)
		{
			if (upgradeProps[j].use && upgradeProps[j].grade >= 10)
			{
				num3++;
			}
		}
		if (num == num3)
		{
			return false;
		}
		return true;
	}

	public bool IsUpgradedItem()
	{
		if (Usage == USAGE.NOT_USING || Template.upgradeCategory == TItem.UPGRADE_CATEGORY.NONE)
		{
			return false;
		}
		int num = 13;
		for (int i = 0; i < num; i++)
		{
			int grade = upgradeProps[i].grade;
			if (grade > 0)
			{
				return true;
			}
		}
		return false;
	}
}
