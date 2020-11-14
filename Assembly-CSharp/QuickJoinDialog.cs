using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuickJoinDialog : Dialog
{
	private Rect crdOutlineBtns = new Rect(12f, 91f, 712f, 268f);

	private Vector2 crdModeSel = new Vector2(40f, 60f);

	private Vector2 crdMapSel = new Vector2(40f, 375f);

	private Color clrSubTitle = new Color(1f, 1f, 1f, 1f);

	private bool isAllmap = true;

	private bool isOffimap;

	private bool isUsermap;

	private Rect crdAllmap = new Rect(35f, 410f, 21f, 20f);

	private Rect crdOffimap = new Rect(185f, 410f, 21f, 20f);

	private Rect crdUsermap = new Rect(335f, 410f, 21f, 20f);

	private List<string> matchList;

	private List<Texture2D> matchIcons;

	private List<int> matchMaskList;

	private List<Texture2D> matchBestIcons;

	public Texture2D iconBest;

	private bool[] chkMatchs;

	private bool[] bests = new bool[11]
	{
		false,
		false,
		false,
		false,
		false,
		false,
		true,
		false,
		false,
		false,
		false
	};

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.QUICKJOIN;
		clrSubTitle = GlobalVars.Instance.GetByteColor2FloatColor(200, 200, 200);
		Texture2D[] array = new Texture2D[11]
		{
			null,
			GlobalVars.Instance.iconTeamMode,
			GlobalVars.Instance.iconsurvivalMode,
			GlobalVars.Instance.iconCTFMode,
			GlobalVars.Instance.iconBlastMode,
			GlobalVars.Instance.iconDefenseMode,
			GlobalVars.Instance.iconBndMode,
			GlobalVars.Instance.iconBungeeMode,
			GlobalVars.Instance.iconEscapeMode,
			GlobalVars.Instance.iconZombieMode,
			GlobalVars.Instance.iconZombieMode
		};
		matchList = new List<string>();
		matchIcons = new List<Texture2D>();
		matchMaskList = new List<int>();
		matchBestIcons = new List<Texture2D>();
		for (Room.ROOM_TYPE rOOM_TYPE = Room.ROOM_TYPE.TEAM_MATCH; rOOM_TYPE < Room.ROOM_TYPE.NUM_TYPE; rOOM_TYPE++)
		{
			if (BuildOption.Instance.Props.IsSupportMode(rOOM_TYPE))
			{
				matchList.Add(StringMgr.Instance.Get(Room.typeStringKey[(int)rOOM_TYPE]));
				matchIcons.Add(array[(int)rOOM_TYPE]);
				matchMaskList.Add(Room.modeMasks[(int)rOOM_TYPE]);
				matchBestIcons.Add((!bests[(int)rOOM_TYPE]) ? null : iconBest);
			}
		}
		chkMatchs = new bool[matchList.Count];
		for (int i = 0; i < chkMatchs.Length; i++)
		{
			chkMatchs[i] = false;
		}
	}

	public override void OnPopup()
	{
		size.x = 736f;
		size.y = 512f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		if (MyInfoManager.Instance.qjModeMask > 0)
		{
			for (int i = 0; i < chkMatchs.Length; i++)
			{
				if ((MyInfoManager.Instance.qjModeMask & matchMaskList[i]) == matchMaskList[i])
				{
					chkMatchs[i] = true;
				}
			}
		}
		isAllmap = false;
		isOffimap = false;
		isUsermap = false;
		switch (MyInfoManager.Instance.qjOfficialMask)
		{
		case 1:
			isOffimap = true;
			break;
		case 2:
			isUsermap = true;
			break;
		default:
			isAllmap = true;
			break;
		}
	}

	private bool CheckMask()
	{
		MyInfoManager.Instance.qjModeMask = 0;
		for (int i = 0; i < chkMatchs.Length; i++)
		{
			if (chkMatchs[i])
			{
				MyInfoManager.Instance.qjModeMask |= matchMaskList[i];
			}
		}
		if (isOffimap)
		{
			MyInfoManager.Instance.qjOfficialMask = 1;
		}
		else if (isUsermap)
		{
			MyInfoManager.Instance.qjOfficialMask = 2;
		}
		else
		{
			MyInfoManager.Instance.qjOfficialMask = -1;
		}
		return MyInfoManager.Instance.qjModeMask != 0;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("QUICK_JOIN"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		LabelUtil.TextOut(crdModeSel, StringMgr.Instance.Get("MODE_SELECT"), "SubTitleLabel", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		GUI.Box(crdOutlineBtns, string.Empty, "LineBoxBlue");
		Rect rect = new Rect(crdOutlineBtns.x + 14f, crdOutlineBtns.y + 21f, 176f, 62f);
		for (int i = 0; i < matchList.Count; i++)
		{
			chkMatchs[i] = GUI.Toggle(new Rect(rect.x, rect.y + 21f, 21f, 20f), chkMatchs[i], string.Empty);
			GUIContent content = new GUIContent(matchList[i], matchIcons[i]);
			if (GlobalVars.Instance.MyButton3(new Rect(rect.x + 30f, rect.y, rect.width, rect.height), content, "BtnBlue"))
			{
				chkMatchs[i] = !chkMatchs[i];
			}
			if ((BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper) && matchBestIcons[i] != null)
			{
				TextureUtil.DrawTexture(new Rect(rect.x + 30f, rect.y, rect.width, rect.height), matchBestIcons[i]);
			}
			rect.x += 222f;
			if (i == 2 || i == 5)
			{
				rect.x = crdOutlineBtns.x + 14f;
				rect.y += 80f;
			}
		}
		LabelUtil.TextOut(crdMapSel, StringMgr.Instance.Get("MAP_SELECT"), "SubTitleLabel", clrSubTitle, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		bool value = isAllmap;
		bool value2 = isOffimap;
		bool value3 = isUsermap;
		value = GUI.Toggle(crdAllmap, value, StringMgr.Instance.Get("ALL_MAP"));
		value2 = GUI.Toggle(crdOffimap, value2, StringMgr.Instance.Get("OFFICIAL_MAP"));
		value3 = GUI.Toggle(crdUsermap, value3, StringMgr.Instance.Get("USER_MAP"));
		if (value && !isAllmap)
		{
			isAllmap = true;
			isOffimap = false;
			isUsermap = false;
		}
		else if (value2 && !isOffimap)
		{
			isAllmap = false;
			isOffimap = true;
			isUsermap = false;
		}
		else if (value3 && !isUsermap)
		{
			isAllmap = false;
			isOffimap = false;
			isUsermap = true;
		}
		bool enabled = GUI.enabled;
		GUI.enabled = CheckMask();
		Rect rc = new Rect(size.x / 2f - 65f, size.y - 54f, 130f, 34f);
		if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("ENTER"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
		{
			CSNetManager.Instance.Sock.SendCS_QUICK_JOIN_REQ(MyInfoManager.Instance.qjModeMask, MyInfoManager.Instance.qjOfficialMask);
			result = true;
		}
		GUI.enabled = enabled;
		Rect rc2 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.skin = skin;
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		return result;
	}
}
