using System;
using UnityEngine;

[Serializable]
public class ItemSettingDialog : Dialog
{
	public Tooltip tooltip;

	public Texture2D premiumIcon;

	public Texture2D slotLock;

	private Rect crdStarGauge = new Rect(4f, 92f, 64f, 12f);

	public Texture2D itemStarGauge;

	public Texture2D itemStarGaugeBg;

	public Texture2D itemStarGauge2;

	private Rect crdX = new Rect(716f, 10f, 34f, 34f);

	private Rect crdBtnWeapon = new Rect(21f, 61f, 148f, 45f);

	private Rect crdBtnShooterTool = new Rect(21f, 113f, 148f, 45f);

	private Vector2 crdItem = new Vector2(135f, 121f);

	private Rect crdItemActionOutline = new Rect(15f, 170f, 730f, 380f);

	private Rect crdItemWeaponOutline = new Rect(15f, 200f, 730f, 350f);

	private Rect crdWeaponKind = new Rect(21f, 168f, 715f, 33f);

	private Rect crdMainKind = new Rect(27f, 206f, 164f, 28f);

	private Rect crdShooterToolList = new Rect(27f, 185f, 700f, 355f);

	private Rect crdWeaponList = new Rect(27f, 212f, 700f, 320f);

	private Rect crdMainWeaponList = new Rect(27f, 237f, 700f, 295f);

	private Rect crdItemBtn = new Rect(3f, 3f, 122f, 105f);

	private Vector2 crdRemain = new Vector2(120f, 98f);

	private Vector2 crdItemUsage = new Vector2(122f, 85f);

	private Color disabledColor = new Color(0.92f, 0.05f, 0.05f, 0.58f);

	private Rect crdInit = new Rect(21f, 556f, 136f, 34f);

	private Rect crdUninstall = new Rect(481f, 556f, 136f, 34f);

	private Rect crdInstall = new Rect(614f, 556f, 136f, 34f);

	private Rect crdSlotOutline = new Rect(196f, 62f, 540f, 98f);

	private Rect crdWeaponSlotList = new Rect(237f, 69f, 472f, 85f);

	private Rect crdSlotBtn = new Rect(41f, 7f, 84f, 84f);

	private Rect crdLock = new Rect(0f, 0f, 68f, 68f);

	private int mainTab;

	private int curWeaponSlot;

	private int curActionSlot;

	private int curItem = -1;

	private Item[] myItems;

	private float focusTime;

	private float lastClickTime;

	private float doubleClickTimeout = 0.2f;

	private Vector2 scrollPositionWeaponSlot = new Vector2(0f, 0f);

	private Vector2 scrollPositionWeapon = new Vector2(0f, 0f);

	private Vector2 scrollPositionShooterTool = new Vector2(0f, 0f);

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private float yOffset = 10f;

	private string[] weaponKindKey = new string[5]
	{
		"ALL",
		"MAIN_WEAPON",
		"AUX_WEAPON",
		"MELEE_WEAPON",
		"SPEC_WEAPON"
	};

	private string[] mainKindKey = new string[4]
	{
		"ASSAULT_WPN",
		"HEAVY_WPN",
		"SUB_MACHINE_WPN",
		"SNIPER_WPN"
	};

	private int weaponKind;

	private int mainKind;

	private bool premiumAccount;

	private Item dragItem;

	public bool IsDragging => dragItem != null;

	public void ResetDragItem()
	{
		dragItem = null;
	}

	public void SetDragItem(Item item)
	{
		dragItem = item;
	}

	public override void Start()
	{
		tooltip.Start();
		id = DialogManager.DIALOG_INDEX.ITEM_SETTING;
		curActionSlot = 0;
		curWeaponSlot = 0;
	}

	public override void OnPopup()
	{
		rc = new Rect(0f, 0f, GlobalVars.Instance.ScreenRect.width, GlobalVars.Instance.ScreenRect.height);
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		ResetDragItem();
	}

	public override void Update()
	{
		focusTime += Time.deltaTime;
		if (IsDragging && Input.GetMouseButtonDown(1))
		{
			ResetDragItem();
		}
	}

