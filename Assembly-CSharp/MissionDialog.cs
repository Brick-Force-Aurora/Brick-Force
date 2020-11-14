using System;
using UnityEngine;

[Serializable]
public class MissionDialog : Dialog
{
	public enum STAMP_STEP
	{
		STAMP_BEGIN,
		STAMP_OUT,
		STAMP_IN,
		STAMP_WAIT,
		STAMP_NONE
	}

	public Texture2D brickKing;

	public Texture2D point;

	public Texture2D gaugeFrame;

	public Texture2D gauge;

	public Texture2D pointIcon;

	public Texture2D coinIcon;

	public Texture2D missionStamp;

	public AudioClip kwang;

	private Rect crdIncomingMsg = new Rect(15f, 59f, 607f, 428f);

	private Rect crdAcceptMission = new Rect(228f, 498f, 180f, 50f);

	private Rect crdX = new Rect(593f, 10f, 34f, 34f);

	private Rect crdBrickKingOutline = new Rect(15f, 59f, 607f, 104f);

	private Rect crdBrickKing = new Rect(26f, 13f, 134f, 150f);

	private Rect crdBrickKingMessage = new Rect(190f, 68f, 420f, 85f);

	private Rect crdMission = new Rect(15f, 175f, 607f, 247f);

	private Rect crdCompensation = new Rect(15f, 434f, 607f, 53f);

	private Rect crdComplete = new Rect(118f, 498f, 180f, 50f);

	private Rect crdGiveup = new Rect(338f, 498f, 180f, 50f);

	private Rect crdPoint = new Rect(48f, 202f, 7f, 7f);

	private Rect crdGaugeFrame = new Rect(82f, 222f, 470f, 32f);

	private Rect crdGauge = new Rect(95f, 230f, 444f, 16f);

	private Vector2 crdDescription = new Vector2(65f, 195f);

	private Vector2 crdProgress = new Vector2(318f, 238f);

	private float offset = 73f;

	private Rect crdPointOutline = new Rect(27f, 445f, 187f, 30f);

	private float pointOffset = 10f;

	private Vector2 stepPos = new Vector2(36f, 449f);

	private Rect crdPointIcon = new Rect(110f, 455f, 12f, 12f);

	private Vector2 pointPos = new Vector2(131f, 449f);

	private Rect crdStamp = new Rect(155f, 409f, 106f, 88f);

	public bool[] reward1IsPoint;

	public bool[] reward2IsPoint;

	public bool[] reward3IsPoint;

	public bool[][] rewardIsPoint;

	public int[] reward1Count;

	public int[] reward2Count;

	public int[] reward3Count;

	public int[][] rewardCount;

