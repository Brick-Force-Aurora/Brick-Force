using System;
using UnityEngine;

[Serializable]
public class ReplaceDstDialog : Dialog
{
	public string[] brickTabKey;

	public TooltipBrick tooltip;

	private string[] brickTab;

	private Brick[] general;

	private Texture[] generalIcon;

	private Brick[] colorbox;

	private Texture[] colorboxIcon;

	private Brick[] accessory;

	private Texture[] accessoryIcon;

	private Brick[] functional;

	private Texture[] functionalIcon;

	private Rect crdSiloArea = new Rect(5f, 48f, 666f, 361f);

	private Rect crdSiloFrame = new Rect(0f, 25f, 666f, 336f);

	private Vector2 crdSilo = new Vector2(64f, 64f);

	private Vector2 crdSiloOffset = new Vector2(1f, 2f);

	private Rect crdSiloTab = new Rect(5f, 0f, 656f, 25f);

	private Rect crdSiloPosition = new Rect(4f, 29f, 660f, 328f);

	private Rect crdOk = new Rect(540f, 412f, 128f, 34f);

	private int currentSilo;

	private Brick currentBrick;

	private Vector2 spGeneral = Vector2.zero;

	private Vector2 spColor = Vector2.zero;

	private Vector2 spDeco = Vector2.zero;

	private Vector2 spFunc = Vector2.zero;

