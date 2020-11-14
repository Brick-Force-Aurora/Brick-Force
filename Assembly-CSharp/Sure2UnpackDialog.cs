using System;
using UnityEngine;

[Serializable]
public class Sure2UnpackDialog : Dialog
{
	private Vector2 crdTitle = new Vector2(15f, 7f);

	private Rect crdSureOutline = new Rect(15f, 50f, 482f, 150f);

	private Rect crdSure = new Rect(15f, 255f, 482f, 74f);

	private Rect crdYes = new Rect(195f, 316f, 150f, 34f);

	private Rect crdNo = new Rect(350f, 316f, 150f, 34f);

	private Vector2 crdIcon = new Vector2(37f, 78f);

	private Vector2 crdExtra = new Vector2(487f, 60f);

	private float unpackOffset = 14f;

	private Item item;

	private TBundle tBundle;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.SURE2UNPACK;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(Item _item, TBundle _bundle)
	{
		item = _item;
		tBundle = _bundle;
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.Box(crdSureOutline, string.Empty, "LineBoxBlue");
		LabelUtil.TextOut(crdTitle, StringMgr.Instance.Get("UNPACK_BUNDLE"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		TextureUtil.DrawTexture(new Rect(crdIcon.x, crdIcon.y, (float)tBundle.CurIcon().width, (float)tBundle.CurIcon().height), tBundle.CurIcon());
		BundleUnit[] array = BundleManager.Instance.Unpack(tBundle.code);
		if (array != null)
		{
			Vector2 pos = crdExtra;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].tItem != null)
				{
					string empty = string.Empty;
					if (array[i].opt >= 1000000)
					{
						empty = StringMgr.Instance.Get("INFINITE");
					}
					else
					{
						empty = array[i].opt.ToString();
						empty = ((!array[i].tItem.IsAmount) ? (empty + StringMgr.Instance.Get("DAYS")) : (empty + StringMgr.Instance.Get("TIMES_UNIT")));
					}
					LabelUtil.TextOut(pos, array[i].tItem.Name + "/" + empty, "MiniLabel", new Color(1f, 1f, 0f), GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
					pos.y += unpackOffset;
				}
			}
		}
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			Vector2 pos2 = new Vector2(size.x / 2f, 210f);
			LabelUtil.TextOut(pos2, StringMgr.Instance.Get("PURCHASE_POLICY_02"), "MissionLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter, 360f);
			Rect rc = new Rect(size.x - 150f, 240f, 120f, 34f);
			if (GlobalVars.Instance.MyButton(rc, StringMgr.Instance.Get("PURCHASE_POLICY_BUTTON"), "BtnAction"))
			{
				if (MyInfoManager.Instance.SiteCode == 1)
				{
					Application.OpenURL("http://helpdesk.netmarble.net/HelpStipulation.asp#ach13");
				}
				else if (MyInfoManager.Instance.SiteCode == 11)
				{
					Application.OpenURL("http://www.tooniland.com/common/html/serviceRules.jsp#");
				}
			}
		}
		GUI.Label(crdSure, StringMgr.Instance.Get("SURE2UNPACK"), "MiddleCenterLabel");
		if (GlobalVars.Instance.MyButton(crdYes, StringMgr.Instance.Get("YES"), "BtnAction"))
		{
			CSNetManager.Instance.Sock.SendCS_UNPACK_BUNDLE_REQ(item.Seq, tBundle.code);
			result = true;
		}
		if (GlobalVars.Instance.MyButton(crdNo, StringMgr.Instance.Get("NO"), "BtnAction"))
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
