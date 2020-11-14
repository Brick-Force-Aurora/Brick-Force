using System;
using UnityEngine;

[Serializable]
public class Equipment
{
	public GameObject mySelf;

	public Color disabledColor = new Color(1f, 1f, 1f, 0.7f);

	public Texture2D selectedFrame;

	public string[] mainTabKey;

	public string[] weaponTabKey;

	public string[] clothTabKey;

	public string[] accessoryTabKey;

	public string[] treasureChestTabKey;

	public string[] mainWeaponTabKey;

	private string[] mainTabs;

	private string[] weaponTabs;

	private string[] clothTabs;

	private string[] accessoryTabs;

	private string[] mainWeaponTabs;

	public Tooltip tooltip;

	private float lastClickTime;

	private float doubleClickTimeout = 0.2f;

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private int mainTab;

	private int[] subTab;

	private Vector2[] scrollPosition;

	private Vector2 scrollPositionTree = Vector2.zero;

	private Item[] myItems;

	private int curItem;

	private Rect crdItemList = new Rect(255f, 118f, 755f, 575f);

	private Rect crdItemListTemp = new Rect(255f, 118f, 755f, 575f);

	private float chatGap = 260f;

	private Vector2 crdItem = new Vector2(160f, 136f);

	private Vector2 crdItemOffset = new Vector2(24f, 22f);

	private Rect crdItemBtn = new Rect(3f, 3f, 154f, 130f);

	private Rect crdItemIcon = new Rect(12f, 6f, 142f, 100f);

	private Rect crdRepairItem = new Rect(56f, 28f, 48f, 46f);

	private Rect crdItemSelect = new Rect(0f, 0f, 160f, 136f);

	private Vector2 crdItemUsage = new Vector2(156f, 107f);

	private Vector2 crdRemain = new Vector2(156f, 122f);

	private Rect crdTree = new Rect(10f, 500f, 230f, 242f);

	private Rect crdFunc = new Rect(255f, 721f, 38f, 38f);

	private Rect crdDeleteBtn = new Rect(730f, 704f, 136f, 34f);

	private Rect crdInstall = new Rect(865f, 704f, 136f, 34f);

	private Rect crdComboFilter = new Rect(784f, 73f, 180f, 30f);

	private Color txtMainColor = new Color(1f, 1f, 1f, 1f);

	private TreeInfo[] treeInfos;

	private int wpnCategory;

	private string tooltipMessage = string.Empty;

	private bool durabilityFullClicked;

	private bool equipable = true;

	private bool unequipable = true;

	private int weaponTab = 1;

	private string[] filterKey = new string[5]
	{
		"ALL",
		"USABLE",
		"NOT_USING",
		"UPGRADABLE",
		"MERGABLE"
	};

	private string[] filterKeyNetmable = new string[4]
	{
		"ALL",
		"USABLE",
		"NOT_USING",
		"MERGABLE"
	};

	private ComboBox cbox;

	private int selFilter;

	private Rect crdStarGauge = new Rect(12f, 116f, 64f, 12f);

	public Texture2D itemStarGauge;

	public Texture2D itemStarGaugeBg;

	public Texture2D itemStarGauge2;

	private bool chatView;

	private float focusTime;

	public int CurItem
	{
		set
		{
			curItem = value;
		}
	}

	public bool Equipable
	{
		get
		{
			return equipable;
		}
		set
		{
			equipable = value;
		}
	}

	public bool Unequipable
	{
		get
		{
			return unequipable;
		}
		set
		{
			unequipable = value;
		}
	}

	public bool CheckFilterCombo()
	{
		if (cbox == null)
		{
			return false;
		}
		return cbox.IsClickedComboButton();
	}

	public void DoFilterCombo()
	{
		string[] array = (!BuildOption.Instance.IsNetmarble && BuildOption.Instance.Props.ApplyItemUpgrade) ? filterKey : filterKeyNetmable;
		GUIContent[] array2 = new GUIContent[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new GUIContent(StringMgr.Instance.Get(array[i]));
		}
		selFilter = cbox.List(crdComboFilter, StringMgr.Instance.Get(array[selFilter]), array2);
	}

