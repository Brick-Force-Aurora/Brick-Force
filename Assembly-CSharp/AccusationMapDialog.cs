using System;
using UnityEngine;

[Serializable]
public class AccusationMapDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIRegMap regMap;

	public UILabel mapName;

	public UITextArea textDetail;

	public UIMyButton ok;

	public UIMyButton exit;

	public UIComboBox reasonCombo;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ACCUSATION_MAP;
		reasonCombo.parentSize = size;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(RegMap reg)
	{
		regMap.SetRegMap(reg);
		mapName.SetText(reg.Alias);
		reasonCombo.ResetList();
		textDetail.ResetText();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		bool enabled = GUI.enabled;
		if (reasonCombo.IsClickedComboButton())
		{
			GUI.enabled = false;
		}
		imgList.Draw();
		labelList.Draw();
		regMap.Draw();
		mapName.Draw();
		textDetail.Draw();
		ok.Draw();
		exit.Draw();
		GUI.enabled = enabled;
		reasonCombo.Draw();
		if (ok.isClick())
		{
			if (IsRightReason())
			{
				if (textDetail.GetInputText().Length > 0)
				{
					CSNetManager.Instance.Sock.SendCS_ACCUSE_MAP_REQ(reasonCombo.Select - 1, regMap.GetRegMapId(), textDetail.GetInputText());
					result = true;
				}
				else
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_04"));
				}
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_REASON_SELECT"));
			}
		}
		if (exit.isClick())
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

	private bool IsRightReason()
	{
		return reasonCombo.Select != 0;
	}
}
