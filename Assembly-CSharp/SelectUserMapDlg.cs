using System;
using UnityEngine;

[Serializable]
public class SelectUserMapDlg : Dialog
{
	private Item item;

	private UserMapInfo[] umi;

	public float doubleClickTimeout = 0.3f;

	public Texture2D slotLock;

	public Texture2D nonAvailable;

	public Texture2D emptySlot;

	public Texture2D premiumIcon;

	public Texture2D slotEmpty;

	public Texture2D selectedMapFrame;

	private Vector2 crdTitle = new Vector2(292f, 34f);

	private Rect crdOutline = new Rect(12f, 70f, 560f, 300f);

	private Rect crdCloseBtn = new Rect(544f, 5f, 34f, 34f);

	private Vector2 crdMapSize = new Vector2(113f, 112f);

	private float crdMapOffset = 15f;

	private Rect crdSlotPosition = new Rect(22f, 100f, 540f, 250f);

	private Rect crdButtonOk = new Rect(400f, 377f, 178f, 34f);

	private Vector2 crdAlias = new Vector2(5f, 93f);

	private Color txtMainClr = Color.white;

	private Vector2 umiScrollPosition = Vector2.zero;

	private int umiSlot;

	private float lastClickTime;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SELECT_USER_MAP;
		txtMainClr = GlobalVars.Instance.GetByteColor2FloatColor(244, 151, 25);
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(Item _item)
	{
		item = _item;
		umiSlot = 0;
		UserMapInfoManager.Instance.Verify();
		umi = UserMapInfoManager.Instance.ToArray();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("RESET_MAP_SLOT"), "BigBtnLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		if (GlobalVars.Instance.MyButton(crdCloseBtn, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		DoSlots();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void DoSlots()
	{
		bool flag = MyInfoManager.Instance.HaveFunction("premium_account") >= 0;
		int num = umi.Length / 4;
		if (umi.Length % 4 > 0)
		{
			num++;
		}
		Rect viewRect = new Rect(0f, 0f, crdMapSize.x * 4f + crdMapOffset * 3f, crdMapSize.y * (float)num);
		if (num > 1)
		{
			viewRect.height += crdMapOffset * (float)(num - 1);
		}
		umiScrollPosition = GUI.BeginScrollView(crdSlotPosition, umiScrollPosition, viewRect);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num2 = 4 * i + j;
				if (num2 < umi.Length)
				{
					Texture2D texture2D = null;
					texture2D = ((umi[num2].Alias.Length <= 0) ? emptySlot : ((!(umi[num2].Thumbnail == null)) ? umi[num2].Thumbnail : nonAvailable));
					Rect rect = new Rect((float)j * (crdMapSize.x + crdMapOffset), (float)i * (crdMapSize.y + crdMapOffset), crdMapSize.x, crdMapSize.y);
					Rect position = new Rect(rect.x, rect.y, rect.width, rect.height);
					TextureUtil.DrawTexture(position, texture2D, ScaleMode.StretchToFill);
					if (GlobalVars.Instance.MyButton(rect, string.Empty, (!umi[num2].IsPremium) ? "BoxMapSelectBorder" : "BoxMapSelectBorderPremium") && (!umi[num2].IsPremium || flag))
					{
						umiSlot = umi[num2].Slot;
						if (Time.time - lastClickTime > doubleClickTimeout)
						{
							lastClickTime = Time.time;
						}
					}
					if (umi[num2].Alias.Length > 0)
					{
						LabelUtil.TextOut(new Vector2(rect.x + crdAlias.x, rect.y + crdAlias.y), umi[num2].Alias, "MiniLabel", txtMainClr, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
					}
					bool flag2 = false;
					if (umi[num2].IsPremium)
					{
						if (!flag)
						{
							flag2 = true;
							TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)slotLock.width) / 2f, rect.y + (rect.height - (float)slotLock.height) / 2f - 13f, (float)slotLock.width, (float)slotLock.height), slotLock, ScaleMode.StretchToFill);
						}
						TextureUtil.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, (float)premiumIcon.width, (float)premiumIcon.height), premiumIcon);
					}
					if (!flag2 && umi[num2].Alias.Length <= 0)
					{
						TextureUtil.DrawTexture(new Rect(rect.x + (rect.width - (float)slotEmpty.width) / 2f, rect.y + (rect.height - (float)slotEmpty.height) / 2f - 13f, (float)slotEmpty.width, (float)slotEmpty.height), slotEmpty, ScaleMode.StretchToFill);
					}
					if (umiSlot == umi[num2].Slot)
					{
						TextureUtil.DrawTexture(rect, selectedMapFrame, ScaleMode.StretchToFill);
					}
				}
			}
		}
		GUI.EndScrollView();
		UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get(umiSlot);
		GUIContent content = new GUIContent(StringMgr.Instance.Get("RESET_MAP_SLOT").ToUpper(), GlobalVars.Instance.iconBlock);
		if (GlobalVars.Instance.MyButton3(crdButtonOk, content, "BtnAction") && userMapInfo != null && userMapInfo != null)
		{
			if (userMapInfo.Alias.Length <= 0)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("EMPTY_SLOT_ALREADY"));
			}
			else if (userMapInfo.IsPremium && !flag)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LOCKED_SLOT_PREMIUM"));
			}
			else
			{
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true))?.InitDialog(item, AreYouSure.SURE.RESET_MAP_SLOT, userMapInfo);
			}
		}
	}
}
