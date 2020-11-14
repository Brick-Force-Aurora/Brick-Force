using System;
using UnityEngine;

[Serializable]
public class DownloadFeeDialog : Dialog
{
	public Texture2D texStarGradeBg;

	public Texture2D texStarGrade;

	public Texture2D nonAvailable;

	private RegMap regMap;

	private Good.BUY_HOW buyHow;

	private Vector2 crdThumbnail = new Vector2(100f, 100f);

	private Vector2 crdLTMapInfo = new Vector2(38f, 69f);

	private Vector2 crdSupportMode = new Vector2(470f, 65f);

	private Rect crdLTMode = new Rect(475f, 100f, 35f, 22f);

	private Rect crdBuyHowBox = new Rect(18f, 220f, 270f, 20f);

	private Vector2 crdBuyHowLabel = new Vector2(30f, 218f);

	private Rect crdBuyPoint = new Rect(25f, 250f, 21f, 22f);

	private Rect crdBuyBrick = new Rect(25f, 277f, 21f, 22f);

	private Rect crdBuyCash = new Rect(25f, 304f, 21f, 22f);

	private Rect crdPriceBox = new Rect(320f, 220f, 270f, 20f);

	private Vector2 crdPriceLabel = new Vector2(340f, 218f);

	private Vector2 crdPrice = new Vector2(475f, 252f);

	private Vector2 crdPointString = new Vector2(480f, 252f);

	private int xCount = 3;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.DOWNLOAD_FEE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	private void DoBuyHow()
	{
		GUI.Box(crdBuyHowBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdBuyHowLabel, StringMgr.Instance.Get("BUY_HOW"), "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			flag = true;
			break;
		case Good.BUY_HOW.BRICK_POINT:
			flag2 = true;
			break;
		case Good.BUY_HOW.CASH_POINT:
			flag3 = true;
			break;
		}
		bool flag4 = GUI.Toggle(crdBuyPoint, flag, StringMgr.Instance.Get("GENERAL_POINT"));
		bool flag5 = GUI.Toggle(crdBuyBrick, flag2, StringMgr.Instance.Get("BRICK_POINT"));
		bool flag6 = GUI.Toggle(crdBuyCash, flag3, TokenManager.Instance.GetTokenString());
		if (!flag && flag4)
		{
			buyHow = Good.BUY_HOW.GENERAL_POINT;
		}
		if (!flag2 && flag5)
		{
			buyHow = Good.BUY_HOW.BRICK_POINT;
		}
		if (!flag3 && flag6)
		{
			buyHow = Good.BUY_HOW.CASH_POINT;
		}
	}

