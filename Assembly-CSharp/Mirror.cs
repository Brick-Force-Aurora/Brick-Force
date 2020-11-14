using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mirror
{
	public RenderTexture selfView;

	public GameObject mySelf;

	public Texture2D gauge;

	public Texture2D masterIcon;

	public Texture2D scrollbarBg;

	public Texture2D scrollbar;

	private Rect crdLeftBg = new Rect(0f, 60f, 244f, 707f);

	private Rect crdNickBg = new Rect(2f, 62f, 239f, 55f);

	private Rect crdselfviewBound = new Rect(2f, 27f, 240f, 383f);

	private Rect crdSelfView = new Rect(-26f, 90f, 300f, 300f);

	private Rect crdMaster = new Rect(207f, 120f, 20f, 20f);

	private Vector2 crdReady = new Vector2(128f, 377f);

	private Rect crdDetailBtn = new Rect(208f, 57f, 34f, 34f);

	private Vector2 crdLTBuffIcon = new Vector2(10f, 127f);

	private Rect crdLine = new Rect(10f, 410f, 234f, 12f);

	private Rect crdLine2 = new Rect(10f, 482f, 234f, 12f);

	private Rect crdLine3 = new Rect(10f, 596f, 234f, 12f);

	private Rect crdShopPoint = new Rect(218f, 420f, 12f, 12f);

	private Vector2 crdGeneralPoint = new Vector2(216f, 425f);

	private Vector2 crdXpLbl = new Vector2(12f, 528f);

	private Vector2 crdXpLbl2 = new Vector2(232f, 550f);

	private Vector2 crdXpGauge = new Vector2(12f, 566f);

	private Vector2 xpGaugeSize = new Vector2(216f, 17f);

	private Vector2 crdXp = new Vector2(130f, 575f);

	private Rect crdRotateL = new Rect(14f, 360f, 35f, 29f);

	private Rect crdRotateR = new Rect(194f, 360f, 35f, 29f);

	private Vector2 crdBadge = new Vector2(12f, 97f);

	private Vector2 crdNickname = new Vector2(53f, 96f);

	private Vector2 crdClanName = new Vector2(53f, 68f);

	private Rect crdClanMark = new Rect(12f, 66f, 30f, 30f);

	private AutoRotator rotator;

	private LookCoordinator cordi;

	private bool flip;

	private float deltaTime;

	private MIRROR_TYPE mirrorType;

	private float pointBooster;

	private float xpBooster;

	private float luck;

	private int armor;

	private float mainAmmoMax;

	private float auxAmmoMax;

	private float grenadeMax1;

	private float grenadeMax2;

	private float hpCooltime;

	private float dashTimeInc;

	private float respawnTimeDec;

	private float fallenDamageDec;

	public MIRROR_TYPE MirrorType
	{
		set
		{
			mirrorType = value;
		}
	}

	public void Start()
	{
		rotator = mySelf.GetComponent<AutoRotator>();
		cordi = mySelf.GetComponent<LookCoordinator>();
		cordi.Init(mirror: true);
		cordi.TestGender = true;
		string[] usings = MyInfoManager.Instance.GetUsings();
		for (int i = 0; i < usings.Length; i++)
		{
			cordi.Equip(usings[i]);
		}
		cordi.TestGender = false;
		Weapon.isInitialize = true;
		cordi.ChangeWeapon(Weapon.TYPE.MAIN);
		Weapon.isInitialize = false;
		flip = false;
		deltaTime = 0f;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (flip)
		{
			if (deltaTime > 0.3f)
			{
				deltaTime = 0f;
				flip = false;
			}
		}
		else if (deltaTime > 1f)
		{
			deltaTime = 0f;
			flip = true;
		}
	}

	private void DoBriefingRoom()
	{
		if (Application.loadedLevelName.Contains("Briefing") && MyInfoManager.Instance.Status == 1)
		{
			LabelUtil.TextOut(new Vector2(crdReady.x + 1f, crdReady.y + 2f), StringMgr.Instance.Get("READY"), "BigLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			LabelUtil.TextOut(crdReady, StringMgr.Instance.Get("READY"), "BigLabel", Color.green, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
	}

	private bool DoBuff(float changeHeight)
	{
		pointBooster = 0f;
		xpBooster = 0f;
		luck = 0f;
		if (MyInfoManager.Instance.HaveFunction("premium_account") >= 0)
		{
			xpBooster += 10f;
			pointBooster += 10f;
		}
		List<long> list = new List<long>();
		for (int i = 0; i < MyInfoManager.Instance.ShooterTools.Length; i++)
		{
			ConsumableDesc consumableDesc = null;
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(MyInfoManager.Instance.ShooterTools[i]);
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL && itemBySequence.Amount > 0 && !list.Contains(itemBySequence.Seq))
			{
				itemBySequence.AmountBuf = itemBySequence.Amount;
				list.Add(itemBySequence.Seq);
			}
			if (itemBySequence != null && itemBySequence.IsAmount && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL && itemBySequence.AmountBuf > 0)
			{
				TSpecial tSpecial = (TSpecial)itemBySequence.Template;
				if (tSpecial.IsConsumableBuff)
				{
					string func = TItem.FunctionMaskToString(tSpecial.functionMask);
					consumableDesc = ConsumableManager.Instance.Get(func);
					if (consumableDesc != null && !consumableDesc.isShooterTool)
					{
						consumableDesc = null;
					}
					if (consumableDesc != null)
					{
						TBuff tBuff = BuffManager.Instance.Get(tSpecial.param);
						if (tBuff != null)
						{
							if (tBuff.IsPoint)
							{
								pointBooster += (float)tBuff.PointRatio;
							}
							if (tBuff.IsXp)
							{
								xpBooster += (float)tBuff.XpRatio;
							}
							if (tBuff.IsLuck)
							{
								luck += (float)tBuff.Luck;
							}
						}
					}
					itemBySequence.AmountBuf--;
				}
			}
		}
		list.Clear();
		list = null;
		string[] usings = MyInfoManager.Instance.GetUsings();
		for (int j = 0; j < usings.Length; j++)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(usings[j]);
			if (tItem != null && tItem.tBuff != null)
			{
				pointBooster += (float)tItem.tBuff.PointRatio;
				xpBooster += (float)tItem.tBuff.XpRatio;
				luck += (float)tItem.tBuff.Luck;
			}
		}
		Item[] usingItems = MyInfoManager.Instance.GetUsingItems();
		for (int k = 0; k < usingItems.Length; k++)
		{
			int num = 0;
			int num2 = 0;
			num = 10;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				pointBooster += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
			num = 9;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				xpBooster += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
			num = 12;
			num2 = usingItems[k].upgradeProps[num].grade;
			if (num2 > 0 && usingItems[k].Template.upgradeCategory != TItem.UPGRADE_CATEGORY.NONE)
			{
				luck += PimpManager.Instance.getValue((int)usingItems[k].Template.upgradeCategory, num, num2 - 1);
			}
		}
		armor = MyInfoManager.Instance.SumArmor();
		mainAmmoMax = MyInfoManager.Instance.SumFunctionFactor("main_ammo_inc");
		auxAmmoMax = MyInfoManager.Instance.SumFunctionFactor("aux_ammo_inc");
		grenadeMax1 = MyInfoManager.Instance.SumFunctionFactor("special_ammo_inc");
		grenadeMax2 = MyInfoManager.Instance.SumFunctionFactor("special_ammo_add");
		hpCooltime = MyInfoManager.Instance.SumFunctionFactor("hp_cooltime");
		dashTimeInc = MyInfoManager.Instance.SumFunctionFactor("dash_time_inc");
		respawnTimeDec = MyInfoManager.Instance.SumFunctionFactor("respwan_time_dec");
		fallenDamageDec = MyInfoManager.Instance.SumFunctionFactor("fallen_damage_reduce");
		if (pointBooster > 0f || xpBooster > 0f || luck > 0f || armor > 0 || mainAmmoMax > 0f || auxAmmoMax > 0f || grenadeMax1 > 0f || hpCooltime > 0f || grenadeMax2 > 0f || dashTimeInc > 0f || respawnTimeDec > 0f || fallenDamageDec > 0f)
		{
			Texture2D icon = BuffManager.Instance.GetBuffDesc(BuffDesc.WHY.ITEM).icon;
			GUI.Button(new Rect(crdLTBuffIcon.x, crdLTBuffIcon.y - changeHeight, (float)icon.width, (float)icon.height), new GUIContent(icon, "item"), "InvisibleButton");
			return true;
		}
		return false;
	}

	public void OnGUI()
	{
		float num = 0f;
		GUI.Box(crdLeftBg, string.Empty, "BoxBase");
		GUI.Box(crdNickBg, string.Empty, "BoxNickBg");
		GUI.BeginGroup(new Rect(crdselfviewBound.x, crdselfviewBound.y - num, crdselfviewBound.width, crdselfviewBound.height));
		TextureUtil.DrawTexture(crdSelfView, selfView);
		GUI.EndGroup();
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
		Color byteColor2FloatColor2 = GlobalVars.Instance.GetByteColor2FloatColor(50, 191, 17);
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
		{
			TextureUtil.DrawTexture(crdMaster, masterIcon, ScaleMode.StretchToFill);
		}
		if (MyInfoManager.Instance.ClanMark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(MyInfoManager.Instance.ClanMark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(MyInfoManager.Instance.ClanMark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(MyInfoManager.Instance.ClanMark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(crdClanMark, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(crdClanMark, amblum);
			}
			GUI.color = color;
		}
		LabelUtil.TextOut(crdClanName, (MyInfoManager.Instance.ClanSeq < 0) ? StringMgr.Instance.Get("NO_CLAN") : MyInfoManager.Instance.ClanName, "Label", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdNickname, MyInfoManager.Instance.Nickname, "Label", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		int level = XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp);
		Texture2D badge = XpManager.Instance.GetBadge(level, MyInfoManager.Instance.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(crdBadge.x, crdBadge.y, (float)badge.width, (float)badge.height), badge);
		}
		DoBriefingRoom();
		float num2 = 0f;
		if (DoBuff(num))
		{
			num2 += 20f;
		}
		if (DoChannelBuff(num, num2))
		{
			num2 += 20f;
		}
		if (DoPCBangBuff(num, num2))
		{
			num2 += 20f;
		}
		if (!DialogManager.Instance.IsModal && GlobalVars.Instance.MyButton(crdDetailBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("DETAIL_USER_INFO")), "BtnDetail"))
		{
			CSNetManager.Instance.Sock.SendCS_PLAYER_DETAIL_REQ(MyInfoManager.Instance.Seq);
		}
		AutoRotator.ROTATE rot = AutoRotator.ROTATE.STOP;
		Rect position = new Rect(crdRotateL.x, crdRotateL.y - num, crdRotateL.width, crdRotateL.height);
		if (GUI.RepeatButton(position, string.Empty, "RotateL"))
		{
			rot = AutoRotator.ROTATE.RIGHT;
		}
		Rect position2 = new Rect(crdRotateR.x, crdRotateR.y - num, crdRotateR.width, crdRotateR.height);
		if (GUI.RepeatButton(position2, string.Empty, "RotateR"))
		{
			rot = AutoRotator.ROTATE.LEFT;
		}
		rotator.Rotate(rot);
		Rect position3 = new Rect(crdLine);
		position3.y -= num;
		Rect position4 = new Rect(crdLine2);
		position4.y -= num;
		Rect position5 = new Rect(crdShopPoint);
		position5.y -= num;
		Vector2 pos = new Vector2(crdGeneralPoint.x, crdGeneralPoint.y - num);
		GUI.Box(position3, string.Empty, "DivideLine");
		GUI.Box(position5, new GUIContent(string.Empty, StringMgr.Instance.Get("GENERAL_POINT")), "ShopPoint");
		LabelUtil.TextOut(pos, MyInfoManager.Instance.Point.ToString("n0"), "MiniLabel", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		position5.y += 20f;
		pos.y += 20f;
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			GUI.Box(position5, new GUIContent(string.Empty, StringMgr.Instance.Get("BRICK_POINT")), "ShopBrick");
			LabelUtil.TextOut(pos, MyInfoManager.Instance.BrickPoint.ToString("n0"), "MiniLabel", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			position5.y += 20f;
			pos.y += 20f;
		}
		GUI.Box(position5, new GUIContent(string.Empty, TokenManager.Instance.GetTokenString()), TokenManager.Instance.currentToken.skin);
		LabelUtil.TextOut(pos, MyInfoManager.Instance.Cash.ToString("n0"), "MiniLabel", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		position5.y += 20f;
		pos.y += 20f;
		GUI.Box(position5, new GUIContent(string.Empty, StringMgr.Instance.Get("FREE")), "ShopCoin");
		LabelUtil.TextOut(pos, MyInfoManager.Instance.FreeCoin.ToString("n0"), "MiniLabel", byteColor2FloatColor2, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
		GUI.Box(position4, string.Empty, "DivideLine");
		if (mirrorType == MIRROR_TYPE.BASE)
		{
			int levelMixLank = XpManager.Instance.GetLevelMixLank(MyInfoManager.Instance.Xp, MyInfoManager.Instance.Rank);
			string text = StringMgr.Instance.Get("XP") + " ( " + XpManager.Instance.GetRank(levelMixLank) + " )";
			LabelUtil.TextOut(crdXpLbl, text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			LabelUtil.TextOut(crdXpLbl2, XpManager.Instance.GetXpCountString(MyInfoManager.Instance.Xp), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
			float ratio = XpManager.Instance.GetRatio(MyInfoManager.Instance.Xp);
			TextureUtil.DrawTexture(new Rect(crdXpGauge.x, crdXpGauge.y, xpGaugeSize.x, xpGaugeSize.y), scrollbarBg, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(crdXpGauge.x, crdXpGauge.y, xpGaugeSize.x * ratio, xpGaugeSize.y), scrollbar, ScaleMode.StretchToFill);
			float num3 = Mathf.Floor(ratio * 10000f) / 100f;
			LabelUtil.TextOut(crdXp, num3.ToString("0.##") + "%", "MiniLabel", Color.white, Color.black, TextAnchor.MiddleCenter);
			GUI.Box(crdLine3, string.Empty, "DivideLine");
		}
		if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
		{
			if (GUI.tooltip == "item")
			{
				Vector2 pos2 = new Vector2(160f, 20f);
				DoBuffRows(ref pos2, calcHeight: true);
				Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				GUI.Window(1103, new Rect(vector.x, vector.y, pos2.x, pos2.y), ShowTooltip, string.Empty, "LineWindow");
				GUI.tooltip = string.Empty;
			}
			else if (GUI.tooltip == "channel")
			{
				Vector2 pos3 = new Vector2(160f, 20f);
				DoChannelBuffRows(ref pos3, calcHeight: true);
				Vector2 vector2 = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				GUI.Window(1104, new Rect(vector2.x, vector2.y, pos3.x, pos3.y), ShowTooltip, string.Empty, "LineWindow");
				GUI.tooltip = string.Empty;
			}
			else if (GUI.tooltip == "pcbang")
			{
				Vector2 pos4 = new Vector2(160f, 20f);
				DoPCBangBuffRows(ref pos4, calcHeight: true);
				Vector2 vector3 = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
				GUI.Window(1105, new Rect(vector3.x, vector3.y, pos4.x, pos4.y), ShowTooltip, string.Empty, "LineWindow");
				GUI.tooltip = string.Empty;
			}
		}
	}

	private void ShowTooltip(int id)
	{
		switch (id)
		{
		case 1103:
		{
			Vector2 pos3 = new Vector2(10f, 10f);
			DoBuffRows(ref pos3, calcHeight: false);
			break;
		}
		case 1104:
		{
			Vector2 pos2 = new Vector2(10f, 10f);
			DoChannelBuffRows(ref pos2, calcHeight: false);
			break;
		}
		case 1105:
		{
			Vector2 pos = new Vector2(10f, 10f);
			DoPCBangBuffRows(ref pos, calcHeight: false);
			break;
		}
		}
	}

	private void DoBuffRows(ref Vector2 pos, bool calcHeight)
	{
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(10f, pos.y), StringMgr.Instance.Get("ITEM_BUFF"), "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 20f;
		if (pointBooster > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("POINT_UP"), pointBooster), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (xpBooster > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("XP_UP"), xpBooster), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (luck > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("LUCK_UP"), luck), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (armor > 0 && BuildOption.Instance.Props.useArmor)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("ARMOR_UP"), armor), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (hpCooltime > 0f && !BuildOption.Instance.IsNetmarbleOrDev)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("HP_COOLTIME_DOWN"), hpCooltime * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (mainAmmoMax > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("MAIN_AMMO_MAX_UP"), mainAmmoMax * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (auxAmmoMax > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("AUX_AMMO_MAX_UP"), auxAmmoMax * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (grenadeMax1 > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP"), grenadeMax1 * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (grenadeMax2 > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("GRENADE_AMMO_MAX_UP02"), grenadeMax2), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (dashTimeInc > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("DASHTIME"), dashTimeInc * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (respawnTimeDec > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("RESPAWNTIME"), respawnTimeDec * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (fallenDamageDec > 0f)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("FALLDAMAGEDEC"), fallenDamageDec * 100f), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
	}

	private bool DoChannelBuff(float changeHeight, float changeWidth)
	{
		if (BuffManager.Instance.IsChannelBuff())
		{
			Texture2D icon = BuffManager.Instance.GetBuffDesc(BuffDesc.WHY.CHANNEL).icon;
			GUI.Button(new Rect(crdLTBuffIcon.x + changeWidth, crdLTBuffIcon.y - changeHeight, (float)icon.width, (float)icon.height), new GUIContent(icon, "channel"), "InvisibleButton");
			return true;
		}
		return false;
	}

	private void DoChannelBuffRows(ref Vector2 pos, bool calcHeight)
	{
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(10f, pos.y), StringMgr.Instance.Get("CHANNEL_BUFF"), "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 20f;
		if (ChannelManager.Instance.CurChannel.FpBonus > 0)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("POINT_UP"), ChannelManager.Instance.CurChannel.FpBonus), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (ChannelManager.Instance.CurChannel.XpBonus > 0)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("XP_UP"), ChannelManager.Instance.CurChannel.XpBonus), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
	}

	private bool DoPCBangBuff(float changeHeight, float changeWidth)
	{
		if (BuffManager.Instance.IsPCBangBuff())
		{
			Texture2D icon = BuffManager.Instance.GetBuffDesc(BuffDesc.WHY.PC_BANG).icon;
			GUI.Button(new Rect(crdLTBuffIcon.x + changeWidth, crdLTBuffIcon.y - changeHeight, (float)icon.width, (float)icon.height), new GUIContent(icon, "pcbang"), "InvisibleButton");
			return true;
		}
		return false;
	}

	private void DoPCBangBuffRows(ref Vector2 pos, bool calcHeight)
	{
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(10f, pos.y), StringMgr.Instance.Get("PCBANG_ICON_TOOLTIP"), "MiniLabel", new Color(0.91f, 0.6f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 20f;
		TBuff tBuff = BuffManager.Instance.Get("pc_bang_fp_up");
		if (tBuff != null && tBuff.IsPoint)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("POINT_UP"), tBuff.PointRatio), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		tBuff = BuffManager.Instance.Get("pc_bang_xp_up");
		if (tBuff != null && tBuff.IsXp)
		{
			if (!calcHeight)
			{
				LabelUtil.TextOut(new Vector2(10f, pos.y), string.Format(StringMgr.Instance.Get("XP_UP"), tBuff.XpRatio), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			}
			pos.y += 20f;
		}
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(10f, pos.y), StringMgr.Instance.Get("PCBANG_BUFF1"), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 20f;
		if (!calcHeight)
		{
			LabelUtil.TextOut(new Vector2(10f, pos.y), StringMgr.Instance.Get("PCBANG_BUFF2"), "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		pos.y += 20f;
	}
}
