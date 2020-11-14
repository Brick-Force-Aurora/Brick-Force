using System;
using UnityEngine;

[Serializable]
public class TutorPopupDlg2 : Dialog
{
	public string[] itemCodes = new string[6]
	{
		"a03",
		"a15",
		"a02",
		"a19",
		"a20",
		"a08"
	};

	private Vector2 crdStartBtn = new Vector2(47f, 92f);

	private Rect crdExpl = new Rect(22f, 336f, 589f, 105f);

	private Rect crdOk = new Rect(414f, 458f, 100f, 34f);

	private Rect crdCancel = new Rect(524f, 458f, 100f, 34f);

	private int sel = -1;

	private TItem selTItem;

	private Rect crdSel;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.TUTOR_Q_POPUP2;
	}

	public override void OnPopup()
	{
		size.x = 634f;
		size.y = 502f;
		sel = 0;
		selTItem = TItemManager.Instance.Get<TItem>(itemCodes[0]);
		crdSel = new Rect(crdStartBtn.x - 1f, crdStartBtn.y - 1f, 169f, 92f);
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public override bool DoDialog()
	{
		bool result = false;
		LabelUtil.TextOut(new Vector2(20f, 30f), StringMgr.Instance.Get("TUTO_MAPEDIT_STARTPUP"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 390f);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < itemCodes.Length; i++)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(itemCodes[i]);
			if (tItem != null)
			{
				Rect rc = new Rect(crdStartBtn.x + (float)num, crdStartBtn.y + (float)num2, 167f, 91f);
				if (GlobalVars.Instance.MyButton(rc, tItem.CurIcon(), new GUIContent(string.Empty, string.Empty), "BtnItemTuto"))
				{
					sel = i;
					selTItem = tItem;
					crdSel = new Rect(rc.x - 1f, rc.y - 1f, 169f, 92f);
				}
			}
			num = ((i != 2) ? (num + 187) : 0);
			if (i == 2)
			{
				num2 += 107;
			}
		}
		GUI.Box(crdExpl, string.Empty, "BoxInnerLine");
		if (sel >= 0 && selTItem != null)
		{
			float x = crdExpl.x + 15f;
			float num3 = crdExpl.y + 15f;
			LabelUtil.TextOut(new Vector2(x, num3), selTItem.Name, "Label", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
			LabelUtil.TextOut(new Vector2(x, num3 + 30f), StringMgr.Instance.Get(selTItem.comment), "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft, 200f);
			GUI.Box(crdSel, string.Empty, "ViewSelected");
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			GlobalVars.Instance.isLoadBattleTutor = true;
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
			if (!Application.CanStreamedLevelBeLoaded("Lobby"))
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("STREAMING_WAIT"));
			}
			else
			{
				Channel bestPlayChannel = ChannelManager.Instance.GetBestPlayChannel();
				if (bestPlayChannel == null)
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
					Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.LOBBY, bestPlayChannel.Id);
				}
			}
			result = true;
		}
		Rect rc2 = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc2, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		return result;
	}
}
