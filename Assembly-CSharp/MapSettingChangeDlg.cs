using System;
using UnityEngine;

[Serializable]
public class MapSettingChangeDlg : Dialog
{
	private RegMap reg;

	private Rect crdOutline = new Rect(18f, 77f, 578f, 530f);

	private Vector2 crdDeveloperVal = new Vector2(550f, 48f);

	private Rect crdContury = new Rect(560f, 47f, 37f, 24f);

	private Rect crdThumbnail = new Rect(33f, 93f, 128f, 128f);

	private Rect crdThumbUp = new Rect(175f, 100f, 22f, 22f);

	private Rect crdThumbDn = new Rect(275f, 100f, 22f, 22f);

	private Rect crdSave = new Rect(375f, 100f, 22f, 22f);

	private Vector2 crdMapAliasLabel = new Vector2(175f, 130f);

	private Vector2 crdLastModifiedLabel = new Vector2(175f, 150f);

	private Vector2 crdSupportModeLabel = new Vector2(175f, 170f);

	private Rect crdModeIcon = new Rect(175f, 200f, 35f, 22f);

	private Vector2 crdTitle1 = new Vector2(43f, 248f);

	private Rect crdOutline1 = new Rect(33f, 268f, 548f, 82f);

	private Vector2 crdTitle2 = new Vector2(43f, 370f);

	private Rect crdOutline2 = new Rect(33f, 390f, 548f, 194f);

	private Rect crdIntroArea = new Rect(33f, 268f, 548f, 82f);

	private Vector2 crdPrice = new Vector2(33f, 640f);

	private Rect crdScreen = new Rect(40f, 400f, 525f, 159f);

	private string mapIntroduce = string.Empty;

	private int maxIntroduceLength = 250;

	private Good.BUY_HOW buyHow;

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 scrollPositionTA = Vector2.zero;

	private float rectHeight;