	public void InitDialog(int cat)
	{
		mainTab = ((cat == 5) ? 1 : 0);
		premiumAccount = (MyInfoManager.Instance.HaveFunction("premium_account") >= 0);
	}

	private bool IsUpgraded(Item item)
	{
		if (item.Usage == Item.USAGE.NOT_USING || item.Template.upgradeCategory == TItem.UPGRADE_CATEGORY.NONE)
		{
			return false;
		}
		int num = 13;
		for (int i = 0; i < num; i++)
		{
			int grade = item.upgradeProps[i].grade;
			if (grade > 0)
			{
				return true;
			}
		}
		return false;
	}

	private void ShowItemStatus(Item item)
	{
		switch (item.Usage)
		{
		case Item.USAGE.UNEQUIP:
			break;
		case Item.USAGE.EQUIP:
			LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("USING"), "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(byte.MaxValue, 72, 48), Color.black, TextAnchor.LowerRight);
			break;
		case Item.USAGE.NOT_USING:
			LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("NOT_USING"), "MiniLabel", GlobalVars.Instance.GetByteColor2FloatColor(31, 220, 0), Color.black, TextAnchor.LowerRight);
			break;
		}
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

	private void DrawItemIcon(Item item, Rect crdIcon)
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
		TextureUtil.DrawTexture(crdIcon, item.Template.CurIcon(), ScaleMode.ScaleToFit);
		GUI.color = color;
	}

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("ITEM_SETTING"), "BigLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
	}

	private void DragItem()
	{
		if (IsDragging)
		{
			Vector2 vector = GlobalVars.Instance.PixelToGUIPoint(MouseUtil.ScreenToPixelPoint(Input.mousePosition));
			Rect position = new Rect(vector.x, vector.y, 90f, 90f);
			Rect position2 = new Rect(vector.x + 3f, vector.y + 3f, 84f, 84f);
			GUI.Box(position, string.Empty, "BoxFadeBlue");
			GUI.Box(position2, string.Empty, "LineBoxBlue");
			TextureUtil.DrawTexture(position2, dragItem.Template.CurIcon(), ScaleMode.ScaleToFit);
		}
	}

	private void DoMainTab()
	{
		if (GlobalVars.Instance.MyButton(crdBtnWeapon, StringMgr.Instance.Get("WEAPON"), "BtnBlue"))
		{
			mainTab = 0;
			ResetDragItem();
		}
		if (GlobalVars.Instance.MyButton(crdBtnShooterTool, StringMgr.Instance.Get("ACTIONPANEL"), "BtnBlue"))
		{
			mainTab = 1;
			ResetDragItem();
		}
		switch (mainTab)
		{
		case 0:
			GUI.Box(crdBtnWeapon, string.Empty, "BtnBlueF");
			break;
		case 1:
			GUI.Box(crdBtnShooterTool, string.Empty, "BtnBlueF");
			break;
		}
	}

	private void DoWeaponSlots()
	{
		string[] array = new string[10]
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
		GUI.Box(crdSlotOutline, string.Empty, "BoxInnerLine");
		int num = 2;
		int num2 = 8;
		if (BuildOption.Instance.IsNetmarble)
		{
			num = 1;
			num2 = 0;
		}
		scrollPositionWeaponSlot = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdSlotBtn.width * 5f + 32f, crdSlotBtn.height * (float)num + (float)num2), position: crdWeaponSlotList, scrollPosition: scrollPositionWeaponSlot, alwaysShowHorizontal: false, alwaysShowVertical: false);
		for (int i = 0; i < MyInfoManager.Instance.WeaponSlots.Length; i++)
		{
			float x = (float)(i % 5) * (crdSlotBtn.width + 8f);
			float y = (float)(i / 5) * (crdSlotBtn.height + 8f);
			Rect rect = new Rect(x, y, crdSlotBtn.width, crdSlotBtn.height);
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
				if (tooltip.ItemSeq == ItemSlot2Tooltip(itemBySequence, i))
				{
					if (i < 2)
					{
						ltTooltip = new Vector2(rc.x + crdWeaponSlotList.x + rect.x + rect.width + 5f, rc.y + crdWeaponSlotList.y + rect.y + 5f);
					}
					else
					{
						ltTooltip = new Vector2(rc.x + crdWeaponSlotList.x + rect.x - tooltip.size.x - 5f, rc.y + crdWeaponSlotList.y + rect.y + 5f);
					}
				}
			}
			string str = "BtnItemFixate";
			if (GUI.Button(rect, new GUIContent(string.Empty, ItemSlot2Tooltip(itemBySequence, i)), str) && (i < 5 || premiumAccount))
			{
				curWeaponSlot = i;
				if (dragItem != null)
				{
					if (dragItem.IsWeaponSlotAble)
					{
						CSNetManager.Instance.Sock.SendCS_SET_WEAPON_SLOT_REQ(i, dragItem.Seq);
					}
					ResetDragItem();
				}
			}
			DrawSlotIcon(itemBySequence, icon, rect);
			LabelUtil.TextOut(new Vector2(rect.x + 5f, rect.y + 5f), custom_inputs.Instance.GetKeyCodeName(array[i]), "MiniLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
			if (curWeaponSlot == i)
			{
				GUI.Box(rect, string.Empty, "BtnItemFixateF");
			}
			bool flag = false;
			if (i >= 5)
			{
				TextureUtil.DrawTexture(new Rect(rect.x + rect.width - (float)premiumIcon.width - 5f, rect.y + 5f, (float)premiumIcon.width, (float)premiumIcon.height), premiumIcon);
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
		GUI.EndScrollView();
	}

	private string ItemSlot2Tooltip(Item item, int i)
	{
		return (item != null) ? ("*" + i.ToString() + item.Seq.ToString()) : string.Empty;
	}

	private void DoWeaponTab()
	{
		string[] array = new string[weaponKindKey.Length];
		for (int i = 0; i < weaponKindKey.Length; i++)
		{
			array[i] = StringMgr.Instance.Get(weaponKindKey[i]);
		}
		weaponKind = GUI.SelectionGrid(crdWeaponKind, weaponKind, array, array.Length, "PopTab");
		if (weaponKind == 1)
		{
			for (int j = 0; j < mainKindKey.Length; j++)
			{
				Rect rect = new Rect(crdMainKind);
				rect.x += (float)j * (crdMainKind.width + 6f);
				if (GlobalVars.Instance.MyButton(rect, StringMgr.Instance.Get(mainKindKey[j]), "BtnBlue"))
				{
					mainKind = j;
					ResetDragItem();
				}
				if (mainKind == j)
				{
					GUI.Box(rect, string.Empty, "BtnBlueF");
				}
			}
		}
	}

	private void DoWeapons()
	{
		DoWeaponSlots();
		GUI.Box(crdItemWeaponOutline, string.Empty, "LineBoxBlue");
		int num = 0;
		int[] array = new int[4]
		{
			1,
			0,
			3,
			2
		};
		myItems = MyInfoManager.Instance.GetItemsByCat(0, 1, weaponKind, (weaponKind != 1) ? (-1) : array[mainKind]);
		int num2 = myItems.Length;
		int num3 = 5;
		int num4 = num2 / num3;
		if (num2 % num3 > 0)
		{
			num4++;
		}
		float num5 = crdItem.x * (float)num3;
		if (num3 > 1)
		{
			num5 += (float)((num3 - 1) * 2);
		}
		float num6 = crdItem.y * (float)num4;
		if (num4 > 0)
		{
			num6 -= yOffset;
		}
		Rect position = crdWeaponList;
		if (weaponKind == 1)
		{
			position = crdMainWeaponList;
		}
		scrollPositionWeapon = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, position.width - 20f, num6), position: position, scrollPosition: scrollPositionWeapon, alwaysShowHorizontal: false, alwaysShowVertical: false);
		float y = scrollPositionWeapon.y;
		float num7 = y + position.height;
		Rect position2 = new Rect(0f, 0f, crdItem.x, crdItem.y);
		num = 0;
		int num8 = 0;
		while (num < num2 && num8 < num4)
		{
			position2.y = (float)num8 * crdItem.y;
			float y2 = position2.y;
			float num9 = y2 + position2.height;
			int num10 = 0;
			while (num < num2 && num10 < num3)
			{
				if (num9 >= y && y2 <= num7)
				{
					position2.x = (float)num10 * (crdItem.x + 2f);
					GUI.BeginGroup(position2);
					TItem tItem = TItemManager.Instance.Get<TItem>(myItems[num].Code);
					if (tooltip.ItemSeq == myItems[num].Seq.ToString())
					{
						if (num10 < num3 - 2)
						{
							ltTooltip = new Vector2(rc.x + position.x + position2.x + position2.width, rc.y + position.y + position2.y - y);
						}
						else
						{
							ltTooltip = new Vector2(rc.x + position.x + position2.x - tooltip.size.x, rc.y + position.y + position2.y - y);
						}
					}
					string str = "BtnItem";
					if (tItem.season == 2)
					{
						str = "BtnItem2";
					}
					if (myItems[num] != null && myItems[num].IsLimitedByStarRate)
					{
						str = "BtnItemLock";
					}
					if (GUI.Button(crdItemBtn, new GUIContent(string.Empty, myItems[num].Seq.ToString()), str))
					{
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
							if (myItems[num].IsWeaponSlotAble)
							{
								SetDragItem(myItems[num]);
							}
						}
						else if (curWeaponSlot < 5 || premiumAccount)
						{
							if (myItems[num].IsWeaponSlotAble)
							{
								CSNetManager.Instance.Sock.SendCS_SET_WEAPON_SLOT_REQ(curWeaponSlot, myItems[num].Seq);
							}
							ResetDragItem();
						}
						curItem = num;
					}
					DrawItemIcon(crdIcon: new Rect(crdItemBtn.x + 4f, crdItemBtn.y + 14f, (float)(int)((double)tItem.CurIcon().width * 0.65), (float)(int)((double)tItem.CurIcon().height * 0.65)), item: myItems[num]);
					if (myItems[num].IsUpgradedItem())
					{
						if (myItems[num].CanUpgradeAble())
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 96f, crdItemBtn.y + 60f, 14f, 14f), GlobalVars.Instance.iconUpgrade, ScaleMode.ScaleToFit);
						}
						else
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 95f, crdItemBtn.y + 59f, 16f, 16f), GlobalVars.Instance.iconUpgradeMax, ScaleMode.ScaleToFit);
						}
					}
					if (myItems[num].IsPCBang)
					{
						TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 2f, crdItemBtn.y + 50f, 24f, 24f), GlobalVars.Instance.iconPCBang, ScaleMode.ScaleToFit);
					}
					Color color = GUI.color;
					GUI.color = GlobalVars.Instance.txtMainColor;
					GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
					GUI.color = color;
					ShowItemStatus(myItems[num]);
					LabelUtil.TextOut(crdRemain, myItems[num].GetRemainString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleRight);
					DrawStarRate(myItems[num]);
					TWeapon tWeapon = (TWeapon)myItems[num].Template;
					if (myItems[num].Durability >= 0)
					{
						float num11 = (float)myItems[num].Durability / (float)tWeapon.durabilityMax;
						num11 *= 100f;
						if (num11 > 10f && num11 <= 30f)
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + crdItemBtn.width / 2f - 24f, crdItemBtn.y + crdItemBtn.height / 2f - 33f, 48f, 46f), GlobalVars.Instance.iconWarnYellow, ScaleMode.StretchToFill);
						}
						else if (num11 <= 10f)
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + crdItemBtn.width / 2f - 24f, crdItemBtn.y + crdItemBtn.height / 2f - 33f, 48f, 46f), GlobalVars.Instance.iconWarnRed, ScaleMode.StretchToFill);
						}
					}
					if (num == curItem)
					{
						GUI.Box(new Rect(crdItemBtn.x - 3f, crdItemBtn.y - 3f, crdItemBtn.width + 6f, crdItemBtn.height + 6f), string.Empty, "BtnItemF");
					}
					GUI.EndGroup();
				}
				num++;
				num10++;
			}
			num8++;
		}
		GUI.EndScrollView();
		DoWeaponTab();
		DoWeaponButtons((0 > curItem || curItem >= myItems.Length) ? null : myItems[curItem]);
	}

	private void DrawStarRate(Item item)
	{
		if (item != null && itemStarGauge != null && itemStarGaugeBg != null)
		{
			TextureUtil.DrawTexture(crdStarGauge, itemStarGaugeBg, ScaleMode.StretchToFill);
			Rect position = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * item.starRate, crdStarGauge.height);
			GUI.BeginGroup(position);
			TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge, ScaleMode.StretchToFill);
			GUI.EndGroup();
			if (item.starRate > 1f)
			{
				float num = item.starRate - 1f;
				position = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * num, crdStarGauge.height);
				GUI.BeginGroup(position);
				TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge2, ScaleMode.StretchToFill);
				GUI.EndGroup();
			}
		}
	}

	private void DoWeaponButtons(Item item)
	{
		GUIContent content = new GUIContent(StringMgr.Instance.Get("SLOT_CLEAR").ToUpper(), GlobalVars.Instance.iconCancel);
		if (GlobalVars.Instance.MyButton3(crdInit, content, "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_CLEAR_WEAPON_SLOT_REQ();
			MyInfoManager.Instance.ClearWeaponSlot();
			ResetDragItem();
		}
		content = new GUIContent(StringMgr.Instance.Get("SLOT_SET").ToUpper(), GlobalVars.Instance.iconEquip);
		if (GlobalVars.Instance.MyButton3(crdInstall, content, "BtnAction") && (curWeaponSlot < 5 || premiumAccount) && item != null)
		{
			if (item.IsWeaponSlotAble)
			{
				CSNetManager.Instance.Sock.SendCS_SET_WEAPON_SLOT_REQ(curWeaponSlot, item.Seq);
			}
			ResetDragItem();
		}
		content = new GUIContent(StringMgr.Instance.Get("SLOT_RESET").ToUpper(), GlobalVars.Instance.iconReformat);
		if (GlobalVars.Instance.MyButton3(crdUninstall, content, "BtnAction") && (curWeaponSlot < 5 || premiumAccount))
		{
			CSNetManager.Instance.Sock.SendCS_SET_WEAPON_SLOT_REQ((sbyte)curWeaponSlot, -1L);
			ResetDragItem();
		}
	}

	private void DoShooterToolSlots()
	{
		string[] array = new string[5]
		{
			"K_SHOOTER1",
			"K_SHOOTER2",
			"K_SHOOTER3",
			"K_SHOOTER4",
			"K_SHOOTER5"
		};
		GUI.Box(crdSlotOutline, string.Empty, "BoxInnerLine");
		Rect position = new Rect(crdSlotBtn);
		GUI.BeginGroup(crdSlotOutline);
		for (int i = 0; i < MyInfoManager.Instance.ShooterTools.Length; i++)
		{
			ConsumableDesc consumableDesc = null;
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(MyInfoManager.Instance.ShooterTools[i]);
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
			{
				TSpecial tSpecial = (TSpecial)itemBySequence.Template;
				string func = TItem.FunctionMaskToString(tSpecial.functionMask);
				consumableDesc = ConsumableManager.Instance.Get(func);
				if (consumableDesc != null && !consumableDesc.isShooterTool)
				{
					consumableDesc = null;
				}
			}
			Texture2D image = null;
			if (itemBySequence != null && consumableDesc != null)
			{
				image = ((!itemBySequence.EnoughToConsume) ? consumableDesc.disable : consumableDesc.enable);
				if (tooltip.ItemSeq == ItemSlot2Tooltip(itemBySequence, i))
				{
					if (i < 2)
					{
						ltTooltip = new Vector2(rc.x + crdSlotOutline.x + position.x + position.width + 5f, rc.y + crdSlotOutline.y + position.y + 5f);
					}
					else
					{
						ltTooltip = new Vector2(rc.x + crdSlotOutline.x + position.x - tooltip.size.x - 5f, rc.y + crdSlotOutline.y + position.y + 5f);
					}
				}
			}
			if (GUI.Button(position, new GUIContent(image, ItemSlot2Tooltip(itemBySequence, i)), "BtnItemFixate"))
			{
				curActionSlot = i;
				if (dragItem != null)
				{
					if (dragItem.IsShooterSlotAble)
					{
						CSNetManager.Instance.Sock.SendCS_SET_SHOOTER_TOOL_REQ((sbyte)i, dragItem.Seq);
					}
					ResetDragItem();
				}
			}
			LabelUtil.TextOut(new Vector2(position.x + 5f, position.y + 5f), custom_inputs.Instance.GetKeyCodeName(array[i]), "MiniLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
			if (curActionSlot == i)
			{
				GUI.Box(position, string.Empty, "BtnItemFixateF");
			}
			position.x += crdSlotBtn.width + 8f;
		}
		GUI.EndGroup();
	}

	private void DoShooterTools()
	{
		DoShooterToolSlots();
		GUI.Box(crdItemActionOutline, string.Empty, "LineBoxBlue");
		int num = 0;
		myItems = MyInfoManager.Instance.GetItemsByCat(0, 5, -1);
		int num2 = myItems.Length;
		int num3 = 5;
		int num4 = num2 / num3;
		if (num2 % num3 > 0)
		{
			num4++;
		}
		float num5 = crdItem.x * (float)num3;
		if (num3 > 1)
		{
			num5 += (float)((num3 - 1) * 2);
		}
		float num6 = crdItem.y * (float)num4;
		if (num4 > 0)
		{
			num6 -= yOffset;
		}
		scrollPositionShooterTool = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdShooterToolList.width - 20f, num6), position: crdShooterToolList, scrollPosition: scrollPositionShooterTool, alwaysShowHorizontal: false, alwaysShowVertical: false);
		float y = scrollPositionShooterTool.y;
		float num7 = y + crdShooterToolList.height;
		Rect position = new Rect(0f, 0f, crdItem.x, crdItem.y);
		num = 0;
		int num8 = 0;
		while (num < num2 && num8 < num4)
		{
			position.y = (float)num8 * crdItem.y;
			float y2 = position.y;
			float num9 = y2 + position.height;
			int num10 = 0;
			while (num < num2 && num10 < num3)
			{
				if (num9 >= y && y2 <= num7)
				{
					position.x = (float)num10 * (crdItem.x + 2f);
					GUI.BeginGroup(position);
					TItem tItem = TItemManager.Instance.Get<TItem>(myItems[num].Code);
					if (tooltip.ItemSeq == myItems[num].Seq.ToString())
					{
						if (num10 < num3 - 2)
						{
							ltTooltip = new Vector2(rc.x + crdShooterToolList.x + position.x + position.width, rc.y + crdShooterToolList.y + position.y - y);
						}
						else
						{
							ltTooltip = new Vector2(rc.x + crdShooterToolList.x + position.x - tooltip.size.x, rc.y + crdShooterToolList.y + position.y - y);
						}
					}
					string str = "BtnItem";
					if (tItem.season == 2)
					{
						str = "BtnItem2";
					}
					if (GUI.Button(crdItemBtn, new GUIContent(string.Empty, myItems[num].Seq.ToString()), str))
					{
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
							if (myItems[num].IsShooterSlotAble)
							{
								SetDragItem(myItems[num]);
							}
						}
						else
						{
							if (myItems[num].IsShooterSlotAble)
							{
								CSNetManager.Instance.Sock.SendCS_SET_SHOOTER_TOOL_REQ((sbyte)curActionSlot, myItems[num].Seq);
							}
							ResetDragItem();
						}
						curItem = num;
					}
					DrawItemIcon(crdIcon: new Rect(crdItemBtn.x + 4f, crdItemBtn.y + 14f, (float)(int)((double)tItem.CurIcon().width * 0.65), (float)(int)((double)tItem.CurIcon().height * 0.65)), item: myItems[num]);
					Color color = GUI.color;
					GUI.color = GlobalVars.Instance.txtMainColor;
					GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
					GUI.color = color;
					ShowItemStatus(myItems[num]);
					LabelUtil.TextOut(crdRemain, myItems[num].GetRemainString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleRight);
					DrawStarRate(myItems[num]);
					if (num == curItem)
					{
						GUI.Box(new Rect(crdItemBtn.x - 3f, crdItemBtn.y - 3f, crdItemBtn.width + 6f, crdItemBtn.height + 6f), string.Empty, "BtnItemF");
					}
					GUI.EndGroup();
				}
				num++;
				num10++;
			}
			num8++;
		}
		GUI.EndScrollView();
		DoShooterToolButtons((0 > curItem || curItem >= myItems.Length) ? null : myItems[curItem]);
	}

	private void DoShooterToolButtons(Item item)
	{
		GUIContent content = new GUIContent(StringMgr.Instance.Get("SLOT_CLEAR").ToUpper(), GlobalVars.Instance.iconCancel);
		if (GlobalVars.Instance.MyButton3(crdInit, content, "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_CLEAR_SHOOTER_TOOLS_REQ();
			MyInfoManager.Instance.ClearShooterTool();
		}
		content = new GUIContent(StringMgr.Instance.Get("SLOT_SET").ToUpper(), GlobalVars.Instance.iconEquip);
		if (GlobalVars.Instance.MyButton3(crdInstall, content, "BtnAction"))
		{
			if (item != null && item.IsShooterSlotAble)
			{
				CSNetManager.Instance.Sock.SendCS_SET_SHOOTER_TOOL_REQ((sbyte)curActionSlot, item.Seq);
			}
			ResetDragItem();
		}
		content = new GUIContent(StringMgr.Instance.Get("SLOT_RESET").ToUpper(), GlobalVars.Instance.iconReformat);
		if (GlobalVars.Instance.MyButton3(crdUninstall, content, "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_SET_SHOOTER_TOOL_REQ((sbyte)curActionSlot, -1L);
		}
	}

	private void DoTooltip(Vector2 offset)
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.ITEM_SETTING && !IsDragging)
		{
			if (lastTooltip != GUI.tooltip)
			{
				focusTime = 0f;
				tooltip.ItemSeq = GUI.tooltip;
				if (tooltip.ItemSeq.Length <= 0)
				{
					tooltip.ItemCode = string.Empty;
				}
				else
				{
					int num = -1;
					try
					{
						string text = tooltip.ItemSeq;
						if (text[0] == '*')
						{
							text = text.Substring(2);
						}
						num = int.Parse(text);
					}
					catch
					{
					}
					if (num >= 0)
					{
						Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(num);
						if (itemBySequence != null)
						{
							tooltip.SetItem(itemBySequence);
							tooltip.ItemCode = itemBySequence.Template.code;
							if (!DialogManager.Instance.IsModal)
							{
								GlobalVars.Instance.PlaySoundMouseOver();
							}
						}
					}
				}
			}
			if (focusTime > 0.3f)
			{
				Vector2 coord = ltTooltip;
				float num2 = coord.y + tooltip.size.y;
				if (num2 > size.y)
				{
					coord.y -= num2 - size.y;
				}
				tooltip.SetCoord(coord);
				GUI.Box(tooltip.ClientRect, string.Empty, "TooltipWindow");
				GUI.BeginGroup(tooltip.ClientRect);
				tooltip.DoDialog();
				GUI.EndGroup();
			}
			lastTooltip = GUI.tooltip;
		}
	}

	public override bool DoDialog()
	{
		premiumAccount = (MyInfoManager.Instance.HaveFunction("premium_account") >= 0);
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Rect position = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		GUI.Box(position, string.Empty, "Window");
		GUI.BeginGroup(position);
		DoTitle();
		DoMainTab();
		switch (mainTab)
		{
		case 0:
			DoWeapons();
			break;
		case 1:
			DoShooterTools();
			break;
		}
		if (GlobalVars.Instance.MyButton(crdX, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
			ResetDragItem();
		}
		DoTooltip(new Vector2(position.x, position.y));
		GUI.EndGroup();
		DragItem();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
