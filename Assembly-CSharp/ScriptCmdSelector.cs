using System;
using UnityEngine;

[Serializable]
public class ScriptCmdSelector : Dialog
{
	private ScriptEditor scriptEditor;

	private Vector2 scrollPosition;

	private int selected;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SCRIPT_CMD_SELECTOR;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.skin = skin;
		int num = 8;
		int num2 = ScriptResManager.Instance.CmdIcon.Length / num;
		if (ScriptResManager.Instance.CmdIcon.Length % num > 0)
		{
			num2++;
		}
		int width = ScriptResManager.Instance.CmdIcon[0].width;
		Rect rect = new Rect(0f, 0f, (float)(width * num), (float)(width * num2));
		scrollPosition = GUI.BeginScrollView(new Rect(4f, 25f, size.x - 8f, size.y - 50f), scrollPosition, rect);
		selected = GUI.SelectionGrid(rect, selected, ScriptResManager.Instance.CmdIcon, num);
		GUI.EndScrollView();
		if (GUI.Button(new Rect(190f, 174f, 90f, 21f), StringMgr.Instance.Get("OK")))
		{
			scriptEditor.AddCmd(ScriptCmdFactory.CreateDefault(selected));
			result = true;
		}
		if (GUI.Button(new Rect(290f, 174f, 90f, 21f), StringMgr.Instance.Get("CANCEL")))
		{
			result = true;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void InitDialog(ScriptEditor _scriptEditor)
	{
		scriptEditor = _scriptEditor;
		selected = 0;
		scrollPosition = Vector2.zero;
	}
}