	private int xCount = 30;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MAP_SETTING_CHANGE;
	}

	public override void OnPopup()
	{
		size.x = 614f;
		size.y = 674f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(RegMap regmap)
	{
		reg = regmap;
		GlobalVars.Instance.ClearComments();
		CSNetManager.Instance.Sock.SendCS_MAP_DETAIL_REQ(reg.Map);
	}

	private void DrawMode(int modeMask)
	{
		int num = 0;
		Rect position = new Rect(crdModeIcon);
		if ((modeMask & 1) != 0)
		{
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconTeamMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 2) != 0)
		{
			int num2 = num % xCount;
			int num3 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num2;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num3;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconsurvivalMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 4) != 0)
		{
			int num4 = num % xCount;
			int num5 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num4;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num5;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconCTFMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 8) != 0)
		{
			int num6 = num % xCount;
			int num7 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num6;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num7;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBlastMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x10) != 0)
		{
			int num8 = num % xCount;
			int num9 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num8;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num9;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconDefenseMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x20) != 0)
		{
			int num10 = num % xCount;
			int num11 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num10;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num11;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBndMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x40) != 0)
		{
			int num12 = num % xCount;
			int num13 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num12;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num13;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconBungeeMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x80) != 0)
		{
			int num14 = num % xCount;
			int num15 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num14;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num15;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconEscapeMode, ScaleMode.StretchToFill);
			num++;
		}
		if ((modeMask & 0x100) != 0)
		{
			int num16 = num % xCount;
			int num17 = num / xCount;
			position.x = crdModeIcon.x + (crdModeIcon.width + 5f) * (float)num16;
			position.y = crdModeIcon.y + (crdModeIcon.height + 5f) * (float)num17;
			TextureUtil.DrawTexture(position, GlobalVars.Instance.iconZombieMode, ScaleMode.StretchToFill);
			num++;
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		if (GlobalVars.Instance.IsIntroChange)
		{
			mapIntroduce = GlobalVars.Instance.intro;
			GlobalVars.Instance.IsIntroChange = false;
		}
		if (GlobalVars.Instance.IsIntroChangeTemp)
		{
			mapIntroduce = GlobalVars.Instance.introTemp;
			GlobalVars.Instance.IsIntroChangeTemp = false;
		}
		if (GlobalVars.Instance.IsPriceChangeTemp)
		{
			reg.DownloadFee = GlobalVars.Instance.downloadPriceTemp;
			GlobalVars.Instance.IsPriceChangeTemp = false;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, reg.Alias, "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdDeveloperVal, StringMgr.Instance.Get("DEVELOPER") + " : " + reg.Developer, "MidLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperRight);
		TextureUtil.DrawTexture(crdContury, BuildOption.Instance.defaultCountryFilter, ScaleMode.StretchToFill);
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		TextureUtil.DrawTexture(crdThumbnail, reg.Thumbnail, ScaleMode.StretchToFill);
		DateTime registeredDate = reg.RegisteredDate;
		if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
		{
			TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
		}
		else if ((reg.tagMask & 8) != 0)
		{
			TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
		}
		else if ((reg.tagMask & 4) != 0)
		{
			TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
		}
		else if ((reg.tagMask & 2) != 0)
		{
			TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
		}
		if (reg.IsAbuseMap())
		{
			float x = crdThumbnail.x + crdThumbnail.width - (float)GlobalVars.Instance.iconDeclare.width;
			TextureUtil.DrawTexture(new Rect(x, crdThumbnail.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
		}
		TextureUtil.DrawTexture(crdThumbUp, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdThumbUp.x + 30f, crdThumbUp.y), reg.Likes.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdThumbDn, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdThumbDn.x + 30f, crdThumbDn.y), reg.DisLikes.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(crdSave, GlobalVars.Instance.iconSave, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(crdSave.x + 30f, crdSave.y), reg.DownloadCount.ToString(), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdMapAliasLabel, StringMgr.Instance.Get("VERSIONINFO") + string.Empty + StringMgr.Instance.Get("MAP_VERSION") + " : " + reg.Version, "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdLastModifiedLabel, StringMgr.Instance.Get("LAST_MODIFIED_DATE") + " : " + DateTimeLocal.ToString(reg.RegisteredDate), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		LabelUtil.TextOut(crdSupportModeLabel, StringMgr.Instance.Get("SUPPORT_MODE"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		DrawMode(reg.ModeMask);
		LabelUtil.TextOut(crdTitle1, StringMgr.Instance.Get("MAP_INTRO"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		GUI.Box(crdOutline1, string.Empty, "BoxInnerLine");
		GUI.SetNextControlName("MapIntroduceInput");
		GUILayout.BeginArea(crdIntroArea);
		scrollPositionTA = GUILayout.BeginScrollView(scrollPositionTA, false, false, GUILayout.Width(crdIntroArea.width), GUILayout.Height(crdIntroArea.height));
		GUILayout.TextArea(mapIntroduce, maxIntroduceLength);
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		LabelUtil.TextOut(crdTitle2, StringMgr.Instance.Get("A_LINE_EVAL"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		GUI.Box(crdOutline2, string.Empty, "BoxInnerLine");
		float num = 0f;
		Rect viewRect = new Rect(crdScreen);
		viewRect.width -= 20f;
		viewRect.height = rectHeight;
		scrollPosition = GUI.BeginScrollView(crdScreen, scrollPosition, viewRect);
		for (int i = 0; i < GlobalVars.Instance.snipets.Count; i++)
		{
			Rect position = new Rect(viewRect.x, viewRect.y + num, 18f, 18f);
			if (GlobalVars.Instance.snipets[i].likeOrDislike == 1)
			{
				TextureUtil.DrawTexture(position, GlobalVars.Instance.iconThumbUp, ScaleMode.StretchToFill);
			}
			else
			{
				TextureUtil.DrawTexture(position, GlobalVars.Instance.iconThumbDn, ScaleMode.StretchToFill);
			}
			Vector2 pos2 = new Vector2(viewRect.x + 30f, viewRect.y + num);
			LabelUtil.TextOut(pos2, "[" + GlobalVars.Instance.snipets[i].nickNameCmt + "]", "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
			GUIStyle style = GUI.skin.GetStyle("MiniLabel");
			float num2 = style.CalcHeight(new GUIContent(GlobalVars.Instance.snipets[i].cmt), viewRect.width - 130f);
			GUI.Label(new Rect(viewRect.x + 130f, viewRect.y + num, viewRect.width - 130f, num2), GlobalVars.Instance.snipets[i].cmt, "MiniLabel");
			num = ((!(num2 <= 20f)) ? (num + (num2 + 4f)) : (num + 20f));
		}
		rectHeight = num;
		GUI.EndScrollView();
		if (GlobalVars.Instance.totalComments > 5 && GlobalVars.Instance.snipets.Count < GlobalVars.Instance.totalComments)
		{
			Vector2 pos3 = new Vector2((crdOutline2.x + crdOutline2.width) / 2f, crdOutline2.y + crdOutline2.height - 25f);
			LabelUtil.TextOut(pos3, StringMgr.Instance.Get("MORE"), "MiniLabel", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
			Vector2 vector = LabelUtil.CalcLength("MiniLabel", StringMgr.Instance.Get("MORE"));
			if (GlobalVars.Instance.MyButton(new Rect(pos3.x + vector.x, pos3.y, 22f, 22f), string.Empty, "BtnArrowDn"))
			{
				CSNetManager.Instance.Sock.SendCS_MORE_COMMENT_REQ(reg.Map, GlobalVars.Instance.snipets[GlobalVars.Instance.snipets.Count - 1].cmtSeq);
			}
		}
		string text = StringMgr.Instance.Get("DOWNLOAD_FEE") + " : " + reg.DownloadFee.ToString();
		LabelUtil.TextOut(crdPrice, text, "MidLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperLeft);
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		Rect rc2 = new Rect(size.x - 374f, size.y - 44f, 176f, 34f);
		GUIContent content = new GUIContent(StringMgr.Instance.Get("MAP_INTRO_CHG").ToUpper(), GlobalVars.Instance.iconDisk);
		if (GlobalVars.Instance.MyButton3(rc2, content, "BtnAction"))
		{
			((MapIntroDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAPINTRO, exclusive: false))?.InitDialog(reg.Map);
		}
		Rect rc3 = new Rect(size.x - 187f, size.y - 44f, 176f, 34f);
		content = new GUIContent(StringMgr.Instance.Get("MAP_PRICE_CHANGE").ToUpper(), GlobalVars.Instance.iconDisk);
		if (GlobalVars.Instance.MyButton3(rc3, content, "BtnAction"))
		{
			((MapPriceChangeDlg)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.MAP_PRICE_CHANGE, exclusive: false))?.InitDialog(reg.Map, reg.DownloadFee);
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}
}
