using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemUpgradeDlg : Dialog
{
	public class NowUseProp
	{
		public bool use;

		public int prop;

		public void reset()
		{
			use = false;
			prop = -1;
		}
	}

	public enum UPGRADE_STATE
	{
		NONE,
		SELECT,
		GAUGE_RISE,
		GAUGE_PROGRESS,
		WAIT,
		GAUGE_END,
		EFFECT,
		GAUGE_DESCEND
	}

	public Texture2D texUpgradeBG;

	public Tooltip tooltip;

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	public Rect crdPanelItem = new Rect(413f, 308f, 156f, 134f);

	private Rect crdPanelUpgrade = new Rect(640f, 65f, 247f, 186f);

	private Rect[] crdUpgradeProps;

	public Rect crdBtnUpgrade = new Rect(812f, 371f, 74f, 74f);

	public Rect crdUpgradeItemListBg = new Rect(21f, 322f, 558f, 243f);

	private Vector2 crdItemStart = new Vector2(126f, 482f);

	private Vector2 crdItem = new Vector2(175f, 138f);

	private Rect crdItemBtn = new Rect(3f, 3f, 156f, 132f);

	private Vector2 crdItemName = new Vector2(2f, 2f);

	private Vector2 crdRemain = new Vector2(150f, 124f);

	public Vector2 scrollPosition = new Vector2(0f, 0f);

	public Rect crdItemSlotList = new Rect(128f, 483f, 712f, 140f);

	private Vector2 crdEnergyTank = new Vector2(183f, 427f);

	public Vector2 crdGagueBG = new Vector2(112f, 427f);

	public UIImage uiGagueBG;

	public UIImage uiGagueBack;

	public UISprite uiGagueLightning;

	private float gaguePercent = 50f;

	public Texture2D texGague;

	public Texture2D texGagueEnd;

	public float gagueUp;

	public float gagueDown;

	public float gagueVariance;

	private bool isGagueUp = true;

	private float curGagueSpeed;

	private float nextGagueGoal;

	private float gagueGoalMinus;

	public float gagueStartSpeed;

	public float gagueMaxSpeed;

	public float gagueAcceleration;

	public float gagueRiseSpeed;

	private float gagueYValue;

	private UPGRADE_STATE upgradeState;

	public UIImage successFace;

	public UIImage failFace;

	public UIImageRotate successRotate;

	public UISprite successEffect;

	public UISprite failEffect;

	public UISprite failEffect2;

	public UISprite failEffect3;

	public UISpriteMoveEmitter bubbleEffect;

	private float focusTime;

	public Texture2D texQuestion;

	public Texture2D energyTankOn;

	public Texture2D energyTankOff;

	public string[] UpgradePropNames;

	private int successUpgradePropID;

	private bool[] useprops;

	private int reqprop = -1;

	private int curTier;

	private bool bProgess;

	public Color disabledColor = new Color(1f, 1f, 1f, 0.7f);

	private string strErr = string.Empty;

	private NowUseProp[] curuseprops;

	private Item upgradee;

	private Item[] myUpgradeItems;

	private Color txtMainClr;

	private int curItem = -1;

	public AudioClip sndUpgradeStart;

	public AudioClip sndUpgradeSuccess;

	public AudioClip sndUpgradeFail;

	public AudioClip sndGagueUp;

	public AudioClip sndGagueDown;

	public override void Start()
	{
		tooltip.Start();
		id = DialogManager.DIALOG_INDEX.ITEM_UPGRADE;
		size.x = (float)texUpgradeBG.width;
		size.y = (float)texUpgradeBG.height;
		crdUpgradeProps = new Rect[5];
		crdUpgradeProps[0] = new Rect(666f, 129f, 21f, 22f);
		crdUpgradeProps[1] = new Rect(666f, 159f, 21f, 22f);
		crdUpgradeProps[2] = new Rect(666f, 189f, 21f, 22f);
		crdUpgradeProps[3] = new Rect(666f, 219f, 21f, 22f);
		crdUpgradeProps[4] = new Rect(666f, 249f, 21f, 22f);
		curuseprops = new NowUseProp[5];
		for (int i = 0; i < curuseprops.Length; i++)
		{
			curuseprops[i] = new NowUseProp();
			curuseprops[i].reset();
		}
	}

	public override void Update()
	{
		focusTime += Time.deltaTime;
		uiGagueLightning.Update();
		if (upgradeState >= UPGRADE_STATE.GAUGE_PROGRESS && upgradeState <= UPGRADE_STATE.GAUGE_END)
		{
			if (upgradeState == UPGRADE_STATE.GAUGE_PROGRESS)
			{
				curGagueSpeed += Time.deltaTime * gagueAcceleration;
				if (curGagueSpeed > gagueMaxSpeed)
				{
					curGagueSpeed = gagueMaxSpeed;
					upgradeState = UPGRADE_STATE.WAIT;
					reqprop = getSelectedProp();
					Item item = myUpgradeItems[curItem];
					if (item != null)
					{
						CSNetManager.Instance.Sock.SendCS_UPGRADE_ITEM_REQ(upgradee.Seq, upgradee.Code, item.Seq, item.Code, reqprop);
					}
					else
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_FOUND_UPGRADER"));
					}
					bProgess = false;
				}
			}
			if (isGagueUp)
			{
				gaguePercent += Time.deltaTime * curGagueSpeed;
			}
			else
			{
				gaguePercent -= Time.deltaTime * curGagueSpeed;
			}
			if (upgradeState == UPGRADE_STATE.GAUGE_END)
			{
				if (gaguePercent > 100f)
				{
					gaguePercent = 100f;
					upgradeState = UPGRADE_STATE.EFFECT;
					if (GlobalVars.Instance.successUpgradePropID >= 0)
					{
						int num = GlobalVars.Instance.successUpgradePropID;
						successUpgradePropID = GlobalVars.Instance.successUpgradePropID;
						SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("ITEMUPGRADE_SUCCESS"), StringMgr.Instance.Get(UpgradePropNames[num])));
						GlobalVars.Instance.successUpgradePropID = -1;
						GlobalVars.Instance.PlayOneShot(sndUpgradeSuccess);
						AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 2.3f, 1E-05f, EffectEnd));
					}
				}
				if (gaguePercent < 0f)
				{
					gaguePercent = 0f;
					upgradeState = UPGRADE_STATE.EFFECT;
					failEffect.ResetTime();
					failEffect2.SetEndTime();
					failEffect3.SetEndTime();
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
					GlobalVars.Instance.PlayOneShot(sndUpgradeFail);
					AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 0.4f, 1E-05f, FailEffect2));
					AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 0.5f, 1E-05f, FailEffect3));
					AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 2.3f, 1E-05f, EffectEnd));
				}
			}
			else if (isGagueUp && nextGagueGoal < gaguePercent)
			{
				if (gaguePercent > 99f)
				{
					gaguePercent = 99f;
				}
				SetGagueGoal(gagueUpProgress: false);
			}
			else if (!isGagueUp && nextGagueGoal > gaguePercent)
			{
				SetGagueGoal(gagueUpProgress: true);
			}
		}
		if (upgradeState == UPGRADE_STATE.EFFECT)
		{
			successRotate.Update();
			successEffect.Update();
			failEffect.Update();
			failEffect2.Update();
			failEffect3.Update();
		}
		bubbleEffect.Update();
	}

	private bool FailEffect2()
	{
		failEffect2.ResetTime();
		return false;
	}

	private bool FailEffect3()
	{
		failEffect3.ResetTime();
		return false;
	}

	private bool EffectEnd()
	{
		upgradeState = UPGRADE_STATE.GAUGE_DESCEND;
		GlobalVars.Instance.PlayOneShot(sndGagueDown);
		AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(GagueDescend, 10f));
		return false;
	}

	private void SetGagueGoal(bool gagueUpProgress)
	{
		isGagueUp = gagueUpProgress;
		if (isGagueUp)
		{
			nextGagueGoal = gagueUp;
		}
		else
		{
			nextGagueGoal = gagueDown;
		}
		nextGagueGoal += UnityEngine.Random.Range(-1f * gagueVariance, gagueVariance);
		nextGagueGoal -= gagueGoalMinus;
		if (gagueGoalMinus > 0f)
		{
			gagueGoalMinus -= 10f;
		}
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		txtMainClr = GlobalVars.Instance.txtMainColor;
		resetUseProps();
		curItem = -1;
		strErr = string.Empty;
		upgradeState = UPGRADE_STATE.NONE;
	}

	public void InitDialog(Item _item)
	{
		upgradee = _item;
	}

	public override bool DoDialog()
	{
		bool result = false;
		TextureUtil.DrawTexture(new Rect(0f, 0f, (float)texUpgradeBG.width, (float)texUpgradeBG.height), texUpgradeBG);
		bubbleEffect.Draw();
		myUpgradeItems = MyInfoManager.Instance.GetItemsByCat(0, 7, 0);
		RemoveNotUseItem();
		DoDoctorText();
		DoEnergyTank();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("ITEM_UPGRADE").ToUpper(), "BigLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		TItem template = upgradee.Template;
		DoItem(crdPanelItem, upgradee);
		if (strErr.Length > 0)
		{
			float x = crdPanelUpgrade.x + crdPanelUpgrade.width / 2f - 90f;
			float y = crdPanelUpgrade.y + crdPanelUpgrade.height / 2f - 30f;
			GUI.Label(new Rect(x, y, 210f, 60f), strErr, "MiniLabel");
		}
		else if (curTier >= 0)
		{
			TextureUtil.DrawTexture(new Rect(734f, 143f, 90f, 90f), texQuestion, ScaleMode.StretchToFill);
		}
		else
		{
			UpgradeSelector((int)template.upgradeCategory);
		}
		if (bProgess)
		{
			if (upgradeState == UPGRADE_STATE.GAUGE_RISE)
			{
				GUI.enabled = false;
			}
			if (GlobalVars.Instance.MyButton(crdBtnUpgrade, string.Empty, "UpgradeCancel") || GlobalVars.Instance.IsEscapePressed())
			{
				GlobalVars.Instance.StopSound();
				AutoFunctionManager.Instance.DeleteAllAutoFunction();
				EffectEnd();
				bProgess = false;
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_CANCELED"));
			}
			if (upgradeState == UPGRADE_STATE.GAUGE_RISE)
			{
				GUI.enabled = true;
			}
		}
		else
		{
			if (!IsEnableUpgradeButton() || upgradeState >= UPGRADE_STATE.WAIT)
			{
				GUI.enabled = false;
			}
			if (GlobalVars.Instance.MyButton(crdBtnUpgrade, string.Empty, "UpgradeStart") && CanUpgradeXpNLv())
			{
				StartUpgrade();
				bProgess = true;
			}
			if (!IsEnableUpgradeButton() || upgradeState >= UPGRADE_STATE.WAIT)
			{
				GUI.enabled = true;
			}
		}
		int num = myUpgradeItems.Length;
		if (num > 0)
		{
			int num2 = 4;
			int num3 = num / num2;
			if (num % num2 > 0)
			{
				num3++;
			}
			float num4 = crdItem.x * (float)num2;
			if (num2 > 1)
			{
				num4 += (float)((num2 - 1) * 2);
			}
			float num5 = crdItem.y * (float)num3;
			if (num3 > 1)
			{
				num5 += (float)((num3 - 1) * 4);
			}
			Rect position = new Rect(crdItemStart.x, crdItemStart.y, crdItem.x, crdItem.y);
			scrollPosition = GUI.BeginScrollView(viewRect: new Rect(crdItemStart.x, crdItemStart.y, crdItemSlotList.width - 20f, num5), position: crdItemSlotList, scrollPosition: scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false);
			int num6 = 0;
			int num7 = 0;
			while (num6 < num && num7 < num3)
			{
				position.y = crdItemStart.y + (float)num7 * crdItem.y;
				int num8 = 0;
				while (num6 < num && num8 < num2)
				{
					position.x = crdItemStart.x + (float)num8 * (crdItem.x + 2f);
					GUI.BeginGroup(position);
					TItem template2 = myUpgradeItems[num6].Template;
					string str = "BtnItem";
					if (template2.season == 2)
					{
						str = "BtnItem2";
					}
					if (GlobalVars.Instance.MyButton(crdItemBtn, new GUIContent(string.Empty, myUpgradeItems[num6].Seq.ToString()), str))
					{
						if (upgradeState == UPGRADE_STATE.NONE)
						{
							upgradeState = UPGRADE_STATE.SELECT;
						}
						strErr = string.Empty;
						curItem = num6;
					}
					if (tooltip.ItemSeq == myUpgradeItems[num6].Seq.ToString())
					{
						if (position.x < 400f)
						{
							ltTooltip = new Vector2(position.x + position.width + 5f, position.y);
						}
						else
						{
							ltTooltip = new Vector2(position.x - tooltip.size.x - 5f, position.y);
						}
					}
					LabelUtil.TextOut(crdItemName, template2.Name, "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					DrawItemIcon(crdIcon: new Rect(crdItemBtn.x + 4f, crdItemBtn.y + 13f, (float)(int)((float)template2.CurIcon().width * 0.9f), (float)(int)((float)template2.CurIcon().height * 0.9f)), item: myUpgradeItems[num6]);
					LabelUtil.TextOut(crdRemain, myUpgradeItems[num6].GetRemainString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleRight);
					if (num6 == curItem)
					{
						TUpgrade tUpgrade = (TUpgrade)template2;
						curTier = tUpgrade.tier;
						GUI.Box(new Rect(crdItemBtn.x - 3f, crdItemBtn.y - 3f, crdItemBtn.width + 6f, crdItemBtn.height + 6f), string.Empty, "BtnItemF");
						if (strErr == string.Empty)
						{
							strErr = CanUseUpgrader(tUpgrade);
						}
					}
					GUI.EndGroup();
					num6++;
					num8++;
				}
				num7++;
			}
			GUI.EndScrollView();
		}
		else
		{
			float x2 = crdUpgradeItemListBg.x + crdUpgradeItemListBg.width / 2f;
			float y2 = crdUpgradeItemListBg.y + crdUpgradeItemListBg.height / 2f;
			LabelUtil.TextOut(new Vector2(x2, y2), StringMgr.Instance.Get("NOTHING_UPGRADER"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		}
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			bProgess = false;
			result = true;
			GlobalVars.Instance.StopSound();
			GlobalVars.Instance.PlayOneShot(GlobalVars.Instance.sndButtonClick);
			AutoFunctionManager.Instance.DeleteAllAutoFunction();
		}
		DoTooltip(new Vector2(base.rc.x, base.rc.y));
		DoGagueEffect();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}

	private void DoEnergyTank()
	{
		for (int i = 0; i < 4; i++)
		{
			if (curItem == i)
			{
				TextureUtil.DrawTexture(new Rect(crdEnergyTank.x + (float)(i * 178), crdEnergyTank.y, (float)energyTankOn.width, (float)energyTankOn.height), energyTankOn);
			}
			else
			{
				TextureUtil.DrawTexture(new Rect(crdEnergyTank.x + (float)(i * 178), crdEnergyTank.y, (float)energyTankOff.width, (float)energyTankOff.height), energyTankOff);
			}
		}
	}

	private void DoGagueEffect()
	{
		if (upgradeState > UPGRADE_STATE.SELECT)
		{
			GUI.BeginGroup(new Rect(crdGagueBG.x, crdGagueBG.y + gagueYValue, (float)uiGagueBG.texImage.width, (float)uiGagueBG.texImage.height));
			uiGagueBack.Draw();
			float num = uiGagueBack.area.x * gaguePercent * 0.01f;
			Vector2 showPosition = uiGagueBack.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = uiGagueBack.showPosition;
			TextureUtil.DrawTexture(new Rect(x, showPosition2.y, num, uiGagueBack.area.y), texGague);
			Vector2 showPosition3 = uiGagueBack.showPosition;
			float x2 = showPosition3.x + num - (float)texGagueEnd.width;
			Vector2 showPosition4 = uiGagueBack.showPosition;
			TextureUtil.DrawTexture(new Rect(x2, showPosition4.y, (float)texGagueEnd.width, (float)texGagueEnd.height), texGagueEnd);
			uiGagueLightning.Draw();
			uiGagueBG.Draw();
			GUI.EndGroup();
			if (upgradeState == UPGRADE_STATE.EFFECT)
			{
				if (isGagueUp)
				{
					successRotate.Draw();
					successFace.Draw();
					successEffect.Draw();
				}
				else
				{
					failFace.Draw();
					failEffect.Draw();
					failEffect2.Draw();
					failEffect3.Draw();
				}
			}
		}
	}

	private void StartUpgrade()
	{
		gaguePercent = 0f;
		gagueGoalMinus = 50f;
		SetGagueGoal(gagueUpProgress: true);
		curGagueSpeed = gagueStartSpeed;
		gagueYValue = (float)(uiGagueBG.texImage.height + 1);
		upgradeState = UPGRADE_STATE.GAUGE_RISE;
		AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(GagueRise, 10f));
		GlobalVars.Instance.PlayOneShot(sndGagueUp);
	}

	private bool GagueRise()
	{
		gagueYValue -= Time.deltaTime * gagueRiseSpeed;
		if (gagueYValue < 0f)
		{
			gagueYValue = 0f;
			upgradeState = UPGRADE_STATE.GAUGE_PROGRESS;
			GlobalVars.Instance.PlayOneShot(sndUpgradeStart);
			return true;
		}
		return false;
	}

	private bool GagueDescend()
	{
		gagueYValue += Time.deltaTime * gagueRiseSpeed;
		if (gagueYValue > (float)(uiGagueBG.texImage.height + 1))
		{
			gagueYValue = (float)(uiGagueBG.texImage.height + 1);
			upgradeState = UPGRADE_STATE.SELECT;
			return true;
		}
		return false;
	}

	private void DoDoctorText()
	{
		LabelUtil.TextOut(new Vector2(130f, 95f), StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR_NAME"), "MiniLabel", new Color(1f, 1f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Label(new Rect(130f, 115f, 245f, 130f), GetDoctorText(), "MiniLabel");
	}

	private string GetDoctorText()
	{
		if (curItem >= 0 || upgradeState == UPGRADE_STATE.NONE)
		{
			switch (upgradeState)
			{
			case UPGRADE_STATE.NONE:
				return StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR1");
			case UPGRADE_STATE.SELECT:
			{
				TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
				if (tUpgrade.tier < 0)
				{
					return string.Format(StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR3"), StringMgr.Instance.Get(tUpgrade.name));
				}
				return string.Format(StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR2"), StringMgr.Instance.Get(tUpgrade.name));
			}
			case UPGRADE_STATE.GAUGE_RISE:
			case UPGRADE_STATE.GAUGE_PROGRESS:
			case UPGRADE_STATE.WAIT:
			case UPGRADE_STATE.GAUGE_END:
				return StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR4");
			case UPGRADE_STATE.EFFECT:
			case UPGRADE_STATE.GAUGE_DESCEND:
				if (isGagueUp)
				{
					return string.Format(StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR5"), StringMgr.Instance.Get(UpgradePropNames[successUpgradePropID]));
				}
				return StringMgr.Instance.Get("ITEMUPGRADE_DOCTOR6");
			default:
				return string.Empty;
			}
		}
		return string.Empty;
	}

	private string CanUseUpgrader(TUpgrade upItem)
	{
		int num = 0;
		int num2 = 13;
		for (int i = 0; i < num2; i++)
		{
			if (UpgradePropManager.Instance.UseProp((int)upgradee.Template.upgradeCategory, i))
			{
				num++;
			}
		}
		int num3 = 0;
		for (int j = 0; j < num2; j++)
		{
			if (upgradee.upgradeProps[j].use && upgradee.upgradeProps[j].grade == upItem.maxLv)
			{
				num3++;
			}
		}
		if (num == num3)
		{
			return StringMgr.Instance.Get("NOT_PROCCESS_UPGRADE_THIS");
		}
		if (XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp) < upItem.playerLv)
		{
			string rank = XpManager.Instance.GetRank(upItem.playerLv, -1);
			return string.Format(StringMgr.Instance.Get("UPGRADE_GEM_IS_POSSIBLE_FROM"), rank);
		}
		return string.Empty;
	}

	private bool CanUseUpgradable()
	{
		TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
		int num = 0;
		int num2 = 13;
		for (int i = 0; i < num2; i++)
		{
			if (UpgradePropManager.Instance.UseProp((int)upgradee.Template.upgradeCategory, i))
			{
				num++;
			}
		}
		int num3 = 0;
		for (int j = 0; j < num2; j++)
		{
			if (upgradee.upgradeProps[j].use && upgradee.upgradeProps[j].grade >= tUpgrade.maxLv)
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

	private bool CanUseUpgraderLevel()
	{
		TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
		if (XpManager.Instance.GetLevel(MyInfoManager.Instance.Xp) < tUpgrade.playerLv)
		{
			return false;
		}
		return true;
	}

	private bool IsEnableUpgradeButton()
	{
		if (myUpgradeItems.Length == 0)
		{
			strErr = StringMgr.Instance.Get("BUY_UPGRADER_ITEM");
			return false;
		}
		if (0 > curItem || curItem >= myUpgradeItems.Length)
		{
			return false;
		}
		if (!myUpgradeItems[curItem].IsAmount || !myUpgradeItems[curItem].EnoughToConsume)
		{
			strErr = StringMgr.Instance.Get("SELECT_CORRECT_UPGRADER");
			return false;
		}
		if (!IsEqualType())
		{
			strErr = StringMgr.Instance.Get("SELECT_CORRECT_UPGRADER");
			return false;
		}
		if (!CanUseUpgraderLevel())
		{
			TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
			string rank = XpManager.Instance.GetRank(tUpgrade.playerLv, -1);
			strErr = string.Format(StringMgr.Instance.Get("UPGRADE_GEM_IS_POSSIBLE_FROM"), rank);
			return false;
		}
		if (!CanUseUpgradable())
		{
			strErr = StringMgr.Instance.Get("SELECT_CORRECT_UPGRADER");
			return false;
		}
		if (strErr.Length > 0)
		{
			return false;
		}
		return true;
	}

	private void RemoveNotUseItem()
	{
		int num = curItem;
		string text = strErr;
		List<Item> list = new List<Item>();
		for (int i = 0; i < myUpgradeItems.Length; i++)
		{
			curItem = i;
			if (IsEqualType())
			{
				list.Add(myUpgradeItems[i]);
			}
			else
			{
				strErr = string.Empty;
			}
		}
		strErr = text;
		curItem = num;
		myUpgradeItems = list.ToArray();
		if (curItem >= myUpgradeItems.Length)
		{
			curItem = 0;
		}
	}

	private bool CanUpgradeXpNLv()
	{
		TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
		if (tUpgrade.tier < 0)
		{
			int selectedProp = getSelectedProp();
			if (selectedProp < 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_SELECT_PROP"));
				return false;
			}
			return true;
		}
		return true;
	}

	private bool IsEqualType()
	{
		if (curItem < 0)
		{
			return false;
		}
		if (myUpgradeItems.Length == 0)
		{
			return false;
		}
		TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
		return (upgradee.Template.upgradeType == tUpgrade.targetType) ? true : false;
	}

	private void DrawItemIcon(Item item, Rect crdIcon)
	{
		Color color = GUI.color;
		Item.USAGE usage = item.Usage;
		if (usage == Item.USAGE.DELETED)
		{
			GUI.color = disabledColor;
		}
		TextureUtil.DrawTexture(crdIcon, item.Template.CurIcon(), ScaleMode.StretchToFill);
		GUI.color = color;
	}

	private void updateCheckState(int id)
	{
		for (int i = 0; i < curuseprops.Length; i++)
		{
			curuseprops[i].use = false;
		}
		if (id >= 0)
		{
			curuseprops[id].use = true;
		}
	}

	private int getSelectedProp()
	{
		for (int i = 0; i < curuseprops.Length; i++)
		{
			if (curuseprops[i].use)
			{
				return curuseprops[i].prop;
			}
		}
		return -1;
	}

	private void resetUseProps()
	{
		for (int i = 0; i < curuseprops.Length; i++)
		{
			curuseprops[i].reset();
		}
	}

	private bool CanUseUpgraderLevel(int propIndex)
	{
		if (curItem < 0 || curItem >= myUpgradeItems.Length)
		{
			return false;
		}
		TUpgrade tUpgrade = (TUpgrade)myUpgradeItems[curItem].Template;
		return upgradee.upgradeProps[propIndex].grade < tUpgrade.maxLv;
	}

	private void UpgradeSelector(int cat)
	{
		int num = 0;
		int num2 = 0;
		string empty = string.Empty;
		int num3 = 13;
		for (int i = 0; i < num3; i++)
		{
			if (UpgradePropManager.Instance.UseProp(cat, i))
			{
				bool enabled = GUI.enabled;
				if (upgradeState != UPGRADE_STATE.SELECT)
				{
					GUI.enabled = false;
				}
				curuseprops[num2].prop = i;
				int grade = upgradee.upgradeProps[i].grade;
				if (CanUseUpgraderLevel(i))
				{
					curuseprops[num2].use = GUI.Toggle(crdUpgradeProps[num], curuseprops[num2].use, StringMgr.Instance.Get(UpgradePropNames[i]));
					empty = grade.ToString() + " -> " + (grade + 1).ToString() + StringMgr.Instance.Get("UP");
				}
				else
				{
					curuseprops[num2].use = false;
					GUI.enabled = false;
					GUI.Toggle(crdUpgradeProps[num], curuseprops[num2].use, StringMgr.Instance.Get(UpgradePropNames[i]));
					GUI.enabled = true;
					empty = grade.ToString() + " / " + StringMgr.Instance.Get("MAX");
				}
				LabelUtil.TextOut(new Vector2(crdUpgradeProps[num].x + 215f, crdUpgradeProps[num].y), empty, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
				if (curuseprops[num2].use)
				{
					updateCheckState(num2);
				}
				if (upgradeState != UPGRADE_STATE.SELECT)
				{
					GUI.enabled = enabled;
				}
				num++;
				num2++;
			}
		}
	}

	public void PlaySoundUpgradeFail()
	{
		GlobalVars.Instance.PlayOneShot(sndUpgradeFail);
	}

	public void UpgradeResultSetting(bool result)
	{
		upgradeState = UPGRADE_STATE.GAUGE_END;
		curGagueSpeed = gagueMaxSpeed * 2f;
		isGagueUp = result;
	}

	public void DoItem(Rect rcBox, Item item)
	{
		if (item != null)
		{
			string b = item.Seq.ToString();
			GUI.Box(rcBox, new GUIContent(string.Empty, b), "NullWindow");
			GUI.BeginGroup(rcBox);
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Code);
			if (tooltip.ItemSeq == b)
			{
				ltTooltip = new Vector2(rcBox.x + rcBox.width + 5f, rcBox.y);
			}
			Rect crdIcon = new Rect(crdItemBtn.x, crdItemBtn.y + 16f, (float)(int)((float)tItem.CurIcon().width * 0.8f), (float)(int)((float)tItem.CurIcon().height * 0.8f));
			DrawItemIcon(item, crdIcon);
			if (item.IsUpgradedItem())
			{
				if (item.CanUpgradeAble())
				{
					TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 110f, crdItemBtn.y + 60f, 14f, 14f), GlobalVars.Instance.iconUpgrade, ScaleMode.ScaleToFit);
				}
				else
				{
					TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 110f, crdItemBtn.y + 60f, 16f, 16f), GlobalVars.Instance.iconUpgradeMax, ScaleMode.ScaleToFit);
				}
			}
			if (item.IsPCBang)
			{
				TextureUtil.DrawTexture(new Rect(crdItemBtn.x + 2f, crdItemBtn.y + 50f, 24f, 24f), GlobalVars.Instance.iconPCBang, ScaleMode.ScaleToFit);
			}
			Color color = GUI.color;
			GUI.color = GlobalVars.Instance.txtMainColor;
			GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
			GUI.color = color;
			GUI.EndGroup();
		}
	}

	private string ItemSlot2Tooltip(Item item, int i)
	{
		return (item != null) ? ("*" + i.ToString() + item.Seq.ToString()) : string.Empty;
	}

	private void DoTooltip(Vector2 offset)
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.ITEM_UPGRADE)
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
}
