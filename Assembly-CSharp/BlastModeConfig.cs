using System;
using UnityEngine;

[Serializable]
public class BlastModeConfig
{
	public Texture2D nonavailable;

	private Rect crdThumbnail = new Rect(15f, 496f, 90f, 90f);

	private Vector2 crdAlias = new Vector2(115f, 546f);

	private Vector2 crdMode = new Vector2(115f, 577f);

	private Rect crdConfigBtn = new Rect(218f, 489f, 19f, 19f);

	public Vector2 crdOptionLT = new Vector2(120f, 612f);

	private Rect crdLine = new Rect(10f, 596f, 234f, 2f);

	private float optionLX = 72f;

	private float optionRX = 190f;

	private float diff_y = 22f;

	private Vector2 crdBox = new Vector2(100f, 18f);

	private float diff_y2 = 15f;

	private string tooltipMessage = string.Empty;

	private string[] weaponOptions = new string[3]
	{
		"USE_ALL_WEAPON",
		"USE_AUX_WEAPON",
		"USE_MELEE_WEAPON"
	};

	public bool isRoom = true;

	public void Start()
	{
	}

	public void OnGUI()
	{
		Texture2D thumbnail = nonavailable;
		RegMap regMap = RegMapManager.Instance.Get(RoomManager.Instance.CurMap);
		if (regMap != null && regMap.Thumbnail != thumbnail)
		{
			thumbnail = regMap.Thumbnail;
		}
		if (null == thumbnail)
		{
			thumbnail = nonavailable;
		}
		GUI.Box(crdLine, string.Empty, "DivideLine");
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null && regMap != null)
		{
			TextureUtil.DrawTexture(crdThumbnail, thumbnail);
			DateTime registeredDate = regMap.RegisteredDate;
			if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconNewmap.width * 0.7f, (float)GlobalVars.Instance.iconNewmap.height * 0.7f), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
			}
			else if ((regMap.tagMask & 8) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconglory.width * 0.7f, (float)GlobalVars.Instance.iconglory.height * 0.7f), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
			}
			else if ((regMap.tagMask & 4) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.iconMedal.width * 0.7f, (float)GlobalVars.Instance.iconMedal.height * 0.7f), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
			}
			else if ((regMap.tagMask & 2) != 0)
			{
				TextureUtil.DrawTexture(new Rect(crdThumbnail.x, crdThumbnail.y, (float)GlobalVars.Instance.icongoldRibbon.width * 0.7f, (float)GlobalVars.Instance.icongoldRibbon.height * 0.7f), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
			}
			if (regMap.IsAbuseMap())
			{
				float x = crdThumbnail.x + crdThumbnail.width - (float)GlobalVars.Instance.iconDeclare.width * 0.7f;
				TextureUtil.DrawTexture(new Rect(x, crdThumbnail.y, (float)GlobalVars.Instance.iconDeclare.width * 0.7f, (float)GlobalVars.Instance.iconDeclare.height * 0.7f), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
			}
			LabelUtil.TextOut(crdAlias, RoomManager.Instance.CurAlias, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			LabelUtil.TextOut(crdMode, room.GetString(Room.COLUMN.TYPE), "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
			DoOption(room);
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq && GlobalVars.Instance.MyButton(crdConfigBtn, new GUIContent(string.Empty, StringMgr.Instance.Get("CHANGE_ROOM_CONFIG")), "ConfigButton"))
			{
				((RoomConfigDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ROOM_CONFIG, exclusive: true))?.InitDialog(room);
			}
		}
		if (Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
		{
			tooltipMessage = GUI.tooltip;
			Vector2 vector = GlobalVars.Instance.ToGUIPoint(Event.current.mousePosition);
			GUIStyle style = GUI.skin.GetStyle("MiniLabel");
			if (style != null)
			{
				Vector2 vector2 = style.CalcSize(new GUIContent(tooltipMessage));
				Rect rc = new Rect(vector.x, vector.y, vector2.x + 20f, vector2.y + 20f);
				GlobalVars.Instance.FitRightNBottomRectInScreen(ref rc);
				GUI.Window(1102, rc, ShowTooltip, string.Empty, "LineWindow");
			}
		}
	}

	public void DoOption(Room room)
	{
		float y = crdOptionLT.y;
		LabelUtil.TextOut(new Vector2(optionLX, y), StringMgr.Instance.Get("ROUND"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
		LabelUtil.TextOut(new Vector2(optionRX, y), room.goal.ToString(), "MiniLabel", new Color(0.91f, 0.6f, 0f, 1f), GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		y += diff_y;
		if (isRoom)
		{
			LabelUtil.TextOut(new Vector2(optionLX, y), StringMgr.Instance.Get("WEAPON_OPTION"), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			GUI.Box(new Rect(optionRX - crdBox.x * 0.5f, y - crdBox.y * 0.5f, crdBox.x, crdBox.y), string.Empty, "BoxTextBg");
			string text = (room.weaponOption >= 0 && room.weaponOption < weaponOptions.Length) ? StringMgr.Instance.Get(weaponOptions[room.weaponOption]) : string.Empty;
			LabelUtil.TextOut(new Vector2(optionRX, y), text, "MiniLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			y += diff_y;
		}
		Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(25, 179, 0);
		LabelUtil.TextOut(new Vector2(crdOptionLT.x, y), StringMgr.Instance.Get("BREAK_INTO") + " : " + ((!room.isBreakInto) ? "off" : "on"), "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		y += diff_y2;
		if (BuildOption.Instance.Props.UseItemDrop)
		{
			LabelUtil.TextOut(new Vector2(crdOptionLT.x, y), StringMgr.Instance.Get("ROOM_SET_ITEMDROP") + " : " + ((!room.isDropItem) ? "off" : "on"), "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			y += diff_y2;
		}
		if (isRoom)
		{
			LabelUtil.TextOut(new Vector2(crdOptionLT.x, y), StringMgr.Instance.Get("TEAM_BALANCE") + " : " + ((!RoomManager.Instance.TeamBalance) ? "off" : "on"), "MiniLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			y += diff_y2;
		}
	}

	private void ShowTooltip(int id)
	{
		LabelUtil.TextOut(new Vector2(10f, 10f), tooltipMessage, "MiniLabel", Color.yellow, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}
}
