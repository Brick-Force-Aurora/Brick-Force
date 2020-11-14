using System;
using UnityEngine;

[Serializable]
public class BattleGuideDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UILabel reloadText;

	public UILabel mouseText;

	public UILabel speedUpText;

	public UIToggle toggle;

	public UIMyButton ok;

	public bool DontShowThisMessageAgain => true;//toggle.toggle;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.BATTLE_GUIDE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		int num = custom_inputs.Instance.KeyIndex("K_RELOAD");
		string text = string.Format(StringMgr.Instance.Get(reloadText.textKey), custom_inputs.Instance.InputKey[num].ToString());
		reloadText.SetText(text);
		num = custom_inputs.Instance.KeyIndex("K_FORWARD");
		text = string.Format(StringMgr.Instance.Get(speedUpText.textKey), custom_inputs.Instance.InputKey[num].ToString());
		speedUpText.SetText(text);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		imgList.Draw();
		labelList.Draw();
		reloadText.Draw();
		mouseText.Draw();
		toggle.Draw();
		ok.Draw();
		if (BuildOption.Instance.Props.useDefaultDash)
		{
			speedUpText.Draw();
		}
		if (ok.isClick())
		{
			if (DontShowThisMessageAgain)
			{
				MyInfoManager.Instance.SaveDonotCommonMask(MyInfoManager.COMMON_OPT.DONOT_BATTLE_GUIDE);
			}
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
