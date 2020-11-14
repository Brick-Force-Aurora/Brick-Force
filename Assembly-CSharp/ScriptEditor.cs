using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptEditor : Dialog
{
	private BrickProperty prop;

	private Brick brick;

	private string alias = string.Empty;

	private bool enableOnAwake;

	private bool visibleOnAwake;

	private List<ScriptCmd> cmdList;

	private int selected;

	private Vector2 scrollPosition = Vector2.zero;

	private Vector2 spId = Vector2.zero;

	private Vector2 spSound = Vector2.zero;

	private Vector2 spWeapon = Vector2.zero;

	private string floatTmp = string.Empty;

	public int maxAlias = 16;

	public int maxDialog = 256;

	public int maxMission = 128;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SCRIPT_EDITOR;
		cmdList = new List<ScriptCmd>();
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(BrickProperty _prop, Brick _brick, BrickInst _inst)
	{
		prop = _prop;
		brick = _brick;
		selected = 0;
		scrollPosition = Vector2.zero;
		spId = Vector2.zero;
		if (_inst.BrickForceScript != null)
		{
			alias = _inst.BrickForceScript.Alias;
			enableOnAwake = _inst.BrickForceScript.EnableOnAwake;
			visibleOnAwake = _inst.BrickForceScript.VisibleOnAwake;
			cmdList.Clear();
			for (int i = 0; i < _inst.BrickForceScript.CmdList.Count; i++)
			{
				cmdList.Add(_inst.BrickForceScript.CmdList[i]);
			}
		}
	}

	public override bool DoDialog()
	{
		if (null == prop || brick == null)
		{
			return true;
		}
		string text = "ID: " + prop.Seq.ToString() + " (Scriptable)";
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(new Rect(4f, 4f, 64f, 64f), brick.Icon);
		GUI.Label(new Rect(72f, 4f, size.x - 76f, 36f), text, "BigLabel");
		alias = GUI.TextField(new Rect(72f, 40f, 256f, 26f), alias, maxAlias);
		enableOnAwake = GUI.Toggle(new Rect(336f, 47f, 128f, 16f), enableOnAwake, StringMgr.Instance.Get("ENABLE_ON_AWAKE"));
		visibleOnAwake = GUI.Toggle(new Rect(336f, 64f, 128f, 16f), visibleOnAwake, StringMgr.Instance.Get("VISIBLE_ON_AWAKE"));
		int count = cmdList.Count;
		Rect rect = new Rect(0f, 0f, 48f, (float)(count * 48));
		Rect position = new Rect(4f, 112f, size.x / 2f - 8f, 208f);
		Texture2D[] array = new Texture2D[cmdList.Count];
		for (int i = 0; i < cmdList.Count; i++)
		{
			array[i] = ScriptResManager.Instance.CmdIcon[cmdList[i].GetIconIndex()];
		}
		scrollPosition = GUI.BeginScrollView(position, scrollPosition, rect);
		int num = selected;
		selected = GUI.SelectionGrid(rect, selected, array, 1);
		if (selected != num)
		{
			if (ui2script(num))
			{
				script2ui(selected);
			}
			else
			{
				selected = num;
			}
		}
		Color color = GUI.color;
		Vector2 vector = new Vector2(52f, 0f);
		for (int j = 0; j < cmdList.Count; j++)
		{
			if (selected == j)
			{
				GUI.color = new Color(0.83f, 0.49f, 0.29f);
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), cmdList[j].GetName(), "Label");
			vector.y += 48f;
		}
		GUI.color = color;
		GUI.EndScrollView();
		if (GUI.Button(new Rect(4f, 72f, 24f, 24f), "+"))
		{
			((ScriptCmdSelector)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.SCRIPT_CMD_SELECTOR, exclusive: false))?.InitDialog(this);
		}
		if (GUI.Button(new Rect(32f, 72f, 24f, 24f), "x"))
		{
			cmdList.RemoveAt(selected);
			VerifySelected();
		}
		if (GUI.Button(new Rect(60f, 72f, 24f, 24f), "u") && 0 < selected)
		{
			ScriptCmd item = cmdList[selected];
			cmdList.RemoveAt(selected--);
			cmdList.Insert(selected, item);
			VerifySelectedVisible(208f);
		}
		if (GUI.Button(new Rect(88f, 72f, 24f, 24f), "d") && cmdList.Count > 1 && selected < cmdList.Count - 1)
		{
			ScriptCmd item2 = cmdList[selected];
			cmdList.RemoveAt(selected++);
			cmdList.Insert(selected, item2);
			VerifySelectedVisible(208f);
		}
		DoParameters();
		if (GUI.Button(new Rect(312f, 357f, 90f, 21f), StringMgr.Instance.Get("OK")) && CheckAlias() && ui2script(selected))
		{
			result = true;
			string text2 = string.Empty;
			for (int k = 0; k < cmdList.Count; k++)
			{
				text2 += cmdList[k].GetDescription();
				if (k < cmdList.Count - 1)
				{
					text2 += ScriptCmd.CmdDelimeters[0];
				}
			}
			CSNetManager.Instance.Sock.SendCS_UPDATE_SCRIPT_REQ(prop.Seq, alias, enableOnAwake, visibleOnAwake, text2);
		}
		if (GUI.Button(new Rect(412f, 357f, 90f, 21f), StringMgr.Instance.Get("CANCEL")))
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

	private bool ui2script(int prev)
	{
		bool result = true;
		if (0 <= prev && prev < cmdList.Count)
		{
			ScriptCmd scriptCmd = cmdList[prev];
			switch (scriptCmd.GetName())
			{
			case "Sleep":
				try
				{
					if (floatTmp.Length <= 0)
					{
						floatTmp = "0";
					}
					((Sleep)scriptCmd).Howlong = float.Parse(floatTmp);
					return result;
				}
				catch (Exception ex)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FLOAT_PARSE_FAIL"));
					Debug.LogError(ex.Message.ToString());
					return false;
				}
			}
		}
		return result;
	}

	private void script2ui(int next)
	{
		if (0 <= next && next < cmdList.Count)
		{
			ScriptCmd scriptCmd = cmdList[next];
			switch (scriptCmd.GetName())
			{
			case "EnableScript":
				break;
			case "ShowDialog":
				break;
			case "PlaySound":
				break;
			case "Exit":
				break;
			case "ShowScript":
				break;
			case "SetMission":
				break;
			case "Sleep":
				floatTmp = ((Sleep)scriptCmd).Howlong.ToString("0.##");
				break;
			}
		}
	}

	private bool CheckAlias()
	{
		bool flag = false;
		BrickProperty[] allScriptables = BrickManager.Instance.GetAllScriptables();
		if (allScriptables != null)
		{
			int num = 0;
			while (!flag && num < allScriptables.Length)
			{
				if (allScriptables[num].Seq != prop.Seq)
				{
					BrickInst brickInst = BrickManager.Instance.GetBrickInst(allScriptables[num].Seq);
					if (brickInst != null && brickInst.BrickForceScript.Alias == alias)
					{
						flag = true;
					}
				}
				num++;
			}
		}
		if (flag)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("DUPLICATED_SCRIPT_ALIAS"));
		}
		return !flag;
	}

	public void AddCmd(ScriptCmd cmd)
	{
		cmdList.Add(cmd);
	}

	private void VerifySelected()
	{
		if (selected >= cmdList.Count)
		{
			selected = cmdList.Count - 1;
		}
		if (selected < 0)
		{
			selected = 0;
		}
	}

	private void VerifySelectedVisible(float scrollHeight)
	{
		float num = (float)(selected * 48);
		float num2 = num + 48f;
		float y = scrollPosition.y;
		float num3 = y + scrollHeight;
		if (!(num > y) || !(num2 < num3))
		{
			if (num3 < num)
			{
				scrollPosition.y += num2 - num3;
			}
			else if (y < num)
			{
				scrollPosition.y += num2 - num3;
			}
			else if (y < num2)
			{
				scrollPosition.y -= y - num;
			}
			else
			{
				scrollPosition.y -= y - num;
			}
		}
	}

	private void DoParameters()
	{
		if (0 <= selected && selected < cmdList.Count)
		{
			ScriptCmd scriptCmd = cmdList[selected];
			switch (scriptCmd.GetName())
			{
			case "EnableScript":
				DoEnableScript((EnableScript)scriptCmd);
				break;
			case "ShowDialog":
				DoShowDialog((ShowDialog)scriptCmd);
				break;
			case "PlaySound":
				DoPlaySound((PlaySound)scriptCmd);
				break;
			case "Sleep":
				DoSleep((Sleep)scriptCmd);
				break;
			case "Exit":
				DoExit((Exit)scriptCmd);
				break;
			case "ShowScript":
				DoShowScript((ShowScript)scriptCmd);
				break;
			case "GiveWeapon":
				DoGiveWeapon((GiveWeapon)scriptCmd);
				break;
			case "TakeAwayAll":
				DoTakeAwayAll((TakeAwayAll)scriptCmd);
				break;
			case "SetMission":
				DoSetMission((SetMission)scriptCmd);
				break;
			}
		}
	}

	private void DoSetMission(SetMission setMission)
	{
		setMission.Progress = GUI.TextField(new Rect(size.x / 2f + 4f, 88f, size.x / 2f - 8f, 20f), setMission.Progress, 32);
		setMission.Title = GUI.TextField(new Rect(size.x / 2f + 4f, 110f, size.x / 2f - 8f, 20f), setMission.Title, 32);
		setMission.SubTitle = GUI.TextField(new Rect(size.x / 2f + 4f, 132f, size.x / 2f - 8f, 20f), setMission.SubTitle, 32);
		setMission.Tag = GUI.TextArea(new Rect(size.x / 2f + 4f, 154f, size.x / 2f - 8f, 168f), setMission.Tag, maxMission);
	}

	private void DoGiveWeapon(GiveWeapon giveWeapon)
	{
		TWeapon[] completeWeaponArray = TItemManager.Instance.GetCompleteWeaponArray();
		if (completeWeaponArray != null)
		{
			int num = -1;
			Texture2D[] array = new Texture2D[completeWeaponArray.Length];
			string[] array2 = new string[completeWeaponArray.Length];
			Vector2 zero = Vector2.zero;
			for (int i = 0; i < completeWeaponArray.Length; i++)
			{
				if (completeWeaponArray[i].code == giveWeapon.WeaponCode)
				{
					num = i;
				}
				if (completeWeaponArray[i].CurIcon() == null)
				{
					Debug.LogError(" null icon name is " + completeWeaponArray[i].Name);
				}
				array[i] = completeWeaponArray[i].CurIcon();
				array2[i] = completeWeaponArray[i].Name;
				zero.x = Mathf.Max((float)array[i].width, zero.x);
				zero.y = Mathf.Max((float)array[i].height, zero.y);
			}
			int num2 = completeWeaponArray.Length;
			Rect rect = new Rect(0f, 0f, zero.x, (float)num2 * zero.y);
			Rect position = new Rect(size.x / 2f + 4f, 112f, size.x / 2f - 8f, 192f);
			spWeapon = GUI.BeginScrollView(position, spWeapon, rect);
			num = GUI.SelectionGrid(rect, num, array, 1);
			if (0 <= num && num < completeWeaponArray.Length)
			{
				giveWeapon.WeaponCode = completeWeaponArray[num].code;
			}
			GUI.EndScrollView();
		}
	}

	private void DoPlaySound(PlaySound playSound)
	{
		Texture[] sndIconArray = ScriptResManager.Instance.GetSndIconArray();
		string[] sndAliasArray = ScriptResManager.Instance.GetSndAliasArray();
		int num = sndAliasArray.Length;
		Rect rect = new Rect(0f, 0f, 48f, (float)(num * 48));
		Rect position = new Rect(size.x / 2f + 4f, 112f, size.x / 2f - 8f, 192f);
		spSound = GUI.BeginScrollView(position, spSound, rect);
		playSound.Index = GUI.SelectionGrid(rect, playSound.Index, sndIconArray, 1);
		Color color = GUI.color;
		Vector2 vector = new Vector2(52f, 0f);
		for (int i = 0; i < sndAliasArray.Length; i++)
		{
			if (i == playSound.Index)
			{
				GUI.color = new Color(0.83f, 0.49f, 0.29f);
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), sndAliasArray[i], "MiniLabel");
			vector.y += 48f;
		}
		GUI.color = color;
		GUI.EndScrollView();
	}

	private void DoSleep(Sleep sleep)
	{
		GUI.Label(new Rect(size.x / 2f + 4f, 112f, 56f, 26f), StringMgr.Instance.Get("WAIT_SEC"));
		floatTmp = GUI.TextField(new Rect(size.x / 2f + 60f, 112f, size.x / 2f - 68f, 26f), floatTmp);
	}

	private void DoExit(Exit exit)
	{
	}

	private void DoTakeAwayAll(TakeAwayAll takeAwayAll)
	{
	}

	private void DoShowScript(ShowScript showScript)
	{
		BrickProperty[] allScriptables = BrickManager.Instance.GetAllScriptables();
		if (allScriptables != null)
		{
			int num = -1;
			List<Texture> list = new List<Texture>();
			for (int i = 0; i < allScriptables.Length; i++)
			{
				Brick brick = BrickManager.Instance.GetBrick(allScriptables[i].Index);
				if (brick != null)
				{
					list.Add(brick.Icon);
				}
				else
				{
					Debug.LogError("Fail to get scriptables icon ");
				}
				if (showScript.Id == allScriptables[i].Seq)
				{
					num = i;
				}
			}
			if (num < 0)
			{
				num = 0;
			}
			Texture[] array = list.ToArray();
			int num2 = array.Length;
			Rect rect = new Rect(0f, 0f, 48f, (float)(num2 * 48));
			Rect position = new Rect(size.x / 2f + 4f, 112f, size.x / 2f - 8f, 192f);
			spId = GUI.BeginScrollView(position, spId, rect);
			num = GUI.SelectionGrid(rect, num, array, 1);
			showScript.Id = allScriptables[num].Seq;
			Color color = GUI.color;
			Vector2 vector = new Vector2(52f, 0f);
			for (int j = 0; j < allScriptables.Length; j++)
			{
				if (j == num)
				{
					GUI.color = new Color(0.83f, 0.49f, 0.29f);
				}
				else
				{
					GUI.color = Color.white;
				}
				BrickInst brickInst = BrickManager.Instance.GetBrickInst(allScriptables[j].Seq);
				if (brickInst == null || brickInst.BrickForceScript == null || brickInst.BrickForceScript.Alias.Length <= 0)
				{
					GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), allScriptables[j].Seq.ToString(), "MiniLabel");
				}
				else
				{
					string text = (allScriptables[j].Seq != prop.Seq) ? brickInst.BrickForceScript.Alias : alias;
					GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), text, "MiniLabel");
				}
				vector.y += 48f;
			}
			GUI.color = color;
			GUI.EndScrollView();
		}
		showScript.Visible = GUI.Toggle(new Rect(size.x / 2f + 4f, 310f, 128f, 13f), showScript.Visible, StringMgr.Instance.Get("VISIBLE_OR"));
	}

	private void DoEnableScript(EnableScript enableScript)
	{
		BrickProperty[] allScriptables = BrickManager.Instance.GetAllScriptables();
		if (allScriptables != null)
		{
			int num = -1;
			List<Texture> list = new List<Texture>();
			for (int i = 0; i < allScriptables.Length; i++)
			{
				Brick brick = BrickManager.Instance.GetBrick(allScriptables[i].Index);
				if (brick != null)
				{
					list.Add(brick.Icon);
				}
				else
				{
					Debug.LogError("Fail to get scriptables icon ");
				}
				if (enableScript.Id == allScriptables[i].Seq)
				{
					num = i;
				}
			}
			if (num < 0)
			{
				num = 0;
			}
			Texture[] array = list.ToArray();
			int num2 = array.Length;
			Rect rect = new Rect(0f, 0f, 48f, (float)(num2 * 48));
			Rect position = new Rect(size.x / 2f + 4f, 112f, size.x / 2f - 8f, 192f);
			spId = GUI.BeginScrollView(position, spId, rect);
			num = GUI.SelectionGrid(rect, num, array, 1);
			enableScript.Id = allScriptables[num].Seq;
			Color color = GUI.color;
			Vector2 vector = new Vector2(52f, 0f);
			for (int j = 0; j < allScriptables.Length; j++)
			{
				if (j == num)
				{
					GUI.color = new Color(0.83f, 0.49f, 0.29f);
				}
				else
				{
					GUI.color = Color.white;
				}
				BrickInst brickInst = BrickManager.Instance.GetBrickInst(allScriptables[j].Seq);
				if (brickInst == null || brickInst.BrickForceScript == null || brickInst.BrickForceScript.Alias.Length <= 0)
				{
					GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), allScriptables[j].Seq.ToString(), "MiniLabel");
				}
				else
				{
					string text = (allScriptables[j].Seq != prop.Seq) ? brickInst.BrickForceScript.Alias : alias;
					GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), text, "MiniLabel");
				}
				vector.y += 48f;
			}
			GUI.color = color;
			GUI.EndScrollView();
		}
		enableScript.Enable = GUI.Toggle(new Rect(size.x / 2f + 4f, 310f, 128f, 13f), enableScript.Enable, StringMgr.Instance.Get("ENABLE_OR"));
	}

	private void DoShowDialog(ShowDialog showDialog)
	{
		Texture[] dlgIconArray = ScriptResManager.Instance.GetDlgIconArray();
		string[] dlgAliasArray = ScriptResManager.Instance.GetDlgAliasArray();
		int num = dlgAliasArray.Length;
		Rect rect = new Rect(0f, 0f, 48f, (float)(num * 48));
		Rect position = new Rect(size.x / 2f + 4f, 112f, size.x / 2f - 8f, 96f);
		spSound = GUI.BeginScrollView(position, spSound, rect);
		showDialog.Speaker = GUI.SelectionGrid(rect, showDialog.Speaker, dlgIconArray, 1);
		Color color = GUI.color;
		Vector2 vector = new Vector2(52f, 0f);
		for (int i = 0; i < dlgAliasArray.Length; i++)
		{
			if (i == showDialog.Speaker)
			{
				GUI.color = new Color(0.83f, 0.49f, 0.29f);
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.Label(new Rect(vector.x, vector.y, size.x / 2f - 60f, 48f), dlgAliasArray[i], "MiniLabel");
			vector.y += 48f;
		}
		GUI.color = color;
		GUI.EndScrollView();
		showDialog.Dialog = GUI.TextArea(new Rect(size.x / 2f + 4f, 212f, size.x / 2f - 8f, 108f), showDialog.Dialog, maxDialog);
	}
}
