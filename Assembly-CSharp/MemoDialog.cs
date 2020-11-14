using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MemoDialog : Dialog
{
	private Vector2 memoSize = new Vector2(253f, 38f);

	public Texture2D attached;

	public Texture2D invited;

	public string[] tabKey;

	public int maxId = 5;

	public int maxTitle = 30;

	public int maxMemoLength = 500;

	private int curTab;

	private Vector2 spList = Vector2.zero;

	private string[] tab;

	private string receiver = string.Empty;

	private string title = string.Empty;

	private string contents = string.Empty;

	private string focusingControlName = string.Empty;

	public Texture2D icon;

	private Vector2 crdTitleSize = new Vector2(260f, 26f);

	public Rect crdIcon = new Rect(0f, 0f, 12f, 12f);

	public Vector2 crdTitle = new Vector2(0f, 0f);

	private Rect crdTab = new Rect(14f, 53f, 324f, 27f);

	private Rect crdOutline = new Rect(11f, 79f, 330f, 430f);

	public Rect crdMemoList = new Rect(26f, 94f, 300f, 390f);

	private Rect crdSelectAll = new Rect(26f, 78f, 21f, 22f);

	public Vector2 crdReceiverLabel = new Vector2(103f, 87f);

	private Rect crdReceiverTxtFld = new Rect(113f, 90f, 211f, 20f);

	public Vector2 crdTitleLabel = new Vector2(103f, 117f);

	private Rect crdTitleTxtFld = new Rect(113f, 120f, 211f, 20f);

	private Rect crdSendBtn = new Rect(95f, 518f, 140f, 34f);

	private Rect crdGet = new Rect(105f, 518f, 120f, 34f);

	private Rect crdDelete = new Rect(225f, 518f, 120f, 34f);

	private Vector2 crdTotMemo = new Vector2(22f, 520f);

	public Rect crdCloseBtn = new Rect(243f, 530f, 90f, 26f);

	public Rect crdMemoTextArea = new Rect(30f, 155f, 280f, 330f);

	private Color txtMainColor;

	private int selectedMemo = -1;

	private Rect crdSelected = new Rect(0f, 0f, 0f, 0f);

	private bool selectAll;

	private bool msgfullDlgOpened;

	private List<Memo> memoDelList;

	public List<Memo> MemoDelList => memoDelList;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.MEMO;
		tab = new string[tabKey.Length];
		memoDelList = new List<Memo>();
	}

	public override void OnPopup()
	{
		rc = new Rect(GlobalVars.Instance.ScreenRect.width / 2f - size.x - 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
		for (int i = 0; i < tabKey.Length; i++)
		{
			tab[i] = StringMgr.Instance.Get(tabKey[i]);
		}
		txtMainColor = GlobalVars.Instance.txtMainColor;
		selectedMemo = -1;
		selectAll = false;
		msgfullDlgOpened = false;
		memoDelList.Clear();
		MemoAllCheckUnCheck(bCheck: false);
	}

	private void MemoAllCheckUnCheck(bool bCheck)
	{
		Memo[] array = MemoManager.Instance.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].check = bCheck;
		}
	}

	private bool IsCheckedMemo()
	{
		Memo[] array = MemoManager.Instance.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].check)
			{
				return true;
			}
		}
		return false;
	}

	public void ReplyTitle(string rcv)
	{
		curTab = 1;
		receiver = rcv;
		if (title.Length > maxTitle)
		{
			title = title.Remove(maxTitle);
		}
		focusingControlName = "MemoTitleInput";
	}

	public void Reply(string rcv, string ttl)
	{
		curTab = 1;
		receiver = rcv;
		title = ttl;
		if (title.Length > maxTitle)
		{
			title = title.Remove(maxTitle);
		}
		focusingControlName = "MemoContentsInput";
	}

	private float DoMemo(int index, Memo memo, Vector2 pos)
	{
		GUI.Box(new Rect(25f, pos.y, 38f, 38f), string.Empty, "BoxInnerLine2");
		pos.x += 25f;
		if (GlobalVars.Instance.MyButton(new Rect(pos.x, pos.y, memoSize.x, memoSize.y), string.Empty, "BtnBlue"))
		{
			((RecvMemoDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.RECV_MEMO, exclusive: false))?.InitDialog(new Vector2(rc.x + size.x + 2f, rc.y), memo, this);
			selectedMemo = index;
		}
		if (index == selectedMemo)
		{
			crdSelected = new Rect(pos.x - 1f, pos.y - 1f, memoSize.x + 2f, memoSize.y + 2f);
		}
		memo.check = GUI.Toggle(new Rect(0f, pos.y + 3f, 21f, 22f), memo.check, string.Empty);
		Color clrText = Color.white;
		if (memo.IsRead)
		{
			clrText = Color.grey;
		}
		LabelUtil.TextOut(new Vector2(pos.x + 5f, pos.y + 2f), memo.sender, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		if (memo.attached.Length == 3 && memo.option > 0)
		{
			if (memo.attached == "000")
			{
				TextureUtil.DrawTexture(new Rect(34f, pos.y + 9f, 20f, 20f), invited);
			}
			else
			{
				TextureUtil.DrawTexture(new Rect(34f, pos.y + 9f, 20f, 20f), attached);
			}
		}
		LabelUtil.TextOut(new Vector2(pos.x + memoSize.x - 22f, pos.y + 2f), memo.SendDate, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
		if (memo.needStringKey())
		{
			string text = GlobalVars.Instance.DelimiterProcess(memo.title);
			LabelUtil.TextOut(new Vector2(pos.x + 5f, pos.y + memoSize.y - 2f), crdTitleSize, text, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.LowerLeft);
		}
		else
		{
			LabelUtil.TextOut(new Vector2(pos.x + 5f, pos.y + memoSize.y - 2f), crdTitleSize, memo.title, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.LowerLeft);
		}
		return pos.y + memoSize.y + 1f;
	}

	public void NextMemoReceiceItem()
	{
		if (memoDelList.Count != 0)
		{
			Memo memo = memoDelList[0];
			if (memo.attached != "000")
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(memo.attached);
				if (tItem != null && null != tItem.CurIcon())
				{
					CSNetManager.Instance.Sock.SendCS_RCV_PRESENT_REQ(memo.seq, memo.attached, memo.option, tItem.IsAmount);
					memoDelList.RemoveAt(0);
				}
			}
			if (memoDelList.Count == 0)
			{
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("MEMO_BOX_WARING03"));
			}
		}
	}

	public void NextMemoDelete()
	{
		if (memoDelList.Count != 0)
		{
			Memo memo = memoDelList[0];
			if (memo.attached != "000")
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(memo.attached);
				if (tItem != null && null != tItem.CurIcon())
				{
					memoDelList.Clear();
					SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("MEMO_BOX_WARING02"));
				}
				else
				{
					CSNetManager.Instance.Sock.SendCS_DEL_MEMO_REQ(memo.seq);
					memoDelList.RemoveAt(0);
				}
			}
			else
			{
				CSNetManager.Instance.Sock.SendCS_DEL_MEMO_REQ(memo.seq);
				memoDelList.RemoveAt(0);
			}
		}
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 15f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("MEMO_BOX"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		GUI.Box(crdOutline, string.Empty, "BoxPopLine");
		int num = curTab;
		curTab = GUI.SelectionGrid(crdTab, curTab, tab, 2, "PopTab");
		if (curTab == 0)
		{
			Memo[] array = MemoManager.Instance.ToArray();
			if (array.Length <= 0)
			{
				selectedMemo = -1;
			}
			else if (array.Length <= selectedMemo)
			{
				selectedMemo = array.Length - 1;
			}
			Vector2 zero = Vector2.zero;
			zero.x = 37f;
			Rect viewRect = new Rect(0f, 0f, memoSize.x, (memoSize.y + 1f) * (float)array.Length);
			bool flag = selectAll;
			selectAll = GUI.Toggle(crdSelectAll, selectAll, StringMgr.Instance.Get("MEMO_BOX_SELECT_ALL"));
			if (flag != selectAll)
			{
				MemoAllCheckUnCheck(selectAll);
			}
			spList = GUI.BeginScrollView(crdMemoList, spList, viewRect);
			for (int i = 0; i < array.Length; i++)
			{
				zero.y = DoMemo(i, array[i], zero);
			}
			if (selectedMemo >= 0)
			{
				GUI.Box(crdSelected, string.Empty, "BtnBlueF");
			}
			GUI.EndScrollView();
			GUI.enabled = (IsCheckedMemo() ? true : false);
			if (GlobalVars.Instance.MyButton(crdGet, StringMgr.Instance.Get("RECEIVING"), "BtnAction"))
			{
				bool flag2 = true;
				Memo memo = null;
				bool isAmount = false;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].check)
					{
						if (array[j].attached != "000")
						{
							TItem tItem = TItemManager.Instance.Get<TItem>(array[j].attached);
							if (tItem != null && null != tItem.CurIcon())
							{
								if (flag2)
								{
									flag2 = false;
									memo = array[j];
									isAmount = tItem.IsAmount;
								}
								memoDelList.Add(array[j]);
							}
							else
							{
								array[j].check = false;
							}
						}
						else
						{
							array[j].check = false;
						}
					}
				}
				if (memo != null)
				{
					CSNetManager.Instance.Sock.SendCS_RCV_PRESENT_REQ(memo.seq, memo.attached, memo.option, isAmount);
					memoDelList.RemoveAt(0);
				}
			}
			GUI.enabled = true;
			GUI.enabled = (IsCheckedMemo() ? true : false);
			if (GlobalVars.Instance.MyButton(crdDelete, StringMgr.Instance.Get("DELETE"), "BtnAction") && IsCheckedMemo())
			{
				bool flag3 = true;
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k].check)
					{
						if (!(array[k].attached != "000"))
						{
							break;
						}
						TItem tItem2 = TItemManager.Instance.Get<TItem>(array[k].attached);
						if (tItem2 == null || !(null != tItem2.CurIcon()))
						{
							break;
						}
						flag3 = false;
						SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("MEMO_BOX_WARING02"));
					}
				}
				bool flag4 = true;
				long num2 = -1L;
				if (flag3)
				{
					for (int l = 0; l < array.Length; l++)
					{
						if (array[l].check)
						{
							memoDelList.Add(array[l]);
							if (flag4)
							{
								flag4 = false;
								num2 = array[l].seq;
							}
						}
					}
					if (num2 >= 0)
					{
						CSNetManager.Instance.Sock.SendCS_DEL_MEMO_REQ(num2);
						memoDelList.RemoveAt(0);
					}
				}
			}
			GUI.enabled = true;
			Color clrText = Color.white;
			if (array.Length >= 80)
			{
				clrText = Color.red;
			}
			LabelUtil.TextOut(crdTotMemo, array.Length.ToString() + " / 100", "Label", clrText, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			LabelUtil.TextOut(crdReceiverLabel, StringMgr.Instance.Get("MEMO_RCV"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			string text = receiver;
			GUI.SetNextControlName("MemoReceiverInput");
			receiver = GUI.TextField(crdReceiverTxtFld, receiver);
			if (receiver.Length > maxId)
			{
				receiver = text;
			}
			LabelUtil.TextOut(crdTitleLabel, StringMgr.Instance.Get("MEMO_TITLE"), "Label", txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			string text2 = title;
			GUI.SetNextControlName("MemoTitleInput");
			title = GUI.TextField(crdTitleTxtFld, title);
			if (title.Length > maxTitle)
			{
				title = text2;
			}
			GUI.SetNextControlName("MemoContentsInput");
			contents = GUI.TextArea(crdMemoTextArea, contents, maxMemoLength);
			if (GlobalVars.Instance.MyButton(crdSendBtn, StringMgr.Instance.Get("SEND_MEMO"), "BtnAction"))
			{
				receiver.Trim();
				title.Trim();
				if (receiver.Length <= 0)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MEMO_RECEIVER"));
				}
				else if (title.Length <= 0)
				{
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_MEMO_TITLE"));
				}
				else
				{
					string a = MyInfoManager.Instance.Nickname.ToLower();
					string b = receiver.ToLower();
					if (a == b)
					{
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANNOT_SELF_MEMO"));
					}
					else
					{
						CSNetManager.Instance.Sock.SendCS_SEND_MEMO_REQ(receiver, title, contents);
						result = true;
					}
				}
			}
			if (focusingControlName.Length > 0)
			{
				Dialog top = DialogManager.Instance.GetTop();
				if (top != null && top.ID == id)
				{
					GUI.FocusControl(focusingControlName);
					focusingControlName = string.Empty;
				}
			}
		}
		if (num == 0 && curTab != 0 && receiver.Length <= 0 && title.Length <= 0 && contents.Length <= 0)
		{
			focusingControlName = "MemoReceiverInput";
		}
		Rect rc = new Rect(size.x - 50f, 10f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
		{
			result = true;
		}
		GUI.skin = skin;
		if (MemoManager.Instance.GetMemoCountPercent() >= 100 && !msgfullDlgOpened)
		{
			msgfullDlgOpened = true;
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("MEMO_BOX_WARING01"));
		}
		return result;
	}

	public void InitDialog()
	{
		receiver = string.Empty;
		title = string.Empty;
		contents = string.Empty;
		curTab = 0;
		spList = Vector2.zero;
	}
}
