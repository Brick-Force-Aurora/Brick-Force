using System;
using UnityEngine;

[Serializable]
public class RenameMapDlg : Dialog
{
	public int maxAlias = 10;

	private UserMapInfo userMap;

	private string newMapAlias;

	public Rect crdOutline = new Rect(0f, 0f, 0f, 0f);

	public Rect crdCurAlias = new Rect(175f, 64f, 194f, 27f);

	public Vector2 crdCurAliasLabel = new Vector2(175f, 64f);

	private Rect crdNewAlias = new Rect(173f, 107f, 194f, 20f);

	public Vector2 crdNewAliasLabel = new Vector2(173f, 105f);

	private Rect crdOk = new Rect(300f, 163f, 90f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.RENAME_MAP;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(UserMapInfo _userMap)
	{
		userMap = _userMap;
		newMapAlias = string.Empty;
	}

	private bool CheckAlias()
	{
		newMapAlias.Trim();
		if (newMapAlias.Length <= 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MAP_NAME"));
			return false;
		}
		if (newMapAlias.Length < 2)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TOO_SHORT_MAP_NAME"));
			return false;
		}
		return true;
	}

	public override bool DoDialog()
	{
		if (userMap == null)
		{
			return true;
		}
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(crdOutline, string.Empty, "BoxPopLine");
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("RENAME_MAP"), "BigLabel", GlobalVars.Instance.txtMainColor, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdCurAliasLabel, StringMgr.Instance.Get("MAP_NAME_IS"), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperRight);
		GUI.Label(crdCurAlias, userMap.Alias);
		LabelUtil.TextOut(crdNewAliasLabel, StringMgr.Instance.Get("NEW_MAP_NAME"), "Label", Color.white, new Color(0f, 0f, 0f, 0f), TextAnchor.UpperRight);
		string text = newMapAlias;
		GUI.SetNextControlName("AliasInput");
		newMapAlias = GUI.TextField(crdNewAlias, newMapAlias, maxAlias);
		if (newMapAlias.Length > maxAlias)
		{
			newMapAlias = text;
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction") && CheckAlias())
		{
			CSNetManager.Instance.Sock.SendCS_CHANGE_USERMAP_ALIAS_REQ((sbyte)userMap.Slot, newMapAlias);
			result = true;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		Dialog top = DialogManager.Instance.GetTop();
		if (top != null && top.ID == id)
		{
			GUI.FocusWindow((int)id);
			GUI.FocusControl("AliasInput");
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}
}