	public void Start()
	{
		cbox = new ComboBox();
		cbox.Initialize(bImage: false, new Vector2(crdComboFilter.width, crdComboFilter.height));
		cbox.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
		cbox.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
		cbox.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
		tooltip.Start();
		curItem = 0;
		mainTab = 0;
		wpnCategory = -1;
		mainTabs = new string[GlobalVars.Instance.equipParentDirNames.Length];
		subTab = new int[20];
		treeInfos = new TreeInfo[GlobalVars.Instance.equipParentDirNames.Length];
		scrollPosition = new Vector2[GlobalVars.Instance.equipParentDirNames.Length];
		for (int i = 0; i < GlobalVars.Instance.equipParentDirNames.Length; i++)
		{
			subTab[i] = 0;
			scrollPosition[i] = Vector2.zero;
			mainTabs[i] = StringMgr.Instance.Get(GlobalVars.Instance.equipParentDirNames[i]);
			treeInfos[i] = new TreeInfo();
			treeInfos[i].childTrees = null;
			treeInfos[i].Name = mainTabs[i];
			if (i == mainTab)
			{
				treeInfos[i].clicked = true;
			}
		}
		weaponTabs = new string[weaponTabKey.Length];
		treeInfos[weaponTab].childTrees = new TreeInfo[weaponTabKey.Length - 1];
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			weaponTabs[j] = StringMgr.Instance.Get(weaponTabKey[j]);
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[weaponTab].childTrees[j - 1] = new TreeInfo();
				treeInfos[weaponTab].childTrees[j - 1].Name = weaponTabs[j];
			}
		}
		mainWeaponTabs = new string[GlobalVars.Instance.ShopMainWpnCatNames.Length];
		treeInfos[weaponTab].childTrees[0].childTrees = new TreeInfo[GlobalVars.Instance.ShopMainWpnCatNames.Length];
		for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
		{
			mainWeaponTabs[k] = StringMgr.Instance.Get(GlobalVars.Instance.ShopMainWpnCatNames[k]);
			if (treeInfos.Length > 0)
			{
				treeInfos[weaponTab].childTrees[0].childTrees[k] = new TreeInfo();
				treeInfos[weaponTab].childTrees[0].childTrees[k].Name = mainWeaponTabs[k];
			}
		}
		clothTabs = new string[clothTabKey.Length];
		treeInfos[weaponTab + 1].childTrees = new TreeInfo[clothTabs.Length - 1];
		for (int l = 0; l < clothTabKey.Length; l++)
		{
			clothTabs[l] = StringMgr.Instance.Get(clothTabKey[l]);
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[weaponTab + 1].childTrees[l - 1] = new TreeInfo();
				treeInfos[weaponTab + 1].childTrees[l - 1].Name = clothTabs[l];
			}
		}
		accessoryTabs = new string[GlobalVars.Instance.accessoryTabs.Length];
		treeInfos[weaponTab + 2].childTrees = new TreeInfo[GlobalVars.Instance.accessoryTabs.Length - 1];
		for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
		{
			accessoryTabs[m] = StringMgr.Instance.Get(GlobalVars.Instance.accessoryTabs[m]);
			if (treeInfos.Length > 0 && m > 0)
			{
				treeInfos[weaponTab + 2].childTrees[m - 1] = new TreeInfo();
				treeInfos[weaponTab + 2].childTrees[m - 1].Name = accessoryTabs[m];
			}
		}
		equipable = true;
		unequipable = true;
		txtMainColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
	}

	private void reloadTreeName()
	{
		for (int i = 0; i < GlobalVars.Instance.equipParentDirNames.Length; i++)
		{
			mainTabs[i] = StringMgr.Instance.Get(GlobalVars.Instance.equipParentDirNames[i]);
			treeInfos[i].Name = mainTabs[i];
		}
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			weaponTabs[j] = StringMgr.Instance.Get(weaponTabKey[j]);
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[weaponTab].childTrees[j - 1].Name = weaponTabs[j];
			}
		}
		for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
		{
			mainWeaponTabs[k] = StringMgr.Instance.Get(GlobalVars.Instance.ShopMainWpnCatNames[k]);
			if (treeInfos.Length > 0)
			{
				treeInfos[weaponTab].childTrees[0].childTrees[k].Name = mainWeaponTabs[k];
			}
		}
		for (int l = 0; l < clothTabKey.Length; l++)
		{
			clothTabs[l] = StringMgr.Instance.Get(clothTabKey[l]);
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[weaponTab + 1].childTrees[l - 1].Name = clothTabs[l];
			}
		}
		for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
		{
			accessoryTabs[m] = StringMgr.Instance.Get(GlobalVars.Instance.accessoryTabs[m]);
			if (treeInfos.Length > 0 && m > 0)
			{
				treeInfos[weaponTab + 2].childTrees[m - 1].Name = accessoryTabs[m];
			}
		}
	}

	private void ShowTooltip2(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private int calcScrollHeight()
	{
		int num = 0;
		for (int i = 0; i < treeInfos.Length; i++)
		{
			if (treeInfos[i].childTrees != null && treeInfos[i].childTrees.Length > 0 && treeInfos[i].bExpand)
			{
				num += 20;
				for (int j = 0; j < treeInfos[i].childTrees.Length; j++)
				{
					num += 20;
					if (treeInfos[i].childTrees[j].childTrees != null && treeInfos[i].childTrees[j].childTrees.Length > 0 && treeInfos[i].childTrees[j].bExpand)
					{
						num += 20;
						for (int k = 0; k < treeInfos[i].childTrees[j].childTrees.Length; k++)
						{
							if (k < treeInfos[i].childTrees[j].childTrees.Length - 1)
							{
								num += 20;
							}
						}
					}
				}
			}
			if (!treeInfos[i].bExpand)
			{
				num += 20;
			}
		}
		return num + 10;
	}

	private void treeClickedNone()
	{
		for (int i = 0; i < GlobalVars.Instance.equipParentDirNames.Length; i++)
		{
			treeInfos[i].clicked = false;
		}
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[weaponTab].childTrees[j - 1].clicked = false;
			}
			if (j == 0)
			{
				for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
				{
					if (treeInfos[weaponTab].childTrees[0].childTrees.Length > 0)
					{
						treeInfos[weaponTab].childTrees[0].childTrees[k].clicked = false;
					}
				}
			}
		}
		for (int l = 0; l < clothTabKey.Length; l++)
		{
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[weaponTab + 1].childTrees[l - 1].clicked = false;
			}
		}
		for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
		{
			if (treeInfos.Length > 0 && m > 0)
			{
				treeInfos[weaponTab + 2].childTrees[m - 1].clicked = false;
			}
		}
	}

	public void Default()
	{
		mainTab = 0;
		subTab[mainTab] = 0;
		for (int i = 0; i < GlobalVars.Instance.equipParentDirNames.Length; i++)
		{
			treeInfos[i].bExpand = false;
			if (i == mainTab)
			{
				treeInfos[i].clicked = true;
			}
			else
			{
				treeInfos[i].clicked = false;
			}
		}
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[weaponTab].childTrees[j - 1].clicked = false;
				treeInfos[weaponTab].childTrees[j - 1].bExpand = false;
			}
		}
		for (int k = 0; k < clothTabKey.Length; k++)
		{
			if (treeInfos.Length > 0 && k > 0)
			{
				treeInfos[weaponTab + 1].childTrees[k - 1].clicked = false;
				treeInfos[weaponTab + 1].childTrees[k - 1].bExpand = false;
			}
		}
		for (int l = 0; l < GlobalVars.Instance.accessoryTabs.Length; l++)
		{
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[weaponTab + 2].childTrees[l - 1].clicked = false;
				treeInfos[weaponTab + 2].childTrees[l - 1].bExpand = false;
			}
		}
		reloadTreeName();
	}

	private bool IsWeaponCategory()
	{
		return (mainTab == weaponTab) ? true : false;
	}

	private void VerifyChatView()
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				chatView = component.bChatView;
			}
			Briefing4TeamMatch component2 = gameObject.GetComponent<Briefing4TeamMatch>();
			if (null != component2)
			{
				chatView = component2.bChatView;
			}
		}
	}

	private void DoEquipment()
	{
		int num = 0;
		if (IsPremiumPCbangTab())
		{
			myItems = MyInfoManager.Instance.GetPCBangPremiumItems(mainTab);
		}
		else if (BuildOption.Instance.IsNetmarble || !BuildOption.Instance.Props.ApplyItemUpgrade)
		{
			if (selFilter == 3)
			{
				myItems = MyInfoManager.Instance.GetItemsByCat(selFilter + 1, mainTab, subTab[mainTab], wpnCategory);
			}
			else
			{
				myItems = MyInfoManager.Instance.GetItemsByCat(selFilter, mainTab, subTab[mainTab], wpnCategory);
			}
		}
		else
		{
			myItems = MyInfoManager.Instance.GetItemsByCat(selFilter, mainTab, subTab[mainTab], wpnCategory);
		}
		int num2 = myItems.Length;
		int num3 = 4;
		int num4 = num2 / num3;
		if (num2 % num3 > 0)
		{
			num4++;
		}
		VerifyChatView();
		if (chatView)
		{
			crdItemList.height = 315f;
		}
		else
		{
			crdItemList.height = crdItemListTemp.height;
		}
		float num5 = crdItem.x * (float)num3;
		if (num3 > 0)
		{
			num5 += crdItemOffset.x * (float)(num3 - 1);
		}
		float num6 = crdItem.y * (float)num4;
		if (num4 > 0)
		{
			num6 += crdItemOffset.y * (float)(num4 - 1);
		}
		Rect viewRect = new Rect(0f, 0f, num5, num6);
		scrollPosition[mainTab] = GUI.BeginScrollView(crdItemList, scrollPosition[mainTab], viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false);
		float y = scrollPosition[mainTab].y;
		float num7 = y + crdItemList.height;
		Rect position = new Rect(0f, 0f, crdItem.x, crdItem.y);
		num = 0;
		int rebuyCount = 0;
		int num8 = 0;
		while (num < num2 && num8 < num4)
		{
			position.y = (float)num8 * crdItem.y;
			if (num8 > 0)
			{
				position.y += (float)num8 * crdItemOffset.y;
			}
			float y2 = position.y;
			float num9 = y2 + position.height;
			int num10 = 0;
			while (num < num2 && num10 < num3)
			{
				position.x = (float)num10 * crdItem.x;
				if (num10 > 0)
				{
					position.x += (float)num10 * crdItemOffset.x;
				}
				if (num9 >= y && y2 <= num7)
				{
					GUI.BeginGroup(position);
					TItem tItem = TItemManager.Instance.Get<TItem>(myItems[num].Code);
					Item item = myItems[num];
					if (tooltip.ItemSeq == myItems[num].Seq.ToString())
					{
						if (num10 < num3 - 2)
						{
							ltTooltip = new Vector2(crdItemList.x + position.x + crdItem.x, crdItemList.y + position.y - y);
						}
						else
						{
							ltTooltip = new Vector2(crdItemList.x + position.x - tooltip.size.x, crdItemList.y + position.y - y);
						}
						float num11 = ltTooltip.y + tooltip.size.y;
						if (num11 > GlobalVars.Instance.UIScreenRect.height)
						{
							ltTooltip.y -= num11 - GlobalVars.Instance.UIScreenRect.height;
						}
					}
					string str = "BtnItem";
					if (tItem.season == 2)
					{
						str = "BtnItem2";
					}
					if (myItems[num].IsLimitedByStarRate)
					{
						str = "BtnItemLock";
					}
					if (GUI.Button(crdItemBtn, new GUIContent(string.Empty, myItems[num].Seq.ToString()), str))
					{
						if (IsWeaponCategory())
						{
							TWeapon tWeapon = (TWeapon)item.Template;
							durabilityFullClicked = (item.Durability < 0 || item.Durability >= tWeapon.durabilityMax);
						}
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
						else if (item.Usage == Item.USAGE.UNEQUIP)
						{
							if (item.IsEquipable)
							{
								if (item.IsLimitedByStarRate)
								{
									SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
								}
								else
								{
									GlobalVars.Instance.PlaySoundItemInstall();
									CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(item.Seq);
								}
							}
							else if (item.Template.type == TItem.TYPE.BUNDLE && item.Amount > 0)
							{
								((Sure2UnpackDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SURE2UNPACK, exclusive: true))?.InitDialog(item, (TBundle)item.Template);
							}
						}
						else if (item.Usage == Item.USAGE.NOT_USING)
						{
							if (item.IsLimitedByStarRate)
							{
								SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
							}
							else
							{
								((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.INIT_ITEM);
							}
						}
						curItem = num;
					}
					bool bForceDark = false;
					DrawItemIcon(myItems[num], crdItemIcon, bForceDark);
					if (item.IsUpgradedItem())
					{
						if (item.CanUpgradeAble())
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 132f, crdItemBtn.y + 73f, 16f, 16f), GlobalVars.Instance.iconUpgrade, ScaleMode.ScaleToFit);
						}
						else
						{
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 129f, crdItemBtn.y + 70f, 22f, 22f), GlobalVars.Instance.iconUpgradeMax, ScaleMode.ScaleToFit);
						}
					}
					if (item.IsPCBang)
					{
						TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 3f, crdItemBtn.y + 67f, (float)GlobalVars.Instance.iconPCBang.width, (float)GlobalVars.Instance.iconPCBang.height), GlobalVars.Instance.iconPCBang, ScaleMode.ScaleToFit);
					}
					Color color = GUI.color;
					GUI.color = txtMainColor;
					GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
					GUI.color = color;
					ShowItemStatus(myItems[num]);
					if (itemStarGauge != null && itemStarGaugeBg != null)
					{
						TextureUtil.DrawTexture(crdStarGauge, itemStarGaugeBg, ScaleMode.StretchToFill);
						Rect position2 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * myItems[num].starRate, crdStarGauge.height);
						GUI.BeginGroup(position2);
						TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge, ScaleMode.StretchToFill);
						GUI.EndGroup();
						if (myItems[num].starRate > 1f)
						{
							float num12 = myItems[num].starRate - 1f;
							position2 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * num12, crdStarGauge.height);
							GUI.BeginGroup(position2);
							TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge2, ScaleMode.StretchToFill);
							GUI.EndGroup();
						}
					}
					LabelUtil.TextOut(crdRemain, myItems[num].GetRemainString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					if (IsWeaponCategory())
					{
						TWeapon tWeapon2 = (TWeapon)item.Template;
						if (item.Durability >= 0)
						{
							float num13 = (float)item.Durability / (float)tWeapon2.durabilityMax;
							num13 *= 100f;
							if (num13 > 10f && num13 <= 30f)
							{
								TextureUtil.DrawTexture(crdRepairItem, GlobalVars.Instance.iconWarnYellow, ScaleMode.ScaleToFit);
							}
							else if (num13 <= 10f)
							{
								TextureUtil.DrawTexture(crdRepairItem, GlobalVars.Instance.iconWarnRed, ScaleMode.ScaleToFit);
							}
						}
					}
					if (num == curItem)
					{
						GUI.Box(crdItemSelect, string.Empty, "BtnItemF");
					}
					GUI.EndGroup();
				}
				num++;
				num10++;
			}
			num8++;
		}
		GUI.EndScrollView();
		if (0 <= curItem && curItem < myItems.Length)
		{
			DoAllFunction(myItems[curItem]);
			DoMainButtonOnContext(myItems[curItem], rebuyCount);
		}
	}

	public void OnGUI()
	{
		if (GlobalVars.Instance.bEraseItemOk)
		{
			curItem = 0;
			GlobalVars.Instance.bEraseItemOk = false;
		}
		DoEquipment();
		DoTreeView();
		DoTooltip();
	}

	private void DoTreeView()
	{
		scrollPositionTree = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdTree.width - 20f, (float)calcScrollHeight()), position: crdTree, scrollPosition: scrollPositionTree, alwaysShowHorizontal: false, alwaysShowVertical: false);
		GUIStyle style = GUI.skin.GetStyle("BtnSelect");
		Vector2 pos = new Vector2(0f, 0f);
		for (int i = 0; i < treeInfos.Length; i++)
		{
			if (BuildOption.Instance.Props.ApplyItemUpgrade || !treeInfos[i].Name.Equals(StringMgr.Instance.Get("UPGRADE")))
			{
				Vector2 vector = LabelUtil.CalcLength("MiniLabel", treeInfos[i].Name);
				style.normal.textColor = txtMainColor;
				Rect rect = new Rect(pos.x, pos.y, vector.x + 10f, vector.y);
				float x = pos.x;
				if (GlobalVars.Instance.MyButton(rect, treeInfos[i].Name, "BtnSelect"))
				{
					wpnCategory = -1;
					mainTab = i;
					subTab[mainTab] = 0;
					curItem = 0;
					treeClickedNone();
					treeInfos[i].clicked = true;
				}
				if (treeInfos[i].clicked)
				{
					GUI.Box(rect, string.Empty, "BtnSelectF");
				}
				if (treeInfos[i].childTrees != null && treeInfos[i].childTrees.Length > 0)
				{
					string empty = string.Empty;
					empty = ((!treeInfos[i].bExpand) ? "BtnArrowDn" : "BtnArrowUp");
					Rect rc = new Rect(pos.x + vector.x + 16f, pos.y, 25f, 23f);
					if (GlobalVars.Instance.MyButton(rc, string.Empty, empty))
					{
						treeInfos[i].bExpand = !treeInfos[i].bExpand;
					}
					if (treeInfos[i].bExpand)
					{
						pos.x += 10f;
						pos.y += 20f;
						float x2 = pos.x;
						Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(243, 189, 116);
						style.normal.textColor = byteColor2FloatColor;
						for (int j = 0; j < treeInfos[i].childTrees.Length; j++)
						{
							Vector2 vector2 = LabelUtil.CalcLength("MiniLabel", treeInfos[i].childTrees[j].Name);
							Rect rect2 = new Rect(pos.x, pos.y, vector2.x + 10f, vector2.y);
							if (GlobalVars.Instance.MyButton(rect2, string.Empty, "BtnSelect"))
							{
								wpnCategory = -1;
								mainTab = i;
								subTab[mainTab] = j + 1;
								curItem = 0;
								treeClickedNone();
								treeInfos[i].childTrees[j].clicked = true;
							}
							if (treeInfos[i].childTrees[j].clicked)
							{
								GUI.Box(rect2, string.Empty, "BtnSelectF");
							}
							LabelUtil.TextOut(pos, "." + treeInfos[i].childTrees[j].Name, "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
							if (treeInfos[i].childTrees[j].childTrees != null && treeInfos[i].childTrees[j].childTrees.Length > 0)
							{
								empty = string.Empty;
								empty = ((!treeInfos[i].childTrees[j].bExpand) ? "BtnArrowDn" : "BtnArrowUp");
								Rect rc2 = new Rect(pos.x + vector2.x + 16f, pos.y, 25f, 23f);
								if (GlobalVars.Instance.MyButton(rc2, string.Empty, empty))
								{
									treeInfos[i].childTrees[j].bExpand = !treeInfos[i].childTrees[j].bExpand;
								}
								if (treeInfos[i].childTrees[j].bExpand)
								{
									pos.x += 10f;
									pos.y += 20f;
									for (int k = 0; k < treeInfos[i].childTrees[j].childTrees.Length; k++)
									{
										Vector2 vector3 = LabelUtil.CalcLength("MiniLabel", treeInfos[i].childTrees[j].childTrees[k].Name);
										Rect rect3 = new Rect(pos.x, pos.y, vector3.x + 10f, vector3.y);
										if (GlobalVars.Instance.MyButton(rect3, string.Empty, "BtnSelect"))
										{
											int[] array = new int[4]
											{
												1,
												0,
												3,
												2
											};
											wpnCategory = array[k];
											mainTab = i;
											subTab[mainTab] = j + 1;
											curItem = 0;
											treeClickedNone();
											treeInfos[i].childTrees[j].childTrees[k].clicked = true;
										}
										if (treeInfos[i].childTrees[j].childTrees[k].clicked)
										{
											GUI.Box(rect3, string.Empty, "BtnSelectF");
										}
										LabelUtil.TextOut(pos, "." + treeInfos[i].childTrees[j].childTrees[k].Name, "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
										if (k < treeInfos[i].childTrees[j].childTrees.Length - 1)
										{
											pos.y += 20f;
										}
									}
								}
							}
							pos.x = x2;
							pos.y += 20f;
						}
					}
				}
				pos.x = x;
				if (!treeInfos[i].bExpand)
				{
					pos.y += 20f;
				}
			}
		}
		GUI.EndScrollView();
	}

	private void DoTooltip()
	{
		if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.enabled)
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
					long num = -1L;
					try
					{
						num = long.Parse(tooltip.ItemSeq);
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
				if (tooltip.ItemCode.Length > 0 && ltTooltip != Vector2.zero)
				{
					ltTooltip = GlobalVars.Instance.ToGUIPoint(ltTooltip);
					tooltip.SetCoord(ltTooltip);
					GUI.Window(1101, tooltip.ClientRect, ShowTooltip, string.Empty, "TooltipWindow");
				}
				else if (GUI.tooltip.Length > 0)
				{
					tooltipMessage = GUI.tooltip;
					Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
					GUIStyle style = GUI.skin.GetStyle("MiniLabel");
					if (style != null)
					{
						Vector2 vector2 = style.CalcSize(new GUIContent(tooltipMessage));
						Rect rc = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
						GlobalVars.Instance.FitRightNBottomRectInScreen(ref rc);
						GUI.Window(1102, rc, ShowTooltip2, string.Empty, "LineWindow");
					}
				}
			}
			lastTooltip = GUI.tooltip;
		}
		else
		{
			GUI.tooltip = string.Empty;
		}
	}

	private void DoAllFunction(Item item)
	{
		float num = 0f;
		float num2 = crdFunc.y;
		if (chatView)
		{
			num2 -= chatGap;
		}
		Color clrText = new Color(0.1953125f, 0.74609375f, 0.06640625f, 1f);
		Rect rc = new Rect(crdFunc.x + num, num2, 38f, 38f);
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			LabelUtil.TextOut(new Vector2(rc.x + 19f, rc.y - 7f), StringMgr.Instance.Get("ITEM_SETTING"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		if (GlobalVars.Instance.MyButton(rc, new GUIContent(string.Empty, StringMgr.Instance.Get("ITEM_SETTING")), "BtnItemSetting"))
		{
			((ItemSettingDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ITEM_SETTING, exclusive: true))?.InitDialog(mainTab);
		}
		if (!IsPremiumPCbangTab())
		{
			num += 48f;
			Rect rc2 = new Rect(crdFunc.x + num, num2, 38f, 38f);
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				LabelUtil.TextOut(new Vector2(rc2.x + 19f, rc2.y - 7f), StringMgr.Instance.Get("ERASE_ALL"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			if (GlobalVars.Instance.MyButton(rc2, new GUIContent(string.Empty, StringMgr.Instance.Get("ERASE_ALL")), "BtnExpired"))
			{
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(AreYouSure.SURE.ERASE_ALL_EXPIRED_ITEM);
			}
		}
		num += 48f;
		Rect rc3 = new Rect(crdFunc.x + num, num2, 38f, 38f);
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			LabelUtil.TextOut(new Vector2(rc3.x + 19f, rc3.y - 7f), StringMgr.Instance.Get("UNWEAR_ALL"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		if (GlobalVars.Instance.MyButton(rc3, new GUIContent(string.Empty, StringMgr.Instance.Get("UNWEAR_ALL")), "BtnUnEquip"))
		{
			Item[] usingItems = MyInfoManager.Instance.GetUsingItems();
			for (int i = 0; i < usingItems.Length; i++)
			{
				if (usingItems[i].IsTakeoffable)
				{
					CSNetManager.Instance.Sock.SendCS_UNEQUIP_REQ(usingItems[i].Seq);
				}
			}
			MyInfoManager.Instance.EquipDefaultItems();
		}
		if (BuildOption.Instance.Props.ApplyItemUpgrade && item.Usage != Item.USAGE.DELETED && item.Usage != Item.USAGE.NOT_USING && item.Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE && item.CanUpgradeAble())
		{
			num += 48f;
			Rect rc4 = new Rect(crdFunc.x + num, num2, 38f, 38f);
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				LabelUtil.TextOut(new Vector2(rc4.x + 19f, rc4.y - 7f), StringMgr.Instance.Get("ITEM_UPGRADE"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			if (GlobalVars.Instance.MyButton(rc4, new GUIContent(string.Empty, StringMgr.Instance.Get("ITEM_UPGRADE")), "BtnItemUpgrade"))
			{
				((ItemUpgradeDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ITEM_UPGRADE, exclusive: true))?.InitDialog(item);
			}
		}
		if (item.Usage == Item.USAGE.NOT_USING && item.IsDecomposable)
		{
			num += 48f;
			Rect rc5 = new Rect(crdFunc.x + num, num2, 38f, 38f);
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				LabelUtil.TextOut(new Vector2(rc5.x + 19f, rc5.y - 7f), StringMgr.Instance.Get("DECOMPOSE_ITEM"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			if (GlobalVars.Instance.MyButton(rc5, new GUIContent(string.Empty, StringMgr.Instance.Get("DECOMPOSE_ITEM")), "BtnHammer"))
			{
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.DECOMPOSE_ITEM);
			}
		}
		if (item.CanBeMerged())
		{
			num += 48f;
			Rect rc6 = new Rect(crdFunc.x + num, num2, 38f, 38f);
			if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
			{
				LabelUtil.TextOut(new Vector2(rc6.x + 19f, rc6.y - 7f), StringMgr.Instance.Get("COMPOSE"), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			}
			if (GlobalVars.Instance.MyButton(rc6, new GUIContent(string.Empty, StringMgr.Instance.Get("COMPOSE")), "BtnCompose"))
			{
				Item[] itemsCanMerge = MyInfoManager.Instance.GetItemsCanMerge(item);
				if (itemsCanMerge.Length < 1)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("COMPOSE_NOT_FOUND"));
				}
				else
				{
					((ItemCombineDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ITEM_COMBINE, exclusive: true))?.InitDialog(item);
				}
			}
		}
		if (IsWeaponCategory() || mainTab == 0)
		{
			TWeapon tWeapon = item.Template as TWeapon;
			if (tWeapon != null)
			{
				float num3 = (item.Durability >= 0) ? ((float)item.Durability / (float)tWeapon.durabilityMax) : 1f;
				num3 *= 100f;
				num += 48f;
				Rect rc7 = new Rect(crdFunc.x + num, num2, 38f, 38f);
				if (!durabilityFullClicked && num3 >= 0f && num3 < 100f && GlobalVars.Instance.MyButton(rc7, new GUIContent(string.Empty, StringMgr.Instance.Get("REPAIR_ITEM")), "BtnRepair"))
				{
					((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.REPAIR_ITEM, ((float)tWeapon.durabilityMax - (float)item.Durability) / (float)tWeapon.durabilityMax);
				}
			}
		}
	}

	private void DrawItemIcon(Item item, Rect crdIcon, bool bForceDark)
	{
		Color color = GUI.color;
		if (item.Usage == Item.USAGE.DELETED)
		{
			GUI.color = disabledColor;
		}
		if (item.IsPremium)
		{
			long num = MyInfoManager.Instance.HaveFunction("premium_account");
			if (num < 0)
			{
				GUI.color = disabledColor;
			}
		}
		if (bForceDark)
		{
			GUI.color = Color.black;
		}
		TextureUtil.DrawTexture(crdIcon, item.Template.CurIcon(), ScaleMode.ScaleToFit);
		GUI.color = color;
	}

	private void ShowTooltip(int id)
	{
		tooltip.DoDialog();
	}

	public void Update()
	{
		focusTime += Time.deltaTime;
		if (GlobalVars.Instance.bReceivedAck)
		{
			GlobalVars.Instance.bReceivedAck = false;
		}
	}

	private void ShowItemStatus(Item item)
	{
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(31, 220, 0);
		switch (item.Usage)
		{
		case Item.USAGE.UNEQUIP:
			break;
		case Item.USAGE.EQUIP:
			byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(byte.MaxValue, 72, 48);
			LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("USING"), "MiniLabel", byteColor2FloatColor, Color.black, TextAnchor.LowerRight);
			break;
		case Item.USAGE.NOT_USING:
			LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("NOT_USING"), "MiniLabel", byteColor2FloatColor, Color.black, TextAnchor.LowerRight);
			break;
		}
	}

	private void DoMainButtonOnContext(Item item, int rebuyCount)
	{
		GUIStyle style = GUI.skin.GetStyle("BtnAction");
		style.fontStyle = FontStyle.Bold;
		Rect rc = new Rect(crdInstall);
		if (chatView)
		{
			rc.y -= chatGap;
		}
		Rect rc2 = new Rect(crdDeleteBtn);
		if (chatView)
		{
			rc2.y -= chatGap;
		}
		switch (item.Usage)
		{
		case Item.USAGE.DELETED:
			if (!IsPremiumPCbangTab())
			{
				Good good = ShopManager.Instance.Get(item.Template.code);
				if (good != null)
				{
					bool enabled = GUI.enabled;
					GUI.enabled = (enabled && good.IsRebuyable());
					if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("REBUY"), "BtnAction"))
					{
						((BuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_TERM, exclusive: true))?.InitDialog(item.Seq, good);
					}
					GUI.enabled = enabled;
				}
				if (GlobalVars.Instance.MyButton(rc2, StringMgr.Instance.Get("DELETE"), "BtnAction"))
				{
					((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.ERASE_AN_EXPIRED_ITEM);
				}
			}
			break;
		case Item.USAGE.EQUIP:
			if (item.IsTakeoffable && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("TAKEOFF"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_UNEQUIP_REQ(item.Seq);
			}
			break;
		case Item.USAGE.UNEQUIP:
			if (item.IsEquipable && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("EQUIP"), "BtnAction"))
			{
				if (item.IsLimitedByStarRate)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
				}
				else
				{
					GlobalVars.Instance.PlaySoundItemInstall();
					CSNetManager.Instance.Sock.SendCS_EQUIP_REQ(item.Seq);
				}
			}
			if (item.Template.type == TItem.TYPE.BUNDLE && item.Amount > 0 && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("INIT_ITEM"), "BtnAction"))
			{
				((Sure2UnpackDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SURE2UNPACK, exclusive: true))?.InitDialog(item, (TBundle)item.Template);
			}
			if (item.CanSpecialUse() && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("INIT_ITEM"), "BtnAction"))
			{
				item.SpecialUse();
			}
			break;
		case Item.USAGE.NOT_USING:
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("EQUIP_IN_INVENTORY"), "BtnAction"))
			{
				if (item.IsLimitedByStarRate)
				{
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("WEAPON_STAR_LIMIT"));
				}
				else
				{
					((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.INIT_ITEM);
				}
			}
			break;
		}
		style.fontStyle = FontStyle.Normal;
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

	private bool IsPremiumPCbangTab()
	{
		return mainTab > 9;
	}
}