	private ReplaceToolDialog replaceToolDlg;

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.REPLACE_DST;
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog(ReplaceToolDialog dlg)
	{
		replaceToolDlg = dlg;
		currentBrick = dlg.Next;
		brickTab = new string[brickTabKey.Length];
		for (int i = 0; i < brickTabKey.Length; i++)
		{
			brickTab[i] = StringMgr.Instance.Get(brickTabKey[i]);
		}
		general = BrickManager.Instance.ToReplaceBrickArray(Brick.CATEGORY.GENERAL);
		generalIcon = BrickManager.Instance.ToReplaceBrickIconArray(Brick.CATEGORY.GENERAL);
		colorbox = BrickManager.Instance.ToReplaceBrickArray(Brick.CATEGORY.COLORBOX);
		colorboxIcon = BrickManager.Instance.ToReplaceBrickIconArray(Brick.CATEGORY.COLORBOX);
		accessory = BrickManager.Instance.ToReplaceBrickArray(Brick.CATEGORY.ACCESSORY);
		accessoryIcon = BrickManager.Instance.ToReplaceBrickIconArray(Brick.CATEGORY.ACCESSORY);
		functional = BrickManager.Instance.ToReplaceBrickArray(Brick.CATEGORY.FUNCTIONAL);
		functionalIcon = BrickManager.Instance.ToReplaceBrickIconArray(Brick.CATEGORY.FUNCTIONAL);
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		Vector2 pos = new Vector2(size.x / 2f, 10f);
		LabelUtil.TextOut(pos, StringMgr.Instance.Get("CHG_REPLACE_DST"), "BigLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter);
		DoSilo();
		if (GlobalVars.Instance.MyButton(crdOk, StringMgr.Instance.Get("OK"), "BtnAction"))
		{
			if (replaceToolDlg.Prev.seq == currentBrick.seq)
			{
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("CAN_NOT_REPLACE_SAME_BRICK"));
			}
			else
			{
				replaceToolDlg.Next = currentBrick;
				result = true;
			}
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

	private void DoSilo()
	{
		GUI.BeginGroup(crdSiloArea);
		GUI.Box(crdSiloFrame, string.Empty, "BoxBrickBar");
		currentSilo = GUI.SelectionGrid(crdSiloTab, currentSilo, brickTab, brickTab.Length, "BoxBrickTab");
		switch (currentSilo)
		{
		case 0:
		{
			int num7 = Mathf.FloorToInt((float)(generalIcon.Length / 10));
			if (generalIcon.Length % 10 > 0)
			{
				num7++;
			}
			spGeneral = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num7 * crdSilo.y + (float)(num7 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spGeneral);
			int num8 = 0;
			for (int num9 = 0; num9 < num7; num9++)
			{
				for (int num10 = 0; num10 < 10; num10++)
				{
					if (num8 >= generalIcon.Length)
					{
						break;
					}
					Rect rect4 = new Rect((float)num10 * (crdSilo.x + crdSiloOffset.x), (float)num9 * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					if (GlobalVars.Instance.MyButton(rect4, generalIcon[num8], new GUIContent(string.Empty, "s" + general[num8].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = general[num8];
					}
					if (currentBrick != null && currentBrick.GetIndex() == general[num8].GetIndex())
					{
						GUI.Box(rect4, string.Empty, "BoxBrickSel");
					}
					num8++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 1:
		{
			int num5 = Mathf.FloorToInt((float)(colorbox.Length / 10));
			if (colorbox.Length % 10 > 0)
			{
				num5++;
			}
			spColor = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num5 * crdSilo.y + (float)(num5 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spColor);
			int num6 = 0;
			for (int m = 0; m < num5; m++)
			{
				for (int n = 0; n < 10; n++)
				{
					if (num6 >= colorbox.Length)
					{
						break;
					}
					Rect rect3 = new Rect((float)n * (crdSilo.x + crdSiloOffset.x), (float)m * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					if (GlobalVars.Instance.MyButton(rect3, colorboxIcon[num6], new GUIContent(string.Empty, "s" + colorbox[num6].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = colorbox[num6];
					}
					if (currentBrick != null && currentBrick.GetIndex() == colorbox[num6].GetIndex())
					{
						GUI.Box(rect3, string.Empty, "BoxBrickSel");
					}
					num6++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 2:
		{
			int num3 = Mathf.FloorToInt((float)(accessory.Length / 10));
			if (accessory.Length % 10 > 0)
			{
				num3++;
			}
			spDeco = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num3 * crdSilo.y + (float)(num3 - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spDeco);
			int num4 = 0;
			for (int k = 0; k < num3; k++)
			{
				for (int l = 0; l < 10; l++)
				{
					if (num4 >= accessory.Length)
					{
						break;
					}
					Rect rect2 = new Rect((float)l * (crdSilo.x + crdSiloOffset.x), (float)k * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					if (GlobalVars.Instance.MyButton(rect2, accessoryIcon[num4], new GUIContent(string.Empty, "s" + accessory[num4].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = accessory[num4];
					}
					if (currentBrick != null && currentBrick.GetIndex() == accessory[num4].GetIndex())
					{
						GUI.Box(rect2, string.Empty, "BoxBrickSel");
					}
					num4++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 3:
		{
			int num = Mathf.FloorToInt((float)(functionalIcon.Length / 10));
			if (functionalIcon.Length % 10 > 0)
			{
				num++;
			}
			spFunc = GUI.BeginScrollView(viewRect: new Rect(0f, 0f, 10f * crdSilo.x + 9f * crdSiloOffset.x, (float)num * crdSilo.y + (float)(num - 1) * crdSiloOffset.y), position: crdSiloPosition, scrollPosition: spFunc);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (num2 >= functionalIcon.Length)
					{
						break;
					}
					Rect rect = new Rect((float)j * (crdSilo.x + crdSiloOffset.x), (float)i * (crdSilo.y + crdSiloOffset.y), crdSilo.x, crdSilo.y);
					if (GlobalVars.Instance.MyButton(rect, functionalIcon[num2], new GUIContent(string.Empty, "s" + functional[num2].GetIndex().ToString()), "ButtonBrick"))
					{
						currentBrick = functional[num2];
					}
					if (currentBrick != null && currentBrick.GetIndex() == functional[num2].GetIndex())
					{
						GUI.Box(rect, string.Empty, "BoxBrickSel");
					}
					num2++;
				}
			}
			GUI.EndScrollView();
			break;
		}
		}
		GUI.EndGroup();
	}
}
