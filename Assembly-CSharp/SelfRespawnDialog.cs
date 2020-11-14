using System;
using UnityEngine;

[Serializable]
public class SelfRespawnDialog : Dialog
{
	private string text;

	public float msgY = 50f;

	private Vector2 sizeOk = new Vector2(100f, 34f);

	private LocalController localCtrl;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SELF_RESPAWN_CONFIRM;
	}

	public override void OnPopup()
	{
		size.x = GlobalVars.Instance.ScreenRect.width;
		rc = new Rect(0f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(string textMore)
	{
		text = textMore;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		LabelUtil.TextOut(new Vector2(GlobalVars.Instance.ScreenRect.width / 2f, msgY), this.text, "BigLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
		verifyLocalController();
		bool enabled = true;
		if (localCtrl != null && localCtrl.SelfRespawnReuseTime != 0f && Time.time - localCtrl.SelfRespawnReuseTime < 300f)
		{
			int num = (int)(300f - (Time.time - localCtrl.SelfRespawnReuseTime));
			int num2 = num / 60;
			int num3 = num % 60;
			string text = string.Format(StringMgr.Instance.Get("SELF_RESPAWN_REMAINTIME"), num2, num3.ToString("00"));
			LabelUtil.TextOut(new Vector2(size.x - 10f, size.y - sizeOk.y - 20f), text, "Label", UILabel.GetLabelColor(UILabel.LABEL_COLOR.MAIN_TEXT), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			enabled = false;
		}
		bool enabled2 = GUI.enabled;
		GUI.enabled = enabled;
		if (GlobalVars.Instance.MyButton(new Rect(GlobalVars.Instance.ScreenRect.width / 2f - 50f, size.y - sizeOk.y - 25f, sizeOk.x, sizeOk.y), StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			result = true;
			BackToScene();
			SelfRespawn();
		}
		GUI.enabled = enabled2;
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
			BackToScene();
		}
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	private void BackToScene()
	{
		GlobalVars.Instance.SetForceClosed(set: false);
		GlobalVars.Instance.tutorFirstScriptOn = true;
		RoomManager.Instance.ClearVote();
	}

	private void SelfRespawn()
	{
		verifyLocalController();
		if (localCtrl != null)
		{
			localCtrl.DropWeaponSkipSetting();
			localCtrl.GetHit(MyInfoManager.Instance.Seq, 1000, 1f, -10, -1, autoHealPossible: true, checkZombie: false);
			localCtrl.SelfRespawnReuseTime = Time.time;
		}
	}

	private void verifyLocalController()
	{
		if (!(localCtrl != null))
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				localCtrl = gameObject.GetComponent<LocalController>();
			}
		}
	}
}
