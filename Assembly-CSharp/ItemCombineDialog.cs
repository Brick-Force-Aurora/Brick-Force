using System;
using UnityEngine;

[Serializable]
public class ItemCombineDialog : Dialog
{
	private enum COMBINE_STATE
	{
		NONE,
		SELECT,
		WAIT,
		COMBINE
	}

	public Tooltip tooltip;

	public Texture2D premiumIcon;

	public Texture2D slotLock;

	public Texture2D combineIcon;

	public Texture2D cancelIcon;

	public UIImageList uiImages;

	public UIChangeColor uiPreCombineEffect;

	public UIChangeColor uiCombineEffect;

	public AudioClip sndCombine;

	private Rect crdX = new Rect(716f, 10f, 34f, 34f);

	private string lastTooltip = string.Empty;

	private Vector2 ltTooltip = Vector2.zero;

	private float yOffset = 10f;

	private Vector2 crdItem = new Vector2(135f, 121f);

	private Rect crdItemActionOutline = new Rect(15f, 185f, 730f, 380f);

	private Rect crdItemList = new Rect(27f, 200f, 700f, 355f);

	private Rect crdItemBtn = new Rect(3f, 3f, 122f, 105f);

	private Vector2 crdRemain = new Vector2(120f, 98f);

	private Vector2 crdItemUsage = new Vector2(122f, 85f);

	private Color disabledColor = new Color(0.92f, 0.05f, 0.05f, 0.58f);

	private Rect crdSrcItem = new Rect(15f, 62f, 160f, 98f);

	private Rect crdAdditiveItem = new Rect(218f, 62f, 160f, 98f);

	private Rect crdDstItem = new Rect(442f, 62f, 160f, 98f);

	private Vector2 crdRemainWise = new Vector2(150f, 80f);

	private Rect crdCombineButton = new Rect(608f, 65f, 143f, 46f);

	private Rect crdCancelButton = new Rect(608f, 114f, 143f, 46f);

	private int curItem = -1;

	private Item[] myItems;

	private float focusTime;

	private Vector2 scrollPositionShooterTool = new Vector2(0f, 0f);

	private Item srcItem;

	private Item additiveItem;

	private Item dstItem;

	private bool dstItemNoDraw;

	private bool premiumAccount;

	private COMBINE_STATE state;

	public override void Start()
	{
		tooltip.Start();
		id = DialogManager.DIALOG_INDEX.ITEM_COMBINE;
	}

	public override void OnPopup()
	{
		rc = new Rect(0f, 0f, GlobalVars.Instance.ScreenRect.width, GlobalVars.Instance.ScreenRect.height);
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
		additiveItem = null;
		dstItem = null;
		uiPreCombineEffect.IsDraw = false;
		uiCombineEffect.IsDraw = false;
	}

	public override void Update()
	{
		focusTime += Time.deltaTime;
		if (dstItemNoDraw)
		{
			uiPreCombineEffect.Update();
		}
		else
		{
			uiCombineEffect.Update();
		}
	}

	public void InitDialog(Item _item)
	{
		srcItem = _item;
		premiumAccount = (MyInfoManager.Instance.HaveFunction("premium_account") >= 0);
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
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("COMPOSE_ITEM"), "BigLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
	}

	private string ItemSlot2Tooltip(Item item, int i)
	{
		return (item != null) ? ("*" + i.ToString() + item.Seq.ToString()) : string.Empty;
	}

