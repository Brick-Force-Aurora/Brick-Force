using System;
using UnityEngine;

[Serializable]
public class KickDialog : Dialog
{
	public Vector2 crdXLT = new Vector2(3f, 3f);

	public Vector2 crdX = new Vector2(14f, 14f);

	public Vector2 crdLeftTop = new Vector2(0f, 0f);

	public Vector2 crdNickname = new Vector2(20f, 0f);

	public float offset = 22f;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.KICK;
	}

	public override void OnPopup()
	{
		CalcSize();
	}

	public override bool DoDialog()
	{
		CalcSize();
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		LabelUtil.TextOut(new Vector2(size.x / 2f, crdLeftTop.y / 2f), StringMgr.Instance.Get("KICKOFF"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		Vector2 vector = crdLeftTop;
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayWhoTookTooLongToWait();
		if (array.Length <= 0)
		{
			result = true;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (GlobalVars.Instance.MyButton(new Rect(vector.x + crdXLT.x, vector.y + crdXLT.y, crdX.x, crdX.y), string.Empty, "X"))
			{
				CSNetManager.Instance.Sock.SendCS_KICK_REQ(array[i].Seq);
			}
			LabelUtil.TextOut(new Vector2(vector.x + crdNickname.x, vector.y + crdNickname.y), array[i].Nickname, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			vector.y += offset;
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void CalcSize()
	{
		size = new Vector2(256f, 0f);
		BrickManDesc[] array = BrickManManager.Instance.ToDescriptorArrayWhoTookTooLongToWait();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsTooLong4Init)
			{
				size.y += offset;
			}
		}
		size.y += offset + crdLeftTop.y;
		rc = new Rect(GlobalVars.Instance.ScreenRect.width - size.x, 256f, size.x, size.y);
	}
}
