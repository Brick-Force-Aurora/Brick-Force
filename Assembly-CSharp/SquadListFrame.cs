using System;
using UnityEngine;

[Serializable]
public class SquadListFrame
{
	private Rect crdOutline = new Rect(15f, 86f, 680f, 380f);

	private Rect crdSquadColumn = new Rect(21f, 91f, 666f, 31f);

	private Vector2 crdTeamName = new Vector2(110f, 107f);

	private Vector2 crdNumPlayers = new Vector2(265f, 107f);

	private Vector2 crdRecord = new Vector2(420f, 107f);

	private Vector2 crdMatchTeamLeader = new Vector2(600f, 107f);

	private Rect crdSquadList = new Rect(21f, 125f, 666f, 337f);

	private Vector2 crdSquad = new Vector2(646f, 27f);

	private float crdStartY = 13f;

	private float crdOffsetY = 28f;

	private Vector2 spSquad = Vector2.zero;

	private int curSquad;

	private Squad selectedSquad;

	public Squad SelectedSquad => selectedSquad;

	public void Start()
	{
	}

	public void Update()
	{
	}

	private float GridOut(Squad squad, float y)
	{
		LabelUtil.TextOut(new Vector2(crdTeamName.x - crdSquadList.x, y), squad.Name, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(crdNumPlayers.x - crdSquadList.x, y), squad.MemberCountString, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(crdRecord.x - crdSquadList.x, y), squad.Record, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(new Vector2(crdMatchTeamLeader.x - crdSquadList.x, y), squad.TeamLeader, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		return y + crdOffsetY;
	}

	public void OnGUI()
	{
		GUI.Box(crdOutline, string.Empty, "LineBoxBlue");
		GUI.Box(crdSquadColumn, string.Empty, "BoxFadeBlue");
		Color txtMainColor = GlobalVars.Instance.txtMainColor;
		LabelUtil.TextOut(crdTeamName, StringMgr.Instance.Get("TEAM_NAME"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdNumPlayers, StringMgr.Instance.Get("NUM_PLAYERS"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdRecord, StringMgr.Instance.Get("RECORD"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		LabelUtil.TextOut(crdMatchTeamLeader, StringMgr.Instance.Get("MATCH_TEAM_LEADER"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Squad[] squadArray = SquadManager.Instance.GetSquadArray();
		string[] array = new string[squadArray.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = string.Empty;
		}
		Rect rect = new Rect(0f, 0f, crdSquad.x, (float)squadArray.Length * (crdSquad.y + 1f));
		selectedSquad = null;
		spSquad = GUI.BeginScrollView(crdSquadList, spSquad, rect);
		curSquad = GUI.SelectionGrid(rect, curSquad, array, 1, "BoxGridStyle");
		float y = crdStartY;
		for (int j = 0; j < squadArray.Length; j++)
		{
			y = GridOut(squadArray[j], y);
			if (j == curSquad)
			{
				selectedSquad = squadArray[j];
			}
		}
		GUI.EndScrollView();
	}
}
