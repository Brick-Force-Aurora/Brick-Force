using System;
using UnityEngine;

[Serializable]
public class WeaponChangeDialog : Dialog
{
	public Texture2D premiumIcon;

	public Texture2D slotLock;

	private Vector2 crdSlotOutline = new Vector2(540f, 196f);

	private Vector2 crdWeaponSlotList = new Vector2(453f, 176f);

	private Vector2 crdSlotBtn = new Vector2(84f, 84f);

	private Rect crdLock = new Rect(0f, 0f, 68f, 68f);

	private string[] key = new string[10]
	{
		"K_WPNCHG1",
		"K_WPNCHG2",
		"K_WPNCHG3",
		"K_WPNCHG4",
		"K_WPNCHG5",
		"K_WPNCHG6",
		"K_WPNCHG7",
		"K_WPNCHG8",
		"K_WPNCHG9",
		"K_WPNCHG0"
	};

	private Color disabledColor = new Color(0.92f, 0.05f, 0.05f, 0.58f);

	private bool premiumAccount;

	private bool done;

	public bool Done
	{
		set
		{
			done = value;
		}
	}

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.WEAPON_CHANGE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		premiumAccount = (MyInfoManager.Instance.HaveFunction("premium_account") >= 0);
		bool flag = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		DoWeaponSlots();
		Rect rc = new Rect(size.x - 44f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			flag = true;
			GlobalVars.Instance.SetForceClosed(set: true);
		}
		if (ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
		{
			flag = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return flag || done;
	}

	private int CheckShortcut()
	{
		for (int i = 0; i < key.Length; i++)
		{
			if (custom_inputs.Instance.GetButtonDown(key[i]))
			{
				return i;
			}
		}
		return -1;
	}

	private void ChangeWeapon(int slot)
	{
		if (slot >= 0 && slot < 10)
		{
			if (slot >= 5 && !premiumAccount)
			{
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ERR_WPN_CHG_PREMIUM_ONLY"));
			}
			else
			{
				long num = MyInfoManager.Instance.WeaponSlots[slot];
				Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(num);
				if (itemBySequence != null && itemBySequence.IsWeaponSlotAble)
				{
					TWeapon tWeapon = (TWeapon)itemBySequence.Template;
					if (!IsLock(tWeapon))
					{
						Item currentWeaponBySlot = MyInfoManager.Instance.GetCurrentWeaponBySlot(tWeapon.slot);
						if (currentWeaponBySlot != null && currentWeaponBySlot.Template.type == TItem.TYPE.WEAPON)
						{
							TWeapon tWeapon2 = (TWeapon)currentWeaponBySlot.Template;
							if (currentWeaponBySlot.Seq == num)
							{
								SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ERR_WPN_CHG_SAME_WEAPON"));
							}
							else if (itemBySequence.IsLimitedByStarRate)
							{
								SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
							}
							else
							{
								done = true;
								CSNetManager.Instance.Sock.SendCS_WEAPON_CHANGE_REQ(slot, num, tWeapon.code, tWeapon2.code);
							}
						}
					}
				}
			}
		}
	}

	public override void Update()
	{
		if (!ZombieVsHumanManager.Instance.IsZombie(MyInfoManager.Instance.Seq))
		{
			int num = CheckShortcut();
			if (0 <= num && num < 10)
			{
				ChangeWeapon(num);
			}
		}
	}

	public void InitDialog()
	{
		done = false;
		premiumAccount = (MyInfoManager.Instance.HaveFunction("premium_account") >= 0);
	}

	private void DrawSlotIcon(Item item, Texture2D icon, Rect crdIcon)
	{
		if (item != null)
		{
			Color color = GUI.color;
			if (item.Usage == Item.USAGE.DELETED)
			{
				GUI.color = disabledColor;
			}
			if (item.IsPremium && !premiumAccount)
			{
				GUI.color = disabledColor;
			}
			TextureUtil.DrawTexture(crdIcon, icon, ScaleMode.ScaleToFit);
			GUI.color = color;
		}
	}

	private bool IsLock(TWeapon tWeapon)
	{
		int weaponOption = RoomManager.Instance.GetCurrentRoomInfo().weaponOption;
		switch (weaponOption)
		{
		case 0:
			return false;
		case 1:
			if (tWeapon.cat == 4)
			{
				return false;
			}
			if (tWeapon.cat == 5)
			{
				return false;
			}
			break;
		}
		if (weaponOption == 2 && tWeapon.cat == 5)
		{
			return false;
		}
		return true;
	}

	private void DoWeaponSlots()
	{
		Rect position = new Rect((size.x - crdSlotOutline.x) / 2f, 49f, crdSlotOutline.x, crdSlotOutline.y);
		Rect position2 = new Rect((size.x - crdWeaponSlotList.x) / 2f, 59f, crdWeaponSlotList.x, crdWeaponSlotList.y);
		GUI.Box(position, string.Empty, "BoxInnerLine");
		GUI.BeginGroup(position2);
		for (int i = 0; i < MyInfoManager.Instance.WeaponSlots.Length; i++)
		{
			float x = (float)(i % 5) * (crdSlotBtn.x + 8f);
			float y = (float)(i / 5) * (crdSlotBtn.y + 8f);
			Rect rect = new Rect(x, y, crdSlotBtn.x, crdSlotBtn.y);
			TWeapon tWeapon = null;
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(MyInfoManager.Instance.WeaponSlots[i]);
			if (itemBySequence != null && !itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.WEAPON)
			{
				tWeapon = (TWeapon)itemBySequence.Template;
			}
			Texture2D icon = null;
			if (itemBySequence != null && tWeapon != null)
			{
				icon = tWeapon.CurIcon();
			}
			string str = "BtnItemFixate";
			if (GUI.Button(rect, string.Empty, str))
			{
				ChangeWeapon(i);
			}
			DrawSlotIcon(itemBySequence, icon, rect);
			LabelUtil.TextOut(new Vector2(rect.x, rect.y + 5f), custom_inputs.Instance.GetKeyCodeName(key[i]), "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (tWeapon != null && IsLock(tWeapon))
			{
				TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)(slotLock.width / 2)) / 2f, rect.y + (rect.height - (float)(slotLock.height / 2)) / 2f, (float)(slotLock.width / 2), (float)(slotLock.height / 2)), slotLock);
			}
			bool flag = false;
			if (i >= 5)
			{
				TextureUtil.DrawTexture(new Rect(rect.x + rect.width - (float)premiumIcon.width - 5f, rect.y, (float)premiumIcon.width, (float)premiumIcon.height), premiumIcon);
				if (!premiumAccount)
				{
					flag = true;
					TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)(slotLock.width / 2)) / 2f, rect.y + (rect.height - (float)(slotLock.height / 2)) / 2f, (float)(slotLock.width / 2), (float)(slotLock.height / 2)), slotLock);
				}
			}
			if (!flag && itemBySequence != null && itemBySequence.IsLimitedByStarRate)
			{
				flag = true;
				TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - crdLock.width) / 2f, rect.y + (rect.height - crdLock.height) / 2f, crdLock.width, crdLock.height), slotLock);
			}
		}
		GUI.EndGroup();
	}

	private void DoTitle()
	{
		LabelUtil.TextOut(new Vector2(size.x / 2f, 10f), StringMgr.Instance.Get("WEAPON_CHANGE"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}
}
