using System;
using UnityEngine;

[Serializable]
public class AreYouSure : Dialog
{
	public enum SURE
	{
		INIT_ITEM,
		DECOMPOSE_ITEM,
		REPAIR_ITEM,
		COMPOSE_ITEM,
		CANCEL_DAILY_MISSION,
		ADD_MAP_SLOT,
		RESET_MAP_SLOT,
		ERASE_ALL_EXPIRED_ITEM,
		ERASE_AN_EXPIRED_ITEM,
		CREATE_CHARACTER,
		RECORD_INIT
	}

	private Vector2 crdTitle = new Vector2(15f, 7f);

	private Rect crdSureOutline = new Rect(15f, 50f, 482f, 108f);

	private Rect crdSure = new Rect(30f, 50f, 452f, 108f);

	private Rect crdCmt = new Rect(30f, 70f, 452f, 108f);

	private Rect crdYes = new Rect(247f, 165f, 128f, 34f);

	private Rect crdNo = new Rect(374f, 165f, 128f, 34f);

	private Rect crdForcePt = new Rect(50f, 125f, 21f, 22f);

	private Rect crdTokenPt = new Rect(260f, 125f, 21f, 22f);

	private Rect crdIncPimped = new Rect(14f, 165f, 21f, 22f);

	private bool incPimped;

	private Item item;

	private SURE sureWhat;

	private float durabilityPt;

	private int record_init = -1;

	private Good.BUY_HOW buyHow;

	private long src = -1L;

	private UserMapInfo umi;

	private string strParam = string.Empty;

	private bool yes;