	private Stamp[] stampFx;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MISSION;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		if (rewardIsPoint == null && rewardCount == null)
		{
			rewardIsPoint = new bool[3][];
			rewardIsPoint[0] = MissionLoadManager.Instance.Reward1IsPoints;
			rewardIsPoint[1] = MissionLoadManager.Instance.Reward2IsPoints;
			rewardIsPoint[2] = MissionLoadManager.Instance.Reward3IsPoints;
			rewardCount = new int[3][];
			rewardCount[0] = MissionLoadManager.Instance.Reward1Counts;
			rewardCount[1] = MissionLoadManager.Instance.Reward2Counts;
			rewardCount[2] = MissionLoadManager.Instance.Reward3Counts;
		}
	}

	public override void OnClose(DialogManager.DIALOG_INDEX popup)
	{
	}

	public override void Update()
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < 3)
		{
			if (stampFx[num] != null && stampFx[num].Update(kwang))
			{
				flag = true;
			}
			num++;
		}
	}

	public void InitDialog()
	{
		stampFx = new Stamp[3];
		int num = MissionManager.Instance.CompletedCount();
		for (int i = 0; i < num; i++)
		{
			stampFx[i] = new Stamp(STAMP_STEP.STAMP_NONE);
		}
	}

	public void Complete(int count)
	{
		count = Mathf.Min(count, 3);
		for (int i = 0; i < count; i++)
		{
			if (stampFx[i] == null)
			{
				stampFx[i] = new Stamp(STAMP_STEP.STAMP_BEGIN);
			}
		}
	}

	public bool StampFxing()
	{
		for (int i = 0; i < 3; i++)
		{
			if (stampFx[i] != null && stampFx[i].IsDoing)
			{
				return true;
			}
		}
		return false;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoTitle();
		if (!MissionManager.Instance.HaveMission && !StampFxing())
		{
			bool enabled = GUI.enabled;
			GUI.enabled = DoIncomingMsg();
			if (GlobalVars.Instance.MyButton(crdAcceptMission, StringMgr.Instance.Get("ACCEPT_MISSION"), "BtnAction") || GlobalVars.Instance.IsReturnPressed())
			{
				CSNetManager.Instance.Sock.SendCS_ACCEPT_DAILY_MISSION_REQ();
			}
			GUI.enabled = enabled;
		}
		else
		{
			DoBrickKing();
			DoMission();
			DoCompensation();
			bool enabled2 = GUI.enabled;
			GUI.enabled = MissionManager.Instance.CanCompleteMission;
			if (GlobalVars.Instance.MyButton(crdComplete, StringMgr.Instance.Get("MISSION_COMPLETE"), "BtnAction"))
			{
				CSNetManager.Instance.Sock.SendCS_COMPLETE_DAILY_MISSION_REQ();
			}
			GUI.enabled = enabled2;
			if (GlobalVars.Instance.MyButton(crdGiveup, StringMgr.Instance.Get("MISSION_GIVEUP"), "BtnAction"))
			{
				((AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: false))?.InitDialog(AreYouSure.SURE.CANCEL_DAILY_MISSION);
			}
		}
		if (GlobalVars.Instance.MyButton(crdX, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
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

	private void DoTitle()
	{
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("BRICK_KINGS_ORDER"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
	}

	private bool DoIncomingMsg()
	{
		GUI.Box(crdIncomingMsg, string.Empty, "BoxFadeBlue");
		Vector2 vector = new Vector2(crdIncomingMsg.x + crdIncomingMsg.width / 2f - (float)(brickKing.width / 2), crdIncomingMsg.y + 50f);
		Rect position = new Rect(vector.x, vector.y, (float)brickKing.width, (float)brickKing.height);
		Color color = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.2f);
		TextureUtil.DrawTexture(position, brickKing, ScaleMode.StretchToFill);
		GUI.color = color;
		if (MissionManager.Instance.CanReceiveMission)
		{
			GUI.Label(crdIncomingMsg, StringMgr.Instance.Get("NEW_MISSION_FROM_BRICK_KING"), "MiddleCenterLabel");
			return true;
		}
		GUI.Label(crdIncomingMsg, StringMgr.Instance.Get("NO_MISSION_FOR_TODAY"), "MiddleCenterLabel");
		return false;
	}

	private void DoBrickKing()
	{
		GUI.Box(crdBrickKingOutline, string.Empty, "BoxPopLine");
		TextureUtil.DrawTexture(crdBrickKing, brickKing, ScaleMode.StretchToFill);
		GUI.Label(crdBrickKingMessage, StringMgr.Instance.Get("BRICK_KINGS_COMMENT"), "Label");
	}

	private void DoMission()
	{
		GUI.Box(crdMission, string.Empty, "BoxFadeBlue");
		Rect position = crdPoint;
		Rect position2 = crdGaugeFrame;
		Rect rect = crdGauge;
		Vector2 pos = crdProgress;
		Vector2 pos2 = crdDescription;
		Mission[] array = MissionManager.Instance.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			TextureUtil.DrawTexture(position, point, ScaleMode.StretchToFill);
			LabelUtil.TextOut(pos2, array[i].Description, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			TextureUtil.DrawTexture(position2, gaugeFrame, ScaleMode.StretchToFill);
			TextureUtil.DrawTexture(new Rect(rect.x, rect.y, array[i].Progress * rect.width, rect.height), gauge, ScaleMode.StretchToFill);
			LabelUtil.TextOut(pos, array[i].ProgressString, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
			position.y += offset;
			position2.y += offset;
			rect.y += offset;
			pos.y += offset;
			pos2.y += offset;
		}
	}

	private void DoCompensation()
	{
		GUI.Box(crdCompensation, string.Empty, "BoxFadeBlue");
		Rect position = new Rect(crdPointOutline.x, crdPointOutline.y, crdPointOutline.width, crdPointOutline.height);
		Vector2 pos = new Vector2(stepPos.x, stepPos.y);
		Rect position2 = new Rect(crdPointIcon.x, crdPointIcon.y, crdPointIcon.width, crdPointIcon.height);
		Vector2 pos2 = new Vector2(pointPos.x, pointPos.y);
		Rect rect = new Rect(crdStamp.x, crdStamp.y, crdStamp.width, crdStamp.height);
		for (int i = 0; i < 3; i++)
		{
			GUI.Box(position, string.Empty, "BoxPopLine");
			LabelUtil.TextOut(pos, string.Format(StringMgr.Instance.Get("MISSION_STEP"), i + 1), "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (!rewardIsPoint[i][(int)BuildOption.Instance.target])
			{
				TextureUtil.DrawTexture(position2, coinIcon, ScaleMode.StretchToFill);
			}
			else
			{
				TextureUtil.DrawTexture(position2, pointIcon, ScaleMode.StretchToFill);
			}
			string text = " x " + rewardCount[i][(int)BuildOption.Instance.target];
			LabelUtil.TextOut(pos2, text, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			if (stampFx[i] != null && stampFx[i].Step != 0)
			{
				float num = stampFx[i].LerpSize();
				float num2 = stampFx[i].LerpOffset();
				Vector2 vector = new Vector2(rect.x - num2, rect.y + rect.height / 2f);
				float width = rect.width * num;
				float num3 = rect.height * num;
				Rect position3 = new Rect(vector.x, vector.y - num3 / 2f, width, num3);
				TextureUtil.DrawTexture(position3, missionStamp, ScaleMode.StretchToFill);
			}
			float num4 = crdPointOutline.width + pointOffset;
			position2.x += num4;
			pos.x += num4;
			position.x += num4;
			pos2.x += num4;
			rect.x += num4;
		}
	}
}
