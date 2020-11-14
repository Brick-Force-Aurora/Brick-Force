using System.Text;
using UnityEngine;

public class PlayerInfoMain : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.BOTTOM;

	private Rect crdGuide = new Rect(0f, 346f, 1024f, 88f);

	private Rect crdNickname = new Rect(252f, 473f, 256f, 26f);

	private Rect crdOk = new Rect(252f, 515f, 256f, 34f);

	public int maxNickname = 10;

	public int minNickname = 2;

	private string nickName = string.Empty;

	private AreYouSure areYouSure;

	public bool IsCreating
	{
		set
		{
			areYouSure.Yes = value;
		}
	}

	private void Start()
	{
		areYouSure = null;
	}

	private bool CheckInput()
	{
		nickName = nickName.Trim();
		if (nickName.Length <= 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("INPUT_NICKNAME"));
			return false;
		}
		if (nickName.Length < minNickname)
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

	private void OnGUI()
	{
		GUI.depth = (int)guiDepth;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.enabled = !DialogManager.Instance.IsModal;
		GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty);
		GUI.BeginGroup(new Rect((float)((Screen.width - 760) / 2), (float)((Screen.height - 600) / 2), 760f, 600f));
		crdGuide.x = (760f - crdGuide.width) / 2f;
		crdNickname.x = (760f - crdNickname.width) / 2f;
		crdOk.x = (760f - crdOk.width) / 2f;
		string text = string.Format(StringMgr.Instance.Get("GUIDE_PLAYER_INFO"), minNickname, maxNickname);
		GUI.Label(crdGuide, text, "MiddleCenterLabel");
		nickName = GUI.TextField(crdNickname, nickName, maxNickname);
		nickName = nickName.Replace(" ", string.Empty);
		nickName = nickName.Replace("\t", string.Empty);
		nickName = nickName.Replace("\n", string.Empty);
		if (BuildOption.Instance.IsAxeso5)
		{
			nickName = RemoveSpecialCharacters(nickName);
		}
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction") && CheckInput() && (areYouSure == null || !areYouSure.Yes))
		{
			areYouSure = (AreYouSure)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ARE_YOU_SURE, exclusive: true);
			if (areYouSure != null)
			{
				areYouSure.InitDialog(AreYouSure.SURE.CREATE_CHARACTER, nickName);
			}
		}
		GUI.EndGroup();
		GUI.enabled = true;
	}

	private void Update()
	{
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
}
