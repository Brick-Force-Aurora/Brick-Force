using System;
using UnityEngine;

[Serializable]
public class AccusationDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UITextArea textDetail;

	public UIMyButton ok;

	public UIMyButton exit;

	public UITextFiled userNameEdit;

	public UIComboBox userNameCombo;

	public UIComboBox reasonCombo;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.ACCUSATION;
		userNameCombo.parentSize = size;
		reasonCombo.parentSize = size;
		reasonCombo.dependentComboBox = userNameCombo;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		userNameCombo.list = null;
		userNameEdit.ResetText();
		reasonCombo.ResetList();
		textDetail.ResetText();
	}

	public void InitDialog(string[] users)
	{
		userNameCombo.list = users;
		userNameCombo.ResetList();
		reasonCombo.ResetList();
		textDetail.ResetText();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		bool enabled = GUI.enabled;
		if (userNameCombo.IsClickedComboButton() || reasonCombo.IsClickedComboButton())
		{
			GUI.enabled = false;
		}
		imgList.Draw();
		labelList.Draw();
		if (userNameCombo.list == null)
		{
			userNameEdit.Draw();
		}
		textDetail.Draw();
		ok.Draw();
		exit.Draw();
		GUI.enabled = enabled;
		reasonCombo.Draw();
		userNameCombo.Draw();
		if (ok.isClick())
		{
			if (IsRightName())
			{
				if (IsRightReason())
				{
					if (textDetail.GetInputText().Length > 0)
					{
						CSNetManager.Instance.Sock.SendCS_ACCUSE_PLAYER_REQ(reasonCombo.Select - 1, GetUserName(), textDetail.GetInputText());
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
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_USER"));
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

	private bool IsRightName()
	{
		if (userNameCombo.list == null)
		{
			if (userNameEdit.GetInputText().Length == 0)
			{
				return false;
			}
			return true;
		}
		return userNameCombo.list.Length <= 1 || userNameCombo.Select != 0;
	}

	private string GetUserName()
	{
		if (userNameCombo.list == null)
		{
			return userNameEdit.GetInputText();
		}
		return userNameCombo.GetSelectString();
	}
}