	private bool DoPrice()
	{
		GUI.Box(crdPriceBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdPriceLabel, StringMgr.Instance.Get("DOWNLOAD_FEE"), "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bool flag = false;
		string text = StringMgr.Instance.Get("BRICK_POINT");
		int num = (!RegMapManager.Instance.IsDeleted(regMap.Map)) ? regMap.DownloadFee : 0;
		int num2 = num;
		switch (buyHow)
		{
		case Good.BUY_HOW.GENERAL_POINT:
			if (BuildOption.Instance.Props.useBrickPoint)
			{
				num2 = num * 10;
			}
			text = StringMgr.Instance.Get("GENERAL_POINT");
			flag = (MyInfoManager.Instance.Point >= num2);
			break;
		case Good.BUY_HOW.BRICK_POINT:
			flag = (MyInfoManager.Instance.BrickPoint >= num2);
			break;
		case Good.BUY_HOW.CASH_POINT:
			text = TokenManager.Instance.GetTokenString();
			num2 = ((!BuildOption.Instance.Props.useBrickPoint) ? Mathf.FloorToInt((float)num * 0.02f) : Mathf.FloorToInt((float)num * 0.2f));
			flag = (MyInfoManager.Instance.Cash >= num2);
			break;
		}
		Debug.Log(BuildOption.Instance.Props.useBrickPoint.ToString() + num + "  " + num2 + "  " + Time.frameCount);
		LabelUtil.TextOut(crdPrice, num2.ToString(), "Label", (!flag) ? Color.red : Color.green, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdPointString, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		return flag;
	}

	private void DrawMode(ushort modeMask)
	{
		Room.MODE_MASK[] array = new Room.MODE_MASK[9]
		{
			Room.MODE_MASK.TEAM_MATCH_MASK,
			Room.MODE_MASK.INDIVIDUAL_MATCH_MASK,
			Room.MODE_MASK.CAPTURE_THE_FALG_MATCH,
			Room.MODE_MASK.EXPLOSION_MATCH,
			Room.MODE_MASK.MISSION_MASK,
			Room.MODE_MASK.BND_MASK,
			Room.MODE_MASK.BUNGEE_MASK,
			Room.MODE_MASK.ESCAPE_MASK,
			Room.MODE_MASK.ZOMBIE_MASK
		};
		Texture2D[] array2 = new Texture2D[9]
		{
			GlobalVars.Instance.iconTeamMode,
			GlobalVars.Instance.iconsurvivalMode,
			GlobalVars.Instance.iconCTFMode,
			GlobalVars.Instance.iconBlastMode,
			GlobalVars.Instance.iconDefenseMode,
			GlobalVars.Instance.iconBndMode,
			GlobalVars.Instance.iconBungeeMode,
			GlobalVars.Instance.iconEscapeMode,
			GlobalVars.Instance.iconZombieMode
		};
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		int num = 0;
		LabelUtil.TextOut(crdSupportMode, StringMgr.Instance.Get("SUPPORT_MODE"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		for (int i = 0; i < array.Length; i++)
		{
			if ((modeMask & (byte)array[i]) != 0)
			{
				int num2 = num % xCount;
				int num3 = num / xCount;
				Rect position = new Rect(crdLTMode.x + (crdLTMode.width + 5f) * (float)num2, crdLTMode.y + (crdLTMode.height + 5f) * (float)num3, crdLTMode.width, crdLTMode.height);
				TextureUtil.DrawTexture(position, array2[i]);
				num++;
			}
		}
	}

	private void PrintMapInfo(RegMap reg, Vector2 pos)
	{
		Texture2D image = (!(reg.Thumbnail == null)) ? reg.Thumbnail : nonAvailable;
		TextureUtil.DrawTexture(new Rect(pos.x + 5f, pos.y + 10f, crdThumbnail.x, crdThumbnail.y), image, ScaleMode.StretchToFill);
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		LabelUtil.TextOut(new Vector2(pos.x + 115f, pos.y - 3f), StringMgr.Instance.Get("DEVELOPER"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 120f, pos.y + 12f), reg.Developer, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 115f, pos.y + 37f), StringMgr.Instance.Get("MAP_NAME_IS") + " " + StringMgr.Instance.Get("MAP_VERSION"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 120f, pos.y + 52f), reg.Alias + reg.Version, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 115f, pos.y + 77f), StringMgr.Instance.Get("LAST_MODIFIED_DATE"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 120f, pos.y + 92f), DateTimeLocal.ToString(reg.RegisteredDate), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 280f, pos.y - 3f), StringMgr.Instance.Get("PLAY_COUNT"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 286f, pos.y + 12f), reg.DisLikes.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 280f, pos.y + 37f), StringMgr.Instance.Get("DOWNLOAD_COUNT"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 286f, pos.y + 52f), reg.DownloadCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 280f, pos.y + 77f), StringMgr.Instance.Get("MAP_EVAL"), "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Rect position = new Rect(pos.x + 286f, pos.y + 95f, 74f, 14f);
		TextureUtil.DrawTexture(position, texStarGradeBg, ScaleMode.StretchToFill);
		Vector2 pos2 = new Vector2(position.x + 80f, position.y - 4f);
		string text = "[ " + reg.GetStarAvgString() + " ]";
		LabelUtil.TextOut(pos2, text, "MiniLabel", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		float num = (float)reg.Likes / 100f;
		TextureUtil.DrawTexture(new Rect(position.x, position.y, position.width * num, position.height), srcRect: new Rect(0f, 0f, num, 1f), image: texStarGrade);
	}

	public override bool DoDialog()
	{
		bool result = false;
		if (regMap == null)
		{
			return true;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("DOWNLOAD"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		PrintMapInfo(regMap, crdLTMapInfo);
		DrawMode(regMap.ModeMask);
		DoBuyHow();
		bool flag = DoPrice();
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		Rect rc2 = new Rect(size.x - 115f, size.y - 44f, 100f, 34f);
		GUIContent content = new GUIContent(StringMgr.Instance.Get("SAVE").ToUpper(), GlobalVars.Instance.iconDisk);
		if (GlobalVars.Instance.MyButton3(rc2, content, "BtnAction"))
		{
			if (flag)
			{
				result = true;
				CSNetManager.Instance.Sock.SendCS_DOWNLOAD_MAP_REQ(regMap.Map, (int)buyHow);
			}
			else
			{
				string arg = string.Empty;
				switch (buyHow)
				{
				case Good.BUY_HOW.GENERAL_POINT:
					arg = StringMgr.Instance.Get("GENERAL_POINT");
					break;
				case Good.BUY_HOW.BRICK_POINT:
					arg = StringMgr.Instance.Get("BRICK_POINT");
					break;
				case Good.BUY_HOW.CASH_POINT:
					arg = TokenManager.Instance.GetTokenString();
					break;
				}
				string msg = string.Format(StringMgr.Instance.Get("MORE_POINT_NEED_TO_SAVE_MAP"), arg);
				MessageBoxMgr.Instance.AddMessage(msg);
			}
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog(RegMap _regMap)
	{
		buyHow = Good.BUY_HOW.BRICK_POINT;
		regMap = _regMap;
	}
}