	private void DoTooltip(Vector2 offset)
	{
		Dialog top = DialogManager.Instance.GetTop();
		if (GUI.tooltip.Length > 0 && top != null && top.ID == DialogManager.DIALOG_INDEX.ITEM_COMBINE)
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

	private void DoItems()
	{
		uiImages.Draw();
		WiseSlot(crdSrcItem, srcItem);
		WiseSlot(crdAdditiveItem, additiveItem);
		if (!dstItemNoDraw)
		{
			WiseSlot(crdDstItem, dstItem);
		}
		GUI.Box(crdItemActionOutline, string.Empty, "LineBoxBlue");
		int num = 0;
		Item source = (srcItem == null) ? dstItem : srcItem;
		myItems = MyInfoManager.Instance.GetItemsCanMerge(source);
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
		scrollPositionShooterTool = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, crdItemList.width - 20f, num6), position: crdItemList, scrollPosition: scrollPositionShooterTool, alwaysShowHorizontal: false, alwaysShowVertical: false);
		float y = scrollPositionShooterTool.y;
		float num7 = y + crdItemList.height;
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
							ltTooltip = new Vector2(rc.x + crdItemList.x + position.x + position.width, rc.y + crdItemList.y + position.y - y);
						}
						else
						{
							ltTooltip = new Vector2(rc.x + crdItemList.x + position.x - tooltip.size.x, rc.y + crdItemList.y + position.y - y);
						}
					}
					string str = "BtnItem";
					if (tItem.season == 2)
					{
						str = "BtnItem2";
					}
					if (GlobalVars.Instance.MyButton(crdItemBtn, new GUIContent(string.Empty, myItems[num].Seq.ToString()), str) && state != COMBINE_STATE.WAIT)
					{
						ResetCombineItem();
						additiveItem = myItems[num];
						state = COMBINE_STATE.SELECT;
						curItem = num;
						AutoFunctionManager.Instance.DeleteAllAutoFunction();
					}
					DrawItemIcon(crdIcon: new Rect(crdItemBtn.x + 4f, crdItemBtn.y + 14f, (float)(int)((double)tItem.CurIcon().width * 0.65), (float)(int)((double)tItem.CurIcon().height * 0.65)), item: myItems[num]);
					Color color = GUI.color;
					GUI.color = GlobalVars.Instance.txtMainColor;
					GUI.Label(crdItemBtn, tItem.Name, "MiniLabel");
					GUI.color = color;
					ShowItemStatus(myItems[num]);
					LabelUtil.TextOut(crdRemain, myItems[num].GetRemainString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleRight);
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
	}

	public void WiseSlot(Rect rcBox, Item item)
	{
		if (item == null)
		{
			GUI.Box(rcBox, string.Empty, "BoxInnerLine");
		}
		else
		{
			string b = item.Seq.ToString() + "a";
			GUI.Box(rcBox, new GUIContent(string.Empty, b), "BoxInnerLine");
			GUI.BeginGroup(rcBox);
			TItem tItem = TItemManager.Instance.Get<TItem>(item.Code);
			if (tooltip.ItemSeq == b)
			{
				if (rcBox.x < 400f)
				{
					ltTooltip = new Vector2(rcBox.x + rcBox.width + 5f, rcBox.y);
				}
				else
				{
					ltTooltip = new Vector2(rcBox.x - tooltip.size.x - 5f, rcBox.y);
				}
			}
			Rect crdIcon = new Rect(crdItemBtn.x + 20f, crdItemBtn.y + 16f, (float)(int)((double)tItem.CurIcon().width * 0.65), (float)(int)((double)tItem.CurIcon().height * 0.65));
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
			LabelUtil.TextOut(crdRemainWise, item.GetRemainString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.MiddleRight);
			GUI.EndGroup();
		}
	}

	private void DoButtons()
	{
		GUIContent content = new GUIContent(StringMgr.Instance.Get("COMPOSE").ToUpper(), combineIcon);
		if (GlobalVars.Instance.MyButton3(crdCombineButton, content, "BtnAction") && state == COMBINE_STATE.SELECT)
		{
			CSNetManager.Instance.Sock.SendCS_MERGE_ITEM_REQ(additiveItem.Seq, srcItem.Seq, srcItem.Code);
			state = COMBINE_STATE.WAIT;
		}
		content = new GUIContent(StringMgr.Instance.Get("CANCEL_COMPOSE").ToUpper(), cancelIcon);
		if (GlobalVars.Instance.MyButton3(crdCancelButton, content, "BtnAction") && state == COMBINE_STATE.SELECT)
		{
			additiveItem = null;
			curItem = -1;
		}
		if (!dstItemNoDraw)
		{
			uiCombineEffect.Draw();
		}
		else
		{
			uiPreCombineEffect.Draw();
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
		if (GlobalVars.Instance.MyButton(crdX, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		DoItems();
		DoButtons();
		DoTooltip(new Vector2(position.x, position.y));
		GUI.EndGroup();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private bool ResetCombineItem()
	{
		if (state != COMBINE_STATE.COMBINE)
		{
			return true;
		}
		if (dstItem != null && srcItem == null)
		{
			srcItem = dstItem;
			dstItem = null;
		}
		state = COMBINE_STATE.NONE;
		uiPreCombineEffect.IsDraw = false;
		uiCombineEffect.IsDraw = false;
		return true;
	}

	public void CombineEnd()
	{
		state = COMBINE_STATE.COMBINE;
		dstItem = srcItem;
		additiveItem = null;
		srcItem = null;
		uiPreCombineEffect.Reset();
		uiCombineEffect.IsDraw = false;
		dstItemNoDraw = true;
		AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 0.2f, 1E-05f, CombineEffectShow));
		AutoFunctionManager.Instance.AddAutoFunction(new AutoFunction(null, 4f, 1E-05f, ResetCombineItem));
		GlobalVars.Instance.PlayOneShot(sndCombine);
	}

	private bool CombineEffectShow()
	{
		dstItemNoDraw = false;
		uiCombineEffect.Reset();
		return true;
	}

	public void CombineFail()
	{
		state = COMBINE_STATE.SELECT;
	}
}
