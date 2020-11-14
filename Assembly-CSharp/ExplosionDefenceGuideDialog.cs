using System;
using UnityEngine;

[Serializable]
public class ExplosionDefenceGuideDialog : Dialog
{
	private Rect crdMsg = new Rect(25f, 406f, 632f, 80f);

	public UIImageList imgList;

	public UILabelList labelList;

	public UIToggle toggle;

	public UIMyButton ok;

	public bool DontShowThisMessageAgain => toggle.toggle;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.EXPLOSION_DEFENCE_GUIDE;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		string text = string.Format(StringMgr.Instance.Get("GUIDE_EXPLOSION_DEFENCE04"), custom_inputs.Instance.GetKeyCodeName("K_ACTION"));
		GUI.Label(crdMsg, text, "UpperLeftLabel");
		imgList.Draw();
		labelList.Draw();
		toggle.Draw();
		ok.Draw();
		if (ok.isClick())
		{
			if (DontShowThisMessageAgain)
			{
				MyInfoManager.Instance.SaveDonotCommonMask(MyInfoManager.COMMON_OPT.DONOT_EXPLOSION_DEFENCE_GUIDE);
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
