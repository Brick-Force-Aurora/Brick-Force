using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shop
{
	public float doubleClickTimeout = 0.2f;

	public GameObject mySelf;

	public Texture2D selectedFrame;

	public Texture2D selectedFrameR;

	public Texture2D iconPoint;

	public Texture2D iconBrick;

	public Texture2D iconSale;

	public Texture2D[] iconNew;

	public Texture2D[] iconHot;

	public Texture2D iconToken;

	public Texture2D gauge;

	public Texture2D itemStarGauge;

	public Texture2D itemStarGaugeBg;

	public Texture2D itemStarGauge2;

	public string[] mainTabKey;

	public string[] weaponTabKey;

	public string[] clothTabKey;

	public string[] accessoryTabKey;

	private string[] mainTabs;

	private string[] weaponTabs;

	private string[] specialweaponTabs;

	private string[] clothTabs;

	private string[] accessoryTabs;

	private string[] mainWeaponTabs;

	private TreeInfo[] treeInfos;

	private Good.BUY_HOW buyHow;

	private string[] options;

	private int selected;

	public Tooltip tooltip;

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private LookCoordinator cordi;

	private float lastClickTime;

	private int mainTab;

	private int wpnCategory;

	private int[] subTab;

	private Vector2[] scrollPosition;

	private Vector2 scrollPositionTree = Vector2.zero;

	private TItem[] preview;

	private Rect crdItemList = new Rect(255f, 118f, 755f, 575f);

	private Rect crdItemListTemp = new Rect(255f, 118f, 755f, 575f);

	private Vector2 crdItem = new Vector2(184f, 136f);

	private Vector2 crdItemOffset = new Vector2(0f, 22f);

	private float chatGap = 260f;

	private Rect crdItemBtn = new Rect(3f, 3f, 154f, 130f);

	private Rect crdItemIcon = new Rect(12f, 6f, 142f, 100f);

	private Vector2 crdItemUsage = new Vector2(156f, 107f);

	private Rect crdItemPriceIcon = new Rect(4f, 116f, 12f, 12f);

	private Rect crdItemSpecialIcon = new Rect(4f, 75f, 28f, 28f);

	private Vector2 crdItemPrice = new Vector2(22f, 122f);

	private Rect crdBuy = new Rect(877f, 704f, 136f, 34f);

	private Rect crdRollbackBtn = new Rect(9f, 412f, 120f, 34f);

	private Rect crdWearBuyBtn = new Rect(9f, 447f, 120f, 34f);

	private Rect crdStarGauge = new Rect(90f, 116f, 64f, 12f);

	private Rect crdComboFilter = new Rect(784f, 73f, 180f, 30f);

	private Rect crdTree = new Rect(10f, 500f, 230f, 242f);

	private int curItem;

	private Good curGood;

	public float maxAtkPow = 100f;

	public float maxRpm = 1000f;

	public float maxRecoilPitch = 5f;

	public float maxMobility = 2f;

	private bool bRefreshOpt;

	private bool bBuy;

	private float focusTime;

	private string tooltipMessage = string.Empty;

	private bool canBuyItem = true;

	private bool wasWearingBundle;

	private bool firstClick;

	private Color txtMainColor = new Color(1f, 1f, 1f, 1f);

	private int mainweaponTab = 4;

	private int defaultClicked;

	private bool activeRelateItem;

	private string[] filterKey = new string[4]
	{
		"ALL",
		"c",
		"POINT",
		"BRICK_POINT"
	};

	private string[] filterKeyNetmable = new string[3]
	{
		"ALL",
		"TOKEN",
		"POINT"
	};

	private ComboBox cbox;

	private int selFilter;

	private Dictionary<string, Good> dicWear;

	private Good[] allGoods;

	private Good[] specialGoods;

	private Good[] newGoods;

	private Good[] hotGoods;

	private Good[] catGoods;

	private Good[] relateGoods;

	private int prevSelFilter = -1;

	private int prevMainTab = -1;

	private int prevSubTab = -1;

	private int prevWpnCategory = -1;

	private float deltaTimeForGoods;

	private bool chatView;

	private float deltaFocus;

	private bool bFocus;

	private int curSelRelate = -1;

	public int CurItem
	{
		set
		{
			curItem = value;
		}
	}

	public bool ActiveRelateItem
	{
		get
		{
			return activeRelateItem;
		}
		set
		{
			activeRelateItem = value;
			curItem = 0;
			bFocus = true;
		}
	}

	public bool NeedRollbackButton
	{
		get
		{
			for (int i = 0; i < preview.Length; i++)
			{
				string usingBySlot = MyInfoManager.Instance.GetUsingBySlot((TItem.SLOT)i);
				if (preview[i] != null && preview[i].code != usingBySlot)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool GetBuyConfirm()
	{
		return bBuy;
	}

	public void SetBuy(bool buy)
	{
		bBuy = buy;
		if (bBuy)
		{
			bRefreshOpt = true;
		}
		else
		{
			curGood = null;
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
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			GUIContent[] array = new GUIContent[filterKey.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new GUIContent(StringMgr.Instance.Get(filterKey[i]));
			}
			selFilter = cbox.List(crdComboFilter, StringMgr.Instance.Get(filterKey[selFilter]), array);
		}
		else
		{
			GUIContent[] array2 = new GUIContent[filterKeyNetmable.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = new GUIContent(StringMgr.Instance.Get(filterKeyNetmable[j]));
			}
			selFilter = cbox.List(crdComboFilter, StringMgr.Instance.Get(filterKeyNetmable[selFilter]), array2);
		}
	}

	public void Start()
	{
		dicWear = new Dictionary<string, Good>();
		cbox = new ComboBox();
		cbox.Initialize(bImage: false, new Vector2(crdComboFilter.width, crdComboFilter.height));
		cbox.setStyleNames("BoxFilterBg", "BtnArrowDn", "BtnArrowUp", "BoxFilterCombo");
		cbox.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
		cbox.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
		tooltip.Start();
		curItem = 0;
		cordi = mySelf.GetComponent<LookCoordinator>();
		preview = new TItem[18];
		InitPreview();
		wpnCategory = -1;
		mainTab = defaultClicked;
		mainTabs = new string[GlobalVars.Instance.ShopParentDirNames.Length];
		subTab = new int[20];
		treeInfos = new TreeInfo[GlobalVars.Instance.ShopParentDirNames.Length];
		scrollPosition = new Vector2[GlobalVars.Instance.ShopParentDirNames.Length];
		for (int i = 0; i < GlobalVars.Instance.ShopParentDirNames.Length; i++)
		{
			subTab[i] = 0;
			scrollPosition[i] = Vector2.zero;
			mainTabs[i] = StringMgr.Instance.Get(GlobalVars.Instance.ShopParentDirNames[i]);
			treeInfos[i] = new TreeInfo();
			treeInfos[i].childTrees = null;
			treeInfos[i].Name = mainTabs[i];
			if (i == defaultClicked)
			{
				treeInfos[i].clicked = true;
			}
		}
		weaponTabs = new string[weaponTabKey.Length];
		treeInfos[mainweaponTab].childTrees = new TreeInfo[weaponTabKey.Length - 1];
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			weaponTabs[j] = StringMgr.Instance.Get(weaponTabKey[j]);
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[mainweaponTab].childTrees[j - 1] = new TreeInfo();
				treeInfos[mainweaponTab].childTrees[j - 1].Name = weaponTabs[j];
			}
		}
		mainWeaponTabs = new string[GlobalVars.Instance.ShopMainWpnCatNames.Length];
		treeInfos[mainweaponTab].childTrees[0].childTrees = new TreeInfo[GlobalVars.Instance.ShopMainWpnCatNames.Length];
		for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
		{
			mainWeaponTabs[k] = StringMgr.Instance.Get(GlobalVars.Instance.ShopMainWpnCatNames[k]);
			if (treeInfos.Length > 0)
			{
				treeInfos[mainweaponTab].childTrees[0].childTrees[k] = new TreeInfo();
				treeInfos[mainweaponTab].childTrees[0].childTrees[k].Name = mainWeaponTabs[k];
			}
		}
		clothTabs = new string[clothTabKey.Length];
		treeInfos[mainweaponTab + 1].childTrees = new TreeInfo[clothTabs.Length - 1];
		for (int l = 0; l < clothTabKey.Length; l++)
		{
			clothTabs[l] = StringMgr.Instance.Get(clothTabKey[l]);
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[mainweaponTab + 1].childTrees[l - 1] = new TreeInfo();
				treeInfos[mainweaponTab + 1].childTrees[l - 1].Name = clothTabs[l];
			}
		}
		accessoryTabs = new string[GlobalVars.Instance.accessoryTabs.Length];
		treeInfos[mainweaponTab + 2].childTrees = new TreeInfo[GlobalVars.Instance.accessoryTabs.Length - 1];
		for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
		{
			accessoryTabs[m] = StringMgr.Instance.Get(GlobalVars.Instance.accessoryTabs[m]);
			if (treeInfos.Length > 0 && m > 0)
			{
				treeInfos[mainweaponTab + 2].childTrees[m - 1] = new TreeInfo();
				treeInfos[mainweaponTab + 2].childTrees[m - 1].Name = accessoryTabs[m];
			}
		}
		txtMainColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		GlobalVars.Instance.GameMode = ChannelManager.Instance.CurChannel.Mode;
		for (int n = 0; n < filterKey.Length; n++)
		{
			if (filterKey[n].Equals("TOKEN"))
			{
				filterKey[n] = TokenManager.Instance.currentToken.name;
			}
		}
		for (int num = 0; num < filterKeyNetmable.Length; num++)
		{
			if (filterKeyNetmable[num].Equals("TOKEN"))
			{
				filterKeyNetmable[num] = TokenManager.Instance.currentToken.name;
			}
		}
	}

	private void reloadTreeName()
	{
		if (mainTabs != null)
		{
			for (int i = 0; i < GlobalVars.Instance.ShopParentDirNames.Length; i++)
			{
				mainTabs[i] = StringMgr.Instance.Get(GlobalVars.Instance.ShopParentDirNames[i]);
				treeInfos[i].Name = mainTabs[i];
			}
			for (int j = 0; j < weaponTabKey.Length; j++)
			{
				weaponTabs[j] = StringMgr.Instance.Get(weaponTabKey[j]);
				if (treeInfos.Length > 0 && j > 0)
				{
					treeInfos[mainweaponTab].childTrees[j - 1].Name = weaponTabs[j];
				}
			}
			for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
			{
				mainWeaponTabs[k] = StringMgr.Instance.Get(GlobalVars.Instance.ShopMainWpnCatNames[k]);
				if (treeInfos.Length > 0)
				{
					treeInfos[mainweaponTab].childTrees[0].childTrees[k].Name = mainWeaponTabs[k];
				}
			}
			for (int l = 0; l < clothTabKey.Length; l++)
			{
				clothTabs[l] = StringMgr.Instance.Get(clothTabKey[l]);
				if (treeInfos.Length > 0 && l > 0)
				{
					treeInfos[mainweaponTab + 1].childTrees[l - 1].Name = clothTabs[l];
				}
			}
			for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
			{
				accessoryTabs[m] = StringMgr.Instance.Get(GlobalVars.Instance.accessoryTabs[m]);
				if (treeInfos.Length > 0 && m > 0)
				{
					treeInfos[mainweaponTab + 2].childTrees[m - 1].Name = accessoryTabs[m];
				}
			}
		}
	}

	public void InitPreview()
	{
		ShopManager.Instance.InitAllGoods();
		activeRelateItem = false;
		firstClick = false;
		reloadTreeName();
		for (int i = 0; i < preview.Length; i++)
		{
			string usingBySlot = MyInfoManager.Instance.GetUsingBySlot((TItem.SLOT)i);
			if (usingBySlot.Length != 3)
			{
				preview[i] = null;
			}
			else
			{
				preview[i] = TItemManager.Instance.Get<TItem>(usingBySlot);
			}
		}
	}

	public void RollbackPreview(bool bAll = true)
	{
		if (wasWearingBundle)
		{
			wasWearingBundle = false;
			cordi.UnequipAll();
			cordi.DefaultSex();
		}
		else
		{
			for (int i = 0; i < preview.Length; i++)
			{
				if (preview[i] != null)
				{
					cordi.Unequip(preview[i].code);
				}
			}
		}
		string[] usings = MyInfoManager.Instance.GetUsings();
		for (int j = 0; j < usings.Length; j++)
		{
			cordi.Equip(usings[j]);
		}
		InitPreview();
		dicWear.Clear();
		if (bAll)
		{
			bBuy = false;
			mainTab = defaultClicked;
			subTab[mainTab] = 0;
			wpnCategory = -1;
			for (int k = 0; k < GlobalVars.Instance.ShopParentDirNames.Length; k++)
			{
				treeInfos[k].bExpand = false;
				if (k == defaultClicked)
				{
					treeInfos[k].clicked = true;
				}
				else
				{
					treeInfos[k].clicked = false;
				}
			}
			for (int l = 0; l < weaponTabKey.Length; l++)
			{
				if (treeInfos.Length > 0 && l > 0)
				{
					treeInfos[mainweaponTab].childTrees[l - 1].clicked = false;
					treeInfos[mainweaponTab].childTrees[l - 1].bExpand = false;
				}
			}
			for (int m = 0; m < clothTabKey.Length; m++)
			{
				if (treeInfos.Length > 0 && m > 0)
				{
					treeInfos[mainweaponTab + 1].childTrees[m - 1].clicked = false;
					treeInfos[mainweaponTab + 1].childTrees[m - 1].bExpand = false;
				}
			}
			for (int n = 0; n < GlobalVars.Instance.accessoryTabs.Length; n++)
			{
				if (treeInfos.Length > 0 && n > 0)
				{
					treeInfos[mainweaponTab + 2].childTrees[n - 1].clicked = false;
					treeInfos[mainweaponTab + 2].childTrees[n - 1].bExpand = false;
				}
			}
		}
	}

	public bool findTree(string strFind)
	{
		float num = 250f;
		float num2 = 20f;
		float num3 = 0f;
		for (int i = 0; i < treeInfos.Length; i++)
		{
			num3 += num2;
			if (treeInfos[i].Name == strFind)
			{
				wpnCategory = -1;
				mainTab = i;
				subTab[mainTab] = 0;
				treeClickedNone();
				treeInfos[i].clicked = true;
				if (num3 > num)
				{
					scrollPositionTree.y = (float)calcScrollHeight() - num3;
				}
				return true;
			}
			if (treeInfos[i].childTrees != null && treeInfos[i].childTrees.Length > 0)
			{
				treeInfos[i].bExpand = true;
				for (int j = 0; j < treeInfos[i].childTrees.Length; j++)
				{
					num3 += num2;
					if (treeInfos[i].childTrees[j].Name == strFind)
					{
						wpnCategory = -1;
						mainTab = i;
						subTab[mainTab] = j + 1;
						treeClickedNone();
						treeInfos[i].childTrees[j].clicked = true;
						if (num3 > num)
						{
							scrollPositionTree.y = (float)calcScrollHeight() - num3;
						}
						return true;
					}
					if (treeInfos[i].childTrees[j].childTrees != null && treeInfos[i].childTrees[j].childTrees.Length > 0)
					{
						treeInfos[i].childTrees[j].bExpand = true;
						if (treeInfos[i].childTrees[j].bExpand)
						{
							for (int k = 0; k < treeInfos[i].childTrees[j].childTrees.Length; k++)
							{
								num3 += num2;
								if (treeInfos[i].childTrees[j].childTrees[k].Name == strFind)
								{
									int[] array = new int[4]
									{
										1,
										0,
										3,
										2
									};
									wpnCategory = array[k];
									mainTab = mainweaponTab;
									subTab[mainTab] = j + 1;
									treeClickedNone();
									treeInfos[i].childTrees[j].childTrees[k].clicked = true;
									if (num3 > num)
									{
										scrollPositionTree.y = (float)calcScrollHeight() - num3;
									}
									return true;
								}
							}
						}
						num3 -= num2 * (float)treeInfos[i].childTrees[j].childTrees.Length;
						treeInfos[i].childTrees[j].bExpand = false;
					}
				}
				num3 -= num2 * (float)treeInfos[i].childTrees.Length;
				treeInfos[i].bExpand = false;
			}
		}
		return false;
	}

	private void treeClickedNone()
	{
		for (int i = 0; i < GlobalVars.Instance.ShopParentDirNames.Length; i++)
		{
			treeInfos[i].clicked = false;
		}
		for (int j = 0; j < weaponTabKey.Length; j++)
		{
			if (treeInfos.Length > 0 && j > 0)
			{
				treeInfos[mainweaponTab].childTrees[j - 1].clicked = false;
			}
			if (j == 0)
			{
				for (int k = 0; k < GlobalVars.Instance.ShopMainWpnCatNames.Length; k++)
				{
					if (treeInfos[mainweaponTab].childTrees[0].childTrees.Length > 0)
					{
						treeInfos[mainweaponTab].childTrees[0].childTrees[k].clicked = false;
					}
				}
			}
		}
		for (int l = 0; l < clothTabKey.Length; l++)
		{
			if (treeInfos.Length > 0 && l > 0)
			{
				treeInfos[mainweaponTab + 1].childTrees[l - 1].clicked = false;
			}
		}
		for (int m = 0; m < GlobalVars.Instance.accessoryTabs.Length; m++)
		{
			if (treeInfos.Length > 0 && m > 0)
			{
				treeInfos[mainweaponTab + 2].childTrees[m - 1].clicked = false;
			}
		}
	}

	private void DrawDefaultPrice(Good good)
	{
		Rect position = new Rect(crdItemSpecialIcon);
		if (good.isSale)
		{
			TextureUtil.DrawTexture(position, iconSale, ScaleMode.StretchToFill);
			position.x += 20f;
		}
		if (good.isNew)
		{
			int num = 0;
			if (BuildOption.Instance.IsNetmarbleOrDev || BuildOption.Instance.IsAxeso5)
			{
				num = 1;
			}
			TextureUtil.DrawTexture(position, iconNew[num], ScaleMode.StretchToFill);
			position.x += 20f;
		}
		if (good.isHot)
		{
			int num2 = 0;
			if (BuildOption.Instance.IsNetmarbleOrDev || BuildOption.Instance.IsAxeso5)
			{
				num2 = 1;
			}
			TextureUtil.DrawTexture(position, iconHot[num2], ScaleMode.StretchToFill);
			position.x += 20f;
		}
		Texture2D mark = TokenManager.Instance.currentToken.mark;
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper || BuildOption.Instance.IsAxeso5)
		{
			if (selFilter == 0)
			{
				if (good.IsCashable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, mark, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultTokenPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				else if (good.IsPointable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconPoint, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				else if (good.IsBrickPointable && BuildOption.Instance.Props.useBrickPoint)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconBrick, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultBrickPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
			}
			else
			{
				switch (selFilter)
				{
				case 1:
					if (good.IsCashable)
					{
						TextureUtil.DrawTexture(crdItemPriceIcon, mark, ScaleMode.StretchToFill);
						LabelUtil.TextOut(crdItemPrice, good.GetDefaultTokenPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
					}
					break;
				case 2:
					if (good.IsPointable)
					{
						TextureUtil.DrawTexture(crdItemPriceIcon, iconPoint, ScaleMode.StretchToFill);
						LabelUtil.TextOut(crdItemPrice, good.GetDefaultPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
					}
					break;
				case 3:
					if (good.IsBrickPointable)
					{
						TextureUtil.DrawTexture(crdItemPriceIcon, iconBrick, ScaleMode.StretchToFill);
						LabelUtil.TextOut(crdItemPrice, good.GetDefaultBrickPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
					}
					break;
				}
			}
		}
		else if (selFilter == 0)
		{
			Rect position2 = new Rect(crdItemPriceIcon);
			if (good.IsPointable)
			{
				TextureUtil.DrawTexture(position2, iconPoint, ScaleMode.StretchToFill);
				position2.x += (float)(mark.width + 2);
			}
			if (good.IsBrickPointable && BuildOption.Instance.Props.useBrickPoint)
			{
				TextureUtil.DrawTexture(position2, iconBrick, ScaleMode.StretchToFill);
				position2.x += (float)(mark.width + 2);
			}
			if (good.IsCashable)
			{
				TextureUtil.DrawTexture(position2, mark, ScaleMode.StretchToFill);
			}
		}
		else
		{
			switch (selFilter)
			{
			case 1:
				if (good.IsCashable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, mark, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultTokenPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 2:
				if (good.IsPointable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconPoint, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			case 3:
				if (good.IsBrickPointable)
				{
					TextureUtil.DrawTexture(crdItemPriceIcon, iconBrick, ScaleMode.StretchToFill);
					LabelUtil.TextOut(crdItemPrice, good.GetDefaultBrickPrice(), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				}
				break;
			}
		}
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

	private void VerifyGoods(int myLevel)
	{
		bool flag = false;
		if (deltaTimeForGoods > 3f)
		{
			flag = true;
			deltaTimeForGoods = 0f;
		}
		if (flag || allGoods == null)
		{
			allGoods = ShopManager.Instance.GetAll(selFilter, myLevel);
		}
		if (flag || prevSelFilter != selFilter || prevMainTab != mainTab || prevSubTab != subTab[mainTab] || prevWpnCategory != wpnCategory)
		{
			prevSelFilter = selFilter;
			prevMainTab = mainTab;
			prevSubTab = subTab[mainTab];
			prevWpnCategory = wpnCategory;
			specialGoods = ShopManager.Instance.GetSpecialOffers(selFilter, myLevel);
			newGoods = ShopManager.Instance.GetNew(selFilter, myLevel);
			hotGoods = ShopManager.Instance.GetHot(selFilter, myLevel);
			catGoods = ShopManager.Instance.GetGoodsByCat(mainTab - 3, subTab[mainTab], selFilter, wpnCategory, myLevel);
		}
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

	private bool IsBundleClicked(int idx)
	{
		if (idx == curItem && curGood != null && curGood.tItem.type == TItem.TYPE.BUNDLE && firstClick)
		{
			return true;
		}
		return false;
	}

	private void OnlyPreviewInit()
	{
		dicWear.Clear();
		for (int i = 0; i < preview.Length; i++)
		{
			string usingBySlot = MyInfoManager.Instance.GetUsingBySlot((TItem.SLOT)i);
			if (usingBySlot.Length != 3)
			{
				preview[i] = null;
			}
			else
			{
				preview[i] = TItemManager.Instance.Get<TItem>(usingBySlot);
			}
		}
	}

	public void OnGUI()
	{
		bool flag = BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper;
		int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
		VerifyGoods(levelMixLank);
		Good[] array = null;
		switch (mainTab)
		{
		case 0:
			array = allGoods;
			break;
		case 1:
			array = specialGoods;
			break;
		case 2:
			array = newGoods;
			break;
		case 3:
			array = hotGoods;
			break;
		default:
			array = catGoods;
			break;
		}
		if (activeRelateItem && relateGoods != null && relateGoods.Length > 0)
		{
			array = relateGoods;
		}
		int num = array.Length;
		if ((NeedRollbackButton || wasWearingBundle) && GlobalVars.Instance.MyButton(crdRollbackBtn, StringMgr.Instance.Get("SHOP_ITEM_RETURN"), "BtnAction"))
		{
			RollbackPreview(bAll: false);
		}
		if (IsWearBtn() && GlobalVars.Instance.MyButton(crdWearBuyBtn, StringMgr.Instance.Get("SHOP_ITEM_PREVIEW_BUY"), "BtnAction"))
		{
			for (int i = 0; i < allGoods.Length; i++)
			{
				if (IsPreviewing(allGoods[i].tItem) && dicWear.ContainsKey(allGoods[i].tItem.code))
				{
					Good good = dicWear[allGoods[i].tItem.code];
					if (good != null)
					{
						good.Check = true;
					}
				}
			}
			((MBuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MBUY_TERM, exclusive: true))?.InitDialog(selFilter, _bWeared: true);
		}
		int num2 = 4;
		int num3 = num / num2;
		if (num % num2 > 0)
		{
			num3++;
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
		float num4 = crdItem.x * (float)num2;
		if (num2 > 0)
		{
			num4 += crdItemOffset.x * (float)(num2 - 1);
		}
		float num5 = crdItem.y * (float)num3;
		if (num3 > 0)
		{
			num5 += crdItemOffset.y * (float)(num3 - 1);
		}
		Rect viewRect = new Rect(0f, 0f, num4, num5);
		scrollPosition[mainTab] = GUI.BeginScrollView(crdItemList, scrollPosition[mainTab], viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false);
		float y = scrollPosition[mainTab].y;
		float num6 = y + crdItemList.height;
		Rect position = new Rect(0f, 0f, crdItem.x, crdItem.y);
		int num7 = 0;
		int num8 = 0;
		while (num7 < num && num8 < num3)
		{
			position.y = (float)num8 * crdItem.y;
			if (num8 > 0)
			{
				position.y += (float)num8 * crdItemOffset.y;
			}
			float y2 = position.y;
			float num9 = y2 + position.height;
			int num10 = 0;
			while (num7 < num && num10 < num2)
			{
				if ((selFilter != 1 || array[num7].IsCashable) && (selFilter != 2 || array[num7].IsPointable))
				{
					if (num9 >= y && y2 <= num6)
					{
						position.x = (float)num10 * crdItem.x;
						if (num10 > 0)
						{
							position.x += (float)num10 * crdItemOffset.x;
						}
						GUI.BeginGroup(position);
						TItem tItem = array[num7].tItem;
						if (tooltip.ItemCode == tItem.code)
						{
							if (num10 < num2 - 2)
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
						bool flag2 = true;
						if (BuildOption.Instance.Props.itemBuyLimit)
						{
							if (array[num7].IsCashable && levelMixLank >= array[num7].minlvTk)
							{
								flag2 = false;
							}
							if (array[num7].IsPointable && levelMixLank >= array[num7].minlvFp)
							{
								flag2 = false;
							}
						}
						else
						{
							flag2 = false;
						}
						string str = "BtnItem";
						if (flag2)
						{
							str = "BtnItemLock";
						}
						if (GlobalVars.Instance.MyButton(crdItemBtn, new GUIContent(string.Empty, tItem.code), str))
						{
							if (!firstClick)
							{
								firstClick = true;
							}
							curGood = array[num7];
							curItem = num7;
							if (flag2)
							{
								canBuyItem = false;
							}
							else
							{
								canBuyItem = true;
							}
							if (Time.time - lastClickTime > doubleClickTimeout)
							{
								lastClickTime = Time.time;
								if (tItem.type == TItem.TYPE.BUNDLE)
								{
									OnlyPreviewInit();
									wasWearingBundle = true;
									cordi.UnequipAll();
									cordi.DefaultSex();
									string[] usings = MyInfoManager.Instance.GetUsings();
									for (int j = 0; j < usings.Length; j++)
									{
										cordi.Equip(usings[j]);
									}
									BundleUnit[] array2 = BundleManager.Instance.Unpack(tItem.code);
									if (array2 != null)
									{
										for (int k = 0; k < array2.Length; k++)
										{
											if (array2[k].tItem != null)
											{
												string usingBySlot = MyInfoManager.Instance.GetUsingBySlot(array2[k].tItem.slot);
												cordi.Unequip(usingBySlot);
												cordi.Equip(array2[k].tItem.code);
											}
										}
									}
								}
								else
								{
									if (wasWearingBundle)
									{
										OnlyPreviewInit();
										wasWearingBundle = false;
										cordi.UnequipAll();
										cordi.DefaultSex();
										string[] usings2 = MyInfoManager.Instance.GetUsings();
										for (int l = 0; l < usings2.Length; l++)
										{
											cordi.Equip(usings2[l]);
										}
									}
									int slot = (int)array[num7].tItem.slot;
									if (0 <= slot && slot < preview.Length)
									{
										if (preview[slot] != null)
										{
											if (dicWear.ContainsKey(array[num7].tItem.code))
											{
												dicWear.Remove(preview[slot].code);
											}
											cordi.Unequip(preview[slot].code);
										}
										if (!dicWear.ContainsKey(array[num7].tItem.code))
										{
											dicWear.Add(array[num7].tItem.code, array[num7]);
										}
										cordi.Equip(array[num7].tItem.code);
										preview[slot] = array[num7].tItem;
									}
								}
							}
							else if (canBuyItem)
							{
								((BuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_TERM, exclusive: true))?.InitDialog(array[num7]);
							}
							else
							{
								MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_BUY_MESSAGE"));
							}
						}
						if (!flag)
						{
							Rect position2 = new Rect(159f, 5f, 21f, 22f);
							bool check = array[num7].Check;
							array[num7].Check = GUI.Toggle(position2, check, string.Empty);
							if (check != array[num7].Check)
							{
								if (tItem.type == TItem.TYPE.BUNDLE)
								{
									OnlyPreviewInit();
									wasWearingBundle = true;
									cordi.UnequipAll();
									cordi.DefaultSex();
									string[] usings3 = MyInfoManager.Instance.GetUsings();
									for (int m = 0; m < usings3.Length; m++)
									{
										cordi.Equip(usings3[m]);
									}
									BundleUnit[] array3 = BundleManager.Instance.Unpack(tItem.code);
									if (array3 != null)
									{
										for (int n = 0; n < array3.Length; n++)
										{
											if (array3[n].tItem != null)
											{
												string usingBySlot2 = MyInfoManager.Instance.GetUsingBySlot(array3[n].tItem.slot);
												cordi.Unequip(usingBySlot2);
												cordi.Equip(array3[n].tItem.code);
											}
										}
									}
								}
								else
								{
									if (wasWearingBundle)
									{
										OnlyPreviewInit();
										wasWearingBundle = false;
										cordi.UnequipAll();
										cordi.DefaultSex();
										string[] usings4 = MyInfoManager.Instance.GetUsings();
										for (int num12 = 0; num12 < usings4.Length; num12++)
										{
											cordi.Equip(usings4[num12]);
										}
									}
									int slot2 = (int)array[num7].tItem.slot;
									if (0 <= slot2 && slot2 < preview.Length)
									{
										if (preview[slot2] != null)
										{
											if (dicWear.ContainsKey(array[num7].tItem.code))
											{
												dicWear.Remove(preview[slot2].code);
											}
											cordi.Unequip(preview[slot2].code);
										}
										if (!dicWear.ContainsKey(array[num7].tItem.code))
										{
											dicWear.Add(array[num7].tItem.code, array[num7]);
										}
										cordi.Equip(array[num7].tItem.code);
										preview[slot2] = array[num7].tItem;
									}
								}
							}
						}
						if (tItem.CurIcon() == null)
						{
							Debug.LogError("Fail to get icon for item " + tItem.code);
						}
						else
						{
							TextureUtil.DrawTexture(crdItemIcon, tItem.CurIcon(), ScaleMode.ScaleToFit);
						}
						Color color = GUI.color;
						GUI.color = txtMainColor;
						GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
						GUI.color = color;
						if (IsUsing(array[num7].tItem))
						{
							LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("USING"), "MiniLabel", new Color(0.92f, 0.69f, 0.21f), Color.black, TextAnchor.LowerRight);
						}
						else if (IsPreviewing(array[num7].tItem) || IsBundleClicked(num7))
						{
							LabelUtil.TextOut(crdItemUsage, StringMgr.Instance.Get("PREVIEWING"), "MiniLabel", new Color(0.92f, 0.69f, 0.21f), Color.black, TextAnchor.LowerRight);
						}
						DrawDefaultPrice(array[num7]);
						if (itemStarGauge != null && itemStarGaugeBg != null)
						{
							TextureUtil.DrawTexture(crdStarGauge, itemStarGaugeBg, ScaleMode.StretchToFill);
							Rect position3 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * array[num7].starRate, crdStarGauge.height);
							GUI.BeginGroup(position3);
							TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge, ScaleMode.StretchToFill);
							GUI.EndGroup();
							if (array[num7].starRate > 1f)
							{
								float num13 = array[num7].starRate - 1f;
								position3 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * num13, crdStarGauge.height);
								GUI.BeginGroup(position3);
								TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge2, ScaleMode.StretchToFill);
								GUI.EndGroup();
							}
						}
						if (num7 == curItem)
						{
							curGood = array[num7];
							TextureUtil.DrawTexture(new Rect(crdItemBtn.x - 3f, crdItemBtn.y - 3f, crdItemBtn.width + 6f, crdItemBtn.height + 6f), selectedFrame, ScaleMode.StretchToFill);
						}
						GUI.EndGroup();
					}
					num7++;
				}
				num10++;
			}
			num8++;
		}
		GUI.EndScrollView();
		if (!canBuyItem)
		{
			GUI.enabled = false;
		}
		Rect rc = new Rect(crdBuy);
		if (chatView)
		{
			rc.y -= chatGap;
		}
		GUIContent content = new GUIContent(StringMgr.Instance.Get("BUY").ToUpper(), GlobalVars.Instance.iconCart);
		if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction") && curGood != null)
		{
			((BuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.BUY_TERM, exclusive: true))?.InitDialog(curGood);
		}
		if (ShopManager.Instance.IsMultiBuy() && !flag)
		{
			rc.x -= 137f;
			content = new GUIContent(StringMgr.Instance.Get("BTN_MULTIPURCHASE").ToUpper(), GlobalVars.Instance.iconCart);
			if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction") && curGood != null)
			{
				((MBuyTermDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MBUY_TERM, exclusive: true))?.InitDialog(selFilter);
			}
		}
		if (!canBuyItem)
		{
			GUI.enabled = true;
		}
		if (curGood != null && (curGood.tItem.grp1 != "none" || curGood.tItem.grp2 != "none" || curGood.tItem.grp3 != "none"))
		{
			rc.x -= 137f;
			GUIContent content2 = new GUIContent(StringMgr.Instance.Get("RELATED_ITEMS").ToUpper(), GlobalVars.Instance.iconDetail);
			if (activeRelateItem)
			{
				content2 = new GUIContent(StringMgr.Instance.Get("SHOPPING").ToUpper(), GlobalVars.Instance.iconCart);
			}
			if (GlobalVars.Instance.MyButton3(rc, content2, "BtnAction") && curGood != null)
			{
				activeRelateItem = !activeRelateItem;
				if (activeRelateItem)
				{
					curItem = -1;
					curSelRelate = 0;
					relateGoods = ShopManager.Instance.GetRelateGoods(curGood.tItem.grp1);
					GUI.FocusControl(string.Empty);
				}
				else
				{
					ShopManager.Instance.InitAllGoods();
					curItem = 0;
					bFocus = true;
				}
			}
		}
		if (BuildOption.Instance.Props.GetTokensURL.Length > 0)
		{
			string text = (!BuildOption.Instance.IsInfernum) ? string.Format(StringMgr.Instance.Get("BUY_TOKEN"), TokenManager.Instance.GetTokenString()) : StringMgr.Instance.Get("BUY_TOKEN2");
			rc.x -= 137f;
			content = new GUIContent(text.ToUpper(), iconToken);
			if (GlobalVars.Instance.MyButton3(rc, content, "BtnAction"))
			{
				string url = BuildOption.Instance.Props.GetTokensURL + BuildOption.Instance.TokensParameter();
				if (MyInfoManager.Instance.SiteCode == 11)
				{
					url = BuildOption.Instance.Props.GetTokensURL2 + BuildOption.Instance.TokensParameter();
				}
				BuildOption.OpenURL(url);
			}
		}
		DoTooltip();
		if (!activeRelateItem)
		{
			DoTreeView();
		}
		else
		{
			OnDrawRelateItem();
		}
		if (bFocus)
		{
			deltaFocus += Time.deltaTime;
			GUI.SetNextControlName("outside");
			GUI.TextField(new Rect(-100f, -100f, 1f, 1f), string.Empty);
			GUI.FocusControl("outside");
			if (deltaFocus > 0.2f)
			{
				bFocus = false;
				deltaFocus = 0f;
			}
		}
	}

	private void DoTreeView()
	{
		scrollPositionTree = GUI.BeginScrollView(viewRect: new Rect(crdTree.x, crdTree.y, 160f, (float)calcScrollHeight()), position: crdTree, scrollPosition: scrollPositionTree, alwaysShowHorizontal: false, alwaysShowVertical: false);
		GUIStyle style = GUI.skin.GetStyle("BtnSelect");
		Vector2 pos = new Vector2(crdTree.x, crdTree.y);
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
				tooltip.ItemCode = GUI.tooltip;
				tooltip.IsShop = true;
				if (!DialogManager.Instance.IsModal && GUI.tooltip.Length > 0)
				{
					TItem tItem = TItemManager.Instance.Get<TItem>(tooltip.ItemCode);
					if (tItem != null)
					{
						GlobalVars.Instance.PlaySoundMouseOver();
					}
				}
			}
			if (focusTime > 0.3f)
			{
				TItem tItem2 = TItemManager.Instance.Get<TItem>(tooltip.ItemCode);
				if (tItem2 != null)
				{
					if (tooltip.ItemCode != string.Empty && ltTooltip != Vector2.zero)
					{
						ltTooltip = GlobalVars.Instance.ToGUIPoint(ltTooltip);
						tooltip.SetCoord(ltTooltip);
						GUI.Window(1101, tooltip.ClientRect, ShowShopTooltip, string.Empty, "TooltipWindow");
					}
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
						GUI.Window(1101, rc, ShowTooltip, string.Empty, "LineWindow");
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

	public void Update()
	{
		focusTime += Time.deltaTime;
		deltaTimeForGoods += Time.deltaTime;
	}

	private void ShowShopTooltip(int id)
	{
		tooltip.DoDialog();
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private bool IsUsing(TItem tItem)
	{
		string usingBySlot = MyInfoManager.Instance.GetUsingBySlot(tItem.slot);
		return usingBySlot == tItem.code;
	}

	private bool IsPreviewing(TItem tItem)
	{
		int slot = (int)tItem.slot;
		if (slot < 0 || slot >= preview.Length)
		{
			return false;
		}
		return preview[slot] != null && preview[slot].code == tItem.code;
	}

	private bool DoBrickPointReceipt(int percent)
	{
		int price = curGood.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int brickPoint = MyInfoManager.Instance.BrickPoint;
		int num2 = brickPoint - num;
		return num2 > 0;
	}

	private bool DoGeneralPointReceipt(int percent)
	{
		int price = curGood.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int point = MyInfoManager.Instance.Point;
		int num2 = point - num;
		return num2 > 0;
	}

	private bool DoCashPointReceipt(int percent)
	{
		int price = curGood.GetPrice(selected, buyHow);
		int num = price - Mathf.CeilToInt((float)price * ((float)percent / 100f));
		int cash = MyInfoManager.Instance.Cash;
		int num2 = cash - num;
		return num2 > 0;
	}

	private int GetWeaponLevel(int cat)
	{
		int result = -1;
		switch (cat)
		{
		case 0:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Heavy);
			break;
		case 1:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Assault);
			break;
		case 2:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Sniper);
			break;
		case 3:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.SubMachine);
			break;
		case 4:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.HandGun);
			break;
		case 5:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Melee);
			break;
		case 6:
			result = XpManager.Instance.GetWeaponLevel((TWeapon.CATEGORY)cat, MyInfoManager.Instance.Special);
			break;
		}
		return result;
	}

	private void VerifyBuyHow(int percent)
	{
		if (!BuildOption.Instance.Props.usePriceDiscount)
		{
			percent = 0;
		}
		if (buyHow == Good.BUY_HOW.GENERAL_POINT && !curGood.CanBuy(Good.BUY_HOW.GENERAL_POINT))
		{
			buyHow = Good.BUY_HOW.BRICK_POINT;
			bRefreshOpt = true;
		}
		if (buyHow == Good.BUY_HOW.BRICK_POINT && !curGood.CanBuy(Good.BUY_HOW.BRICK_POINT))
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
			bRefreshOpt = true;
		}
		if (buyHow == Good.BUY_HOW.CASH_POINT && !curGood.CanBuy(Good.BUY_HOW.CASH_POINT))
		{
			buyHow = Good.BUY_HOW.GENERAL_POINT;
			bRefreshOpt = true;
		}
		options = curGood.GetOptionStrings(buyHow, percent);
		if (bRefreshOpt)
		{
			bRefreshOpt = false;
			ResetSelectedOption();
		}
		if (selected >= options.Length)
		{
			selected = options.Length - 1;
		}
		if (selected < 0)
		{
			selected = 0;
		}
	}

	private void ResetSelectedOption()
	{
		selected = curGood.GetDefaultPriceSel(buyHow);
	}

	private void OnDrawRelateItem()
	{
		if (curGood != null)
		{
			TItem tItem = curGood.tItem;
			if (tItem != null)
			{
				LabelUtil.TextOut(new Vector2(20f, 489f), StringMgr.Instance.Get("RELATED_CHOOSE_SET"), "Label", new Color(0.92f, 0.69f, 0.21f), Color.black, TextAnchor.UpperLeft);
				Rect position = new Rect(20f, 514f, crdItem.x, crdItem.y);
				GUI.BeginGroup(position);
				GUI.Box(crdItemBtn, string.Empty, "BtnItem");
				TextureUtil.DrawTexture(crdItemIcon, tItem.CurIcon(), ScaleMode.ScaleToFit);
				Color color = GUI.color;
				GUI.color = txtMainColor;
				GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
				GUI.color = color;
				DrawDefaultPrice(curGood);
				if (itemStarGauge != null && itemStarGaugeBg != null)
				{
					TextureUtil.DrawTexture(crdStarGauge, itemStarGaugeBg, ScaleMode.StretchToFill);
					Rect position2 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * curGood.starRate, crdStarGauge.height);
					GUI.BeginGroup(position2);
					TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge, ScaleMode.StretchToFill);
					GUI.EndGroup();
					if (curGood.starRate > 1f)
					{
						float num = curGood.starRate - 1f;
						position2 = new Rect(crdStarGauge.x, crdStarGauge.y, crdStarGauge.width * num, crdStarGauge.height);
						GUI.BeginGroup(position2);
						TextureUtil.DrawTexture(new Rect(0f, 0f, crdStarGauge.width, crdStarGauge.height), itemStarGauge2, ScaleMode.StretchToFill);
						GUI.EndGroup();
					}
				}
				GUI.EndGroup();
				LabelUtil.TextOut(new Vector2(20f, 650f), StringMgr.Instance.Get("RELATED_SET"), "Label", new Color(0.92f, 0.69f, 0.21f), Color.black, TextAnchor.UpperLeft);
				GUI.SetNextControlName("grp1");
				Rect rc = new Rect(20f, 675f, 212f, 28f);
				if (curGood.tItem.grp1 != "none" && GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get(curGood.tItem.grp1), "BtnBlue"))
				{
					curSelRelate = 0;
					GUI.FocusControl("grp1");
					relateGoods = ShopManager.Instance.GetRelateGoods(curGood.tItem.grp1);
				}
				GUI.SetNextControlName("grp2");
				Rect rc2 = new Rect(20f, 705f, 212f, 28f);
				if (curGood.tItem.grp2 != "none" && GlobalVars.Instance.MyButton(rc2, StringMgr.Instance.Get(curGood.tItem.grp2), "BtnBlue"))
				{
					curSelRelate = 1;
					GUI.FocusControl("grp2");
					relateGoods = ShopManager.Instance.GetRelateGoods(curGood.tItem.grp2);
				}
				GUI.SetNextControlName("grp3");
				Rect rc3 = new Rect(20f, 735f, 212f, 28f);
				if (curGood.tItem.grp3 != "none" && GlobalVars.Instance.MyButton(rc3, StringMgr.Instance.Get(curGood.tItem.grp3), "BtnBlue"))
				{
					curSelRelate = 2;
					GUI.FocusControl("grp3");
					relateGoods = ShopManager.Instance.GetRelateGoods(curGood.tItem.grp3);
				}
				string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
				if (!nameOfFocusedControl.Contains("grp"))
				{
					switch (curSelRelate)
					{
					case 0:
						GUI.FocusControl("grp1");
						break;
					case 1:
						GUI.FocusControl("grp2");
						break;
					case 2:
						GUI.FocusControl("grp3");
						break;
					}
				}
			}
		}
	}

	private bool IsWearBtn()
	{
		if (dicWear.Count > 0)
		{
			return true;
		}
		return false;
	}
}
