using System;
using UnityEngine;

[Serializable]
public class SquadMemberListFrame
{
	private Rect crdMemberListOutline = new Rect(15f, 86f, 680f, 150f);

	private Rect crdMemberList = new Rect(22f, 119f, 665f, 112f);

	public Vector2[] crdSquadMember;

	private Vector2 crdBadge = new Vector2(3f, 4f);

	private Vector2 crdBadgeSize = new Vector2(34f, 17f);

	private Vector2 crdNickname = new Vector2(38f, 1f);

	private int curMember;

	private NameCard selectedMember;

	public NameCard SelectedMember => selectedMember;

	public void Start()
	{
	}

	public void Update()
	{
	}

	private void aPlayer(Vector2 lt, NameCard member)
	{
		Texture2D badge = XpManager.Instance.GetBadge(member.Lv, member.Rank);
		if (null != badge)
		{
			TextureUtil.DrawTexture(new Rect(lt.x + crdBadge.x, lt.y + crdBadge.y, crdBadgeSize.x, crdBadgeSize.y), badge, ScaleMode.StretchToFill);
		}
		LabelUtil.TextOut(new Vector2(lt.x + crdNickname.x, lt.y + crdNickname.y), member.Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	public void OnGUI()
	{
		NameCard[] squadMemberArrayInclueMe = SquadManager.Instance.GetSquadMemberArrayInclueMe();
		string[] array = new string[squadMemberArrayInclueMe.Length];
		for (int i = 0; i < squadMemberArrayInclueMe.Length; i++)
		{
			array[i] = string.Empty;
		}
		GUI.Box(crdMemberListOutline, string.Empty, "LineBoxBlue");
		int num = squadMemberArrayInclueMe.Length / 2;
		if (squadMemberArrayInclueMe.Length % 2 != 0)
		{
			num++;
		}
		Rect position = new Rect(crdMemberList.x, crdMemberList.y, crdMemberList.width, crdMemberList.height / 4f * (float)num);
		selectedMember = null;
		curMember = GUI.SelectionGrid(position, curMember, array, 2, "BoxGridStyle");
		for (int j = 0; j < squadMemberArrayInclueMe.Length && j < crdSquadMember.Length; j++)
		{
			if (curMember == j)
			{
				selectedMember = squadMemberArrayInclueMe[j];
			}
			aPlayer(crdSquadMember[j], squadMemberArrayInclueMe[j]);
		}
	}
}
