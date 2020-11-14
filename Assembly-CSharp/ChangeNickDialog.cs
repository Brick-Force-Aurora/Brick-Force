using System;
using System.Text;
using UnityEngine;

[Serializable]
public class ChangeNickDialog : Dialog
{
	private Vector2 crdNickCreateProcessComment = new Vector2(24f, 100f);

	private Rect crdNickName = new Rect(80f, 60f, 290f, 28f);

	private Rect crdCheckAvailability = new Rect(170f, 126f, 120f, 34f);

	private Rect crdCancel = new Rect(300f, 126f, 120f, 34f);

	public int maxNickName = 10;

	private string nickName = string.Empty;

	private bool nickNameIsAvailable;

	private string availableName = string.Empty;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.CHANGE_NICK;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		nickName = string.Empty;
	}

	private bool CheckInput()
	{
		nickName = nickName.Trim();
		if (nickName.Length <= 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_NICKNAME"));
			return false;
		}
		if (nickName.Length < 2)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TOO_SHORT_NICKNAME"));
			return false;
		}
		string text = WordFilter.Instance.CheckBadword(nickName);
		if (text.Length > 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), text));
			return false;
		}
		return true;
	}

	public void SetNickNameAvailability(string clanName, bool available)
	{
		nickNameIsAvailable = available;
		availableName = clanName;
	}

	private void DoNickName()
	{
		GUI.SetNextControlName("NickName");
		nickName = GUI.TextField(crdNickName, nickName, maxNickName);
		nickName = nickName.Replace(" ", string.Empty);
		nickName = nickName.Replace("\t", string.Empty);
		nickName = nickName.Replace("\n", string.Empty);
		if (BuildOption.Instance.IsAxeso5)
		{
			nickName = RemoveSpecialCharacters(nickName);
		}
		if (GlobalVars.Instance.MyButton(crdCheckAvailability, StringMgr.Instance.Get("BTN_CHANGE"), "BtnAction") && CheckInput())
		{
			TItem specialItem2HaveFunction = TItemManager.Instance.GetSpecialItem2HaveFunction("nick_name");
			if (specialItem2HaveFunction != null)
			{
				long num = MyInfoManager.Instance.HaveFunction("nick_name");
				if (num >= 0)
				{
					CSNetManager.Instance.Sock.SendCS_USE_CHANGE_NICKNAME_REQ(nickName, num, specialItem2HaveFunction.code);
				}
			}
		}
		string text = StringMgr.Instance.Get("INPUT_NICKNAME");
		if (availableName.Length > 0)
		{
			text = ((!nickNameIsAvailable) ? string.Format(StringMgr.Instance.Get("NAME_IS_NOT_AVAILABLE"), availableName) : string.Format(StringMgr.Instance.Get("NAME_IS_AVAILABLE"), availableName));
		}
		Dialog top = DialogManager.Instance.GetTop();
		if (top != null && top.ID == id)
		{
			GUI.FocusWindow((int)id);
			GUI.FocusControl("NickName");
		}
		LabelUtil.TextOut(crdNickCreateProcessComment, text, "Label", new Color(0.87f, 0.63f, 0.32f, 1f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private string RemoveSpecialCharacters(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in input)
		{
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		DoNickName();
		if (GlobalVars.Instance.MyButton(crdCancel, StringMgr.Instance.Get("CANCEL"), "BtnAction"))
		{
			result = true;
		}
		Rect rc = new Rect(size.x - 44f, 5f, 34f, 34f);
		if (GlobalVars.Instance.MyButton(rc, string.Empty, "BtnClose") || GlobalVars.Instance.IsEscapePressed())
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
}
