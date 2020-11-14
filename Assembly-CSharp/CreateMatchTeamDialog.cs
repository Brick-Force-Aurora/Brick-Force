using System;
using UnityEngine;

[Serializable]
public class CreateMatchTeamDialog : Dialog
{
	public Rect crdOutline = new Rect(0f, 0f, 0f, 0f);

	private Rect crdLeft = new Rect(80f, 95f, 22f, 22f);

	private Rect crdRight = new Rect(230f, 95f, 22f, 22f);

	public Rect crdNumPlayerBg = new Rect(0f, 0f, 0f, 0f);

	public Vector2 crdCreateMatchTeamComment = new Vector2(0f, 0f);

	public Vector2 crdNumPlayerValue = new Vector2(0f, 0f);

	private Rect crdCreate = new Rect(240f, 139f, 90f, 34f);

	public int minPlayer = 4;

	public int maxPlayer = 8;

	private int numPlayer;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CREATE_MATCH_TEAM;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		numPlayer = minPlayer;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CREATE_TEAM"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		LabelUtil.TextOut(crdCreateMatchTeamComment, StringMgr.Instance.Get("CREATE_TEAM_COMMENT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (GlobalVars.Instance.MyButton(crdLeft, string.Empty, "Left"))
		{
			numPlayer--;
			if (numPlayer < minPlayer)
			{
				numPlayer = minPlayer;
			}
		}
		GUI.Box(crdNumPlayerBg, string.Empty, "BoxTextBg");
		LabelUtil.TextOut(crdNumPlayerValue, numPlayer.ToString() + "vs" + numPlayer.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		if (GlobalVars.Instance.MyButton(crdRight, string.Empty, "Right"))
		{
			numPlayer++;
			if (numPlayer > maxPlayer)
			{
				numPlayer = maxPlayer;
			}
		}
		if (GlobalVars.Instance.MyButton(crdCreate, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_CREATE_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, GlobalVars.Instance.wannaPlayMap, GlobalVars.Instance.wannaPlayMode, numPlayer);
			result = true;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
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
}
