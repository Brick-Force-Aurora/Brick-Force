using System;
using UnityEngine;

[Serializable]
public class HelpWindow : Dialog
{
	public Texture2D help_font;

	public Texture2D helpBar_bg;

	public Texture2D brickKey;

	public Texture2D chatKey;

	public Texture2D etcKey;

	public Texture2D keyboard;

	public Texture2D mouse;

	public Texture2D moveKey;

	public Texture2D weaponKey;

	private Rect crdKeyboard = new Rect(10f, 50f, 434f, 140f);

	private Rect crdMouse = new Rect(470f, 50f, 65f, 85f);

	public Vector2 crdHelp1 = new Vector2(460f, 140f);

	public Vector2 crdHelp2 = new Vector2(497f, 151f);

	public Vector2 crdHelp3 = new Vector2(460f, 167f);

	public Vector2 crdHelp4 = new Vector2(497f, 178f);

	private bool bOpenWindow;

	private Vector2 scrollPosition = Vector2.zero;

	private Rect RealRect = new Rect(0f, 50f, 641f, 470f);

	private Rect viewRect = new Rect(0f, 50f, 620f, 650f);

	private bool focus;

	public bool IsOpenWindow => bOpenWindow;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.HELPWINDOW;
		size.x = 642f;
		size.y = 527f;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		if (!focus)
		{
			GUI.FocusWindow(61);
			focus = true;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 vector = new Vector2(0f, 0f);
		GUI.BeginGroup(new Rect(vector.x, vector.y, 641f, 527f));
		GUI.Box(new Rect(0f, 0f, 641f, 527f), string.Empty, "BoxBrickText");
		TextureUtil.DrawTexture(new Rect(0f, 15f, 641f, 15f), helpBar_bg, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(new Rect((float)((641 - help_font.width) / 2), 8f, (float)help_font.width, (float)help_font.height), help_font, ScaleMode.StretchToFill);
		scrollPosition = GUI.BeginScrollView(RealRect, scrollPosition, viewRect);
		TextureUtil.DrawTexture(crdKeyboard, keyboard, ScaleMode.StretchToFill);
		TextureUtil.DrawTexture(crdMouse, mouse, ScaleMode.StretchToFill);
		LabelUtil.TextOut(crdHelp1, StringMgr.Instance.Get("HELP_1"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdHelp2, StringMgr.Instance.Get("HELP_2"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdHelp3, StringMgr.Instance.Get("HELP_3"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(crdHelp4, StringMgr.Instance.Get("HELP_4"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		Vector2 vector2 = new Vector2(10f, 200f);
		Vector2 pos = new Vector2(vector2.x + 30f, vector2.y + 20f);
		GUI.Box(new Rect(10f, pos.y - 20f, 603f, 95f), string.Empty, "BoxBrickText");
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("HELP_TITLE_1"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		TextureUtil.DrawTexture(new Rect(pos.x + 160f, pos.y - 10f, (float)moveKey.width, (float)moveKey.height), moveKey, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y - 6f), StringMgr.Instance.Get("HELP_5"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 7f), StringMgr.Instance.Get("HELP_6"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 20f), StringMgr.Instance.Get("HELP_7"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 33f), StringMgr.Instance.Get("HELP_8"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 46f), StringMgr.Instance.Get("HELP_9"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 440f, pos.y - 6f), StringMgr.Instance.Get("HELP_29"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		pos.y += 100f;
		GUI.Box(new Rect(10f, pos.y - 20f, 603f, 95f), string.Empty, "BoxBrickText");
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("HELP_TITLE_2"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		TextureUtil.DrawTexture(new Rect(pos.x + 160f, pos.y, (float)weaponKey.width, (float)weaponKey.height), weaponKey, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y - 6f), StringMgr.Instance.Get("HELP_10"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 7f), StringMgr.Instance.Get("HELP_11"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 20f), StringMgr.Instance.Get("HELP_12"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 33f), StringMgr.Instance.Get("HELP_13"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 440f, pos.y - 6f), StringMgr.Instance.Get("HELP_14"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 440f, pos.y + 7f), StringMgr.Instance.Get("HELP_15"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 440f, pos.y + 20f), StringMgr.Instance.Get("HELP_16"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		pos.y += 100f;
		GUI.Box(new Rect(10f, pos.y - 20f, 603f, 95f), string.Empty, "BoxBrickText");
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("HELP_TITLE_3"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		TextureUtil.DrawTexture(new Rect(pos.x + 160f, pos.y - 10f, (float)brickKey.width, (float)brickKey.height), brickKey, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y - 6f), StringMgr.Instance.Get("HELP_17"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 7f), StringMgr.Instance.Get("HELP_18"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 20f), StringMgr.Instance.Get("HELP_19"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 33f), StringMgr.Instance.Get("HELP_30"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		pos.y += 100f;
		GUI.Box(new Rect(10f, pos.y - 20f, 603f, 95f), string.Empty, "BoxBrickText");
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("HELP_TITLE_4"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		TextureUtil.DrawTexture(new Rect(pos.x + 160f, pos.y - 10f, (float)chatKey.width, (float)chatKey.height), chatKey, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y - 6f), StringMgr.Instance.Get("HELP_20"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 7f), StringMgr.Instance.Get("HELP_21"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 20f), StringMgr.Instance.Get("HELP_22"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		pos.y += 100f;
		GUI.Box(new Rect(10f, pos.y - 20f, 603f, 95f), string.Empty, "BoxBrickText");
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("HELP_TITLE_5"), "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		TextureUtil.DrawTexture(new Rect(pos.x + 160f, pos.y - 10f, (float)etcKey.width, (float)etcKey.height), etcKey, ScaleMode.StretchToFill);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y - 6f), StringMgr.Instance.Get("HELP_23"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 7f), StringMgr.Instance.Get("HELP_24"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 365f, pos.y + 20f), StringMgr.Instance.Get("HELP_25"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 33f), StringMgr.Instance.Get("HELP_26"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 46f), StringMgr.Instance.Get("HELP_27"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		LabelUtil.TextOut(new Vector2(pos.x + 340f, pos.y + 59f), StringMgr.Instance.Get("HELP_28"), "HelpLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
		GUI.EndScrollView();
		GUI.EndGroup();
		GUI.skin = skin;
		if (GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
			focus = false;
			GlobalVars.Instance.resetMenuEx();
		}
		return result;
	}
}
