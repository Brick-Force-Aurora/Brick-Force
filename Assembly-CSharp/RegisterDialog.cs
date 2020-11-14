using System;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class RegisterDialog : Dialog
{
	public RenderTexture thumbnail;

	public Texture2D pointIcon;

	public Rect crdBasicBox = new Rect(0f, 0f, 100f, 100f);

	public Rect crdThumbnailBox = new Rect(19f, 19f, 128f, 128f);

	public Rect crdThumbnail = new Rect(20f, 20f, 128f, 128f);

	private Rect crdGeneralBox = new Rect(200f, 212f, 393f, 20f);

	private Rect crdSpecialBox = new Rect(200f, 240f, 393f, 20f);

	private Rect crdDeleteBox = new Rect(200f, 268f, 393f, 20f);

	private Rect crdFeeBox = new Rect(200f, 296f, 393f, 20f);

	public Vector2 crdDeveloper = new Vector2(288f, 107f);

	public Vector2 crdDeveloperIs = new Vector2(288f, 107f);

	public Vector2 crdAlias = new Vector2(288f, 60f);

	public Vector2 crdAliasIs = new Vector2(288f, 60f);

	public Vector2 crdModeString = new Vector2(288f, 150f);

	public Vector2 crdModeStringIs = new Vector2(288f, 150f);

	private Vector2 crdGeneralCount = new Vector2(380f, 210f);

	private Vector2 crdGeneralFee = new Vector2(475f, 210f);

	private Vector2 crdGeneralBrickLabel = new Vector2(217f, 210f);

	private Vector2 crdGeneralBrickUnit = new Vector2(392f, 210f);

	private Vector2 crdGeneralPoint = new Vector2(487f, 210f);

	private Vector2 crdSpecialCount = new Vector2(380f, 237f);

	private Vector2 crdSpecialFee = new Vector2(475f, 237f);

	private Vector2 crdSpecialBrickLabel = new Vector2(217f, 237f);

	private Vector2 crdSpecialBrickUnit = new Vector2(392f, 237f);

	private Vector2 crdSpecialPoint = new Vector2(487f, 237f);

	private Vector2 crdDeleteCount = new Vector2(380f, 266f);

	private Vector2 crdDeleteFee = new Vector2(475f, 266f);

	private Vector2 crdDeleteBrickLabel = new Vector2(217f, 266f);

	private Vector2 crdDeleteBrickUnit = new Vector2(392f, 266f);

	private Vector2 crdDeletePoint = new Vector2(487f, 266f);

	private Vector2 crdTotalCount = new Vector2(380f, 294f);

	private Vector2 crdTotalFee = new Vector2(475f, 294f);

	private Vector2 crdTotalBrickLabel = new Vector2(217f, 294f);

	private Vector2 crdTotalUnit = new Vector2(392f, 294f);

	private Vector2 crdTotalPoint = new Vector2(487f, 294f);

	private Vector2 crdRegMsg = new Vector2(20f, 567f);

	private Vector2 crdRegErrMsg = new Vector2(20f, 587f);

	private Rect crdTeamCheckBox = new Rect(208f, 356f, 16f, 16f);

	public Rect crdPointAlias = new Rect(0f, 0f, 7f, 7f);

	public Rect crdPointDeveloper = new Rect(0f, 0f, 7f, 7f);

	public Rect crdPointMode = new Rect(0f, 0f, 7f, 7f);

	private Rect crdPointGeneral = new Rect(205f, 219f, 7f, 7f);

	private Rect crdPointSpecial = new Rect(205f, 246f, 7f, 7f);

	private Rect crdPointDelete = new Rect(205f, 274f, 7f, 7f);

	private Rect crdPointRegister = new Rect(205f, 302f, 7f, 7f);

	private Rect crdPointModeToggle = new Rect(205f, 330f, 7f, 7f);

	private Vector2 crdSelectMode = new Vector2(218f, 323f);

	private Rect crdModeSelectBox = new Rect(200f, 325f, 393f, 20f);

	private Rect crdRegHowBox = new Rect(18f, 212f, 173f, 20f);

	private Vector2 crdRegHowLabel = new Vector2(30f, 209f);

	private Rect crdRegPoint = new Rect(25f, 240f, 16f, 16f);

	private Rect crdRegBrick = new Rect(25f, 267f, 16f, 16f);

	private Rect crdRegCash = new Rect(25f, 294f, 16f, 16f);

	private Rect crdDownloadFeeBox = new Rect(18f, 325f, 173f, 20f);

	private Vector2 crdDownloadFeeLabel = new Vector2(30f, 321f);

	private Rect crdDownloadFee = new Rect(23f, 357f, 128f, 26f);

	private Vector2 crdByBrickPoint = new Vector2(30f, 382f);

	private Rect crdMapIntroTitle = new Rect(18f, 434f, 575f, 20f);

	private Rect crdMapIntroTitlePt = new Rect(22f, 443f, 7f, 7f);

	private Rect crdMapIntroArea = new Rect(23f, 462f, 564f, 80f);

	private UserMapInfo umi;

	private ushort modeMask;

	private bool teamMatch;

	private bool individualMatch;

	private bool mission;

	private bool explosionMatch;

	private bool flagMatch;

	private bool escape;

	private bool zombie;

	private string downloadFee = "0";

	private string mapIntroduce = string.Empty;

	private int maxIntroduceLength = 250;

	private Good.BUY_HOW regHow = Good.BUY_HOW.BRICK_POINT;

	private bool outputErr;

	private int errFlag = -1;

	private float ElapsedErr;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.REGISTER;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		if (!BuildOption.Instance.Props.useBrickPoint)
		{
			regHow = Good.BUY_HOW.GENERAL_POINT;
		}
	}

	private bool DoReceipt(ref int diff, ref int totalFee, ref string minMaxMessage)
	{
		int specialCount = 0;
		int generalCount = 0;
		int deleteCount = 0;
		UserMapInfoManager.Instance.CalcCount(ref generalCount, ref specialCount, ref deleteCount);
		float generalFee = 0f;
		float specialFee = 0f;
		float deleteFee = 0f;
		int num = specialCount + generalCount + deleteCount;
		diff = 0;
		totalFee = 0;
		minMaxMessage = string.Empty;
		string pointString = string.Empty;
		bool flag = UserMapInfoManager.Instance.CalcFee(generalCount, specialCount, deleteCount, regHow, ref totalFee, ref generalFee, ref specialFee, ref deleteFee, ref pointString, ref minMaxMessage, ref diff);
		GUI.Box(crdGeneralBox, string.Empty, "BoxFadeBlue");
		GUI.Box(crdSpecialBox, string.Empty, "BoxFadeBlue");
		GUI.Box(crdFeeBox, string.Empty, "BoxFadeBlue");
		GUI.Box(crdDeleteBox, string.Empty, "BoxFadeBlue");
		GUI.Box(crdModeSelectBox, string.Empty, "BoxFadeBlue");
		TextureUtil.DrawTexture(crdPointGeneral, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdGeneralBrickLabel, StringMgr.Instance.Get("GENERAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdGeneralCount, generalCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdGeneralBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdGeneralFee, Mathf.FloorToInt(generalFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdGeneralPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdPointSpecial, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdSpecialBrickLabel, StringMgr.Instance.Get("SPECIAL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdSpecialCount, specialCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdSpecialBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdSpecialFee, Mathf.FloorToInt(specialFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdSpecialPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdPointDelete, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdDeleteBrickLabel, StringMgr.Instance.Get("DELETE_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDeleteCount, deleteCount.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdDeleteBrickUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDeleteFee, Mathf.FloorToInt(deleteFee).ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdDeletePoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdPointRegister, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdTotalBrickLabel, StringMgr.Instance.Get("ALL_BRICK"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdTotalCount, num.ToString(), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		LabelUtil.TextOut(crdTotalUnit, StringMgr.Instance.Get("UNIT"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdTotalPoint, pointString, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Color clrText = new Color(0.69f, 0.83f, 0.29f);
		if (!flag)
		{
			clrText = new Color(0.83f, 0.49f, 0.29f);
		}
		else if (minMaxMessage.Length > 0)
		{
			clrText = Color.yellow;
		}
		LabelUtil.TextOut(crdTotalFee, totalFee.ToString(), "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		return flag;
	}

	private void DoRegHow()
	{
		GUI.Box(crdRegHowBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdRegHowLabel, StringMgr.Instance.Get("BUY_HOW"), "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		switch (regHow)
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
		bool flag4 = GUI.Toggle(crdRegPoint, flag, StringMgr.Instance.Get("GENERAL_POINT"));
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			bool flag5 = GUI.Toggle(crdRegBrick, flag2, StringMgr.Instance.Get("BRICK_POINT"));
			bool flag6 = GUI.Toggle(crdRegCash, flag3, TokenManager.Instance.GetTokenString());
			if (!flag && flag4)
			{
				regHow = Good.BUY_HOW.GENERAL_POINT;
			}
			if (!flag2 && flag5)
			{
				regHow = Good.BUY_HOW.BRICK_POINT;
			}
			if (!flag3 && flag6)
			{
				regHow = Good.BUY_HOW.CASH_POINT;
			}
		}
		else if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
			bool flag7 = GUI.Toggle(crdRegBrick, flag3, TokenManager.Instance.GetTokenString());
			if (!flag && flag4)
			{
				regHow = Good.BUY_HOW.GENERAL_POINT;
			}
			if (!flag3 && flag7)
			{
				regHow = Good.BUY_HOW.CASH_POINT;
			}
		}
	}

	private void DoDownloadFee()
	{
		GUI.Box(crdDownloadFeeBox, string.Empty, "BoxFadeBlue");
		LabelUtil.TextOut(crdDownloadFeeLabel, StringMgr.Instance.Get("DOWNLOAD_FEE"), "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		downloadFee = GUI.TextField(crdDownloadFee, downloadFee, 9);
		string pattern = "[^0-9]";
		Regex regex = new Regex(pattern);
		downloadFee = regex.Replace(downloadFee, string.Empty);
		if (downloadFee.Length > 1)
		{
			downloadFee = downloadFee.TrimStart('0');
		}
		int num = (!BuildOption.Instance.Props.useBrickPoint) ? 1000 : 100;
		int num2 = 0;
		try
		{
			num2 = int.Parse(downloadFee);
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			downloadFee = num2.ToString();
		}
		catch
		{
		}
		if (BuildOption.Instance.Props.useBrickPoint)
		{
			LabelUtil.TextOut(crdByBrickPoint, StringMgr.Instance.Get("BRICK_POINT") + " (0-100)", "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			LabelUtil.TextOut(crdByBrickPoint, StringMgr.Instance.Get("GENERAL_POINT") + " (0-1000)", "Label", new Color(0.76f, 0.54f, 0.27f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		if (umi == null)
		{
			return true;
		}
		CheckModeMask();
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("REGISTER"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdBasicBox, string.Empty, "BoxFadeBlue");
		GUI.Box(crdThumbnailBox, string.Empty, "BoxBrickOutline");
		TextureUtil.DrawTexture(crdThumbnail, thumbnail);
		TextureUtil.DrawTexture(crdPointDeveloper, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdDeveloperIs, StringMgr.Instance.Get("DEVELOPER_IS"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdDeveloper, MyInfoManager.Instance.Nickname, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdPointAlias, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdAliasIs, StringMgr.Instance.Get("MAP_NAME_IS"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdAlias, umi.Alias, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdPointMode, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdModeStringIs, StringMgr.Instance.Get("POSSIBLE_MODE"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (modeMask == 0)
		{
			LabelUtil.TextOut(crdModeString, StringMgr.Instance.Get("NO_MODE_POSSIBLE"), "Label", new Color(0.83f, 0.49f, 0.29f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			string text = Room.ModeMask2String(modeMask);
			LabelUtil.TextOut(crdModeString, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		DoRegHow();
		int diff = 0;
		int totalFee = 0;
		string minMaxMessage = string.Empty;
		bool flag = DoReceipt(ref diff, ref totalFee, ref minMaxMessage);
		TextureUtil.DrawTexture(crdPointModeToggle, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdSelectMode, StringMgr.Instance.Get("SELECT_MODE"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Rect position = new Rect(crdTeamCheckBox);
		float x = position.x;
		float num = 147f;
		float num2 = 24f;
		int num3 = 0;
		if (BuildOption.Instance.Props.teamMatchMode)
		{
			teamMatch = GUI.Toggle(position, teamMatch, StringMgr.Instance.Get("ROOM_TYPE_TEAM_MATCH"));
			if (teamMatch && (modeMask & 1) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 1;
				teamMatch = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		if (BuildOption.Instance.Props.individualMatchMode)
		{
			individualMatch = GUI.Toggle(position, individualMatch, StringMgr.Instance.Get("ROOM_TYPE_INDIVIDUAL_MATCH"));
			if (individualMatch && (modeMask & 2) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 2;
				individualMatch = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		bool flag2 = (ChannelManager.Instance.CurChannel.Mode == 3) ? (flag2 = true) : (flag2 = false);
		if (BuildOption.Instance.Props.defenseMatchMode && flag2)
		{
			mission = GUI.Toggle(position, mission, StringMgr.Instance.Get("ROOM_TYPE_MISSION"));
			if (mission && (modeMask & 0x10) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 16;
				mission = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		if (BuildOption.Instance.Props.ctfMatchMode)
		{
			flagMatch = GUI.Toggle(position, flagMatch, StringMgr.Instance.Get("ROOM_TYPE_CAPTURE_THE_FLAG"));
			if (flagMatch && (modeMask & 4) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 4;
				flagMatch = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		if (BuildOption.Instance.Props.explosionMatchMode)
		{
			explosionMatch = GUI.Toggle(position, explosionMatch, StringMgr.Instance.Get("ROOM_TYPE_EXPLOSION"));
			if (explosionMatch && (modeMask & 8) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 8;
				explosionMatch = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		if (BuildOption.Instance.Props.escapeMode)
		{
			escape = GUI.Toggle(position, escape, StringMgr.Instance.Get("ROOM_TYPE_ESCAPE"));
			if (escape && (modeMask & 0x80) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 128;
				escape = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		if (BuildOption.Instance.Props.zombieMode)
		{
			zombie = GUI.Toggle(position, zombie, StringMgr.Instance.Get("ROOM_TYPE_ZOMBIE"));
			if (zombie && (modeMask & 0x100) == 0)
			{
				outputErr = true;
				ElapsedErr = 0f;
				errFlag = 256;
				zombie = false;
			}
			num3++;
			position.x += num;
			if (num3 % 3 == 0)
			{
				position.x = x;
				position.y += num2;
			}
		}
		bool enabled = true;
		string text2 = StringMgr.Instance.Get("REG_MAP_POSSIBLE");
		Color clrText = new Color(0.69f, 0.83f, 0.29f);
		if (!flag)
		{
			enabled = false;
			text2 = string.Format(StringMgr.Instance.Get("MORE_POINT_NEED"), diff);
			clrText = new Color(0.83f, 0.49f, 0.29f);
		}
		else if (!teamMatch && !individualMatch && !flagMatch && !explosionMatch && !mission && !escape && !zombie)
		{
			if (!MyInfoManager.Instance.IsGM)
			{
				enabled = false;
			}
			text2 = StringMgr.Instance.Get("GAME_MODE_NEED");
			clrText = new Color(0.83f, 0.49f, 0.29f);
		}
		if (minMaxMessage.Length > 0)
		{
			text2 = text2 + " " + minMaxMessage;
		}
		LabelUtil.TextOut(crdRegMsg, text2, "MissionLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (outputErr)
		{
			switch (errFlag)
			{
			case 1:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_TEAM");
				break;
			case 2:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_INDIVIDUAL");
				break;
			case 8:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_EXPLOSION");
				break;
			case 16:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_MISSION");
				break;
			case 4:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_FLAG");
				break;
			case 128:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_ESCAPE");
				break;
			case 256:
				text2 = StringMgr.Instance.Get("ERROR_REGISTER_ZOMBIE");
				break;
			}
			LabelUtil.TextOut(crdRegErrMsg, text2, "MissionLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 440f);
		}
		DoDownloadFee();
		GUI.Box(crdMapIntroTitle, string.Empty, "BoxFadeBlue");
		TextureUtil.DrawTexture(crdMapIntroTitlePt, pointIcon, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdMapIntroTitle.x + 14f, crdMapIntroTitle.y + 2f), StringMgr.Instance.Get("MAP_INTRO"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.SetNextControlName("MapIntroduceInput2");
		mapIntroduce = GUI.TextArea(crdMapIntroArea, mapIntroduce, maxIntroduceLength);
		Vector2 pos2 = new Vector2(crdMapIntroArea.x, crdMapIntroArea.y + crdMapIntroArea.height + 5f);
		LabelUtil.TextOut(pos2, StringMgr.Instance.Get("CAN_250_BYTE"), "MiniLabel", Color.red, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		Vector2 pos3 = new Vector2(crdMapIntroArea.x + crdMapIntroArea.width, crdMapIntroArea.y + crdMapIntroArea.height + 5f);
		string text3 = mapIntroduce.Length.ToString() + "/" + maxIntroduceLength.ToString() + StringMgr.Instance.Get("BYTE");
		LabelUtil.TextOut(pos3, text3, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		Rect rc2 = new Rect(size.x - 115f, size.y - 44f, 100f, 34f);
		GUI.enabled = enabled;
		if (GlobalVars.Instance.MyButton(rc2, StringMgr.Instance.Get("REGISTER"), "BtnAction"))
		{
			int num4 = 0;
			try
			{
				num4 = int.Parse(downloadFee);
			}
			catch
			{
			}
			ushort num5 = 0;
			if (teamMatch)
			{
				num5 = (ushort)(num5 | 1);
			}
			if (individualMatch)
			{
				num5 = (ushort)(num5 | 2);
			}
			if (BuildOption.Instance.Props.defenseMatchMode && flag2 && mission)
			{
				num5 = (ushort)(num5 | 0x10);
			}
			if (flagMatch)
			{
				num5 = (ushort)(num5 | 4);
			}
			if (explosionMatch)
			{
				num5 = (ushort)(num5 | 8);
			}
			if (escape)
			{
				num5 = (ushort)(num5 | 0x80);
			}
			if (zombie)
			{
				num5 = (ushort)(num5 | 0x100);
			}
			byte[] array = ThumbnailToPNG();
			if (array == null)
			{
				Debug.LogError("Error, Fail to get thumbnail");
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_REGISTER_REQ(umi.Slot, num5, (int)regHow, totalFee, num4, array, mapIntroduce);
			}
			result = true;
		}
		GUI.enabled = true;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog(UserMapInfo _umi)
	{
		umi = _umi;
		if (umi != null)
		{
			CheckModeMask();
			if (BuildOption.Instance.Props.teamMatchMode)
			{
				teamMatch = ((modeMask & 1) != 0);
			}
			if (BuildOption.Instance.Props.individualMatchMode)
			{
				individualMatch = ((modeMask & 2) != 0);
			}
			if (BuildOption.Instance.Props.defenseMatchMode)
			{
				mission = ((modeMask & 0x10) != 0);
			}
			if (BuildOption.Instance.Props.ctfMatchMode)
			{
				flagMatch = ((modeMask & 4) != 0);
			}
			if (BuildOption.Instance.Props.explosionMatchMode)
			{
				explosionMatch = ((modeMask & 8) != 0);
			}
			if (BuildOption.Instance.Props.escapeMode)
			{
				escape = ((modeMask & 0x80) != 0);
			}
			if (BuildOption.Instance.Props.zombieMode)
			{
				zombie = ((modeMask & 0x100) != 0);
			}
		}
	}

	private void CheckModeMask()
	{
		modeMask = BrickManager.Instance.GetPossibleModeMask();
	}

	private byte[] ThumbnailToPNG()
	{
		int width = thumbnail.width;
		int height = thumbnail.height;
		RenderTexture.active = thumbnail;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		return texture2D.EncodeToPNG();
	}

	public override void Update()
	{
		if (outputErr)
		{
			ElapsedErr += Time.deltaTime;
			if (ElapsedErr > 3f)
			{
				outputErr = false;
			}
		}
	}
}
