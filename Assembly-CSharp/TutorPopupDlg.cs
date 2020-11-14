using System;
using UnityEngine;

[Serializable]
public class TutorPopupDlg : Dialog
{
	public Texture2D iconCompensation;

	private Rect crdOk = new Rect(230f, 222f, 90f, 34f);

	private Rect crdCancel = new Rect(330f, 222f, 90f, 34f);

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TUTOR_Q_POPUP;
	}

	public override void OnPopup()
	{
		size.x = 430f;
		size.y = 266f;
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		LabelUtil.TextOut(new Vector2(20f, 30f), StringMgr.Instance.Get("TUTO_MAPEDIT_STARTPUP02"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 390f);
		TextureUtil.DrawTexture(new Rect(22f, 116f, (float)iconCompensation.width, (float)iconCompensation.height), iconCompensation);
		Good good = ShopManager.Instance.Get("a71");
		if (good != null)
		{
			int num = 22 + iconCompensation.width + 20;
			int num2 = 116;
			LabelUtil.TextOut(new Vector2((float)num, (float)num2), good.tItem.Name, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(new Vector2((float)num, (float)(num2 + 25)), StringMgr.Instance.Get(good.tItem.comment), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 200f);
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			GlobalVars.Instance.isLoadBattleTutor = false;
			if (!Application.CanStreamedLevelBeLoaded("Lobby") || !Application.CanStreamedLevelBeLoaded("BattleTutor"))
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
			else
			{
				Channel tutorialableChannel = ChannelManager.Instance.GetTutorialableChannel();
				if (tutorialableChannel == null)
				{
					if (ChannelManager.Instance.IsLastError())
					{
						MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
					}
					else
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
					}
				}
				else
				{
					Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.BATTLE_TUTOR, tutorialableChannel.Id);
				}
			}
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdCancel, StringMgr.Instance.Get("CANCEL"), "BtnAction"))
		{
			if (Application.loadedLevelName.Contains("BfStart"))
			{
				if (!Application.CanStreamedLevelBeLoaded("Lobby"))
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
				}
				else
				{
					Channel bestBuildChannel = ChannelManager.Instance.GetBestBuildChannel();
					if (bestBuildChannel == null)
					{
						if (ChannelManager.Instance.IsLastError())
						{
							MessageBoxMgr.Instance.AddMessage(ChannelManager.Instance.GetBestChannelLastError());
						}
						else
						{
							MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SERVICE_CROWDED"));
						}
					}
					else
					{
						Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, bestBuildChannel.Id);
					}
				}
			}
			else
			{
				Application.LoadLevel("BfStart");
			}
			result = true;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
			if (!Application.loadedLevelName.Contains("BfStart"))
			{
				Application.LoadLevel("BfStart");
			}
		}
		return result;
	}
}