	public bool Yes
	{
		get
		{
			return yes;
		}
		set
		{
			yes = value;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ARE_YOU_SURE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	private void ClearAll()
	{
		item = null;
		umi = null;
		incPimped = false;
		strParam = string.Empty;
		yes = false;
	}

	public void InitDialog(SURE what)
	{
		ClearAll();
		sureWhat = what;
	}

	public void InitDialog(Item _item, SURE what, UserMapInfo _umi)
	{
		ClearAll();
		umi = _umi;
		item = _item;
		sureWhat = what;
	}

	public void InitDialog(Item _item, SURE what, float fDura)
	{
		ClearAll();
		sureWhat = what;
		item = _item;
		durabilityPt = fDura * 1000f;
		buyHow = Good.BUY_HOW.GENERAL_POINT;
	}

	public void InitDialog(Item _item, SURE what, long _iVal = 0)
	{
		ClearAll();
		sureWhat = what;
		item = _item;
		durabilityPt = (float)_iVal;
		src = _iVal;
	}

	public void InitDialog(SURE what, string str)
	{
		ClearAll();
		sureWhat = what;
		strParam = str;
	}

	public void InitDialog(Item _item, SURE what, int val)
	{
		ClearAll();
		item = _item;
		sureWhat = what;
		record_init = val;
	}

	private void DoSure()
	{
		switch (sureWhat)
		{
		case SURE.INIT_ITEM:
			CSNetManager.Instance.Sock.SendCS_INIT_TERM_ITEM_REQ(item.Seq, item.Code);
			break;
		case SURE.DECOMPOSE_ITEM:
			CSNetManager.Instance.Sock.SendCS_DISCOMPOSE_ITEM_REQ(item.Seq, item.Code, item.Opt);
			break;
		case SURE.REPAIR_ITEM:
		{
			int num = 0;
			switch (buyHow)
			{
			case Good.BUY_HOW.GENERAL_POINT:
				num = (int)durabilityPt * 2;
				if (num <= 0)
				{
					num = 1;
				}
				break;
			case Good.BUY_HOW.CASH_POINT:
				num = (int)(durabilityPt * 0.4f);
				if (num <= 0)
				{
					num = 1;
				}
				break;
			}
			if (num > 0)
			{
				CSNetManager.Instance.Sock.SendCS_REPAIR_WEAPON_REQ(item.Seq, item.Code, (int)buyHow, num);
			}
			break;
		}
		case SURE.COMPOSE_ITEM:
			CSNetManager.Instance.Sock.SendCS_MERGE_ITEM_REQ(src, item.Seq, item.Code);
			break;
		case SURE.CANCEL_DAILY_MISSION:
			CSNetManager.Instance.Sock.SendCS_GIVEUP_DAILY_MISSION_REQ();
			MissionManager.Instance.Clear();
			break;
		case SURE.ADD_MAP_SLOT:
			CSNetManager.Instance.Sock.SendCS_INC_EXTRA_SLOTS_REQ(item.Seq, item.Code);
			break;
		case SURE.RESET_MAP_SLOT:
			CSNetManager.Instance.Sock.SendCS_RESET_USER_MAP_SLOTS_REQ(umi.Slot, item.Seq, item.Code);
			break;
		case SURE.ERASE_ALL_EXPIRED_ITEM:
		{
			Item[] deletedItems = MyInfoManager.Instance.GetDeletedItems();
			for (int i = 0; i < deletedItems.Length; i++)
			{
				if (incPimped || !deletedItems[i].IsPimped)
				{
					CSNetManager.Instance.Sock.SendCS_ERASE_DELETED_ITEM_REQ(deletedItems[i].Seq, deletedItems[i].Code);
				}
			}
			break;
		}
		case SURE.ERASE_AN_EXPIRED_ITEM:
			CSNetManager.Instance.Sock.SendCS_ERASE_DELETED_ITEM_REQ(item.Seq, item.Code);
			break;
		case SURE.CREATE_CHARACTER:
			CSNetManager.Instance.Sock.SendCS_CREATE_CHARACTER_REQ(strParam);
			yes = true;
			break;
		case SURE.RECORD_INIT:
			DoSureForRecordInit();
			break;
		}
	}

	private void DoSureForRecordInit()
	{
		switch (record_init)
		{
		case 92:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(0, item.Seq, item.Code);
			break;
		case 93:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(1, item.Seq, item.Code);
			break;
		case 94:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(2, item.Seq, item.Code);
			break;
		case 95:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(7, item.Seq, item.Code);
			break;
		case 96:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(4, item.Seq, item.Code);
			break;
		case 97:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(5, item.Seq, item.Code);
			break;
		case 98:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(6, item.Seq, item.Code);
			break;
		case 99:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(3, item.Seq, item.Code);
			break;
		case 100:
			CSNetManager.Instance.Sock.SendCS_RESET_BATTLE_RECORD_REQ(16, item.Seq, item.Code);
			break;
		}
	}

	private void DoRepairHow()
	{
		bool flag = buyHow == Good.BUY_HOW.GENERAL_POINT;
		bool flag2 = buyHow == Good.BUY_HOW.CASH_POINT;
		bool flag3 = GUI.Toggle(crdForcePt, flag, StringMgr.Instance.Get("GENERAL_POINT"));
		bool flag4 = GUI.Toggle(crdTokenPt, flag2, TokenManager.Instance.GetTokenString());
		if (!flag && flag3)
		{
			buyHow = Good.BUY_HOW.GENERAL_POINT;
		}
		if (!flag2 && flag4)
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
		}
	}

	private void DoEraseExpiredItem()
	{
		if (BuildOption.Instance.Props.ApplyItemUpgrade)
		{
			incPimped = GUI.Toggle(crdIncPimped, incPimped, StringMgr.Instance.Get("INCLUDE_PIMPED"));
		}
	}

	private string DoTitleForRecordInit()
	{
		switch (record_init)
		{
		case 92:
			return StringMgr.Instance.Get("RECORD_FULLY_INIT");
		case 93:
			return StringMgr.Instance.Get("RECORD_TEAM_INIT");
		case 94:
			return StringMgr.Instance.Get("RECORD_INDIVIDUAL_INIT");
		case 95:
			return StringMgr.Instance.Get("RECORD_BUNGEE_INIT");
		case 96:
			return StringMgr.Instance.Get("RECORD_EXPLOSION_INIT");
		case 97:
			return StringMgr.Instance.Get("RECORD_MISSION_INIT");
		case 98:
			return StringMgr.Instance.Get("RECORD_BND_INIT");
		case 99:
			return StringMgr.Instance.Get("RECORD_FLAG_INIT");
		case 100:
			return StringMgr.Instance.Get("RECORD_WEAPON_INIT");
		default:
			return string.Empty;
		}
	}

	private string DoMessageForRecordInit()
	{
		switch (record_init)
		{
		case 92:
			return StringMgr.Instance.Get("CMT_RECORD_FULLY_INIT");
		case 93:
			return StringMgr.Instance.Get("CMT_RECORD_TEAM_INIT");
		case 94:
			return StringMgr.Instance.Get("CMT_RECORD_INDIVIDUAL_INIT");
		case 95:
			return StringMgr.Instance.Get("CMT_RECORD_BUNGEE_INIT");
		case 96:
			return StringMgr.Instance.Get("CMT_RECORD_EXPLOSION_INIT");
		case 97:
			return StringMgr.Instance.Get("CMT_RECORD_MISSION_INIT");
		case 98:
			return StringMgr.Instance.Get("CMT_RECORD_BND_INIT");
		case 99:
			return StringMgr.Instance.Get("CMT_RECORD_FLAG_INIT");
		case 100:
			return StringMgr.Instance.Get("CMT_RECORD_WEAPON_INIT");
		default:
			return string.Empty;
		}
	}

	public override bool DoDialog()
	{
		Rect position = crdSure;
		Rect position2 = crdCmt;
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(crdSureOutline, string.Empty, "LineBoxBlue");
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		switch (sureWhat)
		{
		case SURE.INIT_ITEM:
			text = StringMgr.Instance.Get("EQUIP_IN_INVENTORY");
			text2 = string.Format(StringMgr.Instance.Get("SURE_TO_INIT_ITEM"), item.Template.Name, item.GetRemainString());
			text3 = string.Format(StringMgr.Instance.Get("CMT_EQUIP_WARNING"));
			break;
		case SURE.DECOMPOSE_ITEM:
			text = StringMgr.Instance.Get("DECOMPOSE_ITEM");
			text2 = string.Format(StringMgr.Instance.Get("SURE_TO_DECOMPOSE_ITEM"), item.Template.Name, item.BpBack);
			break;
		case SURE.REPAIR_ITEM:
			DoRepairHow();
			text = StringMgr.Instance.Get("REPAIR_ITEM");
			switch (buyHow)
			{
			case Good.BUY_HOW.GENERAL_POINT:
			{
				int num2 = (int)durabilityPt * 2;
				if (num2 <= 0)
				{
					num2 = 1;
				}
				text2 = string.Format(StringMgr.Instance.Get("SURE_TO_REPAIR_ITEM"), item.Template.Name, num2);
				break;
			}
			case Good.BUY_HOW.CASH_POINT:
			{
				int num = (int)(durabilityPt * 0.4f);
				if (num <= 0)
				{
					num = 1;
				}
				text2 = string.Format(StringMgr.Instance.Get("SURE_TO_REPAIR_ITEM_TOKEN"), item.Template.Name, num, TokenManager.Instance.GetTokenString());
				break;
			}
			}
			position.y -= 25f;
			break;
		case SURE.COMPOSE_ITEM:
			text = StringMgr.Instance.Get("COMPOSE_ITEM");
			text2 = StringMgr.Instance.Get("IS_COMPOSE_ITEM");
			break;
		case SURE.CANCEL_DAILY_MISSION:
			text = StringMgr.Instance.Get("DAILY_MISSION");
			text2 = StringMgr.Instance.Get("SURE_TO_CANCEL_DAILY_MISSION");
			break;
		case SURE.ADD_MAP_SLOT:
			text = StringMgr.Instance.Get("ADD_MAP_SLOT");
			text2 = StringMgr.Instance.Get("ARE_YOU_SURE_ADD_MAP_SLOT");
			break;
		case SURE.RESET_MAP_SLOT:
			text = StringMgr.Instance.Get("RESET_MAP_SLOT");
			text2 = string.Format(StringMgr.Instance.Get("ARE_YOU_SURE_RESET_MAP_SLOT"), umi.Alias);
			break;
		case SURE.ERASE_ALL_EXPIRED_ITEM:
			DoEraseExpiredItem();
			text = StringMgr.Instance.Get("ERASE_ALL");
			text2 = StringMgr.Instance.Get("SURE_DELETE_ALL_EXPIRED");
			break;
		case SURE.ERASE_AN_EXPIRED_ITEM:
			text = StringMgr.Instance.Get("DELETE");
			text2 = ((!item.IsPimped) ? string.Format(StringMgr.Instance.Get("SURE_DELETE_ITEM"), item.Template.Name) : string.Format(StringMgr.Instance.Get("SURE_DELETE_PIMPED_ITEM"), item.Template.Name));
			break;
		case SURE.CREATE_CHARACTER:
			text = StringMgr.Instance.Get("NICK_SET");
			text2 = string.Format(StringMgr.Instance.Get("SURE_TO_NICKNAME_SET"), strParam);
			break;
		case SURE.RECORD_INIT:
			text = DoTitleForRecordInit();
			text2 = DoMessageForRecordInit();
			break;
		}
		if (text.Length > 0)
		{
			LabelUtil.TextOut(crdTitle, text, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		if (text2.Length > 0)
		{
			GUI.Label(position, text2, "MiddleCenterLabel");
		}
		if (text3.Length > 0)
		{
			GUI.Label(position2, text3, "MiddleCenterLabel");
		}
		if (GlobalVars.Instance.MyButton(crdYes, StringMgr.Instance.Get("YES"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			DoSure();
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdNo, StringMgr.Instance.Get("NO"), "BtnAction") || GlobalVars.Instance.IsEscapePressed())
		{
			GlobalVars.Instance.bReceivedAck = true;
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
